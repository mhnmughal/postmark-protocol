# Postmark Protocol: App Store metadata

Updated: September 3, 2026

All customer-facing copy in this document uses US English. The recommended listing describes features implemented in the current project and avoids promises about future content, rankings, awards, or unverified performance.

## Recommended listing

### App name

```text
Postmark Protocol
```

17 characters. Apple permits up to 30 characters. An exact App Store search did not surface a current matching listing, but that does not establish trademark or name clearance.

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

A letter is waiting on your desk, but it cannot find the mailbox on its own. In Postmark Protocol, you plan each journey by placing instruction stamps on a printed route. When the plan feels right, press Dispatch and watch the delivery unfold.

Every mistake leaves a clue. Review the route, move a stamp, and try again until the letter reaches its destination.

BUILD THE ROUTE
Use seven commands: Turn Left, Turn Right, Wait, Reverse, Fast, Slow, and Jump. Place them only on marked spaces, then learn how timing and direction change the result.

MASTER THREE TYPES OF INK
Standard ink runs once each time a letter visits a space. Double ink repeats the command. Permanent ink locks a stamp in place until you reset the sheet.

HANDLE SPECIAL DELIVERIES
Fragile parcels must avoid bumps. Express Mail cannot wait. Registered Mail must visit every checkpoint. Later routes combine these rules with gates, return paths, and carefully timed moves.

SOLVE 36 HANDCRAFTED PUZZLES
Work through six chapters, from the welcoming Local Route to the final Night Shift. Each delivery introduces a clear idea before combining it with earlier lessons.

EARN POSTAL SEALS
Complete a route to earn Delivered. Use an efficient plan to earn Efficient. Find the shortest qualifying route to earn Perfect Route. Your best results and unlocked deliveries are saved locally.

MADE FOR THOUGHTFUL PLAY
Learn through a guided first delivery and consult the built-in handbook whenever you need it. Adjust music, sound effects, and haptics to suit your desk.

Play offline with no account, ads, in-app purchases, subscriptions, or downloaded levels.

Designed for landscape play on iPhone and iPad.
```

## Subtitle and keyword alternatives

Use each subtitle with the keyword set on the same row. This prevents repeated words from wasting keyword space.

| Option | Subtitle | Length | Matching keyword set | Bytes |
| --- | --- | ---: | --- | ---: |
| 1, recommended | Postal Route Logic Puzzles | 26 | `programming,stamp,mail,delivery,envelope,commands,grid,brain,teaser,offline,strategy,casual` | 91 |
| 2 | Program Every Delivery | 22 | `postal,route,puzzle,logic,stamp,mail,envelope,commands,grid,brainteaser,offline,strategy` | 88 |
| 3 | Stamp, Route, Deliver | 21 | `postal,mail,puzzle,logic,programming,envelope,commands,grid,brain,teaser,offline,strategy` | 89 |
| 4 | Mailroom Logic Challenge | 24 | `postal,route,puzzle,programming,stamp,delivery,envelope,commands,grid,brain,teaser,offline` | 90 |
| 5 | A Tactile Programming Game | 26 | `postal,route,puzzle,logic,stamp,mail,delivery,envelope,commands,grid,brainteaser,offline,strategy` | 97 |

Option 1 is the clearest fit for people who want a logic puzzle. Option 2 is the most direct expression of the programming mechanic. Option 3 has the strongest rhythm but communicates the puzzle genre less clearly on its own.

## Category recommendation

| App Store Connect field | Selection | Reason |
| --- | --- | --- |
| Primary category | Games | The product is a complete single-player game with campaign progression, success conditions, failure states, and replayable scores. |
| Primary Games subcategory | Puzzle | Solving deterministic route problems is the central player activity. |
| Secondary Games subcategory | Strategy | Players plan command placement, timing, direction, and resource efficiency before dispatch. |
| Secondary app category | Leave blank | A second app category is optional, and no non-game category describes the product more accurately. |

Do not use Education unless the submitted version is positioned and designed as instructional software. The programming theme supports the puzzle mechanics, but the current app is entertainment rather than a curriculum.

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
Postmark Protocol is a single-player postal route puzzle with 36 bundled levels across six chapters. It does not require an account and contains no advertisements, in-app purchases, subscriptions, or downloaded levels.

On first launch, dismiss the introduction with Got It, choose Start Shift, and open level 1. Select Turn Right, place the stamp on the marked corner, and press Dispatch. After delivery, choose Next or return to the desk and open the level ledger.

Progression is sequential. Later chapters introduce timing gates, return paths, Double and Permanent ink, and Fragile, Express, and Registered parcel rules. The INK control becomes available when ink mechanics are introduced. These mechanics are ordinary campaign progression available to every player. There is no separate reviewer mode or remote feature switch.

The handbook explains all commands and rules. Settings includes separate music and sound effect controls, a haptics toggle, and a confirmed progress reset. A failed save keeps the reset dialog open and displays an error so the player can retry.

No demo account is required.
```

Add the exact steps needed to reach any feature Apple asks to inspect. For an update, replace this draft with notes that describe the actual changes in that submitted version.

## Privacy and support requirements

These publisher-owned items are assigned to the later App Store submission phase. The release compliance check intentionally keeps the final release configuration open until they are completed.

Before submission, the publisher must provide and verify:

1. A public HTTPS privacy policy URL in App Store Connect.
2. An easily accessible privacy policy link inside the app.
3. A Support URL that opens a page with a working contact method.
4. Current review contact details in App Store Connect.
5. App Privacy answers that match the submitted binary and every included third-party SDK.

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
