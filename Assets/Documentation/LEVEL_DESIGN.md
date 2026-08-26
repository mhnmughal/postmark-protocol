# Route design and deterministic rules

The 36 individual `Assets/Data/Levels/Route_01.asset` through `Route_36.asset` ScriptableObjects are the shipping content. `LevelAuthoring` contains their explicit authored polylines and constraints so the content can be recreated without hand-editing serialized YAML. No seeds, rotated copies, random level generator, or runtime switch statement selects layouts.

## Clock and command semantics

One ordinary cell traversal takes two ticks. A tick is presented as 0.24 seconds; changing presentation speed never changes the simulation. LEFT and RIGHT turn 90 degrees, REVERSE turns 180 degrees, and turning does not advance the clock. WAIT pauses one tick before leaving the slot. FAST makes the next traversal one tick; SLOW makes it three. The next slot starts with ordinary speed again.

DOUBLE ink applies the same command twice: LEFT/RIGHT become 180 degrees, REVERSE becomes 360 degrees, WAIT pauses two ticks, SLOW takes four ticks, FAST remains clamped to one tick, and JUMP moves four cells instead of two. A jump must cross a marked gap or bump and land on printed route. It cannot pass through an unmarked missing cell or a closed gate. Skipped checkpoints do not count as verified.

Stamps remain printed throughout a dispatch and execute on every arrival. A waiting parcel does not count as re-entering its own slot. This allows a left-turn junction to send a parcel north on the outward journey and south on its return. PERMANENT ink changes editing only: it cannot be replaced or erased until the player resets the sheet. Neither preview nor failed dispatch consumes or unlocks permanent stamps.

Gate entry uses the arrival tick: a gate is open when `(arrivalTick - openPhase) mod period < openDuration`. Every run starts at tick zero. FRAGILE fails on touching a bump; EXPRESS rejects WAIT during both placement and dispatch validation; REGISTERED must touch every listed checkpoint before entering the destination. Other parcel types do not require checkpoints.

## Chapters

| Chapter | Sheets | Teaching sequence |
|---|---|---|
| Local Route | 01–06 | One guided right turn and dispatch, left, two right turns, consecutive corners, reading heading, replacement and erasure |
| Express Desk | 07–12 | Wait, fast, slow, two appointments, variable speed, three gate synchronization |
| Return Office | 13–18 | Reverse, return branches, revisited cells, longer returns, repeated speed, a two-pass turn junction |
| Ink Lab | 19–24 | Double left, double right, double wait, permanent editing locks, mixed inks, double slow |
| Priority Service | 25–30 | Fragile jumping, express timing, registered checkpoints, gaps plus bumps, express returns, mandatory verification loop |
| Night Shift | 31–36 | Fragile permanent bridge, double-ink express return, four signatures, double jump, double wait loop, combined final delivery |

## Rewards and budgets

Delivered means the simulation reaches the destination with the parcel requirements met. Efficient means the placed stamp count is at or below the route's `par`. Perfect Route uses `optimalSteps`, the minimum legal route distance. Every known solution meets both targets. The editor-only `LevelOptimality` audit exhaustively branches on every reached, undecided slot, including leaving it empty, and tests every available command/ink within the authored stock, total budget and time limit. It removes only observationally identical or dominated dispatch choices (standard versus permanent ink, a double reverse versus no stamp, and clamped double FAST versus FAST). All 36 par counts and route-distance targets were proven minimal after testing 3,485 reachable partial configurations in the latest content revision, with no search hitting its safety cap.

The first twelve sheets allow one spare stamp. Sheets 07, 12, 14, 20, 25, 26, 30, 31, 33 and 35 include an optional slot and enough budget/stock for an extra legal timing stamp: a player can deliver without earning Efficient. Other later sheets retain tight total budgets, including the finale. Sheet 13 offers near/far reversal slots with stock for only one REVERSE: the far return delivers efficiently in 9 steps, but the near return earns Perfect Route in 5. This separates all three seal outcomes without changing scoring. Every sheet exposes a brief contextual lesson and requires stamping. Sheet 01 has exactly one marked slot and teaches a RIGHT turn before dispatch.

## Verification

