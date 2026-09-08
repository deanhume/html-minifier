# Obtaining HTML Minifier

HTML Minifier is a Windows command-line application targeting .NET Framework 4.8.

## Download a release

The recommended way to obtain HTML Minifier is from the
[GitHub Releases page](https://github.com/deanhume/html-minifier/releases).

1. Download the latest release archive.
2. Extract `HtmlMinifier.exe` to a folder of your choice.
3. Run the executable directly or add its folder to your `PATH`.

Verify the installation:

```powershell
HtmlMinifier.exe --version
```

View the available commands:

```powershell
HtmlMinifier.exe --help
```

The application requires Windows with .NET Framework 4.8 installed.

## Obtain the source

Clone the repository to inspect, build, or modify the source:

```powershell
git clone https://github.com/deanhume/html-minifier.git
Set-Location html-minifier
```

To build from source, install Visual Studio 2022 or the Visual Studio Build Tools
with the .NET Framework 4.8 targeting pack, then run:

```powershell
dotnet build HtmlMinifier.sln
```

The compiled executable is written beneath the `ViewMinifier\bin` directory for
the selected configuration.

The source and release artifacts are provided under the
[MIT License](LICENSE.md).
