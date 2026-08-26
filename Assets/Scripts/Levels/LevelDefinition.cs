using System;
using UnityEngine;
using RehmanMughal.PostmarkProtocol.Gameplay;

namespace RehmanMughal.PostmarkProtocol.Levels
{
    [Serializable] public class SlotDefinition { public Vector2Int position; public Ink[] allowedInks = { Ink.Standard }; }
    [Serializable] public class CommandLimit { public Command command; public int count; }
    [Serializable] public class TimedGate
    {
        public Vector2Int position;
        public int period = 4, openPhase, openDuration = 1;
        public bool IsOpen(int tick) { return ((tick - openPhase) % period + period) % period < openDuration; }
    }
    [CreateAssetMenu(menuName = "Postmark Protocol/Level")]
    public sealed class LevelDefinition : ScriptableObject
    {
        public int id, chapter, width = 8, height = 6;
        public string title;
        [TextArea] public string lesson;
        public Vector2Int start, destination;
        public Direction startDirection = Direction.East;
        public Vector2Int[] route = Array.Empty<Vector2Int>();
        public SlotDefinition[] slots = Array.Empty<SlotDefinition>();
        public Command[] availableCommands = Array.Empty<Command>();
        public CommandLimit[] commandLimits = Array.Empty<CommandLimit>();
        public TimedGate[] gates = Array.Empty<TimedGate>();
        public Vector2Int[] hazards = Array.Empty<Vector2Int>();
        public Vector2Int[] gaps = Array.Empty<Vector2Int>();
        public Vector2Int[] checkpoints = Array.Empty<Vector2Int>();
        public ParcelType parcel;
        public int stampLimit = 8, par, optimalSteps, maxTicks = 160;
        public StampPlacement[] knownSolution = Array.Empty<StampPlacement>();
        public bool night;
        public SlotDefinition SlotAt(Vector2Int p) { return Array.Find(slots, s => s.position == p); }
        public bool Contains(Vector2Int p) { return Array.IndexOf(route, p) >= 0; }
    }
}
