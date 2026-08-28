# Design, metadata, and originality policy audit

Review date: 2026-08-28. Read-only inspection of source, authored scene YAML, level assets, provenance records, and saved screenshots. No Unity state changes, physical-device tests, iOS exports, signing, or App Store Connect changes were performed. Saved screenshots show the states captured on 2026-08-26, not a newly observed running session.

## Findings

### P1: No accessible privacy policy or support contact in the current app

- `Assets/Editor/SceneAuthoring.cs:217` defines Settings with audio, haptics, reset, and a local-storage footer. No Privacy or Support control is authored. `Assets/Scripts/UI/SceneUI.cs:98` has no matching action. Searches of `Assets/Scenes/MainDesk.unity` and `Assets/Scenes/Gameplay.unity` found neither privacy nor support entries.
- The App Store metadata draft contains no Privacy Policy URL or Support URL. These could exist separately in App Store Connect, which this audit did not inspect; they are not evidenced here.
- Required action: provide a real maintained privacy-policy URL and support contact, accessible from the app and included in the listing. Keep an offline-readable policy if desired; do not invent an email, URL, or developer identity. A support identity can respect the user's removal of the in-game Credits screen.
- Apple requires an accessible privacy-policy link even for apps collecting no data, and an easy contact path in the app and its support URL. [Guidelines 5.1.1(i) and 1.5](https://developer.apple.com/app-store/review/guidelines/)

### P2: The proposed screenshot set includes an obsolete screen

- `Assets/Documentation/APP_STORE_METADATA.md` recommends `QA/Screens/settings-wide.png` as screenshot 5.
- Visual inspection of that image shows **CREDITS & ORIGINS** next to Reset Progress. The current authoring recipe and both current scenes no longer contain that button, following the user's removal request. Do not reintroduce Credits to match an obsolete screenshot.
- Required action: replace the candidate with a fresh capture of the final Settings screen. Review all remaining candidates against the release UI before selecting them. A screenshot showing an earlier UI is not sufficient evidence of the current state.
- Metadata must reflect the current experience; screenshots should demonstrate the app in use. [Guidelines 2.3 and 2.3.3](https://developer.apple.com/app-store/review/guidelines/)

### P2: Unresolved layout evidence and small phone controls weaken design readiness

- `Assets/Documentation/SESSION_QA_REPORT.md` currently says **FAIL: 3 layout audit findings**, despite all 36 levels passing its functional matrix. The entries concern the running header at levels 1/34 and multiple planning controls at level 2 at 1278x1044. This audit did not reproduce them or establish whether they represent layout defects or capture/resolution drift. Older clean QA summaries do not override this artifact.
- `Assets/Scenes/Gameplay.unity:12783` and `Assets/Editor/SceneAuthoring.cs:210` use a 1600x900 reference canvas matching height. The current Stamp_Slow RectTransform is 206x52; other stamps are authored likewise at `SceneAuthoring.cs:321`. Ink selectors are 55 units high.
- Static inference: on a 390-point-high landscape screen, 52 reference units become approximately 22.5 points. A 23-unit label becomes approximately 10 points. This is a design calculation, not a hardware measurement. The claim of a minimum 56-reference-pixel target in project notes is also inconsistent with these 52-unit controls.
- Required action: audit supported landscape aspect ratios and hit regions in logical points; make primary controls comfortably tappable and resolve/reproduce the three findings. Avoid asserting universal safe-area readiness until then.
- Apple's button guidance generally calls for 44x44-point hit regions; current accessibility guidance lists a 44-point default and 28-point minimum for iOS/iPadOS. This is a usability risk, not proof of rejection. [Buttons](https://developer.apple.com/design/human-interface-guidelines/buttons), [Accessibility](https://developer.apple.com/design/human-interface-guidelines/accessibility)

### P2: The metadata draft overstates verification

- Its opening paragraph says every claim has already been verified and that none are unverifiable. Its description asserts safe-area-aware layout on every screen. The latest integration report and the small-control calculation above do not support an unconditional readiness claim.
- Required action: distinguish implemented features from verified behavior. Keep claims about the actual 36 routes and available mechanics; remove internal QA assurances from consumer copy. Clarify ink semantics as executing once/twice **per visit**, since `RouteSimulator.cs:106` applies stamps on revisits too.

### P3: Art provenance is incomplete, though source is available

- `ART_LICENSES.md` documents only `AppIcon.png`. The game also ships `Assets/Art/WalnutGrain.asset`, authored primitive geometry, materials, and Unity's built-in LegacyRuntime font.
- `Assets/Editor/SceneAuthoring.cs:30` identifies the font; lines 48-80 reproduce material, walnut texture, geometry, and world text creation. `AUDIO_LICENSES.md` identifies all 19 audio assets and their source in `AudioAuthoring.cs`.
- Required action: extend the art inventory to include these assets and their actual origins/redistribution basis. Original-generation records are useful evidence, but neither guarantee exclusive rights nor establish trademark clearance.

## Design/spam assessment: no specific violation established locally

- There are exactly 36 current `Assets/Data/Levels/Route_*.asset` files. `LevelAuthoring.cs` contains explicit route definitions, not a runtime random reskin generator. The current validation artifact reports 36 distinct geometry signatures. This audit inspected the first and last assets and the full explicit recipe; it did not rerun validation.
- The chapters introduce turns, timing gates, reversals/revisits, double/permanent ink, fragile/express/registered parcels, and combined finale problems. Level 1 has one necessary turn; level 36 combines seven stamps, double ink, a locked corner, a gap, checkpoints, and a gate. This provides concrete content beyond a static guide or single repeated demonstration.
- Visually inspected icon, main menu, settings, and level 2/36 screenshots have a coherent postal desk, envelope, stamp, route, and ledger theme. The icon contains no observed brand, third-party logo, or misleading award. The code uses a custom puzzle simulator, authored scene hierarchy, and procedural desk assets. Primitive geometry and a generated icon do not by themselves establish a copycat or spam violation.
- The draft title and keyword list contain no observed competitor names, rankings, prices, or unrelated trend terms. The reported character counts in the draft are not relied upon here.
- Apple's rules address copying, insufficient lasting value, and indistinguishable or duplicate submissions. These are product/submission assessments, not a prohibition on using Unity or AI-assisted tools. [Guidelines 4.1, 4.2, and 4.3](https://developer.apple.com/app-store/review/guidelines/)
- **Limits:** no whole-App-Store comparison, title/trademark clearance, developer portfolio review, seller identity validation, ratings history, acquisition practices, or review correspondence was available. Consequently, 4.1/4.3 and developer conduct cannot be certified clean by repository inspection. Do not submit chapter/theme variants as separate near-identical apps or use paid/fake reviews.

## UI feature transparency

`SceneUI` exposes ordinary first-launch onboarding, chapter progression, handbook, planning, pause, restart/review, results, audio/haptic settings, and confirmed progress reset. Later commands/inks are introduced through progression and the handbook; this is not evidence of a concealed review-only mode. No special review, ratings, referral, secret-code, or developer-screen route was found in its action dispatcher. A separate runtime/package audit is needed before making a whole-binary hidden-feature claim.

**Outcome:** design/spam evidence is reasonably favorable, with concrete privacy/support, metadata freshness, verification, and usability work remaining. This is not Apple approval or a guarantee of acceptance.
