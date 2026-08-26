# Postmark Protocol — project memory

## Project identity
- Game: Postmark Protocol. Tagline: Stamp the instructions. Send the route.
- Tactile offline 2.5D programming puzzle; iPhone/iPad first, Android-compatible architecture.
- Landscape left/right. Bundle ID `com.rehmanmughal.postmarkprotocol`.
- Namespace `RehmanMughal.PostmarkProtocol`; Unity 6000.3.19f1.
- Fresh Unity 2D URP template inspected 2026-08-26. No existing game implementation or Git repository found. No repository initialized.

## MCP setup and verification
- Existing package `com.anklebreaker.unity-mcp`, plugin 2.39.5, discovered project `I:/UnityProject/PostmarkProtocol`, port 7890.
- Explicitly selected the sole running instance. Always pass `port:7890`.
- Created `MCP_ConnectionProbe` cube, read Transform/MeshFilter/BoxCollider/MeshRenderer from Unity, deleted it, verified not found. Connectivity gate PASS.
- No duplicate connection configuration needed; installed MCP already works. Never bypass the tool queue with direct HTTP.

## Game design
- Inspect route, place physical instruction stamps, Dispatch, observe, correct, deliver.
- Seven commands: Left, Right, Wait, Reverse, Fast, Slow, Jump.
- Standard, Double, Permanent ink; Standard, Fragile, Express, Registered parcels.
- Six chapters, six unique authored levels each; progression and Delivered/Efficient/Perfect Route seals saved locally.
- Deterministic timing and known solutions. Precise implemented mechanics will be recorded after core integration.

## Technical architecture
- Planned authored scenes: Bootstrap (persistent save/audio services), MainDesk (navigation), Gameplay (data-driven routes).
- ScriptableObject LevelDefinition, pure deterministic RouteSimulator and PlanState.
- Versioned local save with recovery, original WAVs routed through Master/Music/SFX/UI mixer.
- Scene-authored uGUI with safe-area fitter; instantiate authored prefabs only for level-specific gameplay objects.
- Specialist ownership: gameplay_levels owns core/levels/validation; services owns persistence/audio; art_icon_release owns icon/release inspection; lead owns scenes/UI/integration.

## UI design system
- Full system documented in `Assets/Documentation/UI_UX_SYSTEM.md`; screen-by-screen QA in `Assets/Documentation/UI_QA_REPORT.md`. Summary below.
- Cream paper `#F0E3C2`, dark teal `#1F3735`, vermilion ink `#A33B29` (confirmed exact RGB via live component read, not estimated); warm wood desk. Primary actions are vermilion, secondary actions dark teal, destructive actions vermilion paired with a teal Cancel — never color-only, selected states also carry an Outline component.
- Single built-in `LegacyRuntime` font throughout (no TextMeshPro, no second display face); hierarchy comes from size/weight. A distinct display+body pairing was considered and deliberately deferred — real licensing/re-authoring cost, no current defect.
- Baseline spacing 8/16/24/32/48, minimum 56 reference-pixel touch targets.
- Landscape canvas reference 1600x900, `ScaleWithScreenSize` matching height (confirmed via CanvasScaler read). Safe-area margins are computed from Screen.safeArea.
- Physical envelope Start Shift, ledger, handbook, gear/tag; clipboard pause and receipt results (3 staggered postal-seal stamps, not a star score). `PaperMotion` (0.14s hover/press ease) and `PanelMotion` (0.24s slide-in) replace default Unity button/panel transitions everywhere.
- 2026-08-26 UI/UX audit: all 14 required screens PASS (`UI_QA_REPORT.md`); no generic-template anti-patterns found in the authored hierarchy. Two real text-clipping bugs found and fixed (Level Ledger entries, level 18 Lesson text) — see `QA_REPORT.md`.

## Environment design
- Slight top-down orthographic 2.5D postal desk, cream route sheet, warm directional light with restrained shadows.
- Playable area clear of stamp rack, mail stack, lamp, pencil, clips, ink pads and ledger.
- Camera/paper dimensions and final authored hierarchy pending scene construction.

