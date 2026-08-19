# GlucoseTray – Copilot Instructions

## What this project is
GlucoseTray is a Windows taskbar (system tray) app that helps **diabetics using CGMs
(continuous glucose monitors), primarily Dexcom** view their **most recent blood glucose
(BG) reading at a glance** from their taskbar.

- **Windows-only** at the moment (Windows Forms). Do not assume cross-platform APIs.
- Reads from **two supported platforms today: Dexcom and Nightscout.** New data sources
  should plug in behind the existing read abstractions (`IGlucoseReader` / read strategies)
  without special-casing callers.

Because the users depend on this for health-related information, correctness, clarity, and
reliability matter more than cleverness.

## Audience & documentation
- **Non-technical users come first.** README, settings docs, error messages, and issue
  templates should use plain, friendly language and avoid jargon. Explain *why* and *how*,
  not just *what*.
- Expand technical/architecture notes separately so they never clutter the user-facing docs.
- Keep glucose/medical wording accurate (BG, mg/dL, mmol/L, high/low/critical thresholds),
  and never imply the app should be used for treatment decisions without confirming on the
  official device.

## The settings file (`appsettings.json`) is a user-facing surface
- **Non-technical users edit `appsettings.json` by hand in plain text editors like Notepad.**
  Making that simple and safe is paramount.
- Keep the file forgiving: sensible defaults, clear property names/ordering, and options
  hints (e.g. the `*_OPTIONS` properties) so users know valid values.
- Avoid changes that make hand-editing error-prone (cryptic keys, required fields with no
  defaults, formats that break easily). Prefer resilient parsing and graceful fallback over
  crashing on a malformed edit.
- Credentials must stay protected even as users edit the file (see Security).

## Tech stack
- **.NET 10**, Windows Forms tray app targeting `net10.0-windows7.0`.
- Test project uses **NUnit** + **NSubstitute**.
- Configuration via `appsettings.json` bound to `AppSettings`, reloaded on change.

## Coding style & practices
- Favor **expressive, readable code** and **modern .NET practices** (primary constructors,
  pattern matching, collection expressions, `IOptionsMonitor`, dependency injection, async/await).
- Prefer **interfaces + constructor injection** so behavior is testable (e.g.
  `ICredentialProtector`, `ICredentialMigrator`, `IGlucoseReader`).
- Do **not** add `#region` / `#endregion` blocks.
- Keep methods small and intention-revealing; name things after domain concepts
  (glucose, reading, trend, threshold, stale).
- Only add comments when they explain non-obvious *why*, matching existing style.

## Testing philosophy
- Use **BDD / TDD / ATDD wherever it is reasonable and adds value.** Prefer writing/adjusting
  a failing test first, then making it pass, then refactoring.
- Reuse and extend the **fluent test DSL** under `GlucoseTray.Tests/DSL` rather than
  re-writing setup. Add new `Given/With/When/Then` steps to the existing drivers instead of
  duplicating substitute wiring in individual tests.
- See `.github/instructions/tests.instructions.md` for detailed test conventions.

## Security
- Dexcom passwords and Nightscout tokens are **encrypted at rest** in `appsettings.json`
  using Windows DPAPI (current-user scope) via `ICredentialProtector` / `ICredentialMigrator`.
- Values are re-encrypted on config change and decrypted only at point of use. Preserve this
  behavior; never log or persist credentials in plaintext.

## Distribution
- We ship **two executables** from the same codebase:
  - **`GlucoseTray.exe`** — self-contained, the .NET runtime is **bundled** (larger, no install needed).
  - **`GlucoseTray-Slim.exe`** — framework-dependent, **requires** the matching .NET runtime installed.
- Both are single-file Windows builds produced from publish profiles under
  `GlucoseTray/Properties/PublishProfiles`. Keep both profiles working when changing build/publish config.

## Build & test
- Build: `dotnet build`
- Test: `dotnet test`
- CI: `pr.yml` builds/tests/publishes both profiles on PRs; `generate-release.yml` publishes
  and creates a GitHub Release on `v*` tags.
