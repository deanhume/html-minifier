HTML Minifier
=============

A simple command line tool to minify your HTML, Razor views & Web Forms views. By minifying your HTML on bigger web pages, 
you will save on bytes that your users need to donwload. Less bytes equal faster web pages & faster web pages equal happy users!

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


For more information, check out the blog post [on my site.](http://deanhume.com/Home/BlogPost/a-simple-html-minifier-for-asp-net/2097)


