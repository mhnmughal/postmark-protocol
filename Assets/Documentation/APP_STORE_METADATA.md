# App Store metadata - reviewed draft

Updated: 2026-08-28. **Not published and not ready to submit: publisher URLs/contact and in-app policy/support access are missing.** See [verified remediation](I:/UnityProject/PostmarkProtocol/Assets/Documentation/POLICY_REMEDIATION.md) for current fixes and evidence limits. Physical testing, new iOS exports/builds, signing and Apple account login were excluded.

## App name

```text
Postmark Protocol
```

17 characters; maximum 30. Name availability and trademark clearance remain unverified.

## Subtitle

```text
A Postal Programming Puzzle
```

27 characters; maximum 30.

## Promotional text

```text
Plan the route with instruction stamps, then dispatch your letter. Solve 36 handcrafted deliveries across six chapters, with no ads or accounts.
```

144 characters; maximum 170.

## Keywords

```text
mail,stamp,route,logic,offline,brainteaser,delivery,commands,sorting,envelope,indie,relaxing
```

92 ASCII bytes; maximum 100 bytes. No observed competitor names or unrelated terms.

## Description

```text
Stamp the instructions. Send the route.

Postmark Protocol is an offline puzzle about programming a letter's journey. Read the route, place instruction stamps on marked spaces, then press Dispatch and watch your plan play out.

PLAN, DISPATCH, REFINE
Choose from Turn Left, Turn Right, Wait, Reverse, Fast, Slow and Jump as new commands become available. Adjust your plan after a returned delivery, then try again.

36 HANDCRAFTED DELIVERIES
Explore six chapters: Local Route, Express Desk, Return Office, Ink Lab, Priority Service and Night Shift. Routes introduce turns, timing gates, return journeys, ink rules and parcel constraints before combining them in the finale.

THREE WAYS TO INK A STAMP
Standard ink executes a command once on each visit. Double ink executes it twice on each visit. Permanent ink locks a stamp against editing until you reset the sheet. Stamps remain on the route throughout a dispatch.

EARN YOUR POSTAL SEALS
Delivered marks a successful delivery. Efficient rewards meeting the route's par stamp count. Perfect Route requires both par efficiency and the optimal route distance. Keep your best seals and replay completed deliveries.

YOUR POSTAL DESK
Progress and audio settings are saved locally. Play without an account, advertisements or in-app purchases. Includes a handbook, a first-delivery tutorial, separate music and sound controls, and a haptics toggle.

Designed for landscape iPhone and iPad.
```

1436 characters; maximum 4000. This describes implemented gameplay, not universal safe-area readiness or verified absence of engine networking. Final copy must match the version submitted.

Field requirements: [Apple platform version information](https://developer.apple.com/help/app-store-connect/reference/app-information/platform-version-information/).

## Required owner information - do not upload these placeholders

| Field | Current state |
| --- | --- |
| Public Privacy Policy URL | Not supplied or verified |
| Public Support URL and working contact | Not supplied or verified |
| Rights holder / copyright | Confirm legal owner; do not infer from code namespace |
| Review contact | Confirm current name, email and phone privately in App Store Connect |
| App Privacy answers | Engine Diagnostics and automatic analytics are now disabled; final declaration must still match the submitted artifact |
| Category, age questionnaire, price and territories | Owner decisions; a general-audience puzzle is not automatically Kids Category |
| Trader status / regional compliance / agreements | Complete for the actual business and selected storefronts |

Privacy Policy and Support buttons, panels and external actions were removed from Settings at the user's request. In-app policy/support access is unresolved again. The empty [AppInformation.asset](I:/UnityProject/PostmarkProtocol/Assets/Data/AppInformation.asset) remains for release configuration checks only; supplying its values alone does not restore in-app access. No Credits screen was restored.

Apple requires a privacy URL and accurate app/partner data practices: [Manage app privacy](https://developer.apple.com/help/app-store-connect/manage-app-information/manage-app-privacy/).

## Fresh screenshot candidates

The previous recommendation is retired. Use the new [candidate set and manifest](I:/UnityProject/PostmarkProtocol/QA/StoreCandidates/README.md): four images each for iPhone 2778x1284 and iPad 2732x2048, captured from the current UI and normalized to RGB PNG without alpha. Lead with level 1 and level 36 gameplay; menu and Settings are optional supporting images. These are Editor captures, not final installed-iOS verification.

- `QA/Screens/settings-wide.png` shows a removed Credits control.
- `QA/Screens/gameplay-ipad.png` is a 2778x1284 capture; its filename is not evidence of an iPad screenshot.
- `QA/Screens/first-delivery-seals.png` predates later presentation fixes.
- Files in QA/StoreCandidates replace those older recommendations. Reconcile them with the submitted binary after owner contact details are configured; physical testing and iOS export were excluded.

The verified dimensions appear in Apple's current [screenshot specifications](https://developer.apple.com/help/app-store-connect/reference/app-information/screenshot-specifications/). Do not label the old phone-sized file as an iPad capture or upload internal QA reports as screenshots.

## Notes for Review - draft, reconcile with submitted version

Postmark Protocol is a single-player postal route puzzle with 36 bundled levels in six chapters. There are no application accounts, advertisements, in-app purchases, subscriptions or downloaded levels.

On first launch, dismiss the introduction with Got It. Choose Start Shift. On level 1, choose Turn Right, stamp the single marked corner, and press Dispatch. After delivery choose Next, or return to the desk and open the ledger.

Progression is sequential. Later chapters introduce timing, return paths, Double/Permanent ink, parcel constraints and the Night Shift presentation. From chapter 4, the INK button opens the Standard/Double/Permanent selector. This is ordinary progression shared by all players; there is no separate reviewer mode or remote feature switch.

The scrollable handbook explains commands. Settings contains music and sound controls, a haptics toggle, a progress reset with a confirmation dialog. Reset failures remain on the dialog with an error and retry. No demo login is required.

Before submission: restore suitable in-app policy/support access through an approved design, supply the owner's public policy/support URLs and contact, reconcile the published policy with actual operations, and attach a concise walkthrough for later chapters so App Review can inspect every mechanic without guessing. Do not add a secret bypass. For updates, describe the actual version changes.

## Revision notes

Removed unsupported blanket QA, network and safe-area assurances. Corrected name/keyword counts, per-visit ink semantics, and par-versus-stamp-budget seal wording. Removed stale screenshot endorsements. Nothing was uploaded to App Store Connect.
