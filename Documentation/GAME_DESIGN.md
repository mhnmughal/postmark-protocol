# Postmark Protocol — implemented game design

**Stamp the instructions. Send the route.** An offline, landscape postal programming puzzle for iPhone and iPad. The player programs an envelope's route; the envelope is never directly steered. The desk uses warm walnut, ivory paper, teal print and vermilion ink, with a cool night treatment for the final chapter.

## Player loop

Inspect the printed route and parcel restrictions. Select a command and ink, then tap a marked slot or drag the stamp onto it. Replace or erase ordinary ink while planning. Dispatch runs a deterministic delivery. Observe the route, review a returned parcel, correct the plan and try again. Successful delivery records progress and opens the next route. There are no timers requiring fast reactions, accounts, purchases, advertisements, online features or alternate hidden modes.

## Commands and the clock

| Command | Effect |
|---|---|
| LEFT | Turn 90 degrees counterclockwise, then move |
| RIGHT | Turn 90 degrees clockwise, then move |
| WAIT | Stay for one tick, then make an ordinary move |
| REVERSE | Turn 180 degrees, then move |
| FAST | Next movement takes one tick instead of two |
| SLOW | Next movement takes three ticks instead of two |
| JUMP | Move two cells over a marked gap/bump and land on the printed route |

North increases the grid's y coordinate. Every run starts at tick zero; a normal movement takes two ticks. Turning consumes no simulation tick. Speed changes apply only to the next movement. Gate labels give open phase/period; entry is allowed when `(arrivalTick - phase) mod period < openDuration`. Display animation is presentation only.

Stamps are persistent instructions attached to cells, not a sequence consumed from a list. They execute again each time the parcel re-enters that cell. A WAIT pauses once per execution and cannot repeatedly trigger itself while the parcel stands still. All runs have a finite tick bound.

## Ink

Standard ink executes once. Double ink executes the same command twice: two quarter-turns reverse heading, two reverses restore heading, WAIT lasts two ticks, SLOW makes a four-tick move, FAST remains at its one-tick minimum, and JUMP advances four cells. Jumps must cross designated obstacles; they cannot cross an unmarked missing cell or a closed gate.

Permanent ink executes once and locks that placement against replacement/erasure. Reset clears the entire sheet, including permanent stamps; a failed dispatch does not clear it. Restricted slots show a double or lock marker, and labels/symbols communicate ink rules in addition to color.

## Parcels

| Parcel | Rule |
|---|---|
| Standard letter | Ordinary routing; may touch bumps |
| Fragile parcel | Touching a marked bump fails; jump over it |
| Express mail | WAIT is illegal during planning and dispatch validation |
| Registered mail | Touch every verification checkpoint before entering the destination; jumping over one does not verify it |

Parcel labels and distinct marks accompany these behaviors. The actual content is one parcel per sheet, with no network or multiplayer dependency.

## Campaign

Six chapters contain six explicit authored route sheets each: Local Route (01–06), Express Desk (07–12), Return Office (13–18), Ink Lab (19–24), Priority Service (25–30) and Night Shift (31–36). The campaign introduces right/left turns, timing, reverse/revisits, ink, parcel restrictions, then combined final challenges. Each next level unlocks after delivery; previously unlocked levels can be replayed from the ledger.

The first sheet has one marked slot requiring RIGHT. Its short tutorial asks the player to pick up the stamp, place it and dispatch. Later lessons appear within the current route, not as a multi-page introduction. `LEVEL_DESIGN.md` contains the full 36-sheet table, stock, par, lesson and solution placements.

## Rewards and failure

Delivered is earned by legal arrival. Efficient additionally requires placed stamps at or below par. Perfect Route requires both par and the minimum route-distance target. These are postal seals, not a generic star score. Results and best counts persist locally and do not regress on replay.

Optional slots on representative sheets allow a legal extra timing stamp, producing delivery without efficiency. Sheet 13 makes route quality a separate decision: a near reversal gives the shortest route; a far reversal uses the same one-stamp par but takes four more steps, earning Efficient without Perfect Route. The final sheet retains its strict seven-stamp allowance.

Failure returns the parcel with a contextual explanation: off-route travel, closed gate, fragile damage, missed verification, invalid jump/placement or looping beyond the time limit. Try Again and Review retain the plan; Restart resets the sheet. Permanent ink restrictions are always enforced until reset.

## Navigation, feedback and settings

The authored interface includes Main Desk, chapter selection, level ledger, gameplay HUD, pause, settings, handbook, reset confirmation and success/failure panels. Mouse and touch support both tap-to-place and drag/drop without keyboard dependency. Pause stops dispatch animation; panels animate using unscaled time. The route uses moving/turning/jumping parcel feedback, a mailbox lid/delivery animation, stamp impact, a small camera impulse, local sound and optional haptics.

Music, effects and haptic controls change real systems and persist locally. Reset Progress asks for confirmation and retains those preferences. Safe-area anchors and landscape scaling support different screen shapes; physical iPhone/iPad readability, controls and performance remain explicit QA gates, not assumptions based on responsive code.

## Production boundaries

The campaign and deterministic rules are implemented and automatically tested. Art/audio origins, package/network boundaries and iOS configuration have separate evidence documents. Human manual QA, physical-device behavior, signed distribution and final export must be reported using observed results in the final audit; this design document does not certify a release.
