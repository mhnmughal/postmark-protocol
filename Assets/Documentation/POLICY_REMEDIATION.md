# Policy remediation - verified changes

Date: 2026-08-28.

**Follow-up: Privacy Policy and Support were removed from Settings at the user's request. In-app policy/support access is unresolved again.** Diagnostics, reset handling and responsive UI fixes remain. No physical-device tests, iOS export/build, signing, Apple login or App Store Connect upload were performed.

## Changes

- **Privacy and Support removed:** Both Settings buttons, their panels and runtime URL/email handlers were removed from MainDesk and Gameplay. Reset Progress is centered above Close Settings. The scene-authoring recipe and QA navigation match the removal. No Credits screen was restored.
- **Missing owner information stays explicit:** [AppInformation.asset](I:/UnityProject/PostmarkProtocol/Assets/Data/AppInformation.asset) retains empty privacyPolicyUrl, supportUrl and supportEmail fields for release configuration checks only; the runtime UI no longer references it. The preflight checks missing configuration, not complete App Store compliance. Filling this asset alone will not restore accessible in-app policy/support information.
- **Diagnostics clarified and disabled:** The installed Unity 6000.3.19f1 Editor exposes EngineDiagnosticsSettings.enabled separately from the old Insights flag. It was true; it is now false, and GetEngineDiagnosticsEnabledDefaultBuildValue returns false. PlayerSettings.submitAnalytics is false. UnityConnect, Analytics initialization, ads initialization, Insights, crash and performance reporting flags are disabled. There is no custom active build-profile override. These values were reread after the final tests. This establishes the current configuration, not a packet-capture or final-binary no-data certification.
- **Reset failure:** If the first atomic reset write fails, the previous session progress and future-version protection are restored. If backup clearing fails, the operation remains unconfirmed with an explicit retry message. The UI stays on the reset dialog and only announces completion after success.
- **Responsive layout:** The authored UI fits a 960x540 reference area inside the actual safe area. Canvas scale updates immediately, eliminating the one-frame stale scale observed during Editor resolution changes. Commands use two columns; from chapter 4 the INK button opens the ink selector. Menus/ledger rows were resized, long instructions now fit, instruction/parcel text has a solid contrasting background, and slider handles no longer stretch beyond their rows.
- **Route interaction:** The board camera fits the route into a reserved area without covering controls. An authored transparent input surface accepts nearby marked-space taps; overlapping targets choose the nearest marked center. No reviewer mode or hidden unlock was introduced.
- **Metadata/screenshots:** Overstated network/QA assurances remain removed. Review notes describe the ink selector and remaining Settings controls. Settings screenshot candidates must reflect the removal; earlier captures showing Privacy/Support are historical.

## Earlier remediation verification (before the requested removal)

| Check | Result |
| --- | --- |
| NUnit, final job 8d8a9dbaa53c | 68 passed, 0 failed, 0 skipped |
| CompilationPipeline | 0 errors/warnings when checked after the final code changes |
| Planning layouts | All 36 levels at 1136x640, 2778x1284, 2732x2048 and 1278x1044 |
| Menus and gameplay panels | Text/control bounds checked at all four resolutions |
| Layout findings in final sweep | 0 |
| Main UI control height, smallest phone simulation | Minimum 47.3 logical points at 1136x640 / 2; not a physical measurement |
| Live deliveries | Levels 1, 19, 34 and 36: authored controls, live presentation, perfect seals and file reload passed |
| Privacy/Support | Earlier navigation/scroll checks are historical; these controls and panels have now been removed by request |
| Reset fault injection | Blocked atomic temporary file: failure stays on dialog, progress preserved, retry persisted |
| Nearby route-slot selection | Tested at all four resolutions |
| Player data | QA used an isolated repository; original repository restored on completion |
| Screenshot formats | Eight PNGs checked: RGB color type 2, no alpha; exact dimensions and SHA-256 recorded |

Latest removal evidence: [Settings-Removal-Verification.md](I:/UnityProject/PostmarkProtocol/QA/Settings-Removal-Verification.md). The follow-up broad sweep in [Policy-UI-Verification.md](I:/UnityProject/PostmarkProtocol/QA/Policy-UI-Verification.md) retained two unexpected-resolution findings; targeted fixed-size rechecks passed. The zero-finding table above describes the earlier remediation run, not that follow-up sweep.
The first exploratory sweep is retained as [Policy-UI-Verification-first-pass.md](I:/UnityProject/PostmarkProtocol/QA/Policy-UI-Verification-first-pass.md); its clipping/scale findings were repaired and rerun. The older 2026-08-26 SESSION_QA_REPORT is historical, not the current layout result. The full 36-level live session was not rerun in this remediation; all 36 levels received layout checks and the core NUnit suite ran.

Selected current Settings, menu and gameplay captures were visually inspected. This is not a claim of exhaustive visual accessibility, VoiceOver support, OS lifecycle testing, hardware haptics or device performance.

## Fresh screenshot candidates

[Candidate manifest](I:/UnityProject/PostmarkProtocol/QA/StoreCandidates/manifest.json) records eight exact files:

- iPhone: 2778x1284, menu / level 1 / level 36 / settings.
- iPad: 2732x2048, menu / level 1 / level 36 / settings.

The screenshots came from Unity Game View. Only PNG alpha removal/format normalization was performed; no content was composited or altered. These are UI candidates, not proof of the final installed iOS binary. Missing public details and in-app policy/support access remain release gaps. Reconcile images with that binary when excluded release work resumes.

Apple currently accepts these landscape dimensions for the 6.5-inch iPhone and 13-inch iPad screenshot groups. [Screenshot specifications](https://developer.apple.com/help/app-store-connect/reference/app-information/screenshot-specifications/)

## Remaining owner action

Before submission, provide the public privacy-policy URL and a working support URL/contact, and restore suitable in-app access through a user-approved design. The latest request intentionally removed the Settings implementation. The published policy must identify the actual publisher and match its support-message handling and the game's local data behavior. Populating AppInformation alone does not close the in-app access gap. App Store Connect also needs the actual public Support URL.

No categorical “no data collected” label was submitted. The final manifest/binary/network checks remain excluded, and wider seller/account obligations from the original policy audit are not certified by these code fixes.
