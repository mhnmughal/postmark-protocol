# Postmark Protocol: App Store metadata

Updated: September 12, 2026

All customer-facing copy in this document uses US English. The recommended listing describes features implemented in the current project and avoids promises about future content, rankings, awards, or unverified performance. No em dash appears anywhere in the customer-facing copy below.

## Recommended listing

### App name

```text
Postmark Protocol
```

17 characters. Apple permits up to 30 characters. A fresh search on September 12, 2026 found no current App Store game named Postmark Protocol; the closest result was an unrelated, unpublished business email tool called Postmark. That is a naming check, not trademark or legal clearance, and the publisher should still confirm the name is free to use before submission.

### Subtitle

```text
Postal Route Logic Puzzles
```

26 characters. Apple permits up to 30 characters.

### Promotional text

```text
Plan each postal route with instruction stamps, then dispatch your letter through 36 handcrafted logic puzzles across six distinctive chapters.
```

### Keywords

```text
programming,stamp,mail,delivery,envelope,commands,grid,brain,teaser,offline,strategy,casual
```

91 ASCII bytes. Apple permits up to 100 bytes. This set does not repeat words from the recommended app name or subtitle and does not use competitor names.

### Description

```text
Stamp the instructions. Send the route.

Every letter on your desk needs a plan before it can find its way home. In Postmark Protocol, you build that plan by placing instruction stamps along a printed route, then press Dispatch and watch it play out exactly as written.

When something goes wrong, the route shows you why. Move a stamp, try again, and get a little closer each time until the letter reaches its destination.

BUILD THE ROUTE
Choose from seven commands, Turn Left, Turn Right, Wait, Reverse, Fast, Slow, and Jump, and place them on marked spaces. Timing and direction both matter, and you will feel the difference the first time you get one wrong.

LEARN THREE KINDS OF INK
Standard ink fires once per visit. Double ink fires twice. Permanent ink locks a stamp in place until you reset the sheet, so plan carefully before you commit.

HANDLE SPECIAL DELIVERIES
Fragile parcels cannot take a bump. Express Mail will not wait around. Registered Mail needs a checkpoint visit at every stop. Later routes mix these rules with timed gates, return paths, and moves measured down to the cycle.

WORK THROUGH 36 HANDCRAFTED DELIVERIES
Six chapters carry you from the easygoing Local Route to the tense final Night Shift, each one teaching a new idea before folding it into everything you already know.

EARN YOUR POSTAL SEALS
Deliver the letter and earn Delivered. Do it efficiently and earn Efficient. Find the shortest route that still qualifies and earn Perfect Route. Every result and every delivery you unlock is saved right on your device.

BUILT FOR UNHURRIED PLAY
A short guided first delivery gets you started, and the handbook is there whenever you need a reminder. Music, sound effects, and haptics are all yours to adjust.

No account, no ads, no in-app purchases, no subscriptions, and no downloaded content. Everything plays offline, the same on iPhone and iPad, in landscape.
```

Every claim in this description matches an implemented feature; see the claim-to-project check table below.

## Subtitle and keyword alternatives

Use each subtitle with the keyword set on the same row. Every set below was checked word by word against its own subtitle and against the app name so nothing is wasted repeating a word already searchable elsewhere, and none use a competitor's name.

| Option | Subtitle | Length | Matching keyword set | Bytes |
| --- | --- | ---: | --- | ---: |
| 1, recommended | Postal Route Logic Puzzles | 26 | `programming,stamp,mail,delivery,envelope,commands,grid,brain,teaser,offline,strategy,casual` | 91 |
| 2 | Stamp, Plan, Deliver | 20 | `programming,mail,envelope,commands,grid,brain,teaser,offline,strategy,casual,postal,route,puzzle` | 96 |
| 3 | A Tactile Logic Puzzle | 22 | `programming,stamp,mail,delivery,envelope,commands,grid,brain,teaser,offline,strategy,postal,route` | 97 |
| 4 | Program the Perfect Route | 25 | `stamp,mail,delivery,envelope,commands,grid,brain,teaser,offline,strategy,casual,postal,puzzle` | 93 |
| 5 | Puzzles for Patient Minds | 25 | `programming,stamp,mail,delivery,envelope,commands,grid,brain,teaser,offline,strategy,postal,route` | 97 |