`Postmark Protocol > Author 36 Route Sheets` rebuilds the content then runs the validation suite. `Postmark Protocol > Validate All Routes and Rules` validates without altering the assets. Validation checks asset count, unique IDs and layouts, geometry, slots, gates, checkpoints, par, budgets, deterministic repeated known-solution frames, negative empty plans, and focused rule cases. Results are written to `LEVEL_VALIDATION.txt` only after all checks pass.

The EditMode NUnit suite independently executes each of the 36 known solutions and tests permanent reset, express WAIT, fragile jump, verification-loop bypass, and gate timing. Simulation and automated UI dispatch are not substitutes for the requested human manual playtest or physical-device QA; those must be tracked separately in the project audit.

Verified in Unity on 2026-08-26: EditMode job `2bd3ea3e78bd` passed all 41 gameplay tests, with zero failures and zero skipped tests. The editor validation additionally passed 31 focused rule checks and 35 empty-plan negative cases. Validation also removes each individual solution stamp in turn to ensure it serves a necessary purpose.

After tightening return gate windows on sheets 17, 29 and 32, editor validation passed again, including every individual solution-stamp removal. Combined regression job `a10d595d7739` passed all 56 gameplay and services tests (41 gameplay, 15 services), with zero failures or skipped tests. Unity's compilation pipeline then reported zero errors and zero warnings.

The later exact audit found early-REVERSE shortcuts on sheets 17, 29 and 32 despite individual stamp necessity. These were fixed with a timed return-exit gate. Sheet 18 similarly uses its exit gate to require the full loop. `LEVEL_OPTIMALITY.txt` records the resulting proof for all 36 sheets. The same audit replayed 900 single-command mutations twice each: 885 failed safely and 15 were legitimate equivalent or alternate solutions; all outcomes were deterministic. The first sheet was updated to the required single RIGHT-turn tutorial, and all 36 empty plans now fail.

Final regression after those fixes: Unity EditMode job `eb3d65db2e56` passed 62/62 tests (47 gameplay, 15 services), zero failures or skips. New regression cases cover the single-RIGHT tutorial, the three early-reverse shortcuts, and exact WAIT animation-frame counts.

Latest optional-slot/reward revision: Unity EditMode job `e67520da5874` passed 66/66 tests (51 gameplay, 15 services), zero failures/skips. Three cases verify a legal extra stamp awards Delivered without Efficient; the level13 far-return case verifies Efficient without Perfect. Exact target audit now explores 3,485 configurations with zero mismatches; 900 mutations remain deterministic, including 16 legitimate alternatives.

## Individual shipping sheets

Coordinates are zero-based `(x,y)`; north increases `y`. Each solution lists printed placements, not a temporal command queue: persistent stamps execute again when revisited. The objective is safe delivery to the listed destination, including every verification for Registered mail. Available stock is additionally constrained by each asset's commandLimits and stampLimit.

### 01 — First Class

- Objective: deliver to {x: 4, y: 2}; parcel **Standard**. Par **1** stamps; optimal route **5** cells.
- Available commands: Left, Right.
- Mechanics and learning: Choose RIGHT. Stamp the single marked corner, then press DISPATCH.
- Intended solution: (4,4) Right/Standard.

### 02 — Around the Corner

- Objective: deliver to {x: 4, y: 4}; parcel **Standard**. Par **1** stamps; optimal route **6** cells.
- Available commands: Left, Right.
- Mechanics and learning: Choose LEFT, then tap the dotted slot. Dispatch when ready.
- Intended solution: (4,1) Left/Standard.

### 03 — Right Address

- Objective: deliver to {x: 2, y: 1}; parcel **Standard**. Par **2** stamps; optimal route **10** cells.
- Available commands: Left, Right.
- Mechanics and learning: Two RIGHT turns guide the letter around the block.
- Intended solution: (5,4) Right/Standard; (5,1) Right/Standard.

### 04 — Two Corners

- Objective: deliver to {x: 6, y: 4}; parcel **Standard**. Par **2** stamps; optimal route **9** cells.
- Available commands: Left, Right.
- Mechanics and learning: Stamp both corners. Tap a stamped slot to replace it.
- Intended solution: (3,1) Left/Standard; (3,4) Right/Standard.

### 05 — The Courtyard

