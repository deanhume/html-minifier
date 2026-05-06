# HTML Minifier - Copilot Instructions

## Project Overview

HTML Minifier is a fast and efficient command-line tool for minifying HTML, Razor views, and Web Forms views. The tool removes unnecessary whitespace and comments to reduce file sizes and improve load times.

## Technology Stack

- **Language**: C# (.NET Framework 4.8)
- **Build System**: MSBuild / Visual Studio
- **Testing Framework**: MSTest
- **Project Structure**:
  - `ViewMinifier/` - Main application code
  - `HtmlMinifier.Tests/` - Unit tests
  - `Tool/` - Build tools and utilities

## Core Functionality

### Main Entry Point
- **Program.cs**: Main application entry point that processes command-line arguments and orchestrates minification

### Key Components

1. **Program.cs**: 
   - Handles command-line argument parsing
   - Processes files and directories recursively
   - Tracks minification statistics (total processed, saved bytes)
   - Supports batch processing of multiple files/folders

2. **StreamReaderExtension.cs**: 
   - Core minification logic
   - Removes HTML comments (with framework-aware options)
   - Removes JavaScript comments
   - Preserves `<pre>` tag content
   - Handles Razor syntax (`@model`, `@using`, `@inherits`)
   - Supports line length limits

3. **Features.cs**: 
   - Configuration class for minification options
   - Properties: `IgnoreHtmlComments`, `IgnoreJsComments`, `IgnoreKnockoutComments`, `MaxLength`

4. **StringExtension.cs**: 
   - File extension validation
   - Supported extensions: `.cshtml`, `.vbhtml`, `.aspx`, `.html`, `.htm`, `.ascx`, `.master`, `.inc`

## Command-Line Interface

### Usage Patterns
```bash
HtmlMinifier.exe "C:\Folder"                          # Process folder recursively
HtmlMinifier.exe "C:\Folder" "60000"                  # With max line length
HtmlMinifier.exe "C:\Folder" ignorehtmlcomments       # Preserve HTML comments
HtmlMinifier.exe "C:\Folder" ignorejscomments         # Preserve JS comments
HtmlMinifier.exe "C:\Folder" ignoreknockoutcomments   # Preserve Knockout.js comments
HtmlMinifier.exe "file1.html" "file2.html"            # Process specific files
```

### Feature Flags
- `ignorehtmlcomments` - Preserves HTML comments (for Angular, etc.)
- `ignorejscomments` - Preserves JavaScript comments
- `ignoreknockoutcomments` - Preserves Knockout.js-specific comments
- Numeric argument - Sets maximum line length

## Minification Rules

### What Gets Minified
1. **Whitespace**: Multiple spaces/newlines compressed to single space
2. **HTML Comments**: Removed (unless `ignorehtmlcomments` flag is set)
   - Exception: Preserves conditional comments (`<!--[if IE]>`)
   - Exception: Preserves `#include` virtual comments
3. **JavaScript Comments**: Removed from `<script>` blocks (unless `ignorejscomments` flag)
4. **Line breaks**: Removed between tags (`> <` becomes `><`)
5. **Razor triple-slash comments**: Removed (`/// comment`)

### What Gets Preserved
1. **Pre tags**: Content inside `<pre>` tags is completely preserved
2. **Razor declarations**: `@model`, `@using`, `@inherits` moved to their own lines
3. **String literals**: Content inside quotes is preserved
4. **Knockout comments**: Can be preserved with `ignoreknockoutcomments` flag
5. **UTF-8 BOM**: Preserved in output files

## Testing

### Test Structure
- Tests located in `HtmlMinifier.Tests/`
- Test categories:
  - `MinificationTests.cs` - Core minification logic tests
  - `ArgumentsTests.cs` - Command-line argument parsing tests
  - `FileExtensionTests.cs` - File extension validation tests
  - `EdgeCaseTests.cs` - Edge case handling
  - `PerformanceBenchmarkTests.cs` - Performance testing

### Running Tests
```bash
dotnet test HtmlMinifier.sln
```

## Code Style Guidelines

1. **Error Handling**: 
   - Use try-catch blocks for file I/O operations
   - Log errors to console with descriptive messages
   - Track error counts and exit with appropriate codes

2. **Naming Conventions**:
   - Public methods: PascalCase
   - Private fields: camelCase with underscore prefix
   - Properties: PascalCase

3. **Documentation**:
   - XML documentation comments on public methods
   - Clear parameter descriptions
   - Return value documentation

4. **Performance Considerations**:
   - Use `StringBuilder` for string concatenation
   - Regex operations with proper exception handling
   - Efficient file I/O with proper disposal patterns

## Special Considerations

### Razor-Specific Handling
- `@model` declarations are moved to the top of the file on their own line
- `@using` and `@inherits` get their own lines but aren't moved to top
- `@:` text syntax is converted to `<text></text>` tags

### Framework Compatibility
- **Knockout.js**: Use `ignoreknockoutcomments` to preserve `<!-- ko -->` comments
- **Angular**: Use `ignorehtmlcomments` to preserve directive comments
- **ASP.NET Web Forms**: Full support for `.aspx`, `.ascx`, `.master` files

### File Size Limits
- Max line length can be specified to avoid compiler limits (e.g., 65K character limit)
- Breaks lines at `><` boundaries when length exceeded

## Known Issues & Limitations

- Regex timeout handling for very large/complex files
- Requires appropriate file access permissions
- Windows-specific paths (uses backslashes)
- .NET Framework dependency (not cross-platform)

## Build & Release

- Solution file: `HtmlMinifier.sln`
- Output: `HtmlMinifier.exe` (command-line executable)
- CI/CD: GitHub Actions workflow (`.github/workflows/dotnet-desktop.yml`)

## Contributing Guidelines

When making changes:
1. Run all existing tests to ensure no regressions
2. Add tests for new features or bug fixes
3. Follow existing code style and patterns
4. Update documentation if adding new features
5. Consider backward compatibility with existing command-line arguments
6. Test with various file types (.cshtml, .aspx, .html, etc.)

## Common Tasks

### Adding a New Feature Flag
1. Update `Features.cs` constructor to parse the new flag
2. Add property to `Features` class
3. Update minification logic to respect the flag
4. Add tests in `ArgumentsTests.cs` and `MinificationTests.cs`
5. Update README.md with usage examples

### Fixing a Minification Bug
1. Add a failing test case in `MinificationTests.cs`
2. Fix the issue in `StreamReaderExtension.cs`
3. Verify all tests pass
4. Consider edge cases and add additional tests

### Adding Support for New File Extensions
1. Update `StringExtension.IsHtmlFile()` method
2. Add test in `FileExtensionTests.cs`
3. Update documentation

## License
MIT License - Copyright (C) Dean Hume 2013 - 2025
