# App Store metadata (ASO draft)

Date: 2026-08-26. Draft App Store Connect listing copy for Postmark Protocol, written to Apple's field limits and checked against Guideline 4.3 (Design — Spam) and 5.6 (Developer Code of Conduct). Every claim below is something already verified in this project's own documentation (`QA_REPORT.md`, `FINAL_AUDIT.md`, `NETWORK_AUDIT.md`) — nothing here is aspirational or unverifiable, since false/unsubstantiated claims are themselves a review risk (Guideline 2.3 Accurate Metadata).

## Compliance notes for this listing

- **No keyword stuffing in the app name.** The name field is `Postmark Protocol` only — matching the configured Product Name, no bolted-on keyword string. Apple's own guidance (2.3.7 / the spirit of 4.3) treats a keyword-stuffed name/subtitle as a spam signal; all searchable terms beyond the name live in the dedicated 100-character Keywords field instead, where they belong.
- **No repeated words across Name/Subtitle/Keywords.** Apple's search index already credits words in the Name and Subtitle; repeating them in Keywords wastes the 100-char budget and is the single most common ASO mistake. Keywords below deliberately add only new terms.
- **No competitor names, no unrelated trending terms.** Both are explicit App Store Review violations (keyword spam / trademark risk), not just bad practice.
- **No claims that can't be backed.** "Offline," "no ads," "no accounts," "no tracking" are all directly verified in `NETWORK_AUDIT.md` and `FINAL_AUDIT.md` — safe, accurate, and a genuine differentiator worth leading with.

## App Name (30 char max)

```
Postmark Protocol
```
18 characters. Clean, matches Product Name and bundle branding exactly — no edit needed for launch.

## Subtitle (30 char max)

```
A Postal Programming Puzzle
```
27 characters. Carries the actual hook (it's a programming puzzle, not a match-3 or hidden-object game) that the name alone doesn't convey.

## Promotional text (170 char max, editable anytime without a new build)

```
Stamp the instructions, dispatch the letter, watch it find its way. 36 handcrafted routes. No ads, no accounts, no tracking — just an offline postal puzzle.
```
158 characters — leaves a little headroom. This field can be revised per season/update without a review cycle, so it's the right place for anything time-sensitive later (e.g. "Chapter 7 now available") — do not use it for anything unverifiable.

## Keywords (100 char max, comma-separated, no spaces after commas)

```
mail,stamp,route,logic,offline,brainteaser,delivery,commands,sorting,envelope,indie,relaxing
```
94 characters. Deliberately excludes "postmark," "protocol," "postal," "programming," "puzzle" since those are already indexed from the Name/Subtitle fields.

## Description (4000 char max)

```
Stamp the instructions. Send the route.

Postmark Protocol is a tactile offline puzzle about programming a letter's journey — not steering it. Read the route, place physical instruction stamps on the marked spaces, then press Dispatch and watch your plan play out exactly as written.

THE LOOP
Inspect the route. Stamp your commands — Turn Left, Turn Right, Wait, Reverse, Fast, Slow, Jump. Dispatch the letter. Watch it move. If it doesn't arrive, the reason is always clear: correct your stamps and try again.

36 HANDCRAFTED DELIVERIES
Six chapters, six routes each, each teaching one new idea at a time: turns and timing in the Local Route and Express Desk, reversing and revisiting in the Return Office, ink behavior in the Ink Lab, fragile/express/registered parcel rules in Priority Service, and every mechanic combined for a Night Shift finale.

INK, NOT JUST COMMANDS
Standard ink runs your stamp once. Double ink runs it twice. Permanent ink locks a stamp in place for that route. Every ink rule is shown with an icon and label, never color alone.

POSTAL SEALS, NOT STARS
Every delivery can earn three seals: Delivered for reaching the mailbox, Efficient for meeting the stamp budget, and Perfect Route for the optimal path. Your best result for each is kept — replaying never costs you progress.

COMPLETELY OFFLINE
No accounts. No ads. No analytics. No tracking. No network connection of any kind, checked and required. Your delivery record lives only on your device.

Postmark Protocol is built for landscape iPhone and iPad, with real music and sound-effect controls, a haptics toggle, and safe-area-aware layout on every screen.
```

## Screenshot caption suggestions

Existing `QA/Screens/` captures are legitimate candidates (game-accurate, not staged mockups — required by Guideline 2.3.3). Suggested caption per shot, kept short since App Store screenshot text overlays compete with the screenshot itself:

1. `main-menu-current.png` — "Your postal desk."
2. `gameplay-level1-current.png` — "Stamp the instructions."
3. `pointer-dispatch.png` — "Dispatch and watch it work."
4. `first-delivery-seals.png` — "Earn Delivered, Efficient, Perfect Route."
5. `settings-wide.png` — "No accounts. No ads. No tracking."
6. `gameplay-ipad.png` — "Built for iPhone and iPad."

## What this does not cover

Category selection, age rating questionnaire, privacy "nutrition label" answers, pricing, and territory availability are App Store Connect account decisions, not ASO copy — none of that is guessed here.
