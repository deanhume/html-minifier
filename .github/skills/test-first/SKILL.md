---
name: test-first
description: >-
  Enforces a strict test-driven development (TDD) workflow for the HTML Minifier
  repo. Use this skill whenever you are asked to add, change, or fix any
  behavior in the minification pipeline, feature flags, file-extension handling,
  or any other production code. It forces you to write a failing test first,
  run it to watch it fail (red), and then ask the user for permission before
  writing any implementation code.
---

# Test-First (TDD) Skill

This skill makes you follow a strict **red → confirm → green** workflow. You
**must not** write or modify production code before completing the steps below
in order. There are no exceptions unless the user explicitly says "skip the
test" or "no test needed" for this change.

## Mandatory workflow

### 1. Write the test first

Before touching any production code, write a test that captures the desired new
behavior or reproduces the bug.

- Tests live in `HtmlMinifier.Tests/`. Behavior tests go in
  `MinificationTests.cs`, flag-parsing tests in `ArgumentsTests.cs`, and file
  extension tests in `FileExtensionTests.cs`.
- Tests call `StreamReaderExtension.MinifyHtmlCode(string, Features)` directly —
  no file I/O.
- Put HTML input strings and expected-output strings in
  `HtmlMinifier.Tests/DataHelpers.cs` as static string constants
  (raw `\r\n`-delimited).
- For bug fixes, follow the naming pattern
  `GithubIssueNN_ShouldReturnCorrectly` and add a comment linking to the issue.
- Use `noFeatures` (`new Features(new string[0])`) when no flags are needed.

The test **must** assert the *target* behavior (what the code should do after
your fix), so that it fails against the current code.

### 2. Run the test and watch it fail (RED)

Run the new test and confirm it fails for the right reason (an assertion
failure showing the gap, not a compile error or unrelated crash):

```bash
dotnet test HtmlMinifier.Tests\HtmlMinifier.Tests.csproj --filter "FullyQualifiedName~<YourTestName>"
```

- If it **passes**, the test is not actually exercising new behavior — fix the
  test before continuing.
- If it fails to **compile**, fix only the test code (or add the minimal new
  member signature it references) until it compiles and fails on the assertion.

Show the user the failing test output so the red state is visible.

### 3. Ask the user before implementing (CONFIRM)

After demonstrating the failing test, you **must** stop and ask the user for
permission to proceed using the `ask_user` tool. Do not write implementation
code in the same turn.

Ask a clear yes/no question, for example:
> "The test is written and failing as expected. May I proceed to implement the
> fix?"

Only continue once the user approves. If they want changes to the test or
approach, revise and re-run before asking again.

### 4. Implement until green (GREEN)

Once approved, write the minimal production code (in `StreamReaderExtension.cs`,
`Features.cs`, `StringExtension.cs`, or `Program.cs` as appropriate) to make the
test pass. Then re-run the focused test, and finally the full suite to confirm
no regressions:

```bash
dotnet test HtmlMinifier.sln
```

## Hard rules

- **Never** write or edit production code before the test exists, fails, and the
  user has approved.
- **Never** weaken or delete the failing assertion just to make it pass — make
  the production code satisfy the test.
- If the user asks you to skip the test, comply but state clearly that the TDD
  workflow was bypassed at their request.
