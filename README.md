HTML Minifier <image src="https://ci.appveyor.com/api/projects/status/v7d4iw3f9dua1ah6" width="100">
=============

A simple command line tool to minify your HTML, Razor views & Web Forms views. By minifying your HTML on bigger web pages,
you will save on bytes that your users need to donwload. Less bytes equal faster web pages & faster web pages equal happy users!

## Getting Started

Go from HTML that looks like this:

    <h2>
        Admin Menu</h2>
    <ul>
        <li>@Html.ActionLink("Edit blog entries", "List", "Admin")</li>
        <li>@Html.ActionLink("View Comments", "CommentList", "Admin")</li>
        <li>@Html.ActionLink("Clear Cache", "ClearCache", "Admin")</li>
    </ul>

To HTML that looks like this:

    <h2> Admin Menu</h2><ul><li>@Html.ActionLink("Edit blog entries", "List", "Admin")</li><li>@Html.ActionLink("View Comments", "CommentList", "Admin")</li><li>@Html.ActionLink("Clear Cache", "ClearCache", "Admin")</li></ul>

## Usage Examples

In order to use from the command line, you simply pass through a folder path that contains all of the files you want to minify. The minifier will process all images in the root and subfolders.

    C:\>HtmlMinifier.exe "C:\Folder"

If you'd like to restrict the number of characters per line and force it to break to the next line, use the minifier with the following optional value (where the number is the max character count).

    C:\>HtmlMinifier.exe "C:\Folder" "60000"

There is also the option to disable certain minification features. For example, if you use a you rely on HTML comments (Knockout, Angular, etc.) you might want to leave them in the minified HTML.

    C:\>HtmlMinifier.exe "C:\Folder" ignorehtmlcomments

You can also disable the minification of JavaScript Comments

    C:\>HtmlMinifier.exe "C:\Folder" ignorejscomments

If you use knockoutJS, you can optionally disable the minification of [Knockout](http://knockoutjs.com/) Comments

    C:\>HtmlMinifier.exe "C:\Folder" ignoreknockoutcomments

If you want to minify individual folders you can do:

    C:\>HtmlMinifier.exe "C:\Folder\fld1" "C:\Folder\fld2"

If you want to minify individual files you can do:

    C:\>HtmlMinifier.exe "C:\Folder\file1.html" "C:\Folder\file2.html"

If you'd like to find out how to use this with MSBUILD and your next publish, please follow this [link.](http://deanhume.com/Home/BlogPost/a-simple-html-minifier-for-asp-net/2097)

## License

(C) Dean Hume 2016, released under the MIT license

