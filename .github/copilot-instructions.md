# HTML Minifier — Copilot Instructions

## Build & Test

```bash
# Full test suite
dotnet test HtmlMinifier.sln

# Single test by name
dotnet test HtmlMinifier.Tests\HtmlMinifier.Tests.csproj --filter "FullyQualifiedName~GithubIssue54_ShouldReturnCorrectly"

# Build
dotnet build HtmlMinifier.sln
```

## Architecture

The tool is a C# (.NET Framework 4.8) CLI that minifies HTML, Razor (`.cshtml`/`.vbhtml`), and Web Forms (`.aspx`/`.ascx`/`.master`) files.

**Data flow:**
1. `Program.cs` — parses CLI args, builds a `Features` config, walks file/folder paths, calls `StreamReaderExtension.MinifyHtmlCode`, writes output in-place.
2. `Features.cs` — parses all CLI flags (`ignorehtmlcomments`, `ignorejscomments`, `ignoreknockoutcomments`, numeric `MaxLength`) from `string[] args`.
3. `StreamReaderExtension.cs` — all minification logic lives here as static methods. `MinifyHtmlCode(string, Features)` is the main entry point; it calls:
   - `MinifyHtml` — Regex-based whitespace/comment stripping
   - `EnsureMaxLength` — splits output at `><` boundaries if over limit
   - `ReArrangeDeclarations` — moves `@model` to top; adds newline after `@using`/`@inherits`
4. `StringExtension.cs` — `IsHtmlFile()` extension method for file extension gating.

## Key Conventions

### Minification pipeline order (in `MinifyHtml`)
1. Remove JS comments from `<script>` blocks (unless `IgnoreJsComments`)
2. Extract `<pre>` blocks using `{{PRE_TAG_CONTENT_N}}` placeholders to protect their contents
3. Escape `/*` as `{{{SLASH_STAR}}}` before running the CSS multiline comment regex, then restore it afterward
4. Replace `@:` lines with `<text>...</text>` tags (`ReplaceTextLine`)
5. Remove `/// ...` (Razor triple-slash) and `// ...` (JS single-line) comments
6. Collapse whitespace, remove whitespace around tags (`> <` → `><`)
7. Remove HTML comments — with special exceptions for `<!--[if ...]>` (IE conditionals) and `<!-- #include virtual` — and optionally preserve `<!-- ko` (Knockout) comments
8. Restore placeholders

### Razor declaration handling
- **`@model`** is extracted and inserted at the very top of the file.
- **`@using`** and **`@inherits`** get a `\r\n` appended but stay in place.

### Testing patterns
- Tests call `StreamReaderExtension.MinifyHtmlCode(string, Features)` directly — no file I/O.
- All test HTML strings and expected results live in `HtmlMinifier.Tests/DataHelpers.cs` as static string constants (raw `\r\n`-delimited).
- Bug-fix tests follow the naming pattern `GithubIssueNN_ShouldReturnCorrectly` with a comment linking to the issue URL.
- `noFeatures` is the canonical zero-config `Features` instance: `new Features(new string[0])`.

### Adding a new feature flag
1. Add a property to `Features.cs`.
2. Parse it in the `Features` constructor using `args.Contains(...)`.
3. Wire up the behavior in `StreamReaderExtension.MinifyHtml`.
4. Add tests in `ArgumentsTests.cs` (flag parsing) and `MinificationTests.cs` (behavior).

### Adding a new file extension
1. Update `StringExtension.IsHtmlFile()`.
2. Add a test in `FileExtensionTests.cs`.
