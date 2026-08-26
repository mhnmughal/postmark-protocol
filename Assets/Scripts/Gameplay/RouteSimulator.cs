using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RehmanMughal.PostmarkProtocol.Levels;

namespace RehmanMughal.PostmarkProtocol.Gameplay
{
    public enum Command { Left, Right, Wait, Reverse, Fast, Slow, Jump }
    public enum Ink { Standard, Double, Permanent }
    public enum ParcelType { Standard, Fragile, Express, Registered }
    public enum Direction { North, East, South, West }
    [Serializable] public class StampPlacement
    {
        public Vector2Int position;
        public Command command;
        public Ink ink;
        public StampPlacement() { }
        public StampPlacement(Vector2Int p, Command c, Ink i = Ink.Standard) { position = p; command = c; ink = i; }
    }
    public sealed class PlanState
    {
        readonly LevelDefinition level;
        readonly List<StampPlacement> stamps = new List<StampPlacement>();
        public IReadOnlyList<StampPlacement> Stamps => stamps.AsReadOnly();
        public PlanState(LevelDefinition definition) { level = definition; }
        public bool TryPlace(Vector2Int position, Command command, Ink ink, out string reason)
        {
            reason = "";
            var slot = level.SlotAt(position);
            if (slot == null) reason = "Choose a dotted command slot.";
            else if (!level.availableCommands.Contains(command)) reason = "This command is not issued for this route.";
            else if (!slot.allowedInks.Contains(ink)) reason = "This slot requires a different ink.";
            else if (level.parcel == ParcelType.Express && command == Command.Wait) reason = "Express mail cannot WAIT.";
            var previous = stamps.Find(s => s.position == position);
            if (previous != null && previous.ink == Ink.Permanent) reason = "Permanent ink is locked. Reset the sheet to change it.";
            if (reason.Length > 0) return false;
            if (previous == null && stamps.Count >= level.stampLimit) { reason = "The stamp allowance is used. Replace or erase a stamp."; return false; }
            var limit = Array.Find(level.commandLimits, l => l.command == command);
            if (limit != null && stamps.Count(s => s.command == command && s != previous) >= limit.count)
            { reason = "No more " + command.ToString().ToUpperInvariant() + " stamps are available."; return false; }
            if (previous != null) stamps.Remove(previous);
            stamps.Add(new StampPlacement(position, command, ink));
            return true;
        }
        public bool TryRemove(Vector2Int position, out string reason)
        {
            reason = "";
            var stamp = stamps.Find(s => s.position == position);
            if (stamp == null) { reason = "There is no stamp here."; return false; }
            if (stamp.ink == Ink.Permanent) { reason = "Permanent ink is locked. Reset the sheet to change it."; return false; }
            stamps.Remove(stamp); return true;
        }
        public void Reset() { stamps.Clear(); }
    }
    public sealed class SimulationFrame
    {
        public Vector2Int position;
        public Direction direction;
        public int tick;
        public string eventName;
        public float duration;
    }
    public sealed class SimulationResult
    {
        public bool success;
        public string reason;
        public int steps, ticks;
        public readonly List<SimulationFrame> frames = new List<SimulationFrame>();
    }
    public static class RouteSimulator
    {
        public static Vector2Int Delta(Direction d)
        { return d == Direction.North ? Vector2Int.up : d == Direction.East ? Vector2Int.right : d == Direction.South ? Vector2Int.down : Vector2Int.left; }
        public static SimulationResult Run(LevelDefinition level, IEnumerable<StampPlacement> placements)
        {
            var result = new SimulationResult();
            if (level == null) { result.reason = "Route sheet is missing."; return result; }
            var plan = new PlanState(level);
            foreach (var stamp in placements ?? Enumerable.Empty<StampPlacement>())
            {
                if (stamp == null) { result.reason = "Invalid stamp."; return result; }
                if (plan.Stamps.Any(s => s.position == stamp.position)) { result.reason = "Two stamps occupy one slot."; return result; }
                if (!plan.TryPlace(stamp.position, stamp.command, stamp.ink, out var error)) { result.reason = error; return result; }
            }
            var stamps = plan.Stamps.ToDictionary(s => s.position);
            var position = level.start;
            var direction = level.startDirection;
            var visited = new HashSet<Vector2Int>();
            Action<string, int> frame = (name, elapsed) => result.frames.Add(new SimulationFrame
            { position = position, direction = direction, tick = result.ticks, eventName = name, duration = elapsed * .24f });
            frame("Dispatch", 0);
            while (result.ticks <= level.maxTicks)
            {
                if (!level.Contains(position)) { result.reason = "The parcel left the printed route."; frame("Off route", 0); return result; }
                if (level.parcel == ParcelType.Fragile && level.hazards.Contains(position))
                { result.reason = "The fragile parcel hit a bump. Jump over it."; frame("Damaged", 0); return result; }
                visited.Add(position);
                if (position == level.destination)
                {
                    if (level.parcel == ParcelType.Registered && level.checkpoints.Any(p => !visited.Contains(p)))
                    { result.reason = "Registered mail needs every verification checkpoint."; frame("Unverified", 0); return result; }
                    result.success = true; result.reason = "Delivered safely."; frame("Delivered", 0); return result;
                }
                int travelTicks = 2, distance = 1;
                if (stamps.TryGetValue(position, out var stamp))
                {
                    int repetitions = stamp.ink == Ink.Double ? 2 : 1;
                    for (int i = 0; i < repetitions; i++)
                    {
                        switch (stamp.command)
                        {
                            case Command.Left: direction = (Direction)(((int)direction + 3) % 4); break;
                            case Command.Right: direction = (Direction)(((int)direction + 1) % 4); break;
                            case Command.Reverse: direction = (Direction)(((int)direction + 2) % 4); break;
                            case Command.Wait: result.ticks++; frame("Wait", 1); break;
                            case Command.Fast: travelTicks = Math.Max(1, travelTicks - 1); break;
                            case Command.Slow: travelTicks++; break;
                            case Command.Jump: distance += i == 0 ? 1 : 2; break;
                        }
                    }
                    // WAIT already emitted one timed frame per execution above.
                    if (stamp.command != Command.Wait) frame(stamp.command.ToString(), 0);
                }
                var next = position + Delta(direction) * distance;
                if (distance > 1)
                {
                    bool crossedObstacle = false;
                    for (int i = 1; i < distance; i++)
                    {
                        var crossed = position + Delta(direction) * i;
                        if (level.gaps.Contains(crossed) || level.hazards.Contains(crossed)) crossedObstacle = true;
                        else if (!level.Contains(crossed)) { result.reason = "JUMP only crosses marked gaps or bumps."; frame("Invalid jump", 0); return result; }
                        if (level.gates.Any(g => g.position == crossed && !g.IsOpen(result.ticks + travelTicks)))
                        { result.reason = "A closed gate blocks the jump."; frame("Gate closed", 0); return result; }
                    }
                    if (!crossedObstacle) { result.reason = "JUMP needs a marked obstacle ahead."; frame("Invalid jump", 0); return result; }
                }
                result.ticks += travelTicks;
                if (result.ticks > level.maxTicks) break;
                var gate = Array.Find(level.gates, g => g.position == next);
                if (gate != null && !gate.IsOpen(result.ticks))
                { result.reason = "The gate was closed at arrival. Adjust the timing."; frame("Gate closed", travelTicks); return result; }
                position = next; result.steps += distance;
                frame(distance > 1 ? "Jump" : level.checkpoints.Contains(position) ? "Verified" : "Move", travelTicks);
            }
            result.reason = "The route keeps looping. Change a turn or timing stamp."; frame("Time limit", 0); return result;
        }
    }
}
