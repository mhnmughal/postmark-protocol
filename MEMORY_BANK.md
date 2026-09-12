# Postmark Protocol — project memory

## App Store policy and metadata scan - 2026-09-03

- `Documentation/APP_STORE_METADATA.md` is the current US English listing draft. Recommended subtitle: `Postal Route Logic Puzzles`; primary category: Games; Games subcategories: Puzzle and Strategy; leave the optional secondary app category blank.
- The recommended promotional text is 143 characters, description is 1,716 characters, subtitle is 26 characters and keyword set is 91 ASCII bytes. Five paired subtitle/keyword options are documented, and the customer-facing metadata contains no em dash.
- Source-aligned claims are limited to the implemented 36 levels, six chapters, seven commands, three inks, four parcel rules, postal seals, handbook, local save, offline play, and current audio/haptics controls.
- `Documentation/APPLE_POLICY_AUDIT.md` is the current source and configuration scan. No hidden feature, deceptive commerce, review manipulation, tracking, advertising, account, runtime request, or source-level spam signal was found.
- ReleaseCompliance currently reports one issue: configure a real HTTPS privacy policy URL and a support URL/email. AppInformation has empty privacy/support fields, and the current app has no easily accessible in-app privacy link. These are assigned to the later App Store submission phase; the current metadata and project-policy scan are complete.
- Unity services, engine diagnostics, analytics submission and crash reporting are disabled in current settings. Recheck service flags and dependencies before completing App Store Connect privacy answers.
- Guideline 5.6 account conduct cannot be verified from the repository. The publisher must maintain truthful account identity/contact, respectful review replies, honest discovery practices and responsive support.

## Package cleanup - 2026-09-03

- Unity Package Manager reduced the project from 35 remaining direct dependencies to 9, completing the earlier cleanup from the original 53. Current direct roots: AnkleBreaker MCP, Input System, UGUI, Test Framework, Audio, JSON Serialize, Physics, Image Conversion and Screen Capture.
- NUnit, IMGUI, UI, UIElements and HierarchyCore remain only as dependency-resolved transitive packages. Do not add them back as direct manifest entries.
- Rider/Visual Studio integrations and unused Accessibility API, Adaptive Performance, AI, Android JNI, Animation API, AssetBundle, Cloth, Director, Particle System, Physics 2D, Terrain, Terrain Physics, Tilemap, Umbra, UnityWebRequest, UnityWebRequestTexture, Vector Graphics, Vehicles, VR, Wind and XR direct roots were removed through Package Manager.
- Production assets/scenes were unchanged by this cleanup. `Documentation/PACKAGE_AUDIT.md` is the current dependency rationale. Package resolution completed, AnkleBreaker remained connected, and CompilationPipeline reported 0 errors/warnings.

## Audio slider accuracy - 2026-08-28

- Music and Sound Effects now use native Unity Slider values 0-100 in whole 1% steps; save data and mixer inputs remain normalized 0-1. Older fractional settings are rounded to the displayed percent when SceneUI initializes, so label, handle and actual mixer input agree.
- Visible track, fill area and handle travel now share the same 270-unit endpoints in MainDesk, Gameplay and the authoring recipe. The previous track was 290 units while fill/handle travel was only 270. The existing 320x80 touch area remains unchanged.
- `SessionQA.BeginSliderReview()` reuses the isolated save fixture. `QA/Slider-Verification.md` passed all six scene/resolution combinations: both scenes at 1136x640, 2778x1284 and 2732x2048. Native tap/drag checks cover endpoints, intermediate percentages, clamping, labels, geometry, independent Music/SFX/UI mixer values, legacy settings and file reload. No physical touch or subjective loudness claim.
- Current Settings screenshot candidates and their hash manifest were refreshed. The user's repository was restored after QA. Regression suite: 68/68 NUnit tests passed (job 8fd6b8e50dbc), with 0 compilation errors/warnings. Privacy/Support/Credits remain removed; no iOS export/build or signing was performed.

