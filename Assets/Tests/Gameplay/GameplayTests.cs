using System;
using System.Linq;
using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using RehmanMughal.PostmarkProtocol.Gameplay;
using RehmanMughal.PostmarkProtocol.Levels;

namespace RehmanMughal.PostmarkProtocol.Tests
{
    public sealed class GameplayTests
    {
        static LevelDefinition Load(int id)
        {
            var level = AssetDatabase.LoadAssetAtPath<LevelDefinition>("Assets/Data/Levels/Route_" + id.ToString("00") + ".asset");
            Assert.That(level, Is.Not.Null, "Missing authored route " + id);
            return level;
        }
        [TestCaseSource(nameof(LevelIds))]
        public void EveryAuthoredRouteDeliversAndReplaysIdentically(int id)
        {
            var level = Load(id);
            var first = RouteSimulator.Run(level, level.knownSolution);
            var second = RouteSimulator.Run(level, level.knownSolution);
            Assert.That(first.success, Is.True, first.reason);
            Assert.That(RouteSimulator.Run(level,Array.Empty<StampPlacement>()).success,Is.False,"An empty sheet must not deliver");
            Assert.That(first.steps, Is.LessThanOrEqualTo(level.optimalSteps));
            Assert.That(first.frames.Select(f => f.position + ":" + f.direction + ":" + f.tick + ":" + f.eventName),
                Is.EqualTo(second.frames.Select(f => f.position + ":" + f.direction + ":" + f.tick + ":" + f.eventName)));
        }
        public static int[] LevelIds => Enumerable.Range(1,36).ToArray();
        [Test]
        public void FirstDeliveryTeachesExactlyOneRightTurn()
        {
            var level=Load(1);
            Assert.That(level.slots.Length,Is.EqualTo(1));
            Assert.That(level.knownSolution.Length,Is.EqualTo(1));
            Assert.That(level.knownSolution[0].command,Is.EqualTo(Command.Right));
        }
        [TestCase(7,1)]
        [TestCase(21,2)]
        public void WaitHasExactlyOneFramePerExecution(int id,int expected)
        {
            var level=Load(id);
            Assert.That(RouteSimulator.Run(level,level.knownSolution).frames.Count(f=>f.eventName=="Wait"),Is.EqualTo(expected));
        }
        [TestCase(17)]
        [TestCase(29)]
        [TestCase(32)]
        public void EarlyReverseCannotBypassTimedReturn(int id)
        {
            var level=Load(id);
            var turnaround=level.knownSolution.First(s=>s.command==Command.Reverse || s.ink==Ink.Double && s.command==Command.Right);
            var altered=level.knownSolution.Where(s=>s!=turnaround).Select(s=>new StampPlacement(s.position,s.command,s.ink)).ToArray();
            var early=altered.First(s=>s.command==Command.Fast || s.command==Command.Slow);
            early.command=turnaround.command; early.ink=turnaround.ink;
            var result=RouteSimulator.Run(level,altered);
            Assert.That(result.success,Is.False);
            Assert.That(result.reason,Does.Contain("gate"));
        }
        [TestCase(7,5,4,Command.Wait)]
        [TestCase(26,6,1,Command.Slow)]
        [TestCase(35,6,0,Command.Wait)]
        public void OptionalExtraStampDeliversWithoutEfficiencySeal(int id,int x,int y,Command command)
        {
            var level=Load(id);
            var stamps=level.knownSolution.Concat(new[]{new StampPlacement(new Vector2Int(x,y),command)}).ToArray();
            AssertSeals(level,stamps,false,false);
        }
        [Test]
        public void LongReturnEarnsEfficiencyButNotPerfectRoute()
        {
            AssertSeals(Load(13),new[]{new StampPlacement(new Vector2Int(6,2),Command.Reverse)},true,false);
        }
        static void AssertSeals(LevelDefinition level,StampPlacement[] stamps,bool efficient,bool perfect)
        {
            var result=RouteSimulator.Run(level,stamps);
            Assert.That(result.success,Is.True,result.reason);
            string path=Path.Combine(Path.GetTempPath(),"postmark-seal-test-"+Guid.NewGuid().ToString("N")+".json");
            try
            {
                var save=new SaveRepository(path);
                save.RecordDelivery(level.id,stamps.Length,result.steps,level.par,level.optimalSteps);
                var progress=save.GetLevel(level.id);
                Assert.That(progress.delivered,Is.True);
                Assert.That(progress.efficient,Is.EqualTo(efficient));
                Assert.That(progress.perfect,Is.EqualTo(perfect));
            }
            finally { foreach(var suffix in new[]{"",".bak",".tmp"}) if(File.Exists(path+suffix)) File.Delete(path+suffix); }
        }
        [Test]
        public void PermanentInkRequiresReset()
        {
            var level = Load(22); var plan = new PlanState(level); var stamp = level.knownSolution.First(s => s.ink == Ink.Permanent);
            Assert.That(plan.TryPlace(stamp.position,stamp.command,stamp.ink,out _),Is.True);
            Assert.That(plan.TryRemove(stamp.position,out _),Is.False);
            Assert.That(plan.TryPlace(stamp.position,stamp.command,Ink.Standard,out _),Is.False);
            plan.Reset(); Assert.That(plan.Stamps.Count,Is.Zero);
        }
        [Test]
        public void ExpressCannotWait()
        {
            var level = Load(26); var plan = new PlanState(level);
            Assert.That(plan.TryPlace(level.slots[0].position,Command.Wait,Ink.Standard,out _),Is.False);
        }
        [Test]
        public void FragileNeedsJump()
        {
            var level = Load(25);
            Assert.That(RouteSimulator.Run(level,level.knownSolution.Where(s => s.command != Command.Jump)).success,Is.False);
        }
        [Test]
        public void RegisteredCannotSkipOuterLoop()
        {
            var level = Load(30);
            var altered = level.knownSolution.Select(s => new StampPlacement(s.position,s.command,s.ink)).ToArray();
            altered.First(s => s.position == new Vector2Int(2,3)).command = Command.Right;
            Assert.That(RouteSimulator.Run(level,altered).success,Is.False);
        }
        [Test]
        public void TimingStampIsRequired()
        {
            var level = Load(7);
            Assert.That(RouteSimulator.Run(level,level.knownSolution.Where(s => s.command != Command.Wait)).success,Is.False);
        }
    }
}
