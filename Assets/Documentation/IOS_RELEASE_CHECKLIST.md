# iOS release checklist

Audit date: 2026-08-26. This file records observed configuration, not a claim of App Store approval or completed physical-device QA. Update build/test rows with the actual final integration evidence.

## Verified project configuration

| Item | Observed value | Evidence |
|---|---|---|
| Unity | 6000.3.19f1, WindowsEditor | MCP `Application.unityVersion` / `Application.platform` |
| Product | Postmark Protocol | PlayerSettings readback / ProjectSettings.asset |
| Company | RehmanMughal | PlayerSettings / serialized settings |
| Bundle ID | com.rehmanmughal.postmarkprotocol | iOS application identifier |
| Version / build | 1.0.0 / 1 | PlayerSettings / serialized settings |
| Scripting backend | IL2CPP | MCP readback |
| Architecture | ARM64 (iOS architecture value 1) | MCP readback |
| Graphics | Metal only | MCP readback |
| Minimum OS | iOS 15.0 | Existing baseline retained |
| Devices | iPhoneAndiPad | MCP readback |
| Orientation | Autorotation, landscape left/right only | MCP readback: portrait False, upside-down False, left True, right True |
| iPad full screen | True | MCP readback; avoids portrait multitasking contract |
| Managed stripping | Low | MCP readback |
| Unity splash | Disabled | MCP readback |
| iOS module | Installed and supported | `BuildPipeline.IsBuildTargetSupported(iOS,iOS) = True` |
| Icon source | Assets/Art/AppIcon.png, 1024 x 1024 RGB24 opaque | Native image inspection; SHA-256 in ART_LICENSES.md |
| Icon assignment | Default and 19/19 iOS platform slots | Application 5, Spotlight 4, Settings 5, Notifications 4, Marketing 1; every slot read back correct asset |
| Small icon QA | PASS at 1024, 180, 120, 60 pixels | Visual inspection; QA/Icon thumbnails |
| Sensitive permission descriptions | Camera, microphone, location empty; Bluetooth serialized empty | PlayerSettings and serialized settings |

## Package and network audit

Manifest audit found no ad SDK, Firebase, remote-config service, web-view SDK, or third-party tracking framework. Eighteen unused template/network packages were removed through Package Manager and verified absent from manifest and lockfile. Eight unreferenced template assets were moved to OS trash, including the URP settings. See PACKAGE_AUDIT.md for exact removals and retained engine/Editor dependency rationale. This is not yet a final stripped-player package PASS.

Retain dependencies actually used by the game: Input System, UGUI, Unity Test Framework, necessary built-in engine modules, and AnkleBreaker MCP for development. Production rendering uses built-in forward; URP, rendering core, and Shader Graph are removed. Both AnkleBreaker assembly definitions explicitly restrict compilation to `Editor`, so its development HTTP bridge is not a player runtime network dependency. Its Editor news/devlog features require the retained core/texture UnityWebRequest modules. No network action should be added to runtime gameplay. See NETWORK_AUDIT.md.

Source scan at this audit time found no matches in Assets C# for `UnityWebRequest`, `HttpClient`, `System.Net`, sockets, Firebase, analytics, remote config, advertisements, web views, or `Application.OpenURL`. Runtime code was still under active development, so rerun this scan on final sources and inspect the exported player before declaring the final offline audit passed. Presence of a built-in web-request module is not evidence of a call, but remove unused modules where dependency resolution allows.

## Build and release gates

| Gate | Status / next evidence required |
|---|---|
| Editor build target | iOS (confirmed via `unity_project_info`) |
| Automated tests | PASS — 66/66 EditMode, job `37cdc16334b7`. See `QA_REPORT.md`. |
| Development build | PASS — `unity_build` IL2CPP Xcode export, 0 errors/0 warnings, output `Builds/iOS`, 48.2s. |
| iOS Xcode export | PASS — release export also succeeded, 0 errors/0 warnings, output `Builds/iOS-Release`, 16.4s. `Unity-iPhone.xcodeproj` present and inspected. |
| Exported icon catalog | PASS — `Images.xcassets/AppIcon.appiconset` contains all 19 slots in the actual exported release directory, not just the Player Settings assignment. |
| Exported Info.plist | PASS — landscape-left/right only, no portrait; no camera/location/microphone/Bluetooth/photo-library usage-description keys present at all. |
| Exported artifact SDK/network scan | PASS — no Firebase/AdMob/Analytics/Facebook/network call sites in the Xcode-buildable target directories (`Unity-iPhone/`, `Libraries/`, `Classes/`, `Data/`); no AnkleBreaker/Editor assembly in `Data/Managed`. |
| Physical iPhone/iPad QA | EXTERNAL — not performed; no device attached to this host. Verify launch, all levels, persistence, audio, landscape safe areas, thermal behavior and frame time on devices. |
| Apple signing | EXTERNAL — account credentials absent: team ID False / manual provisioning profile False by MCP query. |
| Xcode archive | EXTERNAL — host is WindowsEditor. Requires a macOS/Xcode host and the developer's authorized signing identity. |
| App Store Connect | EXTERNAL — requires developer account access, metadata, privacy declarations, screenshots, signed upload, and review; not performed. |

No personal-data permission is required by the game's design. Final exported Info.plist, privacy manifests, linked frameworks, and App Store privacy answers must agree with the compiled artifact; do not infer submission compliance merely from empty Unity descriptions. Confirm current Apple submission requirements at release time.

## Reproduction

All Unity configuration and reads above used AnkleBreaker `unity_execute_code` on project port 7890. Never call the bridge HTTP endpoint directly. Editor path observed: `L:/Unity Engine/Editors/6000.3.19f1/Editor/Unity.exe`.

Do not mark the game fully released until every build, device, signing, and submission gate above has direct evidence.