## Current policy-review status - 2026-08-28

- The earlier user request removed Privacy Policy and Support buttons, panels and runtime external actions from both MainDesk and Gameplay Settings. Reset Progress is centered above Close Settings. No Credits UI was restored; first-launch introduction remains.
- [Policy remediation](I:/UnityProject/PostmarkProtocol/Documentation/POLICY_REMEDIATION.md) retains disabled Engine Diagnostics/automatic analytics, reset error/retry handling and responsive UI fixes. Public privacy/support URLs and contact are still missing, and accessible in-app information is unresolved again. AppInformation remains release configuration only; filling it does not restore the removed UI. No publisher identity was invented.
- Earlier remediation evidence: 68/68 NUnit tests (job 8d8a9dbaa53c); a zero-finding planning-layout sweep at four resolutions plus four representative deliveries. Latest removal checks are in QA/Settings-Removal-Verification.md: both Settings flows passed, refreshed phone/iPad images, 0 compilation errors/warnings. The follow-up broad sweep retained two unexpected-resolution bounds findings; targeted fixed-size rechecks passed, and its report remains FAIL honestly. The original SESSION_QA_REPORT is historical; its three layout findings were superseded by the new sweep, not rewritten as a new full live run.
- Fresh RGB candidate screenshots: QA/StoreCandidates, with exact iPhone/iPad dimensions and manifest. Metadata copy/review notes updated; nothing uploaded or published.
- Main UI uses 960x540 fitted to safe area, two-column commands, and an INK selector from chapter 4. Main UI controls were at least 47.3 simulated logical points on the smallest tested phone; route spaces accept nearby taps.
- Physical tests, iOS export/builds, signing and Apple account login remain excluded by the user. No final-binary/network privacy certification or account-policy clearance is claimed.

## Repository handoff - 2026-08-28

- Repository: https://github.com/mhnmughal/postmark-protocol.git; working branch `main`, upstream `origin/main`. The user explicitly requested this memory update, a commit and a push of the accumulated policy/UI work.
- Preserve the latest Settings design: music, sound effects, haptics, centered Reset Progress and Close Settings. Do not restore Privacy Policy, Support or Credits without a new user request. Both saved scenes and the SceneAuthoring recipe match this design.
- Current screenshot candidates are in `QA/StoreCandidates`, with exact dimensions and SHA-256 hashes in `manifest.json`. Older Privacy/Support screenshots in `QA/Screens` are historical evidence, not the current app UI or upload recommendations.
- ReleaseCompliance still rejects missing public privacy/support configuration. Populating AppInformation is not full submission clearance and does not restore in-app access. The access gap and excluded release checks remain open.
- Latest completed Unity checks are recorded in `QA/Settings-Removal-Verification.md`; the wider follow-up report retains its two findings. Do not convert those findings into a blanket PASS. The last verification left Unity out of Play Mode with MainDesk saved and clean.

