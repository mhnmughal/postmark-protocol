using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using RehmanMughal.PostmarkProtocol.Gameplay;
using RehmanMughal.PostmarkProtocol.Levels;

namespace RehmanMughal.PostmarkProtocol.Editor
{
    public static class LevelValidation
    {
        static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
        [MenuItem("Postmark Protocol/Validate All Routes and Rules")]
        public static string RunAll()
        {
            var levels = AssetDatabase.FindAssets("t:LevelDefinition", new[] { "Assets/Data/Levels" })
                .Select(g => AssetDatabase.LoadAssetAtPath<LevelDefinition>(AssetDatabase.GUIDToAssetPath(g))).OrderBy(l => l.id).ToArray();
            Require(levels.Length == 36, "Expected 36 level assets, got " + levels.Length);
            var ids = new HashSet<int>(); var layouts = new HashSet<string>(); var report = new StringBuilder();
            foreach (var level in levels)
            {
                string prefix = "Level " + level.id + ": ";
                Require(ids.Add(level.id), prefix + "Duplicate ID");
                Require(level.chapter == (level.id - 1) / 6 + 1, prefix + "Invalid chapter");
                Require(!string.IsNullOrEmpty(level.title) && !string.IsNullOrEmpty(level.lesson), prefix + "Missing label or lesson");
                Require(level.Contains(level.start) && level.Contains(level.destination) && level.start != level.destination, prefix + "Invalid terminals");
                Require(level.route.Distinct().Count() == level.route.Length, prefix + "Duplicate route cells");
                Require(level.route.All(p => p.x >= 0 && p.x < level.width && p.y >= 0 && p.y < level.height), prefix + "Out of bounds");
                Require(level.slots.All(s => level.Contains(s.position) && s.allowedInks.Length > 0), prefix + "Invalid slot");
                Require(level.slots.Select(s => s.position).Distinct().Count() == level.slots.Length, prefix + "Duplicate slot");
                Require(level.checkpoints.All(level.Contains) && level.hazards.All(level.Contains), prefix + "Invalid checkpoint or bump");
                Require(level.gaps.All(p => !level.Contains(p)), prefix + "Gap overlaps the route");
                Require(level.gates.All(g => level.Contains(g.position) && g.period > 0 && g.openDuration > 0 && g.openDuration <= g.period && g.openPhase >= 0 && g.openPhase < g.period), prefix + "Invalid gate");
                Require(level.parcel != ParcelType.Registered || level.checkpoints.Length > 0, prefix + "Registered parcel has no verification");
                Require(level.par <= level.stampLimit && level.knownSolution.Length <= level.par, prefix + "Invalid par/budget");
                var signature = string.Join(";", level.route.OrderBy(p => p.x).ThenBy(p => p.y).Select(p => p.x + "," + p.y));
                Require(layouts.Add(signature), prefix + "Duplicated spatial layout");
                var first = RouteSimulator.Run(level, level.knownSolution);
                Require(first.success, prefix + first.reason + " at tick " + first.ticks);
                Require(first.steps <= level.optimalSteps, prefix + "Solution misses perfect-route target");
                var second = RouteSimulator.Run(level, level.knownSolution);
                Require(first.frames.Count == second.frames.Count && first.ticks == second.ticks && first.steps == second.steps, prefix + "Non-deterministic result");
                for (int i = 0; i < first.frames.Count; i++)
                    Require(first.frames[i].position == second.frames[i].position && first.frames[i].direction == second.frames[i].direction && first.frames[i].tick == second.frames[i].tick && first.frames[i].eventName == second.frames[i].eventName, prefix + "Non-deterministic frame");
                Require(!RouteSimulator.Run(level, Array.Empty<StampPlacement>()).success, prefix + "Solved with no stamps");
                foreach (var stamp in level.knownSolution)
                    Require(!RouteSimulator.Run(level, level.knownSolution.Where(s => s != stamp)).success, prefix + "Unnecessary authored solution stamp at " + stamp.position);
                report.AppendLine(prefix + level.title + " PASS (" + first.steps + " cells, " + first.ticks + " ticks, " + level.knownSolution.Length + " stamps)");
            }
            int assertions = RuleChecks();
            string summary = "PASS: 36 unique route assets, 72 deterministic solution runs, 36 empty-plan failures, " + assertions + " rule checks.\n" + report;
            System.IO.Directory.CreateDirectory("Documentation");
            System.IO.File.WriteAllText("Documentation/LEVEL_VALIDATION.txt", summary);
            Debug.Log(summary);
            return summary;
        }
        public static int RuleChecks()
        {
            int count = 0;
            Action<bool, string> check = (condition, message) => { Require(condition, message); count++; };
            var level = ScriptableObject.CreateInstance<LevelDefinition>();
            try
            {
                level.start = Vector2Int.zero; level.destination = new Vector2Int(4,0); level.startDirection = Direction.East;
                level.route = Enumerable.Range(0,5).Select(x => new Vector2Int(x,0)).ToArray();
                level.slots = new[] { new SlotDefinition { position = new Vector2Int(1,0), allowedInks = (Ink[])Enum.GetValues(typeof(Ink)) }, new SlotDefinition { position = new Vector2Int(2,0), allowedInks = (Ink[])Enum.GetValues(typeof(Ink)) } };
                level.availableCommands = (Command[])Enum.GetValues(typeof(Command)); level.stampLimit = 2;
                Func<Command,Ink,SimulationResult> run = (c,i) => RouteSimulator.Run(level,new[] { new StampPlacement(new Vector2Int(1,0),c,i) });
                check(RouteSimulator.Run(level,Array.Empty<StampPlacement>()).ticks == 8,"Normal travel takes two ticks");
                check(run(Command.Wait,Ink.Standard).ticks == 9,"WAIT takes one tick");
                check(run(Command.Wait,Ink.Double).ticks == 10,"Double WAIT takes two ticks");
                check(run(Command.Fast,Ink.Standard).ticks == 7,"FAST applies to one segment");
                check(run(Command.Slow,Ink.Standard).ticks == 9,"SLOW applies to one segment");
                check(run(Command.Slow,Ink.Double).ticks == 10,"Double SLOW executes twice");
                check(!run(Command.Left,Ink.Standard).success,"Left changes direction");
                check(!run(Command.Right,Ink.Standard).success,"Right changes direction");
                check(!run(Command.Reverse,Ink.Standard).success,"Reverse changes direction");
                check(run(Command.Reverse,Ink.Double).success,"Double reverse restores direction");
                check(!run(Command.Jump,Ink.Standard).success,"JUMP requires a designated obstacle");
                level.hazards = new[] { new Vector2Int(2,0) }; level.parcel = ParcelType.Fragile;
                check(!RouteSimulator.Run(level,Array.Empty<StampPlacement>()).success,"Fragile bump collision fails");
                check(run(Command.Jump,Ink.Standard).success,"JUMP clears fragile bump");
                level.parcel = ParcelType.Standard;
                check(RouteSimulator.Run(level,Array.Empty<StampPlacement>()).success,"Standard parcel tolerates bumps");
                level.parcel = ParcelType.Express;
                check(!run(Command.Wait,Ink.Standard).success,"Express WAIT rejected by simulator");
                var plan = new PlanState(level);
                check(!plan.TryPlace(new Vector2Int(1,0),Command.Wait,Ink.Standard,out _),"Express WAIT rejected during planning");
                level.parcel = ParcelType.Registered; level.checkpoints = new[] { new Vector2Int(2,0) };
                check(RouteSimulator.Run(level,Array.Empty<StampPlacement>()).success,"Registered checkpoint recorded");
                check(!run(Command.Jump,Ink.Standard).success,"Jumping over checkpoint does not verify it");
                level.parcel = ParcelType.Standard;
                check(plan.TryPlace(new Vector2Int(1,0),Command.Fast,Ink.Permanent,out _),"Permanent placement allowed");
                check(!plan.TryRemove(new Vector2Int(1,0),out _),"Permanent erasure locked");
                check(!plan.TryPlace(new Vector2Int(1,0),Command.Slow,Ink.Standard,out _),"Permanent replacement locked");
                plan.Reset(); check(plan.Stamps.Count == 0,"Reset clears permanent ink");
                check(plan.TryPlace(new Vector2Int(1,0),Command.Wait,Ink.Standard,out _),"Standard placement allowed");
                check(plan.TryPlace(new Vector2Int(1,0),Command.Slow,Ink.Standard,out _) && plan.Stamps.Count == 1,"Replacement does not consume second budget");
                level.stampLimit = 1;
                check(!plan.TryPlace(new Vector2Int(2,0),Command.Wait,Ink.Standard,out _),"Stamp budget enforced");
                check(plan.TryRemove(new Vector2Int(1,0),out _),"Standard erasure allowed");
                check(!plan.TryPlace(Vector2Int.zero,Command.Wait,Ink.Standard,out _),"Non-slot placement rejected");
                level.commandLimits = new[] { new CommandLimit { command = Command.Wait, count = 0 } };
                check(!plan.TryPlace(new Vector2Int(1,0),Command.Wait,Ink.Standard,out _),"Command limit enforced");
                level.commandLimits = Array.Empty<CommandLimit>();
                level.gates = new[] { new TimedGate { position = new Vector2Int(2,0),period = 4,openPhase = 1,openDuration = 1 } };
                check(!RouteSimulator.Run(level,Array.Empty<StampPlacement>()).success,"Closed timed gate rejects arrival");
                check(run(Command.Wait,Ink.Standard).success,"WAIT synchronizes gate");
                level.gates = Array.Empty<TimedGate>(); level.maxTicks = 3;
                check(!RouteSimulator.Run(level,Array.Empty<StampPlacement>()).success,"Time limit terminates simulation");
            }
            finally { UnityEngine.Object.DestroyImmediate(level); }
            return count;
        }
    }
}