Option 1 states the genre plainly and is the safest default. Option 2 leans on the game's own verb, stamp, plan, deliver, which doubles as a one-line pitch. Option 3 foregrounds the tactile, physical feel of placing a stamp rather than the programming angle, useful if screenshots are doing the genre-explaining work instead. Option 4 speaks directly to the programming-minded player who searches for that mechanic first. Option 5 is the most emotional and brand-like of the five; it reads well but explains the mechanic least, so pair it only with a description and screenshots that do that explaining clearly (both do, in the current draft).

## Category recommendation

| App Store Connect field | Selection | Reason |
| --- | --- | --- |
| Primary category | Games | The product is a complete single-player game with campaign progression, success conditions, failure states, and replayable scores. |
| Primary Games subcategory | Puzzle | Solving deterministic route problems is the central player activity. This is one of Apple's 16 published Games subcategories. |
| Secondary Games subcategory | Strategy | Players plan command placement, timing, direction, and resource efficiency before dispatch. Also one of the 16 published subcategories; Apple allows up to two. |
| Secondary app category | Leave blank | A second top-level category is optional, and no non-game category describes the product more accurately than Games already does. |

Do not use Education unless the submitted version is positioned and designed as instructional software. The programming theme supports the puzzle mechanics, but the current app is entertainment rather than a curriculum, and Apple reviews Education listings against that standard specifically.

A newer requirement to flag for submission: starting September 2026, Apple requires every app or game to declare in App Store Connect whether it includes social media capabilities before it can be submitted or notarized. Postmark Protocol has no social features, accounts, sharing, or network access of any kind, so this should be answered No, but the publisher must still make that declaration explicitly; it will not default correctly on its own.

## Claim-to-project check

| Listing claim | Current project evidence |
| --- | --- |
| 36 handcrafted puzzles | 36 authored level assets across six named chapters |
| Seven commands | Left, Right, Wait, Reverse, Fast, Slow, and Jump |
| Three ink types | Standard, Double, and Permanent |
| Special deliveries | Standard, Fragile, Express, and Registered parcel rules |
| Postal seals | Delivered, Efficient, and Perfect Route results |
| Guided first delivery | First-launch introduction and level 1 tutorial flow |
| Built-in handbook | Scrollable command and rule handbook |
| Saved locally | Progress, best results, tutorial state, and preferences use the local save file |
| Offline, no account | No runtime account, login, remote content, or network request implementation found |
| No ads or purchases | No runtime advertising, in-app purchase, subscription, or StoreKit implementation found |
| Music, effects, and haptics | Separate audio controls and an optional haptics setting are implemented |

These are source and configuration findings for the current checkout. Reconcile the copy with the exact binary submitted to Apple after any code, package, or service configuration change.

## App Review notes draft

```text
Postmark Protocol is an offline, single-player puzzle game with 36 built-in levels across six chapters. There is no account of any kind, and no advertising, analytics, in-app purchases, subscriptions, or downloadable content.

To try it: on first launch, tap Got It to dismiss the welcome screen, then tap Start Shift to open level 1. Tap the Turn Right stamp, place it on the marked corner of the route, and tap Dispatch to run it. After the letter is delivered, tap Next for the following level, or tap Desk and open the Delivery Ledger to pick any unlocked level directly.

Progression is linear and the same for every player. Later chapters add timing gates, return paths, Double and Permanent ink, and Fragile, Express, and Registered parcel rules, each introduced with its own short lesson the first time it appears. The Ink button only appears once ink mechanics are introduced, which is expected behavior, not a hidden or reviewer-only feature. There is no alternate reviewer mode, remote configuration, or server-controlled content of any kind; every player sees the same 36 levels in the same order.

Settings, reachable from the Main Menu or the in-level pause screen, has independent Music and Sound Effects volume controls, a Haptic Feedback toggle, Privacy Policy and Support buttons, and a Reset Progress option that asks for confirmation before it clears saved progress. If a progress reset cannot be saved to disk, the app keeps the confirmation dialog open and shows the reason, rather than reporting success it cannot guarantee.

No demo account, login, or special access is needed to review any part of the app.
```

