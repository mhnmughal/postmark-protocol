# Network audit

> **Current configuration, 2026-08-28:** EngineDiagnosticsSettings.enabled, its default-build value, and PlayerSettings.submitAnalytics were verified false after remediation. Insights, Analytics/Ads initialization, cloud crash and performance reporting are disabled. No active custom build-profile override exists. Privacy/Support UI and runtime URL/email actions were subsequently removed by request. These are configuration/source findings, not final-binary or packet-capture certification. See [POLICY_REMEDIATION.md](I:/UnityProject/PostmarkProtocol/Documentation/POLICY_REMEDIATION.md); historical export evidence below remains excluded from current validation.

Date: 2026-08-26. Status: static game-source audit clear at this snapshot; final compiled-artifact and offline-device checks pending.

## Game runtime

Case-insensitive searches of game C# under `Assets` for `UnityWebRequest`, `HttpClient`, `System.Net`, sockets, Firebase, analytics, remote configuration, advertisements, web views, `Application.OpenURL`, and HTTP/HTTPS URLs found no runtime call sites at this audit snapshot.

`PostmarkProtocol.Runtime.asmdef` references only `Unity.ugui` and `Unity.InputSystem`. No third-party networking, advertising, tracking, analytics, or cloud SDK was identified in the manifest. Local save data and authored local assets do not require an account or remote endpoint.

Unity's analytics module and unused web-request asset-bundle/audio/WWW modules were removed through Package Manager. Package inclusion alone does not establish whether native networking code appears in the final stripped executable; this remains a build-artifact inspection task.

## Editor-only networking is intentional

AnkleBreaker MCP is the required local development bridge. Its exact assembly definitions were inspected:

| Assembly definition | Compilation restriction |
|---|---|
| `Library/PackageCache/com.anklebreaker.unity-mcp@9032874ff0b8/Editor/AnkleBreaker.UnityMCP.Editor.asmdef` | `includePlatforms: ["Editor"]` |
| `Library/PackageCache/com.anklebreaker.unity-mcp@9032874ff0b8/1-Scripts/Editor/WelcomeWindow/UnityMCP.Editor.Welcome.asmdef` | `includePlatforms: ["Editor"]` |

Observed Editor source call sites include:

- `Editor/MCPBridgeServer.cs`: `System.Net.HttpListener` for the development bridge.
- `Editor/MCPInstanceRegistry.cs`: `System.Net` / sockets for Editor instance discovery.
- `Editor/MCPUpdateChecker.cs`: `UnityWebRequest` for package-update checks.
- `Editor/MCPNewsService.cs`: `UnityWebRequest` for package news.
- `1-Scripts/Editor/WelcomeWindow/UnityMcpWelcomeDevlogFetcher.cs`: `UnityWebRequest` and `UnityWebRequestTexture` for the welcome window.

Those files are inside Editor-restricted assemblies. This is why the engine's core and texture web-request modules remain installed: they are compile dependencies of development tools, not game runtime call sites. Their existence must not be confused with permission to add networking to the game. All Unity operations in this task used MCP tools rather than direct bridge HTTP calls.

## Remaining release verification

1. ~~Repeat the static scan after final code and native haptics integration; review any new native plugin.~~ Done 2026-08-26: rescanned `Assets/Scripts` and `Assets/Plugins`, still clean.
2. ~~Inspect the release BuildReport, exported Xcode sources/frameworks, Info.plist, and privacy manifests. Confirm no Editor assembly, analytics SDK, or unrelated native framework ships.~~ Done 2026-08-26: release IL2CPP Xcode export succeeded (0 errors/0 warnings, `Builds/iOS-Release`). Searched the actual Xcode-buildable target directories (`Unity-iPhone/`, `Libraries/`, `Classes/`, `Data/`) — no Firebase/AdMob/Analytics/Facebook framework, no AnkleBreaker/Editor assembly in `Data/Managed`. The only "Analytics" hit anywhere under the build output was Unity's own IL2CPP transpiler toolchain scratch files in `Il2CppOutputProject/IL2CPP/build/deploy_*`, which is not part of the app the Xcode project compiles. Exported `Info.plist` carries no camera/location/microphone/Bluetooth/photo-library usage-description key. See `QA_REPORT.md`.
3. Install on iPhone/iPad and run in airplane mode from first launch through tutorial, level progression, settings, quit/relaunch, and saved progress. **Still open — EXTERNAL, no physical device on this host.**
4. Observe network activity for the installed release build. Editor traffic is not a valid measurement of player behavior. **Still open — EXTERNAL, same reason as #3.**
5. Record device/OS/build number, test results, and artifact path in the final QA/release audit.

Do not promote this static audit to compiled-binary verification or an end-to-end offline PASS without those observations.
