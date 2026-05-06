# HTML Minifier

![Github Actions Status](https://github.com/deanhume/html-minifier/actions/workflows/dotnet-desktop.yml/badge.svg)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)

A fast and efficient command-line tool to minify your HTML, Razor views, and Web Forms views. Reduce file sizes, improve load times, and deliver a better user experience by removing unnecessary whitespace and comments from your HTML files.

## ✨ Features

- 🚀 **Fast Processing** - Minifies files and entire directory trees quickly
- 📁 **Flexible Input** - Process individual files, specific folders, or entire directory structures
- ⚙️ **Configurable** - Control line length and selectively disable minification features
- 🔧 **Framework Support** - Special handling for Knockout.js, Angular, and other comment-dependent frameworks
- 🎯 **Targeted Minification** - Choose what to minify: HTML comments, JavaScript comments, or Knockout comments
- 🔄 **CI/CD Ready** - Easy integration with MSBuild and build pipelines

## 📦 Installation

1. Download the latest release from the [releases page](https://github.com/deanhume/html-minifier/releases)
2. Extract the `HtmlMinifier.exe` to your desired location
3. Add the tool to your PATH or reference it directly

## 🚀 Quick Start

Transform verbose HTML into compact, optimized code:

### Before:

```html
<h2>
    Admin Menu</h2>
<ul>
    <li>@Html.ActionLink("Edit blog entries", "List", "Admin")</li>
    <li>@Html.ActionLink("View Comments", "CommentList", "Admin")</li>
    <li>@Html.ActionLink("Clear Cache", "ClearCache", "Admin")</li>
</ul>
```

### After:

```html
<h2> Admin Menu</h2><ul><li>@Html.ActionLink("Edit blog entries", "List", "Admin")</li><li>@Html.ActionLink("View Comments", "CommentList", "Admin")</li><li>@Html.ActionLink("Clear Cache", "ClearCache", "Admin")</li></ul>
```

## 📖 Usage

### Getting Help

View all available options:

```bash
HtmlMinifier.exe --help
```

Check the version:

```bash
HtmlMinifier.exe --version
```

### Basic Usage

Minify all HTML files in a folder (including subfolders):

```bash
HtmlMinifier.exe "C:\Folder"
```

### Advanced Options

#### Limit Line Length

Restrict the maximum number of characters per line:

```bash
HtmlMinifier.exe "C:\Folder" "60000"
```

#### Preserve Comments

For frameworks that rely on HTML comments (Angular, Knockout, etc.), preserve them:

```bash
HtmlMinifier.exe "C:\Folder" ignorehtmlcomments
```

#### Preserve JavaScript Comments

Keep JavaScript comments in your code:

```bash
HtmlMinifier.exe "C:\Folder" ignorejscomments
```

#### Preserve Knockout Comments

Specifically preserve [Knockout.js](http://knockoutjs.com/) comments:

```bash
HtmlMinifier.exe "C:\Folder" ignoreknockoutcomments
```

### Multiple Targets

#### Minify Multiple Folders

```bash
HtmlMinifier.exe "C:\Folder\fld1" "C:\Folder\fld2"
```

#### Minify Specific Files

```bash
HtmlMinifier.exe "C:\Folder\file1.html" "C:\Folder\file2.html"
```

## 🔨 Build Integration

Integrate HTML Minifier into your build process for automatic minification during deployment. Check out this [detailed guide on using HTML Minifier with MSBuild](https://deanhume.com/a-simple-html-minifier-for-asp-net/).

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📝 Requirements

- .NET Framework (Windows)
- Supports HTML, Razor (.cshtml), and Web Forms (.aspx) files

## 📄 License

Copyright (C) Dean Hume 2013 - 2025

Released under the [MIT License](LICENSE.md)

---

**Made with ❤️ by [Dean Hume](https://deanhume.com)**