Add the exact steps needed to reach any feature Apple asks to inspect. For an update, replace this draft with notes that describe the actual changes in that submitted version.

## Privacy and support requirements

These publisher-owned items are assigned to the later App Store submission phase. The release compliance check intentionally keeps the final release configuration open until they are completed.

Before submission, the publisher must still provide and verify:

1. ~~A public HTTPS privacy policy URL in App Store Connect.~~ Done as of September 12, 2026: live at [sites.google.com/view/privacypolicyforpostmarkprotoc/home](https://sites.google.com/view/privacypolicyforpostmarkprotoc/home), matching the drafted copy at `PRIVACY_POLICY_FOR_GOOGLE_SITE.md`. Set this as the Privacy Policy URL in App Store Connect. `AppInformation.asset` also has this URL now, so `ReleaseCompliance`'s build gate accepts it.
2. ~~An easily accessible privacy policy link inside the app.~~ Done as of September 12, 2026: Settings has a working Privacy Policy button showing the in-app policy text.
3. ~~A Support URL that opens a page with a working contact method.~~ Done as of September 12, 2026: live at [sites.google.com/view/postmark-protocol-suppor/home](https://sites.google.com/view/postmark-protocol-suppor/home), matching the drafted copy at `SUPPORT_PAGE_FOR_GOOGLE_SITE.md`. Set this as the Support URL in App Store Connect. `AppInformation.asset` also has this URL now; Settings' Support button now opens it directly (falling back to `mailto:` only if this URL were ever unset).
4. Current review contact details in App Store Connect.
5. App Privacy answers that match the submitted binary and every included third-party SDK.

One remaining, optional polish item: the published privacy policy page identifies the game but not a publisher name. Consider adding one (for example, the company name used in Player Settings, `heromughalsoft`) so the policy names an accountable party as directly as its App Store Connect entry will.

The policy should identify the publisher, explain that gameplay progress and preferences are stored locally, describe any data collected by the app or third parties, state the purposes and retention rules, explain deletion choices, and provide a working privacy contact. Do not publish a policy containing placeholders.

Current project settings disable Unity services, engine diagnostics, analytics submission, and crash reporting. No application-authored telemetry, tracking, ads, or runtime network request was found. Recheck these settings and the final dependency graph before selecting App Store Connect privacy answers.

## Guideline 5.6 developer conduct scan

The current project contains no forced purchase flow, unnecessary data gate, review filtering, custom rating prompt, referral manipulation, chart manipulation, misleading feature gate, or hidden reviewer behavior. The metadata above avoids unverifiable superlatives and describes only implemented behavior.

Repository inspection cannot verify developer-account conduct. The publisher must ensure that the legal identity, address, phone number, email, and website in App Store Connect are truthful and current; customer review replies are respectful and do not contain personal information or marketing spam; discovery is not manipulated; and reported issues receive reasonable support.

## Official Apple references

- [App Review Guidelines](https://developer.apple.com/app-store/review/guidelines/)
- [App information fields](https://developer.apple.com/help/app-store-connect/reference/app-information/app-information/)
- [Platform version information](https://developer.apple.com/help/app-store-connect/reference/app-information/platform-version-information/)
- [Manage app privacy](https://developer.apple.com/help/app-store-connect/manage-app-information/manage-app-privacy/)
- [Choosing a category](https://developer.apple.com/app-store/categories/)

## Submission position

The recommended metadata and current project-policy scan are complete and aligned with Apple's published field limits. Privacy and support setup remains part of the later App Store submission phase. Apple makes the final review decision, so no project audit can guarantee approval.