- Objective: deliver to {x: 3, y: 1}; parcel **Standard**. Par **3** stamps; optimal route **15** cells.
- Available commands: Left, Right.
- Mechanics and learning: Read each turn from the letter's direction, not your own.
- Intended solution: (1,4) Right/Standard; (6,4) Right/Standard; (6,1) Right/Standard.

### 06 — Sorting Circuit

- Objective: deliver to {x: 6, y: 2}; parcel **Standard**. Par **5** stamps; optimal route **16** cells.
- Available commands: Left, Right.
- Mechanics and learning: Plan the whole route. ERASE removes standard ink; RESET clears the sheet.
- Intended solution: (3,0) Left/Standard; (3,2) Left/Standard; (1,2) Right/Standard; (1,4) Right/Standard; (6,4) Right/Standard.

### 07 — One Beat Later

- Objective: deliver to {x: 6, y: 4}; parcel **Standard**. Par **3** stamps; optimal route **8** cells.
- Available commands: Left, Right, Wait, Fast, Slow.
- Mechanics and learning: WAIT holds for one tick. The gate's numbers show its open phase.
- Intended solution: (4,2) Left/Standard; (4,4) Right/Standard; (1,2) Wait/Standard.

### 08 — Quick Collection

- Objective: deliver to {x: 5, y: 1}; parcel **Standard**. Par **3** stamps; optimal route **9** cells.
- Available commands: Left, Right, Wait, Fast, Slow.
- Mechanics and learning: FAST makes the next step take one tick instead of two.
- Intended solution: (1,3) Right/Standard; (5,3) Right/Standard; (1,1) Fast/Standard.

### 09 — Handle with Time

- Objective: deliver to {x: 6, y: 3}; parcel **Standard**. Par **4** stamps; optimal route **11** cells.
- Available commands: Left, Right, Wait, Fast, Slow.
- Mechanics and learning: SLOW makes the next step take three ticks.
- Intended solution: (3,4) Right/Standard; (3,1) Left/Standard; (6,1) Left/Standard; (1,4) Slow/Standard.

### 10 — Two Appointments

- Objective: deliver to {x: 5, y: 0}; parcel **Standard**. Par **5** stamps; optimal route **12** cells.
- Available commands: Left, Right, Wait, Fast, Slow.
- Mechanics and learning: Each WAIT adds one tick. Time both crossings.
- Intended solution: (2,1) Left/Standard; (2,4) Right/Standard; (5,4) Right/Standard; (1,1) Wait/Standard; (3,4) Wait/Standard.

### 11 — Variable Service

- Objective: deliver to {x: 7, y: 4}; parcel **Standard**. Par **5** stamps; optimal route **12** cells.
- Available commands: Left, Right, Wait, Fast, Slow.
- Mechanics and learning: FAST and SLOW affect one segment. Look ahead to the next gate.
- Intended solution: (1,1) Left/Standard; (4,1) Left/Standard; (4,4) Right/Standard; (1,3) Fast/Standard; (2,1) Slow/Standard.

### 12 — Closing Time

- Objective: deliver to {x: 2, y: 5}; parcel **Standard**. Par **7** stamps; optimal route **15** cells.
- Available commands: Left, Right, Wait, Fast, Slow.
- Mechanics and learning: Three gate schedules, one route. Stamp timing between the turns.
- Intended solution: (3,0) Left/Standard; (3,3) Right/Standard; (6,3) Left/Standard; (6,5) Left/Standard; (1,0) Wait/Standard; (3,1) Fast/Standard; (4,3) Slow/Standard.

### 13 — Return to Sender

- Objective: deliver to {x: 0, y: 2}; parcel **Standard**. Par **1** stamps; optimal route **5** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: REVERSE turns the letter around. A shorter return earns Perfect Route. The optional far slot at (6,2) produces a valid 9-cell route with only the Efficient seal.
- Intended solution: (4,2) Reverse/Standard.

### 14 — Wrong-Way Sack

- Objective: deliver to {x: 6, y: 4}; parcel **Standard**. Par **3** stamps; optimal route **13** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: Turn around at the end, then take the upper branch.
- Intended solution: (6,1) Reverse/Standard; (2,1) Right/Standard; (2,4) Right/Standard.

### 15 — Back Through Sorting

