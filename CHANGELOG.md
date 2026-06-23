# Changelog

All notable changes to HTML Minifier will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased] — v1.9.3

### Fixed
- **Issue #47** — Regex literals containing `//` (e.g. `/\//g`) were incorrectly stripped as JS line comments
- **Issue #30** — Downlevel-revealed IE conditional comments (`<!--[if !IE]><!--> ... <!--<![endif]-->`) were being partially stripped
- **Issue #38** — Edge case fixes in HTML structure handling

### Added
- **`<textarea>` and `<code>` whitespace protection** — content inside these tags is now preserved exactly, matching the existing `<pre>` tag behaviour
- **Attribute whitespace normalisation** — excess whitespace inside attribute values (e.g. `class="  foo   bar  "`) is collapsed to single spaces
- **Parallel file processing** — files and folders are now processed in parallel for significantly faster batch runs
- **`--version` and `--help` CLI flags** — new command-line options for quick reference
- **Improved console reporting** — output now shows bytes saved per file and total savings across a run
- **GitHub Pages site** — marketing/documentation site added at `docs/index.html`
- **81 tests** covering issues #47, #30, #49, textarea/code protection, attribute normalisation, edge cases, and performance benchmarks

### Changed
- Updated to Visual Studio 2022 build toolchain
- GitHub Actions CI/CD workflow added

---

## [v1.9.2] — Previous Release

- Parallel processing improvements
- Performance and reporting enhancements

## [v1.9.1]

- Minor bug fixes

## [v1.9]

- General improvements and stability fixes

## [v1.8]

- Added support for additional file types

## [v1.7]

- Performance improvements

## [v1.6]

- Bug fixes

## [v1.5]

- Added Knockout.js comment support (`ignoreknockoutcomments` flag)

## [v1.4]

- Added JavaScript comment removal

## [v1.3]

- Added Razor syntax support (`@model`, `@using`, `@inherits`)

## [v1.2]

- Initial stable release

[Unreleased]: https://github.com/deanhume/html-minifier/compare/v1.9.2...HEAD
[v1.9.2]: https://github.com/deanhume/html-minifier/compare/v1.9.1...v1.9.2
[v1.9.1]: https://github.com/deanhume/html-minifier/compare/v1.9...v1.9.1
[v1.9]: https://github.com/deanhume/html-minifier/compare/v1.8...v1.9
[v1.8]: https://github.com/deanhume/html-minifier/compare/v1.7...v1.8
[v1.7]: https://github.com/deanhume/html-minifier/compare/v1.6...v1.7
[v1.6]: https://github.com/deanhume/html-minifier/compare/v1.5...v1.6
[v1.5]: https://github.com/deanhume/html-minifier/compare/v1.4...v1.5
[v1.4]: https://github.com/deanhume/html-minifier/compare/v1.3...v1.4
[v1.3]: https://github.com/deanhume/html-minifier/compare/v1.2...v1.3
