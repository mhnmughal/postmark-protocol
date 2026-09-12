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
    public static class LevelOptimality
    {
        sealed class Search
        {
            public LevelDefinition level;
            public int nodes, solutions, bestStamps = int.MaxValue, bestSteps = int.MaxValue;
            public bool exhausted;
            public StampPlacement[] fewest, shortest;
            public void Visit(List<StampPlacement> stamps, HashSet<Vector2Int> decided)
            {
                if (++nodes > 2000000) { exhausted = true; return; }
                var result = RouteSimulator.Run(level, stamps);
                SlotDefinition next = null;
                foreach (var frame in result.frames)
                {
                    if (decided.Contains(frame.position)) continue;
                    next = level.SlotAt(frame.position);
                    if (next != null) break;
                }
                if (next == null)
                {
                    if (!result.success) return;
                    solutions++;
                    if (stamps.Count < bestStamps) { bestStamps = stamps.Count; fewest = stamps.ToArray(); }
                    if (result.steps < bestSteps) { bestSteps = result.steps; shortest = stamps.ToArray(); }
                    return;
                }
                decided.Add(next.position);
                Visit(stamps, decided);
                if (stamps.Count < level.stampLimit && !exhausted)
                {
                    foreach (var command in level.availableCommands)
                    foreach (var ink in next.allowedInks)
                    {
                        // Editing locks are observationally identical after dispatch.
                        if (ink == Ink.Permanent && next.allowedInks.Contains(Ink.Standard)) continue;
                        if (ink == Ink.Double && command == Command.Reverse) continue; // Same route as an empty slot, at higher cost.
                        if (ink == Ink.Double && command == Command.Fast && next.allowedInks.Contains(Ink.Standard)) continue;
                        var candidate = new StampPlacement(next.position,command,ink);
                        var plan = new PlanState(level); bool valid = true;
                        foreach (var s in stamps) if (!plan.TryPlace(s.position,s.command,s.ink,out _)) { valid=false; break; }
                        if (!valid || !plan.TryPlace(candidate.position,candidate.command,candidate.ink,out _)) continue;
                        stamps.Add(candidate); Visit(stamps,decided); stamps.RemoveAt(stamps.Count-1);
                        if (exhausted) break;
                    }
                }
                decided.Remove(next.position);
            }
        }
        static string Describe(StampPlacement[] stamps)
        { return stamps == null ? "none" : string.Join("; ",stamps.Select(s => s.position+" "+s.command+"/"+s.ink)); }
        [MenuItem("Postmark Protocol/Audit Exact Route Targets")]
        public static string RunAll()
        {
            var report=new StringBuilder(); int allNodes=0, mutations=0, alternatives=0, mismatches=0;
            var levels=AssetDatabase.FindAssets("t:LevelDefinition",new[]{"Assets/Data/Levels"})
                .Select(g=>AssetDatabase.LoadAssetAtPath<LevelDefinition>(AssetDatabase.GUIDToAssetPath(g))).OrderBy(l=>l.id);
            foreach(var level in levels)
            {
                var search=new Search { level=level };
                search.Visit(new List<StampPlacement>(),new HashSet<Vector2Int>());
                allNodes+=search.nodes;
                bool targets=search.bestStamps==level.par && search.bestSteps==level.optimalSteps;
                if(!targets || search.exhausted) mismatches++;
                report.AppendLine($"{level.id:00} {level.title}: {(search.exhausted ? "INCOMPLETE NODE LIMIT" : targets ? "PROVEN" : "TARGET MISMATCH")} / min stamps {search.bestStamps} vs par {level.par} / min steps {search.bestSteps} vs target {level.optimalSteps} / {search.nodes} states / {search.solutions} solutions");
                if(!targets) report.AppendLine("Fewest: "+Describe(search.fewest)+"\nShortest: "+Describe(search.shortest));
                for(int i=0;i<level.knownSolution.Length;i++)
                foreach(Command command in Enum.GetValues(typeof(Command)))
                {
                    if(command==level.knownSolution[i].command)continue;
                    var altered=level.knownSolution.Select(s=>new StampPlacement(s.position,s.command,s.ink)).ToArray();
                    altered[i].command=command;
                    var result=RouteSimulator.Run(level,altered);
                    var repeated=RouteSimulator.Run(level,altered);
                    if(result.success!=repeated.success || result.reason!=repeated.reason || result.steps!=repeated.steps || result.ticks!=repeated.ticks)
                        throw new InvalidOperationException("Mutation nondeterminism at "+level.id);
                    mutations++; if(result.success) alternatives++;
                }
            }
            report.Insert(0,$"Exact reachable-plan audit: {allNodes} states, {mismatches} target mismatches/incomplete searches. Command mutations: {mutations}, valid alternatives: {alternatives}.\n");
            string resultText=report.ToString();
            System.IO.File.WriteAllText("Documentation/LEVEL_OPTIMALITY.txt",resultText);
            Debug.Log(resultText);
            return resultText;
        }
    }
}
