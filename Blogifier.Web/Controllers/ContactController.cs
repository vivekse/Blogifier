using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogifier.Core.Common;
using Blogifier.Core.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Blogifier.Web.Controllers
{
    [Route("contact")]
    public class ContactController : Controller
    {
        private readonly ILogger<ContactController> _logger;
        private readonly string _theme;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
            _theme = $"~/{ApplicationSettings.BlogThemesFolder}/{BlogSettings.Theme}/";
        }
        public IActionResult Index()
        {
            return View(_theme + "Contact.cshtml", new BlogBaseModel());
        }
    }
}