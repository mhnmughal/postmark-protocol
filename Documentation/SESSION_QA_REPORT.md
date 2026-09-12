# Automated Play Mode session integration QA

UTC: 2026-09-12 10:17:43Z  
Result: **FAIL: 1 layout audit findings**  
Completed levels: 36/36  
Elapsed: 829.8 seconds

This is automated Editor Play Mode integration, **not manual playtesting**, physical touch testing, visual QA, an OS relaunch, or iOS device QA. The runner invokes authored Button listeners, SceneUI placement, live RoutePresentation animation, and the local save repository. No direct core-simulator-only result is accepted.

The user save is isolated by an Editor-only repository swap and restored at termination. Test save: `C:/Users/mhn13/AppData/Local/Temp/heromughalsoft/Postmark Protocol\SessionQA-20260912-100353-0c7cf36d\save.json`. Each persisted record is independently read from disk.

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

Layout findings: 1.
- Intro: 941x954: IntroTitle outside screen; Panel running header outside screen; GotItLabel outside screen; IntroBody outside screen; GotIt control outside screen

## Editor profiling during live delivery

Target frame rate: 60. Samples: 45726. Real-time simulation at scale 1; paused frames excluded. These measurements include Editor and QA overhead and do **not** establish iPhone/iPad performance.

| Counter | Mean | Maximum |
| --- | --- | --- |
| Frame time | 16.696 ms | 239.449 ms |
| Main Thread | 16.689 ms | 38.895 ms |
| GC Allocated In Frame | 144819.6 B | 454524 B |
| Draw Calls Count | 57.5 | 148 |