## Completed work
- [x] Fresh project inspection and Unity version
- [x] AnkleBreaker connection/create/read/delete verification
- [x] Full production brief preserved locally
- [x] AI rules and initial memory bank
- [x] Core systems and 36 level assets
- [x] All screens and environment (authored MainDesk/Gameplay hierarchy verified via MCP)
- [x] Original audio assets and real mixer, original icon and all 19 iOS slots, base platform configuration
- [x] Automated QA — 66/66 EditMode tests (job `37cdc16334b7`)
- [x] Automated Play Mode integration QA — 36/36 levels PASS, 0 layout findings, 0 console errors (`SESSION_QA_REPORT.md`)
- [x] Build/export — dev + release iOS Xcode exports both succeed, 0 errors/0 warnings, icon catalog and Info.plist verified in the exported artifact (`QA_REPORT.md`, `FINAL_AUDIT.md`)
- [ ] Human manual QA on physical device, signed Xcode archive, App Store Connect submission — EXTERNAL, requires macOS/Xcode/Apple signing credentials/physical iPhone or iPad not available on this host

## Known problems / open gates
- iOS module installed; IL2CPP ARM64 Metal iOS15 landscape-only configured. Apple team/profile absent; signed archive and physical iPhone/iPad checks require macOS/Xcode, authorized credentials and devices — the sole remaining external blocker, see `FINAL_AUDIT.md`.
- 2026-08-26 QA pass found and fixed two real bugs: Fragile parcel mark buried inside the taller parcel mesh (`RoutePresentation.ResetParcel`), and Lesson/ledger-level text clipping at the design reference resolution (Best Fit enabled on `Lesson` and `LevelLabel_0..5`). Also hardened `SessionQA`/`VisualQA` against Game View resolution drift during unattended automation, which had produced false "outside screen" audit findings. All reverified clean; see `QA_REPORT.md`.
- No other known problems open. Every non-external completion-gate item in `PRODUCTION_BRIEF.md` §88 has direct observed evidence as of this date.

## Integration checkpoint
- Runtime assembly `PostmarkProtocol.Runtime`: `LevelDefinition`, `PlanState`, `RouteSimulator`, `SaveService`, `AudioService`, `PostalApplication`, `SceneUI`, `RoutePresentation`, `RouteCell`, `SafeArea`, `StampDrag`, `PaperMotion`, `PanelMotion`.
- Authored scene recipe `Assets/Editor/SceneAuthoring.cs`, executed via AnkleBreaker in Edit Mode, saves Bootstrap/MainDesk/Gameplay and gameplay prefabs. No UI hierarchy construction at runtime.
- `LevelAuthoring` creates 36 explicit data assets. Exact path-guided search verified minimum par stamps and optimal route steps on every level (3151 reachable configurations), 900 command mutations deterministic.
- Unity EditMode job eb3d65db2e56: 62/62 PASS after correcting first tutorial, timing shortcuts and WAIT duplication.
- Real Master/Music/SFX/UI audio mixer, 18 original SFX + 24-second original loop, native iOS light haptic bridge. Playback/device QA pending.
- UI visual pass found main text over paper contrast issue; recipe removes unused main paper. Integration fixed FAST timing clamp, duplicate hop, stamped checkpoint badges, gap connectors, ink restriction labels, stale route clock, eraser text, reset-progress scene escape.
- Final built-in forward scene camera: orthographic 5.45, position (0,13,-8), target (0,0,.1). Route root (-1.35,.13,.3), 1.03 cell spacing, 8x6 max grid. Route paper 9.6x7.35; warm light, separate night wood.
- 2026-08-26 final QA pass: EditMode job `37cdc16334b7` 66/66 PASS. New `Assets/Editor/SessionQA.cs` Play Mode integration harness drives the real authored UI through all 36 levels (dispatch, pause/resume, restart/review, replay, seals, isolated-file persistence, screenshots, layout audit) — clean PASS after fixing Fragile parcel mark occlusion and Lesson/ledger text clipping (see `QA_REPORT.md`). Dev and release iOS Xcode exports both succeed with 0 errors/0 warnings (`Builds/iOS`, `Builds/iOS-Release`), icon catalog and Info.plist verified directly in the exported artifact. Remaining gate is external: signed archive/physical device/App Store Connect need macOS, Xcode and Apple signing credentials this host does not have.

## Important decisions
- Original work only; no template game framework or external runtime services.
- All Unity authoring occurs through AnkleBreaker MCP in Edit Mode, saved into scene/prefab assets; runtime code only controls authored objects and level-specific instances.
- Parallel specialists are explicitly requested by the production brief. Source ownership is isolated; lead integrates and verifies.
