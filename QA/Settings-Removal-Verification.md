# Settings removal verification

Handbook: 1278x1044: PASS text extents and screen bounds

MainDesk Settings: 1278x1044: PASS text extents and screen bounds

MainDesk Settings/Reset/Cancel/Close: PASS; no reset confirmed

Level 2: 2778x1284: PASS text extents and screen bounds

Gameplay Settings: 2778x1284: CloseLabel outside screen; Close control outside screen

Gameplay Settings/Reset/Cancel/Close: PASS; no reset confirmed

Gameplay Settings, separate frame recheck: 2778x1284: PASS text extents and screen bounds

## Follow-up verification (2026-08-28)

Both saved scenes contain only Reset Progress and Close Settings in the lower Settings button area. Privacy/Support panels and runtime URL/email handlers are absent. Reset is centered at (0, -133), size 430x80. CompilationPipeline: 0 errors/warnings. Phone and iPad Settings candidates were recaptured, visually inspected and normalized to RGB; the manifest was refreshed.

The full follow-up sweep retained two bounds findings, so QA/Policy-UI-Verification.md correctly remains FAIL rather than being relabeled. Both occurred at unexpected Game View sizes (Level 2 at 1278x1044 during the 2778x1284 pass; Handbook at 1278x36 during the 2732x2048 pass). Fixed-size targeted rechecks passed at 2778x1284 and 1278x1044 respectively. One immediate same-call Gameplay Settings check also flagged Close; the separate-frame recheck passed. These results do not establish continuous resizing or exhaustive hover-state coverage.

All four resolutions passed menu and gameplay Settings bounds checks and reset failure/retry checks in the sweep. Four representative deliveries passed. The isolated QA repository was restored. Targeted Settings > Reset > Cancel > Close navigation passed in both scenes without confirming any reset on the user's save. No physical tests, iOS exports/builds, signing or Apple login were performed.
