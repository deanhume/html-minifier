HTML Minifier
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

In order to use from the command line, you simply pass through a folder path that contains all your images. The minifier will process all images in the root and subfolders.

    C:\>HtmlMinifier.exe "C:\ImagesFolder"

If you'd like to find out how to use this with MSBUILD and your next publish, please follow this [link.](http://deanhume.com/Home/BlogPost/a-simple-html-minifier-for-asp-net/2097)

## Maintainers

* [@deanohume](http://github.com/deanhume)

## License

(C) Dean Hume 2014, released under the MIT license

