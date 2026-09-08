# Contributing to HTML Minifier

Bug fixes, tests, documentation improvements, and focused enhancements are
welcome. By submitting a contribution, you agree that it may be distributed
under the project's [MIT License](LICENSE.md).

For substantial changes, open an issue before starting work so the proposed
behavior and scope can be agreed upon. Bug reports and enhancement proposals
should follow [SUPPORT.md](SUPPORT.md).

## Development requirements

- Windows.
- Git.
- Visual Studio 2022 or Visual Studio Build Tools.
- The .NET Framework 4.8 targeting pack.
- A .NET SDK capable of running the repository's `dotnet` commands.

Clone your fork and create a branch from `master`:

```powershell
git clone https://github.com/<your-user-name>/html-minifier.git
Set-Location html-minifier
git switch -c fix/short-description
```

## Build and test

Build the solution:

```powershell
dotnet build HtmlMinifier.sln
```

Run the full test suite:

```powershell
dotnet test HtmlMinifier.sln
```

Run a specific test when iterating:

```powershell
dotnet test HtmlMinifier.Tests\HtmlMinifier.Tests.csproj --filter "FullyQualifiedName~TestName"
```

## Requirements for acceptable contributions

A contribution should:

- Address a single, clearly described problem or enhancement.
- Remain compatible with Windows and .NET Framework 4.8.
- Preserve existing command-line and minification behavior unless the change is
  intentional, documented, and covered by tests.
- Include tests for production-code changes and regression tests for bug fixes.
- Pass the full build and test suite.
- Avoid unrelated formatting, refactoring, dependency, or generated-file changes.
- Update user-facing documentation when commands, flags, supported file types, or
  output behavior change.
- Contain no secrets, credentials, private data, or code the contributor does not
  have permission to submit.
- Be licensed under the MIT License.

## Coding standard

Follow the existing style in the files you modify and the
[Microsoft C# coding conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
where the repository does not establish a local convention.

In particular:

- Use four spaces for indentation and do not introduce tabs.
- Keep code compatible with the language version and framework used by the
  existing project; do not migrate the project format or target framework as part
  of an unrelated contribution.
- Use descriptive names and keep methods focused.
- Reuse existing helpers and patterns instead of duplicating logic.
- Handle errors explicitly; do not silently ignore invalid input or failures.
- Add comments only when they explain behavior that is not clear from the code.

Tests use MSTest. Minification tests should call
`StreamReaderExtension.MinifyHtmlCode(string, Features)` directly and avoid file
I/O unless file handling is the behavior under test. Add shared input and expected
output strings to `HtmlMinifier.Tests\DataHelpers.cs` when that matches the
existing test pattern.

## Pull requests

Before opening a pull request:

1. Rebase or merge the latest `master` branch into your branch.
2. Review the diff and remove unrelated changes.
3. Run `dotnet build HtmlMinifier.sln` and `dotnet test HtmlMinifier.sln`.
4. Write a concise title and explain what changed, why it changed, and how it was
   tested.
5. Link the relevant issue, if one exists.

Maintainers may request changes when a pull request does not meet these
requirements. Acceptance is based on correctness, compatibility, test coverage,
maintainability, and fit with the project's scope.
