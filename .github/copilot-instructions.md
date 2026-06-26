# Copilot Instructions

## Build & Test

```bash
# Build the solution
msbuild HtmlMinifier.sln /p:Configuration=Debug

# Run all tests
dotnet test HtmlMinifier.sln

# Run a specific test by name
dotnet test HtmlMinifier.sln --filter "FullyQualifiedName~MinificationTests.ReadHtml_WithStandardText_ShouldReturnCorrectly"
```

## Architecture

This is a .NET Framework 4.8 console application that minifies HTML/Razor/WebForms files in-place.

- **ViewMinifier/** — Main application (outputs `HtmlMinifier.exe`)
  - `Program.cs` — CLI entry point, file/directory traversal, argument parsing
  - `StreamReaderExtension.cs` — Core minification engine (regex-based whitespace/comment removal, `<pre>` preservation, Razor declaration re-arrangement)
  - `Features.cs` — Configuration parsed from CLI args (`IgnoreHtmlComments`, `IgnoreJsComments`, `IgnoreKnockoutComments`, `MaxLength`)
  - `StringExtension.cs` — `IsHtmlFile()` extension for supported file types
- **HtmlMinifier.Tests/** — MSTest unit tests with input/expected pairs in `Data/` folder

The minification pipeline in `StreamReaderExtension.MinifyHtmlCode` follows this order:
1. Remove JS comments from `<script>` blocks
2. Extract and preserve `<pre>` tag content
3. Strip CSS/multi-line comments
4. Convert `@:` Razor text lines to `<text></text>`
5. Collapse whitespace and remove line breaks between tags
6. Remove HTML comments (respecting feature flags)
7. Restore `<pre>` content
8. Enforce max line length by splitting at `><` boundaries
9. Re-arrange Razor declarations (`@model` to top, `@using`/`@inherits` on own lines)

## Key Conventions

- Tests use the Arrange/Act/Assert pattern with test data loaded from `HtmlMinifier.Tests/Data/` (pairs of `Foo.txt` input and `FooResult.txt` expected output)
- The tool modifies files **in-place** — it overwrites the original with minified content
- All public methods have XML doc comments
- Feature flags are positional CLI arguments (not `--flag` style): `ignorehtmlcomments`, `ignorejscomments`, `ignoreknockoutcomments`
- Supported file extensions: `.cshtml`, `.vbhtml`, `.aspx`, `.html`, `.htm`, `.ascx`, `.master`, `.inc`
- Static fields `totalProcessed`/`totalSaved` on `Program` track cumulative byte counts across a run