## Project identity
- Game: Postmark Protocol. Tagline: Stamp the instructions. Send the route.
- Tactile offline 2.5D programming puzzle; iPhone/iPad first, Android-compatible architecture.
- Landscape left/right. Current iOS bundle ID `com.heromughalsoft.PostmarkProtocol`.
- Namespace `RehmanMughal.PostmarkProtocol`; Unity 6000.3.19f1.
- Fresh Unity 2D URP template inspected 2026-08-26; no existing game implementation or Git repository was present at that initial inspection. Git was subsequently configured; see the current repository handoff above.

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
- Full system documented in `Documentation/UI_UX_SYSTEM.md`; screen-by-screen QA in `Documentation/UI_QA_REPORT.md`. Summary below.
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
- [x] App Store Connect screenshot set — `SessionQA.BeginStoreScreenshots()` (menu: Postmark Protocol/QA/Capture App Store Screenshots) captures the live authored UI (Main Menu, Level 1, Level 36, Settings) at the current mandatory sizes, iPhone 6.9" (2868x1320 landscape) and iPad 13" (2752x2064 landscape), into separate `QA/AppStoreScreenshots/iPhone-6.9/` and `QA/AppStoreScreenshots/iPad-13/` folders. See `QA/AppStoreScreenshots/CAPTURE_LOG.md`.

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
- 2026-08-26, explicit user request: no developer name appears anywhere in-game (Main Desk footer text cleared; Credits panel and both its opening buttons removed from MainDesk and Gameplay scenes, plus the corresponding `SceneAuthoring.cs` authoring code, so a rebuild from the recipe won't reintroduce it). This is a deliberate deviation from `PRODUCTION_BRIEF.md` §31/§77 (which asked for a Credits screen), recorded here per AI_RULES #2 rather than silently dropped. `ResetProgress` in Settings was recentered to fill the row the removed Credits button used to share.
- 2026-08-26, explicit user request: added a first-launch "WELCOME TO THE DESK" Intro overlay shown once before Main Menu (short, two-line body — not the multi-paragraph tutorial the brief warns against), dismissed by a single GOT IT button. Reuses the existing `SaveService` tutorialSeen mechanism (new `"intro"` key), no save-schema change. Both changes reverified with full 36-level `SessionQA` passes.
- 2026-09-12, explicit user request: restored in-app Privacy Policy and Support access to Settings, which an earlier session (`50a0d75`) had removed. Added `PrivacyPolicy`/`Support` buttons and a new `PrivacyPanel` (built from `HandbookPanel`'s exact structure: Title + scrollable body + Close) to both scenes' `SettingsPanel`, wired `SceneUI.appInfo` to `Assets/Data/AppInformation.asset`. `Support` opens `mailto:` with `AppInformation.supportEmail` (now set to the address the user provided) or `supportUrl` if present; `PrivacyPolicy` shows the in-app policy text directly, so the app has real accessible privacy/support info without waiting on a public URL. `privacyPolicyUrl`/`supportUrl` remain blank until the user supplies real links — `ReleaseCompliance`'s build preprocessor will keep correctly blocking a build until then, which is its job, not a bug. Drafted paste-ready Privacy Policy and Support page copy for an external site at `Documentation/PRIVACY_POLICY_FOR_GOOGLE_SITE.md` / `SUPPORT_PAGE_FOR_GOOGLE_SITE.md`.
- 2026-09-12: found and fixed a real QA-harness bug (not a product bug) while reverifying the above — `VisualQA.LayoutAudit()` was checking element bounds against the screen while `PanelMotion`'s 0.24s slide-in animation was still in progress, so any panel audited too soon after `Show()` had its bottom-row content still below its resting position, reporting dozens of false "outside screen" findings on `Close`/`GotIt`/`BackMain`/`Desk`/`Review`-type buttons across nearly every panel (a player never sees this — it's a normal slide-in, not a stuck state). Fixed by settling all `PanelMotion` instances to rest before checking bounds, in the one shared audit function rather than patching every `Delay()` call site. Full 36-level `SessionQA` went from 78 false findings to 1 (a single already-documented Game View resolution-drift artifact at Play Mode start, reporting a bogus non-1600x900 resolution — pre-existing, unrelated, not chased further). Also hardened `VisualQA.Resolution()` to reuse an existing matching Game View size entry instead of always adding a duplicate (harmless with the real fix in place, but cheap insurance against future entry pileup on long runs).
- 2026-09-12: re-captured the App Store Connect screenshot set (`QA/AppStoreScreenshots/`) after the Settings layout change, deleting the prior set first. Same 4 screens (Main Menu, Level 1, Level 36, Settings) at iPhone 6.9" (2868x1320) and iPad 13" (2752x2064), landscape.
- 2026-09-12, explicit user request: removed `com.anklebreaker.unity-mcp` from `Packages/manifest.json` entirely, now that the AI-assisted development phase using it is finished. It was always Editor-only tooling and was already confirmed absent from every exported build (`NETWORK_AUDIT.md`), so this changes nothing about the shipped game; it drops the project to 8 direct dependencies. No `.asmdef` in the project referenced it, so no code depended on it and compilation was unaffected. Updated `AI_RULES.md` #6, #10 and #26 (which had mandated its use and still described Privacy/Support as removed, both stale after this session's earlier restoration of those Settings controls), and added dated update notes to `PACKAGE_AUDIT.md` and `NETWORK_AUDIT.md` rather than rewriting their historical findings. This was the last MCP-driven change in the project: any further Unity Editor work now requires either a human at the Editor or a different tool the user explicitly authorizes.
- 2026-09-12, same day: the package removal's `PlayerSettings` reserialization flipped `defaultScreenOrientation` from 4 (AutoRotation) to 3 (LandscapeRight, fixed). Initially treated this as an unintended regression and reverted it; the revert didn't hold because the still-running Editor kept re-flushing its own in-memory value back over the file. Asked the user directly which behavior they wanted, and they confirmed **3 (fixed LandscapeRight, no rotation at all, not even between the two landscape directions) is the deliberate, wanted setting**, not a bug. Recorded here per AI_RULES #2 as a deviation from the originally documented "autorotation, landscape left/right only" design (`IOS_RELEASE_CHECKLIST.md`, `PRODUCTION_BRIEF.md`'s landscape intent). No file change needed since 3 is already what's on disk. The `allowedAutorotateToPortrait/PortraitUpsideDown/LandscapeLeft/LandscapeRight` flags are now vestigial (iOS ignores them once orientation is fixed rather than AutoRotation) and were left as-is since Unity does not use them in this mode. If this ever needs revisiting, it requires a human at the Editor (Project Settings > Player > Resolution and Presentation > Orientation) since AnkleBreaker is gone and a direct file edit alone does not hold while Unity is running.
- 2026-09-12, explicit user request: `AppInformation.asset` now has real `privacyPolicyUrl` (`https://sites.google.com/view/privacypolicyforpostmarkprotoc/home`) and `supportUrl` (`https://sites.google.com/view/postmark-protocol-suppor/home`) values, both confirmed live and matching the previously drafted copy via a fetch check. Settings' Support button will now open `supportUrl` directly rather than falling back to `mailto:`. Reading `AppInformation.Ready`'s logic against these values, `ReleaseCompliance`'s build gate should now pass, but this has not been confirmed by an actual build attempt since Unity access ended with AnkleBreaker's removal earlier the same day. One optional polish flagged for the user: the published privacy policy page does not name a publisher; the Player Settings company name (`heromughalsoft`) would be a natural choice if they want to add one.
- 2026-09-12, user reinstalled AnkleBreaker MCP to report and fix a real navigation bug: Settings > Privacy Policy > Back > Close Settings did not close Settings. Root cause: `SceneUI` uses one shared `returnPanel` field so any panel's Close can return to "wherever we came from". `Settings` sets `returnPanel` to its own caller (MainMenu or PausePanel) when opened; `PrivacyPolicy` was doing the same thing (`returnPanel=currentPanel`) when opened from inside Settings, overwriting that outer target with `SettingsPanel`. After Privacy's Back correctly returned to Settings, `returnPanel` was left stuck at `SettingsPanel`, so Settings' own Close button then just re-showed Settings instead of returning to MainMenu/PausePanel. `Handbook` never had this problem because it is only ever opened directly from MainMenu/PausePanel, never nested inside another returnPanel-using panel. Fixed by giving Privacy's Close button its own dedicated action (renamed `Close` to `ClosePrivacy` on the button in both scenes and in `SceneAuthoring.cs`, added a `SceneUI.Act` case that hardcodes `Show("SettingsPanel")`) instead of routing it through the shared field at all, since Privacy is always entered from Settings specifically. Reproduced the exact reported steps live in both scenes before and after the fix (confirmed broken, then confirmed fixed), and separately re-checked the direct Settings-close path and Handbook's open/close path for regressions; both still correct.
