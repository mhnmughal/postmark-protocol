using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using RehmanMughal.PostmarkProtocol.Gameplay;
using RehmanMughal.PostmarkProtocol.Levels;

namespace RehmanMughal.PostmarkProtocol.Editor
{
    // These explicit polylines are the authored route sheets, not seeded or rotated variants.
    public static class LevelAuthoring
    {
        sealed class Sheet
        {
            public readonly LevelDefinition level;
            readonly List<Vector2Int> walk = new List<Vector2Int>();
            readonly Dictionary<Vector2Int, StampPlacement> solution = new Dictionary<Vector2Int, StampPlacement>();
            readonly Dictionary<Vector2Int, SlotDefinition> slots = new Dictionary<Vector2Int, SlotDefinition>();
            readonly Dictionary<Command,int> extraStock = new Dictionary<Command,int>();
            int spareStamps;
            public Sheet(int id, string title, string lesson, string polyline)
            {
                level = ScriptableObject.CreateInstance<LevelDefinition>();
                level.id = id; level.chapter = (id - 1) / 6 + 1; level.title = title; level.lesson = lesson; level.night = id > 30;
                var vertices = polyline.Split(' ').Select(Point).ToArray();
                walk.Add(vertices[0]);
                for (int i = 1; i < vertices.Length; i++)
                {
                    var delta = vertices[i] - vertices[i - 1];
                    if (delta.x != 0 && delta.y != 0) throw new Exception("Diagonal authored route " + id);
                    var step = new Vector2Int(Math.Sign(delta.x), Math.Sign(delta.y));
                    while (walk[walk.Count - 1] != vertices[i]) walk.Add(walk[walk.Count - 1] + step);
                }
                level.start = walk[0]; level.destination = walk[walk.Count - 1];
                level.startDirection = Facing(walk[1] - walk[0]);
                level.route = walk.Distinct().ToArray();
                for (int i = 1; i < walk.Count - 1; i++)
                {
                    int change = ((int)Facing(walk[i + 1] - walk[i]) - (int)Facing(walk[i] - walk[i - 1]) + 4) % 4;
                    if (change == 0) continue;
                    var command = change == 1 ? Command.Right : change == 3 ? Command.Left : Command.Reverse;
                    if (solution.TryGetValue(walk[i], out var existing) && existing.command != command)
                        throw new Exception("Incompatible revisited corner in " + id);
                    Stamp(walk[i].x + "," + walk[i].y, command);
                }
                level.availableCommands = id <= 6 ? new[] { Command.Left, Command.Right } : id <= 12
                    ? new[] { Command.Left, Command.Right, Command.Wait, Command.Fast, Command.Slow }
                    : id <= 24 ? new[] { Command.Left, Command.Right, Command.Wait, Command.Reverse, Command.Fast, Command.Slow }
                    : (Command[])Enum.GetValues(typeof(Command));
            }
            public Sheet Stamp(string at, Command command, Ink ink = Ink.Standard)
            {
                var p = Point(at);
                solution[p] = new StampPlacement(p, command, ink);
                slots[p] = new SlotDefinition { position = p, allowedInks = level.id >= 19 ? (Ink[])Enum.GetValues(typeof(Ink)) : new[] { Ink.Standard } };
                return this;
            }
            public Sheet Slot(string at, params Ink[] inks)
            { var p = Point(at); slots[p] = new SlotDefinition { position = p, allowedInks = inks.Length == 0 ? new[] { Ink.Standard } : inks }; return this; }
            public Sheet Gate(string at, int period, int phase, int duration = 1)
            { level.gates = level.gates.Concat(new[] { new TimedGate { position = Point(at), period = period, openPhase = phase, openDuration = duration } }).ToArray(); return this; }
            public Sheet Gap(string at)
            { var p = Point(at); level.gaps = level.gaps.Concat(new[] { p }).ToArray(); level.route = level.route.Where(x => x != p).ToArray(); return this; }
            public Sheet Bump(string at)
            { level.hazards = level.hazards.Concat(new[] { Point(at) }).ToArray(); return this; }
            public Sheet Parcel(ParcelType type) { level.parcel = type; return this; }
            public Sheet Verify(params string[] at) { level.checkpoints = at.Select(Point).ToArray(); return this; }
            public Sheet Commands(params Command[] commands) { level.availableCommands = commands; return this; }
            public Sheet Spare(string at, Command command = Command.Wait)
            {
                Slot(at,Ink.Standard); spareStamps++;
                if(!extraStock.ContainsKey(command)) extraStock[command]=0;
                extraStock[command]++;
                return this;
            }
            public Sheet ExtraRoute(string points)
            { level.route=level.route.Concat(points.Split(' ').Select(Point)).Distinct().ToArray(); return this; }
            public LevelDefinition Finish()
            {
                level.knownSolution = solution.Values.ToArray(); level.slots = slots.Values.ToArray();
                level.par = solution.Count; level.stampLimit = solution.Count + Math.Max(spareStamps,level.id < 13 ? 1 : 0);
                level.commandLimits = level.availableCommands.Select(c => new CommandLimit { command = c, count = Math.Max(1, solution.Values.Count(s => s.command == c)+(extraStock.TryGetValue(c,out var bonus) ? bonus : 0)) }).ToArray();
                level.optimalSteps = walk.Count - 1;
                return level;
            }
        }
        static Vector2Int Point(string p) { var parts = p.Split(','); return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1])); }
        static Direction Facing(Vector2Int delta) { return delta.y > 0 ? Direction.North : delta.x > 0 ? Direction.East : delta.y < 0 ? Direction.South : Direction.West; }
        static Sheet D(int id, string title, string lesson, string path) { return new Sheet(id, title, lesson, path); }

        [MenuItem("Postmark Protocol/Author 36 Route Sheets")]
        public static string BuildAssets()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Data")) AssetDatabase.CreateFolder("Assets", "Data");
            if (!AssetDatabase.IsValidFolder("Assets/Data/Levels")) AssetDatabase.CreateFolder("Assets/Data", "Levels");
            var sheets = new[] {
                D(1,"First Class", "Choose RIGHT. Stamp the single marked corner, then press DISPATCH.", "1,4 4,4 4,2"),
                D(2,"Around the Corner", "Choose LEFT, then tap the dotted slot. Dispatch when ready.", "1,1 4,1 4,4"),
                D(3,"Right Address", "Two RIGHT turns guide the letter around the block.", "1,4 5,4 5,1 2,1"),
                D(4,"Two Corners", "Stamp both corners. Tap a stamped slot to replace it.", "0,1 3,1 3,4 6,4"),
                D(5,"The Courtyard", "Read each turn from the letter's direction, not your own.", "1,0 1,4 6,4 6,1 3,1"),
                D(6,"Sorting Circuit", "Plan the whole route. ERASE removes standard ink; RESET clears the sheet.", "0,0 3,0 3,2 1,2 1,4 6,4 6,2"),
                D(7,"One Beat Later", "WAIT holds for one tick. The gate's numbers show its open phase.", "0,2 4,2 4,4 6,4").Stamp("1,2",Command.Wait).Gate("2,2",4,1).Spare("5,4"),
                D(8,"Quick Collection", "FAST makes the next step take one tick instead of two.", "1,0 1,3 5,3 5,1").Stamp("1,1",Command.Fast).Gate("1,2",4,3),
                D(9,"Handle with Time", "SLOW makes the next step take three ticks.", "0,4 3,4 3,1 6,1 6,3").Stamp("1,4",Command.Slow).Gate("2,4",6,5),
                D(10,"Two Appointments", "Each WAIT adds one tick. Time both crossings.", "0,1 2,1 2,4 5,4 5,0").Stamp("1,1",Command.Wait).Stamp("3,4",Command.Wait).Gate("2,1",4,1).Gate("4,4",5,1),
                D(11,"Variable Service", "FAST and SLOW affect one segment. Look ahead to the next gate.", "1,4 1,1 4,1 4,4 7,4").Stamp("1,3",Command.Fast).Stamp("2,1",Command.Slow).Gate("1,2",4,3).Gate("3,1",6,4),
                D(12,"Closing Time", "Three gate schedules, one route. Stamp timing between the turns.", "0,0 3,0 3,3 6,3 6,5 2,5").Stamp("1,0",Command.Wait).Stamp("3,1",Command.Fast).Stamp("4,3",Command.Slow).Gate("2,0",4,1).Gate("3,2",5,0).Gate("5,3",6,5).Spare("4,5"),
                D(13,"Return to Sender", "REVERSE turns the letter around. A shorter return earns Perfect Route.", "3,2 4,2 0,2").ExtraRoute("5,2 6,2").Slot("6,2",Ink.Standard),
                D(14,"Wrong-Way Sack", "Turn around at the end, then take the upper branch.", "4,1 6,1 2,1 2,4 6,4").Spare("4,4"),
                D(15,"Back Through Sorting", "A return journey crosses the same cells in the opposite direction.", "2,3 2,5 2,1 6,1 6,4 4,4"),
                D(16,"Countercurrent", "Follow the arrow through the return before planning the staircase.", "4,4 7,4 1,4 1,2 3,2 3,0 6,0"),
                D(17,"Return Window", "FAST triggers on both visits. The exit gate opens after the complete return.", "3,1 6,1 1,1 1,4 4,4").Stamp("4,1",Command.Fast).Gate("5,1",4,3).Gate("1,2",8,0),
                D(18,"Round Trip", "The same LEFT leads north, then south. Complete the loop to reach the gate on time.", "1,2 0,2 2,2 2,4 5,4 5,2 2,2 2,0 6,0").Gate("2,1",8,4),
                D(19,"Double Impression", "DOUBLE ink executes twice. Two LEFT turns make a REVERSE.", "4,3 7,3 1,3 1,0 5,0").Stamp("7,3",Command.Left,Ink.Double).Slot("7,3",Ink.Double).Commands(Command.Left,Command.Right),
                D(20,"A Second Right", "Two RIGHT turns also reverse. Inspect the ink border on each slot.", "3,2 3,0 3,5 6,5 6,2 5,2").Stamp("3,0",Command.Right,Ink.Double).Slot("3,0",Ink.Double).Commands(Command.Left,Command.Right,Command.Wait).Spare("6,3"),
                D(21,"Drying Time", "DOUBLE WAIT pauses for two ticks before moving.", "0,3 4,3 4,0 7,0 7,4").Stamp("1,3",Command.Wait,Ink.Double).Slot("1,3",Ink.Double).Gate("2,3",5,1),
                D(22,"Indelible Address", "PERMANENT ink locks after placement. RESET is the only way to erase it.", "0,0 2,0 2,4 4,4 4,2 7,2").Stamp("2,0",Command.Left,Ink.Permanent).Slot("2,0",Ink.Permanent),
                D(23,"Carbon Return", "Mix a double turn, permanent corner and timed slowdown.", "5,4 7,4 2,4 2,1 5,1").Stamp("7,4",Command.Left,Ink.Double).Slot("7,4",Ink.Double).Stamp("2,4",Command.Left,Ink.Permanent).Slot("2,4",Ink.Permanent).Stamp("2,3",Command.Slow).Gate("2,2",5,4),
                D(24,"Ink Inspector", "DOUBLE SLOW takes four ticks. Permanent turns demand a committed plan.", "0,1 4,1 4,5 1,5 1,3 2,3").Stamp("1,1",Command.Slow,Ink.Double).Slot("1,1",Ink.Double).Gate("2,1",5,1).Stamp("4,5",Command.Left,Ink.Permanent).Slot("4,5",Ink.Permanent),
                D(25,"Handle with Care", "FRAGILE parcels cannot touch bumps. JUMP from the preceding slot.", "0,2 5,2 5,5 2,5").Parcel(ParcelType.Fragile).Stamp("1,2",Command.Jump).Bump("2,2").Spare("3,5"),
                D(26,"No Waiting", "EXPRESS mail cannot WAIT. Use speed to meet the gate.", "0,5 3,5 3,2 6,2 6,0").Parcel(ParcelType.Express).Stamp("1,5",Command.Fast).Gate("2,5",4,3).Spare("6,1",Command.Slow),
                D(27,"Signed For", "REGISTERED mail must visit every numbered checkpoint before delivery.", "0,1 2,1 2,4 6,4 6,1 4,1").Parcel(ParcelType.Registered).Verify("2,3","5,4"),
                D(28,"Bridge Repair", "JUMP clears a marked gap. Land on the printed route.", "0,0 5,0 5,4 3,4 3,2 1,2").Parcel(ParcelType.Fragile).Stamp("1,0",Command.Jump).Gap("2,0").Stamp("5,1",Command.Jump).Bump("5,2"),
                D(29,"Express Return", "Complete the return before the exit gate opens. Express mail never waits.", "4,2 7,2 2,2 2,5 0,5 0,3").Parcel(ParcelType.Express).Stamp("5,2",Command.Slow).Gate("6,2",4,1).Gate("2,3",8,4),
                D(30,"The Verification Loop", "Collect the outer signature before taking the exit. The junction is visited twice.", "0,3 2,3 2,5 6,5 6,3 2,3 2,0 5,0").Parcel(ParcelType.Registered).Verify("4,5","2,1").Spare("4,0"),
                D(31,"After-Hours Crossing", "A fragile delivery, a permanent turn and a timed bridge.", "0,1 5,1 5,5 1,5 1,3").Parcel(ParcelType.Fragile).Stamp("1,1",Command.Jump).Gap("2,1").Stamp("3,1",Command.Slow).Gate("4,1",5,2).Stamp("5,5",Command.Left,Ink.Permanent).Slot("5,5",Ink.Permanent).Spare("1,4"),
                D(32,"Midnight Express", "Double ink reverses the return. Meet both gates without waiting.", "3,4 7,4 1,4 1,0 5,0 5,2").Parcel(ParcelType.Express).Stamp("7,4",Command.Right,Ink.Double).Slot("7,4",Ink.Double).Stamp("4,4",Command.Fast).Gate("5,4",4,3).Gate("1,3",8,4),
                D(33,"Four Signatures", "A registered circuit needs all four signatures. Budget every turn.", "0,2 2,2 2,5 7,5 7,1 4,1 4,3 6,3").Parcel(ParcelType.Registered).Verify("1,2","4,5","6,1","4,2").Stamp("2,2",Command.Left,Ink.Permanent).Slot("2,2",Ink.Permanent).Spare("5,3"),
                D(34,"Long Leap", "DOUBLE JUMP advances four cells across the marked obstacle run.", "0,4 6,4 6,0 2,0 2,2 4,2").Parcel(ParcelType.Fragile).Stamp("1,4",Command.Jump,Ink.Double).Slot("1,4",Ink.Double).Gap("2,4").Gap("3,4").Gap("4,4").Stamp("6,3",Command.Slow).Gate("6,2",5,1),
                D(35,"Final Collection", "A double pause opens the first gate. Do not miss the return-loop signature.", "0,2 3,2 3,5 6,5 6,2 3,2 3,0 7,0").Parcel(ParcelType.Registered).Verify("5,5","5,0").Stamp("1,2",Command.Wait,Ink.Double).Slot("1,2",Ink.Double).Gate("2,2",5,1).Stamp("3,2",Command.Left,Ink.Permanent).Slot("3,2",Ink.Permanent).Spare("6,0"),
                D(36,"Postmark Protocol", "One final sheet: double return, two signatures, a bridge, and exact gate timing.", "2,1 0,1 4,1 4,5 7,5 7,2 6,2").Parcel(ParcelType.Registered).Verify("1,1","5,5").Stamp("0,1",Command.Right,Ink.Double).Slot("0,1",Ink.Double).Stamp("4,1",Command.Left,Ink.Permanent).Slot("4,1",Ink.Permanent).Stamp("4,2",Command.Jump).Gap("4,3").Stamp("5,5",Command.Slow,Ink.Double).Slot("5,5",Ink.Double).Gate("6,5",7,3)
            };
            foreach (var sheet in sheets)
            {
                var level = sheet.Finish();
                string path = "Assets/Data/Levels/Route_" + level.id.ToString("00") + ".asset";
                var current = AssetDatabase.LoadAssetAtPath<LevelDefinition>(path);
                if (current == null) AssetDatabase.CreateAsset(level, path);
                else { EditorUtility.CopySerialized(level, current); UnityEngine.Object.DestroyImmediate(level); EditorUtility.SetDirty(current); }
            }
            AssetDatabase.SaveAssets();
            return "Authored 36 route sheets. " + LevelValidation.RunAll();
        }
    }
}