- Objective: deliver to {x: 4, y: 4}; parcel **Standard**. Par **4** stamps; optimal route **15** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: A return journey crosses the same cells in the opposite direction.
- Intended solution: (2,5) Reverse/Standard; (2,1) Left/Standard; (6,1) Left/Standard; (6,4) Left/Standard.

### 16 — Countercurrent

- Objective: deliver to {x: 6, y: 0}; parcel **Standard**. Par **5** stamps; optimal route **18** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: Follow the arrow through the return before planning the staircase.
- Intended solution: (7,4) Reverse/Standard; (1,4) Left/Standard; (1,2) Left/Standard; (3,2) Right/Standard; (3,0) Left/Standard.

### 17 — Return Window

- Objective: deliver to {x: 4, y: 4}; parcel **Standard**. Par **4** stamps; optimal route **14** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: FAST triggers on both visits. The exit gate opens after the complete return.
- Intended solution: (6,1) Reverse/Standard; (1,1) Right/Standard; (1,4) Right/Standard; (4,1) Fast/Standard.

### 18 — Round Trip

- Objective: deliver to {x: 6, y: 0}; parcel **Standard**. Par **6** stamps; optimal route **19** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: The same LEFT leads north, then south. Complete the loop to reach the gate on time.
- Intended solution: (0,2) Reverse/Standard; (2,2) Left/Standard; (2,4) Right/Standard; (5,4) Right/Standard; (5,2) Right/Standard; (2,0) Left/Standard.

### 19 — Double Impression

- Objective: deliver to {x: 5, y: 0}; parcel **Standard**. Par **3** stamps; optimal route **16** cells.
- Available commands: Left, Right.
- Mechanics and learning: DOUBLE ink executes twice. Two LEFT turns make a REVERSE.
- Intended solution: (7,3) Left/Double; (1,3) Left/Standard; (1,0) Left/Standard.

### 20 — A Second Right

- Objective: deliver to {x: 5, y: 2}; parcel **Standard**. Par **4** stamps; optimal route **14** cells.
- Available commands: Left, Right, Wait.
- Mechanics and learning: Two RIGHT turns also reverse. Inspect the ink border on each slot.
- Intended solution: (3,0) Right/Double; (3,5) Right/Standard; (6,5) Right/Standard; (6,2) Right/Standard.

### 21 — Drying Time

- Objective: deliver to {x: 7, y: 4}; parcel **Standard**. Par **4** stamps; optimal route **14** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: DOUBLE WAIT pauses for two ticks before moving.
- Intended solution: (4,3) Right/Standard; (4,0) Left/Standard; (7,0) Left/Standard; (1,3) Wait/Double.

### 22 — Indelible Address

- Objective: deliver to {x: 7, y: 2}; parcel **Standard**. Par **4** stamps; optimal route **13** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: PERMANENT ink locks after placement. RESET is the only way to erase it.
- Intended solution: (2,0) Left/Permanent; (2,4) Right/Standard; (4,4) Right/Standard; (4,2) Left/Standard.

### 23 — Carbon Return

- Objective: deliver to {x: 5, y: 1}; parcel **Standard**. Par **4** stamps; optimal route **13** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: Mix a double turn, permanent corner and timed slowdown.
- Intended solution: (7,4) Left/Double; (2,4) Left/Permanent; (2,1) Left/Standard; (2,3) Slow/Standard.

### 24 — Ink Inspector

- Objective: deliver to {x: 2, y: 3}; parcel **Standard**. Par **5** stamps; optimal route **14** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow.
- Mechanics and learning: DOUBLE SLOW takes four ticks. Permanent turns demand a committed plan.
- Intended solution: (4,1) Left/Standard; (4,5) Left/Permanent; (1,5) Left/Standard; (1,3) Left/Standard; (1,1) Slow/Double.

### 25 — Handle with Care

- Objective: deliver to {x: 2, y: 5}; parcel **Fragile**. Par **3** stamps; optimal route **11** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: FRAGILE parcels cannot touch bumps. JUMP from the preceding slot.
- Intended solution: (5,2) Left/Standard; (5,5) Left/Standard; (1,2) Jump/Standard.

### 26 — No Waiting

