﻿using Blogifier.Core.Common;
using Blogifier.Core.Data.Domain;
using Blogifier.Core.Data.Interfaces;
using Blogifier.Core.Data.Models;
using Blogifier.Core.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Newsletter
{
    [Route("blogifier/widgets/[controller]")]
    public class NewsletterController : Controller
    {
        IUnitOfWork _db;
        static readonly string key = "NEWSLETTER";

        public NewsletterController(IUnitOfWork db)
        {
            _db = db;
        }

        [HttpPut("subscribe")]
        public async Task Subscribe([FromBody]CustomFieldItem item)
        {
            var emails = Emails();

            if (emails != null)
            {
                if (!emails.Contains(item.CustomValue))
                {
                    emails.Add(item.CustomValue);
                    _db.CustomFields.SetCustomField(CustomType.Application, 0, item.CustomKey, string.Join(",", emails));
                }
            }
            else
            {
                _db.CustomFields.SetCustomField(CustomType.Application, 0, item.CustomKey, item.CustomValue);
            }
        }

        [Authorize]
        [MustBeAdmin]
        [HttpGet("settings")]
        public IActionResult Settings(string search = "")
        {
            var profile = _db.Profiles.Single(b => b.IdentityName == User.Identity.Name);
            var emails = Emails();

            if (!string.IsNullOrEmpty(search))
            {
                emails = emails.Where(e => e.Contains(search)).ToList();
            }

            dynamic settings = new
            {
                Emails = emails,
                Pager = new Pager(1)
            };

            var info = new PackageInfo();

            var model = new AdminSettingsModel {
                Profile = profile,
                Settings = settings,
                PackageItem = info.GetAttributes()
            };

            return View("~/Views/Shared/Components/Newsletter/Settings.cshtml", model);
        }

        [Authorize]
        [MustBeAdmin]
        [HttpPut("remove/{id}")]
        public async Task Remove(string id)
        {
            var emails = Emails();
            if (emails != null && emails.Contains(id))
            {
                emails.Remove(id);
                _db.CustomFields.SetCustomField(CustomType.Application, 0, key, string.Join(",", emails));
            }
        }

        List<string> Emails()
        {
            var field = _db.CustomFields.GetValue(CustomType.Application, 0, key);
            return string.IsNullOrEmpty(field) ? null : field.Split(',').ToList();
        }
    }
}