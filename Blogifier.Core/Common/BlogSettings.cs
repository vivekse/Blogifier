﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Blogifier.Core.Common
{
    public class BlogSettings
    {
        public static string Title { get; set; } = "Project Codify";
        public static string Description { get; set; } = "";
        public static string Logo { get; set; } = "/embedded/lib/img/logo.png";
        public static string Cover { get; set; } = "/embedded/lib/img/header.jpg";
        public static string Theme { get; set; } = "Standard";
        public static string Head { get; set; } = "";
        public static string Footer { get; set; } = "";

        public static IList<SelectListItem> BlogThemes { get; set; }

        // posts
        public static int ItemsPerPage { get; set; } = 10;
        public static string PostCover { get; set; } = "/embedded/lib/img/cover.png";
        public static string PostFooter { get; set; } = "";     
    }
}