# Postmark Protocol — AI development rules

1. Never replace production systems with placeholders.
2. Never silently remove a requirement from `Assets/Documentation/PRODUCTION_BRIEF.md`.
3. Never create buttons that do nothing.
4. Never create fake settings.
5. Investigate compiler warnings caused by project code.
6. No runtime game networking. Privacy/Support controls and external browser/mail actions were removed at the user's request.
7. No analytics.
8. No advertisements.
9. No tracking.
10. No third-party SDKs without explicit authorization. AnkleBreaker is authorized Editor tooling only.
11. No remote configuration.
12. No hidden gameplay modes.
13. No production debug tools.
14. Never generate the primary production UI entirely at runtime.
15. Important UI must be authored GameObjects saved in the scene hierarchy.
16. Major environment composition must be visible in Edit Mode.
17. Inspect and respect existing architecture before modification. Do not destroy working systems.
18. Update `MEMORY_BANK.md` after major changes.
19. Test changes after implementation.
20. Never claim verification without observed evidence.
21. Never mark the game complete with any acceptance gate unfinished.
22. Maintain responsive landscape UI and safe-area support.
23. Document the legal origin of every shipped audio and graphical asset.
24. Do not copy another game's assets, code, scenes, or layouts.
25. Maintain coherent postal mechanics and visual design.
26. Use AnkleBreaker MCP for Unity operations. Target port 7890 explicitly; never call its internal HTTP bridge directly.
27. Coordinate parallel ownership. Scene authorship is owned by the lead agent. Do not save over another agent's scene.
28. Debug/editor utilities belong in Editor assemblies; no remote push or new repository without authorization.
29. Record unexecuted device, signing, export, and manual QA gates honestly. A deterministic simulation is not a manual playtest.
