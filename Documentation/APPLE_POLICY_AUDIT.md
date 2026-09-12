# Postmark Protocol: Apple policy and metadata scan

Date: September 3, 2026

## Decision

The current project has no source-level indication of hidden functionality, deceptive commerce, review manipulation, tracking, advertising, account collection, or a spam-style duplicate app. The revised US English metadata describes implemented gameplay and stays within Apple's published limits.

The requested metadata and project-policy scan are complete. Public privacy policy information, support contact information, and in-app privacy access are tracked as later App Store submission tasks. The Unity release compliance check will continue to hold the final release configuration until those publisher details are supplied.

This is a source, scene, package, configuration, and metadata review. Apple determines final acceptance. Developer-account conduct, legal ownership, trademark clearance, agreements, tax status, regional obligations, and the contents of an uploaded binary must be verified by the publisher.

## Current privacy configuration

| Check | Result |
| --- | --- |
| Public Privacy Policy URL in project data | Missing |
| Public Support URL in project data | Missing |
| Support email in project data | Missing |
| Easily accessible in-app privacy policy link | Missing |
| Application-authored analytics or telemetry | None found |
| Advertising or tracking SDK | None found |
| Runtime network request implementation | None found |
| Unity services | Disabled |
| Unity engine diagnostics | Disabled |
| Analytics submission | Disabled |
| Crash reporting API | Disabled |
| Local data | Campaign progress, results, tutorial state, and settings |

Apple's Guideline 5.1.1 requires a privacy policy link in App Store Connect and within the app in an easily accessible manner. The policy must explain what data is collected, how it is collected, every use, third-party sharing and protection, retention and deletion, and how consent can be withdrawn where applicable. A claim of no collected data must include the behavior of the submitted binary and all third-party SDKs, not only application-authored scripts.

Before submission, configure real publisher-owned values in `Assets/Data/AppInformation.asset`, provide the App Store Connect URLs, make the privacy policy accessible inside the app, and reconcile the App Privacy questionnaire with the final dependency graph and binary. The Support URL must open a page that provides a working way to contact the developer.

## Guideline 5.6 developer code of conduct

### 5.6.1 App Store customer reviews

No custom rating prompt, review gate, sentiment filter, paid-review mechanism, customer-chat system, or marketing notification was found. If a rating prompt is added later, use Apple's provided review request API and do not condition features or rewards on a rating.

Publisher action: keep review replies respectful, concise, relevant, free of personal information, and free of promotional spam.

### 5.6.2 Developer identity

The repository cannot verify the identity and contact information stored in the developer account. The current Unity company name and bundle identifier must be reconciled with the truthful legal or trading identity used in App Store Connect.

Publisher action: keep the developer name, address, phone number, email, website, tax information, and agreements accurate and current.

### 5.6.3 Discovery fraud

No referral scheme, install incentive, chart manipulation, copied metadata, competitor name, or misleading keyword was found in the proposed listing. The keyword alternatives are paired with subtitles to avoid repeated terms.

Publisher action: do not purchase reviews or installs, manipulate search results, impersonate another product, or use unrelated names and terms for discovery.

### 5.6.4 App quality

The project contains a complete 36-level campaign with explicit rules, progression, failure feedback, replay, scoring, settings, and a handbook. The reset flow handles a save failure without presenting a false success state.

Publisher action: maintain support, investigate repeated user reports, keep metadata current, and correct issues that affect the submitted version.

## Other App Review areas

### 1. Safety

The game has no user-generated content, messaging, social feed, account profile, financial service, medical claim, location feature, camera, microphone, contacts, or photo-library access in application-authored code. Its content is a nonviolent postal logic puzzle. Complete the age-rating questionnaire from the exact submitted content rather than assuming a rating from this audit.

### 2. Performance and metadata

The game features described in [APP_STORE_METADATA.md](I:/UnityProject/PostmarkProtocol/Documentation/APP_STORE_METADATA.md) correspond to current implementation: 36 levels, six chapters, seven commands, three ink types, four parcel rules, local progress, a handbook, and separate audio and haptics controls.

No remote feature flag, downloaded code, reviewer-specific branch, secret unlock, hidden purchase, or dormant network feature was found. Later mechanics are normal campaign progression and are disclosed in the App Review notes draft.

Privacy and support configuration belongs to the later submission phase. All URLs eventually supplied to Apple must be functional and direct reviewers to the promised information.

### 3. Business

No in-app purchase, subscription, advertising, donation, tipping, marketplace, cryptocurrency, gambling, or external payment implementation was found. Do not select commerce-related disclosures or make monetization claims that differ from the submitted version.

### 4. Design and spam risk

The project has a specific postal desk presentation, a custom deterministic route simulator, 36 authored puzzles, progressive mechanics, and its own campaign structure. No direct source-level evidence of a copycat, template reskin, thin web wrapper, content aggregator, or duplicated bundle was found.

Repository evidence cannot establish worldwide trademark clearance or determine whether the developer account contains closely related duplicate submissions. Keep this campaign in one app, use assets with documented rights, and avoid submitting near-identical variants as separate apps.

### 5. Legal

No account data, health data, financial data, precise location, contacts, photos, microphone input, advertising identifier, or tracking permission flow was found. Local save data does not leave the device through application-authored code. Privacy disclosures must still cover Apple platform behavior and every included third-party component in the final binary.

Rights to the title, icon, fonts, audio, artwork, and store screenshots remain the publisher's responsibility. Do not imply Apple endorsement, use protected third-party branding, or make unverified awards and ranking claims.

## Metadata and category result

- App name: `Postmark Protocol`
- Recommended subtitle: `Postal Route Logic Puzzles`
- Primary category: Games
- Primary Games subcategory: Puzzle
- Secondary Games subcategory: Strategy
- Secondary app category: leave blank
- Description: US English, source-aligned, and below 4,000 characters
- Promotional text: below 170 characters
- Subtitle alternatives: all at or below 30 characters
- Keyword alternatives: all at or below 100 ASCII bytes

## Later submission actions

1. Publish a complete privacy policy at a stable HTTPS URL.
2. Add an easily accessible privacy policy link inside the app.
3. Publish a support page with a working contact method and enter its URL in App Store Connect.
4. Verify truthful developer identity and review contact details.
5. Complete the App Privacy and age-rating questionnaires from the submitted binary.
6. Confirm all store text, screenshots, and review notes match the submitted version.
7. Confirm title and asset rights and avoid duplicate or copycat submissions.

## Official Apple references

- [App Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)
- [App information fields](https://developer.apple.com/help/app-store-connect/reference/app-information/app-information/)
- [Platform version information](https://developer.apple.com/help/app-store-connect/reference/app-information/platform-version-information/)
- [Manage app privacy](https://developer.apple.com/help/app-store-connect/manage-app-information/manage-app-privacy/)
- [Choosing a category](https://developer.apple.com/app-store/categories/)
