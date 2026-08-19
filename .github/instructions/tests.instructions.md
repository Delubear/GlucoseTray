---
applyTo: "GlucoseTray.Tests/**/*.cs"
---

# GlucoseTray – Test Conventions

## Approach
- Practice **BDD / TDD / ATDD wherever reasonable and valuable**: write or adjust a failing
  test first, make it pass, then refactor. Describe behavior, not implementation details.
- Frameworks: **NUnit** (`[Test]`) + **NSubstitute** for fakes. Do not introduce other
  test/mocking libraries.
- Name tests for the behavior and condition, e.g.
  `ShouldReEncryptCredentialsWhenConfigurationChanges`.

## Use and grow the fluent DSL
Tests should read like plain-language scenarios by reusing the drivers under
`GlucoseTray.Tests/DSL` (`Read` and `Display`). Each area follows the same pattern:

- **Provider** – owns the NSubstitute fakes and constructs the system under test.
- **Driver** – fluent `Given…` / `With…` arrange steps and the `When…` act step.
- **AssertionDriver** – fluent `Then…` assertions.
- **BehaviorDriver** – ties Given/When/Then together for a scenario.

### Rules
- **Do not duplicate substitute wiring** in individual test methods. If setup is missing,
  **add a new `Given/With/When/Then` step to the existing driver** and reuse it.
- Keep steps small, chainable (`return this;`), and named after domain concepts
  (glucose, reading, trend, threshold, stale, Dexcom, Nightscout).
- Prefer extending a driver over adding ad-hoc `Substitute.For<>()` calls inside a test.
- When a new feature needs a new seam, add the interface + a driver step rather than
  reaching into concrete types.

## Domain reminders
- Assert real glucose behavior: unit handling (mg/dL vs mmol/L), low/high/critical
  thresholds, stale (out-of-date) readings, and dark-mode display.
- Credentials are encrypted at rest; when testing credential flows, substitute
  `ICredentialProtector` / `ICredentialMigrator` rather than touching real files where possible.
