# UI QA report

> Latest Settings removal: see [Settings-Removal-Verification.md](I:/UnityProject/PostmarkProtocol/QA/Settings-Removal-Verification.md). Earlier zero-finding sweep claims below predate this follow-up: its broad run retained two unexpected-resolution warnings, with fixed-size targeted rechecks passing. Privacy/support in-app access was removed by request and remains unresolved.

> **Current UI verification, 2026-08-28:** The new [Policy-UI-Verification.md](I:/UnityProject/PostmarkProtocol/QA/Policy-UI-Verification.md) passes with zero layout findings at 1136x640, 2778x1284, 2732x2048 and 1278x1044, covering all 36 planning layouts and all authored panels. Main UI targets measured at least 47.3 simulated logical points on the smallest phone case. Selected fresh screenshots were visually inspected. Earlier assertions below are historical; device touch/safe-area/accessibility behavior is not certified. See [remediation details](I:/UnityProject/PostmarkProtocol/Documentation/POLICY_REMEDIATION.md).

Date: 2026-08-26. Evidence sources: the automated Play Mode integration session (`SESSION_QA_REPORT.md`, all 36 levels, 0 layout findings at the real 1600×900 design resolution), live hierarchy/RectTransform/TextMesh inspection via AnkleBreaker MCP, and direct screenshot review (`QA/Screens/`, `QA/Playthrough/`). "Buttons pressed" means `SessionQA.cs` invoked the real `Button.onClick` through the authored UI, not a simulated tap.

| Screen | Visual alignment | Text alignment | Clipping | Responsiveness | Safe area | Buttons functional | Navigation | Result |
|---|---|---|---|---|---|---|---|---|
| Main Desk | PASS | PASS | PASS | PASS at 1600×900 (design ref); not yet device-verified at notch/Dynamic Island extremes | PASS (under `SafeArea`) | PASS — Start Shift, Ledger, Handbook, Settings all invoked | PASS | **PASS** |
| Chapter Selection | PASS | PASS | PASS | PASS | PASS | PASS — Chapter_0..5, Back all invoked | PASS | **PASS** |
| Level Selection / Ledger | PASS (fixed this session — see below) | PASS (fixed this session) | PASS | PASS | PASS | PASS — Level_0..5, Back invoked | PASS | **PASS** |
| Gameplay HUD | PASS | PASS | PASS | PASS | PASS | PASS — Pause invoked every level | PASS | **PASS** |
| Stamp Command Tray | PASS | PASS | PASS | PASS | PASS | PASS — every command + Ink + Erase exercised via known-solution placement across all 36 levels | PASS | **PASS** |
| Dispatch Control | PASS | PASS | PASS | PASS | PASS | PASS — invoked every level, both empty-plan and known-solution paths | PASS | **PASS** |
| Tutorial (contextual) | PASS | PASS | PASS | PASS | PASS | N/A (no dedicated buttons; dismisses on action) | PASS | **PASS** |
| Pause | PASS | PASS | PASS | PASS | PASS | PASS — Resume, Restart invoked every level; Handbook/Settings/Desk reachable from pause | PASS | **PASS** |
| Success (Delivered) | PASS | PASS | PASS | PASS | PASS | PASS — Next, Review, Desk invoked | PASS | **PASS**, with one documented timing note (see below) |
| Failure (Returned) | PASS | PASS | PASS | PASS | PASS | PASS — Try Again (Review), Desk invoked every level via empty-plan test | PASS | **PASS** |
| Settings | PASS | PASS | PASS | PASS | PASS | PASS — Music/Sound sliders, Haptics toggle, Reset Progress, Close all invoked and value-verified on disk | PASS | **PASS** |
| Handbook | PASS | PASS | PASS | PASS | PASS | PASS — Close invoked | PASS | **PASS** |
| Dialogs (Reset confirmation) | PASS | PASS | PASS | PASS | PASS | PASS — Cancel and confirmed-Reset both invoked and disk-verified | PASS | **PASS** |
| First-launch Intro (new this session) | PASS | PASS | PASS | PASS | PASS | PASS — GotIt invoked, dismisses to Main Menu and marks the "intro" key seen (verified on disk) | PASS | **PASS** |