- Objective: deliver to {x: 6, y: 0}; parcel **Express**. Par **4** stamps; optimal route **11** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: EXPRESS mail cannot WAIT. Use speed to meet the gate.
- Intended solution: (3,5) Right/Standard; (3,2) Left/Standard; (6,2) Right/Standard; (1,5) Fast/Standard.

### 27 — Signed For

- Objective: deliver to {x: 4, y: 1}; parcel **Registered**. Par **4** stamps; optimal route **14** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: REGISTERED mail must visit every numbered checkpoint before delivery.
- Intended solution: (2,1) Left/Standard; (2,4) Right/Standard; (6,4) Right/Standard; (6,1) Right/Standard.

### 28 — Bridge Repair

- Objective: deliver to {x: 1, y: 2}; parcel **Fragile**. Par **6** stamps; optimal route **15** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: JUMP clears a marked gap. Land on the printed route.
- Intended solution: (5,0) Left/Standard; (5,4) Left/Standard; (3,4) Left/Standard; (3,2) Right/Standard; (1,0) Jump/Standard; (5,1) Jump/Standard.

### 29 — Express Return

- Objective: deliver to {x: 0, y: 3}; parcel **Express**. Par **5** stamps; optimal route **15** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: Complete the return before the exit gate opens. Express mail never waits.
- Intended solution: (7,2) Reverse/Standard; (2,2) Right/Standard; (2,5) Left/Standard; (0,5) Left/Standard; (5,2) Slow/Standard.

### 30 — The Verification Loop

- Objective: deliver to {x: 5, y: 0}; parcel **Registered**. Par **5** stamps; optimal route **20** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: Collect the outer signature before taking the exit. The junction is visited twice.
- Intended solution: (2,3) Left/Standard; (2,5) Right/Standard; (6,5) Right/Standard; (6,3) Right/Standard; (2,0) Left/Standard.

### 31 — After-Hours Crossing

- Objective: deliver to {x: 1, y: 3}; parcel **Fragile**. Par **5** stamps; optimal route **15** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: A fragile delivery, a permanent turn and a timed bridge.
- Intended solution: (5,1) Left/Standard; (5,5) Left/Permanent; (1,5) Left/Standard; (1,1) Jump/Standard; (3,1) Slow/Standard.

### 32 — Midnight Express

- Objective: deliver to {x: 5, y: 2}; parcel **Express**. Par **5** stamps; optimal route **20** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: Double ink reverses the return. Meet both gates without waiting.
- Intended solution: (7,4) Right/Double; (1,4) Left/Standard; (1,0) Left/Standard; (5,0) Left/Standard; (4,4) Fast/Standard.

### 33 — Four Signatures

- Objective: deliver to {x: 6, y: 3}; parcel **Registered**. Par **6** stamps; optimal route **21** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: A registered circuit needs all four signatures. Budget every turn.
- Intended solution: (2,2) Left/Permanent; (2,5) Right/Standard; (7,5) Right/Standard; (7,1) Right/Standard; (4,1) Right/Standard; (4,3) Right/Standard.

### 34 — Long Leap

- Objective: deliver to {x: 4, y: 2}; parcel **Fragile**. Par **6** stamps; optimal route **18** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: DOUBLE JUMP advances four cells across the marked obstacle run.
- Intended solution: (6,4) Right/Standard; (6,0) Right/Standard; (2,0) Right/Standard; (2,2) Right/Standard; (1,4) Jump/Double; (6,3) Slow/Standard.

### 35 — Final Collection

- Objective: deliver to {x: 7, y: 0}; parcel **Registered**. Par **6** stamps; optimal route **21** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: A double pause opens the first gate. Do not miss the return-loop signature.
- Intended solution: (3,2) Left/Permanent; (3,5) Right/Standard; (6,5) Right/Standard; (6,2) Right/Standard; (3,0) Left/Standard; (1,2) Wait/Double.

### 36 — Postmark Protocol

- Objective: deliver to {x: 6, y: 2}; parcel **Registered**. Par **7** stamps; optimal route **17** cells.
- Available commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Mechanics and learning: One final sheet: double return, two signatures, a bridge, and exact gate timing.
- Intended solution: (0,1) Right/Double; (4,1) Left/Permanent; (4,5) Right/Standard; (7,5) Right/Standard; (7,2) Right/Standard; (4,2) Jump/Standard; (5,5) Slow/Double.
