# Automated Play Mode session integration QA

UTC: 2026-08-26 13:29:41Z  
Result: **FAIL: 3 layout audit findings**  
Completed levels: 36/36  
Elapsed: 832.0 seconds

This is automated Editor Play Mode integration, **not manual playtesting**, physical touch testing, visual QA, an OS relaunch, or iOS device QA. The runner invokes authored Button listeners, SceneUI placement, live RoutePresentation animation, and the local save repository. No direct core-simulator-only result is accepted.

The user save is isolated by an Editor-only repository swap and restored at termination. Test save: `C:/Users/mhn13/AppData/Local/Temp/RehmanMughal/Postmark Protocol\SessionQA-20260826-131549-7d4882cc\save.json`. Each persisted record is independently read from disk.

| Level | Empty-plan return | Restart/review | Known solution | Pause/resume | Replay | Seals/file save | Next/menu/ledger |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 01 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 02 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 03 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 04 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 05 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 06 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 07 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 08 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 09 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 10 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 11 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 12 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 13 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 14 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 15 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 16 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 17 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 18 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 19 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 20 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 21 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 22 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 23 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 24 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 25 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 26 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 27 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 28 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 29 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 30 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 31 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 32 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 33 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 34 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 35 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |
| 36 | PASS | PASS | PASS | PASS | PASS | PASS | PASS |

Successful full completion additionally checks settings controls and file persistence, handbook, initial locks, actual live mixer mute/half-volume response before and after transitions, and confirmed progress reset retaining settings.

## Screenshots and layout

Planning and delivered screenshots are saved under `QA/Playthrough/level-XX-planning.png` and `level-XX-delivered.png`. Active text extents and screen bounds are audited on menus and every level's planning, pause, returned and delivered screens. Full audit output is in `QA/LayoutAudit.txt`.

Layout findings: 3.
- Level 1 delivered: 1278x1044: Panel running header outside screen
- Level 2 planning: 1278x1044: Stamp_SlowLabel outside screen; DispatchLabel outside screen; Stamp_WaitLabel outside screen; TrayTitle outside screen; EraseLabel outside screen; Stamp_LeftLabel outside screen; Stamp_FastLabel outside screen; Stamp_JumpLabel outside screen; Stamp_RightLabel outside screen; PauseLabel outside screen; Stamp_ReverseLabel outside screen
- Level 34 delivered: 1278x1044: Panel running header outside screen

## Editor profiling during live delivery

Target frame rate: 60. Samples: 45673. Real-time simulation at scale 1; paused frames excluded. These measurements include Editor and QA overhead and do **not** establish iPhone/iPad performance.

| Counter | Mean | Maximum |
| --- | --- | --- |
| Frame time | 16.707 ms | 97.686 ms |
| Main Thread | 16.700 ms | 76.270 ms |
| GC Allocated In Frame | 164182.4 B | 1088745 B |
| Draw Calls Count | 56.4 | 150 |