## Fixed this session

Two genuine text-overflow bugs were found and fixed (not artifacts — reproduced at the real 1600×900 design resolution both before and after the fix):

1. **Level Ledger entries clipped.** All six `LevelLabel_0..5` texts needed 47px in a 43px box. Fixed with Best Fit (min 13, max 21).
2. **Level 18 Lesson text clipped.** Needed 50px in a 44px box. Fixed with Best Fit (min 14, max 22).

Both reverified clean with a full 36-level automated pass afterward (0 layout findings). Full root-cause writeup, including a false-positive found and fixed in the audit tool itself, is in `QA_REPORT.md`.

## Investigated and cleared (not bugs)

- A stray "START" label and a duplicate "POST" label were visible in three screenshots left in `QA/Screens/` from earlier in development. Live inspection (dumping every active `TextMesh` in the Gameplay scene) confirmed the **current** code renders exactly one legitimate "POST" mailbox insignia and no "START" text at all — the screenshots were stale, from before a fix already recorded in `ARCHITECTURE.md`'s changelog ("duplicate hop", "stamped checkpoint badges"). The stale screenshots were deleted this session; the surviving `QA/Screens/` files were re-confirmed live.
- The Success screen's postal-seal row can appear briefly empty in a screenshot taken mid-animation: seals stamp in one at a time (0.19s each, staggered, with sound) rather than all at once, by design. `SessionQA`'s own on-disk assertions confirm the seal *data* is always correct at that point; this is a capture-timing observation about the QA harness, not a rendering defect. See `UI_UX_SYSTEM.md`.

## Not covered by this pass (honest gaps)

- Physical device testing (notch/Dynamic Island exact margins, touch-target feel, thermal/frame-time) — no iPhone/iPad attached to this host.
- A true "generic UI" gut-check was done via direct screenshot review rather than side-by-side competitive benchmarking; reviewer judgment is that the postal-desk identity is unmistakable and none of the anti-patterns in the brief (blue panels, giant Play button, star ratings, neon) are present anywhere in the authored hierarchy.

## Updated this session (post-launch changes)

- **Credits panel and its opening buttons removed** (both `MainDesk` and `Gameplay` scenes) per explicit request — no in-game credits/attribution screen ships. `ResetProgress` was recentered in the Settings panel row it used to share with the Credits button, rather than leaving a lopsided single-button row. Dead `Act()` switch cases (`"Credits"`, `"BackSettings"`) removed from `SceneUI.cs`.
- **First-launch Intro overlay added**: a short, non-multi-paragraph welcome panel ("WELCOME TO THE DESK") shown once before Main Menu on a save that hasn't seen it, dismissed by a single "GOT IT" button. Built by duplicating the existing `ResetDialog` panel (guarantees identical component/structure fidelity to every other authored panel) and reusing the existing `tutorialSeen` persistence mechanism with a new `"intro"` key — no save-schema change. Verified live: correct panel shown on a genuinely fresh save, correct dismissal and persistence, clean layout audit, and exercised end-to-end by `SessionQA` across the full 36-level automated run.
- Both changes reverified with a full 36-level `SessionQA` pass after each: 36/36 functional completion, no navigation breakage, no console errors.

## Verdict

Every listed screen category is PASS. No dead buttons, no placeholder text, no runtime-only primary UI generation (all screens are authored `MainDesk.unity`/`Gameplay.unity` hierarchy, confirmed via direct scene dumps). No further UI redesign is warranted — the existing system already implements this brief's visual language, button hierarchy, panel structure, and interaction principles; the two real defects found were fixed and reverified, not designed around.
