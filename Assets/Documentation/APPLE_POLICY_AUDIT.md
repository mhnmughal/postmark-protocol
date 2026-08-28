# Postmark Protocol - Apple policy audit

> **Follow-up remediation:** The findings below describe the initial audit snapshot. See [POLICY_REMEDIATION.md](I:/UnityProject/PostmarkProtocol/Assets/Documentation/POLICY_REMEDIATION.md) for the implemented fixes and final verification. Reset failure handling, diagnostics configuration, responsive layouts and screenshot/metadata recommendations have been addressed. Privacy/Support screens were subsequently removed at the user's request. In-app access and real publisher URLs/contact details remain unresolved. This remains short of submission clearance.

Date: 2026-08-28. **Verdict: not cleared for submission.** No concealed game feature or specific spam/copycat implementation was found in this source review. Confirmed privacy/support gaps and quality/metadata issues remain.

Reviewed all five categories of the current [App Review Guidelines](https://developer.apple.com/app-store/review/guidelines/) (page dated June 8, 2026), plus the App Store Connect requirements relevant to this project. This is a project audit, not Apple approval, legal certification, a review of every regional agreement, or an inspection of the developer's account.

## Scope and evidence

Inspected 12 runtime C# files, the sole native iOS plugin, three included scenes, all 53 authored buttons, 14 panels, 36 level assets, editor/test assembly boundaries, package/player/service configuration, metadata, provenance and existing QA artifacts. The separate [design audit](I:/UnityProject/PostmarkProtocol/QA/Policy-Design-Audit.md) includes screenshot observations.

No new device tests, iOS exports/builds, signing, Apple account login, uploads or App Store Connect changes. No runtime, scene or player-setting changes. Existing exports and previous test results are historical evidence, not proof of the current submitted binary. Excluded work is not a PASS.

## Prioritized findings

### P1 - Missing privacy policy and support access

**Confirmed in the current app.** [SceneUI.cs](I:/UnityProject/PostmarkProtocol/Assets/Scripts/UI/SceneUI.cs:92) has no Privacy/Support action; [SceneAuthoring.cs](I:/UnityProject/PostmarkProtocol/Assets/Editor/SceneAuthoring.cs:219) authors neither entry point. Both saved scenes contain only a local-save label. The listing draft supplied no actual privacy/support URLs. No App Store Connect account was accessed, so its existing fields are unknown.

Guidelines 5.1.1(i) and 1.5 are relevant: all apps need accessible privacy information and a way to contact the developer. Offline gameplay is not an exemption.

**Close when:** the owner supplies real public URLs/contact details; Settings exposes Privacy and Support; the policy matches actual data practices and explains retention/reset/contact. An offline-readable copy can accompany the public link. Keep gameplay independent of a network connection. No Credits screen or decorative developer name is needed.

App Store Connect requires a public privacy URL and partner-inclusive privacy answers: [Manage app privacy](https://developer.apple.com/help/app-store-connect/manage-app-information/manage-app-privacy/). Its Support URL must lead to real contact information: [Platform version information](https://developer.apple.com/help/app-store-connect/reference/app-information/platform-version-information/).

### P2 - No-data claims exceed the evidence

**Unresolved, not proof of telemetry.** No application-authored runtime network, analytics, ads or tracking call sites were found. However, [UnityConnectSettings.asset](I:/UnityProject/PostmarkProtocol/ProjectSettings/UnityConnectSettings.asset:7) has the overall connection enabled and `m_EngineDiagnosticsEnabled: 1`; the adjacent Insights flag, analytics, purchasing, ads and cloud-diagnostics flags are disabled. [ProjectSettings.asset](I:/UnityProject/PostmarkProtocol/ProjectSettings/ProjectSettings.asset:93) also contains `submitAnalytics: 1`.

This audit could not establish the precise Unity 6000.3 semantics of those legacy/engine flags. Their presence does not establish transmission; static game-source absence does not exclude engine behavior. Do not publish categorical no-analytics/no-data assertions until resolved against the actual shipping configuration.

Local progress/settings are stored in `Application.persistentDataPath/postmark-save.json`. Apple distinguishes on-device processing from off-device collection; SDK behavior matters separately. [App Privacy Details](https://developer.apple.com/app-store/app-privacy-details/)

No source `.xcprivacy` file was found under Assets. That alone is not a violation: the final engine/app/SDK manifests were not verified in this review. When artifact verification resumes, reconcile required-reason API declarations with actual usage rather than inventing reason codes. [Apple SDK requirements](https://developer.apple.com/support/third-party-SDK-requirements/)

**Close when:** engine behavior is established, unnecessary collection is absent/disabled or correctly handled, and the policy, privacy label and final manifests agree. Final artifact/runtime checks remain excluded from this session.

### P2 - Reset can imply success after persistence failure

[SceneUI.cs](I:/UnityProject/PostmarkProtocol/Assets/Scripts/UI/SceneUI.cs:116) ignores the Boolean returned by `SaveService.ResetProgress()`, then changes scenes or displays zero deliveries. [SaveService.cs](I:/UnityProject/PostmarkProtocol/Assets/Scripts/Persistence/SaveService.cs:169) requires two successful writes to remove recoverable old progress, and can return false.

If storage fails, the visible reset can differ from what survives relaunch. This is an error-handling and customer-trust concern under 2.1/5.6.4, not evidence of deliberate deception, nor an account-deletion violation in this account-free game.

**Close when:** the UI checks the result, communicates failure without claiming durable erasure, allows retry, and an existing save/UI test covers the failure path. Not fixed by this review.

### P2 - Latest layout report contradicts blanket PASS summaries

[SESSION_QA_REPORT.md](I:/UnityProject/PostmarkProtocol/Assets/Documentation/SESSION_QA_REPORT.md:3) records **36/36 functional completions but FAIL: 3 layout audit findings**, at 1278x1044: running header at levels 1/34, and several planning controls at level 2. Older QA/final/UI summaries claim zero findings.

The discrepancy needs reproduction: it may include capture/resolution drift rather than three established player-facing bugs. Existing results also predate later UI edits. This review did not rerun them.

Static sizing is another concern: [SceneAuthoring.cs](I:/UnityProject/PostmarkProtocol/Assets/Editor/SceneAuthoring.cs:210) uses a 1600x900 canvas matching height, and stamp controls are 206x52 at line 321. At a 390-point landscape height, that suggests approximately 22.5-point control height and 10-point text for a 23-unit label. This is an estimate, not a hardware measurement. Apple's general button target is 44x44 points. [Buttons](https://developer.apple.com/design/human-interface-guidelines/buttons)

**Close when:** supported aspect ratios, actual hit regions and legibility are verified; genuine defects are repaired; contradictory audit summaries are reconciled. HIG sizing is a usability signal, not an automatic rejection formula.

### P2 - Metadata and screenshot recommendations were stale

The previous draft claimed everything was verified, guaranteed no networking, and endorsed every screen's safe area. It described Efficient using the hard stamp budget instead of par, and omitted that stamps execute again on revisits.

Its proposed Settings capture shows a removed Credits control; the capture named `gameplay-ipad.png` is 2778x1284, so the name does not establish the correct device family. Some proposed result images predate presentation fixes. This conflicts with the current app, relevant to 2.3/2.3.3.

**Documentation correction completed:** [APP_STORE_METADATA.md](I:/UnityProject/PostmarkProtocol/Assets/Documentation/APP_STORE_METADATA.md) removes unsupported assurances, corrects mechanics/counts, withdraws the old screenshot recommendation, and adds owner fields and draft review notes. Name/subtitle/promo/keywords/description measure 17/27/144/92/1436 respectively (keywords are ASCII bytes).

**Still open:** actual owner information, final screenshot set and reconciliation with the submitted version. Nothing was published.

### P3 - Asset provenance needs a complete inventory

[ART_LICENSES.md](I:/UnityProject/PostmarkProtocol/Assets/Documentation/ART_LICENSES.md) records the generated icon but not all procedural materials, WalnutGrain, primitive geometry or the built-in LegacyRuntime font. [SceneAuthoring.cs](I:/UnityProject/PostmarkProtocol/Assets/Editor/SceneAuthoring.cs:30) identifies the font and reproducible procedural asset creation. Audio has a separate 19-asset inventory.

No copied artwork was established. Extend the inventory and retain applicable Unity/font redistribution terms; generation history is not trademark clearance or a guarantee of exclusive rights. Relevant to 5.2. Not changed here.

## Hidden-functionality assessment

Findings relevant to 2.3.1(a), 2.5.2 and 4.7:

- Build settings include only Bootstrap, MainDesk and Gameplay. All 53 actual scene buttons resolve to ordinary actions and sit below SceneUI, where listeners attach. No unknown action, orphan button or hidden persistent callback was found.
- Fourteen panels cover introduction, menu, chapters, ledger, handbook, settings, reset confirmation, gameplay, pause and results. No developer/reviewer console was found.
- Later commands/chapters/inks are earned progression. Double/Permanent controls appear from chapter 4; Night Shift changes presentation. These should be explained in review notes, not misrepresented as launch-only behavior.
- No remote config, IP/date/reviewer checks, secret unlock gestures, external code loading, webview, referral/rating gate, account, purchase, ad or tracking call site was identified in the game runtime.
- Seven authoring/QA scripts and both test assemblies are Editor-only. AnkleBreaker networking is confined to Editor assemblies; engine web-request module names are not evidence of a game feature.
- Serialized `knownSolution` arrays are consumed by Editor validation/QA, not a player auto-solve action.
- Stamp instructions are fixed game data interpreted by RouteSimulator, not downloaded executable programs.
- The sole native plugin calls UIKit haptics. No camera, microphone, contacts, location or photo permission request was found.

**Conclusion:** no concealed runtime feature identified within source/scene scope. This is not a forensic certification of an unexamined final binary. Review notes should disclose every mechanic and provide a straightforward later-chapter walkthrough without secret reviewer behavior.

## Design / spam assessment

For 4.1, 4.2 and 4.3, local evidence is favorable: 36 explicit route assets, six progressing chapters, seven commands, three inks, four parcel types, seals, settings, authored screens and a custom deterministic simulator. Existing validation records distinct geometry and optimal routes. This is substantive playable content, not merely a website wrapper or static illustration.

No specific copied app, competitor asset or duplicate-bundle distribution scheme was established. Unity, procedural shapes or AI-assisted artwork do not by themselves establish a spam finding. More cosmetic features would not resolve the concrete issues above.

**Unknown:** whole-App-Store similarity, title/trademark clearance, the developer's other submissions, reused templates across accounts and acquisition practices. This repository cannot clear those. Keep the 36 routes in this one app; avoid near-identical chapter/theme variants as separate submissions.

## Section 5.6 - account conduct is not code style

| Clause | Project/account assessment |
| --- | --- |
| 5.6.1 | No rating prompt, review filter, customer chat or marketing notification exists here. External customer responses were not inspected. |
| 5.6.2 | Seller identity, rights holder and account contact information are unverified. Missing in-app contact is actionable. Removed Credits UI is not evidence of false identity. |
| 5.6.3 | No review/reward/ranking manipulation mechanism found in source. Paid acquisition, review vendors, referral campaigns and account history are unknown. |
| 5.6.4 | Existing functional results are useful, but layout findings and reset error handling remain. Refunds, crash rates and user complaints were not available. |

Apple expects respectful customer handling, accurate identity, no discovery/review manipulation and maintained quality. [Guideline 5.6](https://developer.apple.com/app-store/review/guidelines/#developer-code-of-conduct)

Do not buy/incentivize fake reviews, impersonate a seller, disguise a rejected app under another account, or submit misleading review notes. No such conduct is alleged here. If Apple has already issued a 5.6 rejection, the exact notice and affected app/account are needed to assess that issue; adding features alone may not answer it.

## Complete guideline-category coverage

Statuses below are findings of applicability, not blanket passes. Subclauses were screened under their parent groups. This matrix records the current product rather than reproducing Apple's rules.

| Guideline group | Current project finding |
| --- | --- |
| 1.1 | Postal puzzle content; no identified sexual, violent, hateful, deceptive-device or dangerous material |
| 1.2, 1.2.1 | No shared UGC, creator platform, chat or social service |
| 1.3 | Kids Category not established; reassess links/privacy if the owner chooses it |
| 1.4, 1.7 | No medical, physical-harm or criminal-reporting functionality |
| 1.5 | Open privacy/support finding above |
| 1.6 | Local progress/settings; no credentials; reset error path open |
| 2.1, 2.2 | Implemented game, but unresolved QA findings; no new release/device evidence |
| 2.3.1-2.3.13 | Metadata revised; final screenshots, age answers, rights and review details still open |
| 2.4 | Landscape iPhone/iPad target; small-control/layout risk; performance/device checks excluded; no tvOS/macOS app submitted here |
| 2.5.1-2.5.6, 2.5.8-2.5.9 | Public haptic API; local save; no downloaded code/browser/background feature; final OS/audio/engine verification excluded |
| 2.5.11-2.5.18 | No Siri, call blocking, facial login, activity recording, file browser, widgets, App Clips, Matter or ads |
| 3.1.1-3.1.5 | No IAP, subscriptions, external checkout, paid hardware unlock, NFT/crypto or reader/business service; base price remains an account decision |
| 3.2.1-3.2.2 | No app catalog, fundraising, loans, trading, ad manipulation or rating/download unlock requirement |
| 4.1-4.3 | Substantive content observed; no specific copycat/spam implementation; external portfolio comparison unknown |
| 4.4-4.5 | No extensions, Apple Music, push messaging or Game Center; local seals are not Game Center achievements |
| 4.7 | No downloadable miniapp/emulator/streaming platform |
| 4.8 | No primary account or social login; Sign in with Apple is not triggered by this implementation |
| 4.9-4.10 | No Apple Pay or monetized OS capability |
| 5.1.1-5.1.2 | Privacy/support and engine-data questions open; no app account, tracking call or runtime AI data sharing found |
| 5.1.3-5.1.5 | No health research/location functionality; child-directed status requires owner decision |
| 5.2 | Procedural/icon/audio origins documented partly; inventory, rights and title clearance remain |
| 5.3-5.5 | No real-money prizes, sweepstakes, gambling, VPN or MDM |
| 5.6 | Account-level evidence unavailable; see separate analysis |
| Before / after submission | Final artifact, metadata, review contact, agreements and ongoing operation remain owner/release responsibilities |

Numbered sections Apple marks intentionally omitted add no project checklist items. Adding any presently absent service requires a fresh applicability check.

## App Store Connect completion checklist

1. Supply and verify public privacy/support URLs and accessible in-app entry points. Never submit placeholder URLs.
2. Resolve engine/SDK data behavior before selecting App Privacy answers. Privacy labels and manifests are separate checks.
3. Choose the truthful age-questionnaire answers from actual content. Do not promise a worldwide 4+ result or select Kids Category solely because the game is nonviolent. [Age rating](https://developer.apple.com/help/app-store-connect/manage-app-information/set-an-app-age-rating/)
4. Confirm app name, category, rights holder, contact details, description, keywords, current screenshots and version-specific review notes. No demo credentials are needed for the current account-free app.
5. Confirm seller identity and actual trader status. Apple requests trader status even without EU distribution, and displays verified trader contact details for EU distribution. The audit cannot choose the user's legal status. [DSA requirements](https://developer.apple.com/help/app-store-connect/manage-compliance-information/manage-european-union-digital-services-act-trader-requirements/)
6. Complete relevant agreements, pricing, tax/banking and territory-specific approvals in the owner's account. No blanket all-territory clearance was performed.
7. Make the export-compliance determination against actual incorporated cryptography. The save checksum uses SHA-256 for integrity, which alone is not a complete app encryption assessment. Do not guess the encryption declaration. [Export compliance](https://developer.apple.com/help/app-store-connect/manage-app-information/overview-of-export-compliance/)
8. When release work resumes, reconcile the final binary, SDK/privacy declarations, supported OS/device behavior and store assets. This review did not perform excluded physical testing, iOS export, signing or login.
9. Make no accessibility-label claims such as full VoiceOver/Larger Text support without verifying them. Current uGUI controls and a haptics toggle alone are not that evidence.
10. Keep customer support functional and review communications accurate after release.

## Changes and limits of this audit

Changed documentation only: this report, the metadata draft, the design audit and correction notices on older QA/network/project summaries. No identified runtime issue was silently marked fixed. No personal contact details, seller identity or compliance declarations were invented.

Historical 66-test and 36-level results were read, not rerun. The strongest current conclusion is **no hidden feature or specific spam implementation found, with concrete submission gaps still open**.
