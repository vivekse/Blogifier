﻿using Blogifier.Core.Common;
using Blogifier.Core.Data.Domain;
using Blogifier.Core.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blogifier.Core.Controllers.Api
{
    [Authorize]
    [Route("blogifier/api/[controller]")]
    public class PackagesController : Controller
    {
        IUnitOfWork _db;
        ILogger _logger;

        public PackagesController(IUnitOfWork db, ILogger<AssetsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpPut("enable/{id}")]
        public void Enable(string id)
        {
            var disabled = Disabled();
            if (disabled != null && disabled.Contains(id))
            {
                disabled.Remove(id);
                _db.CustomFields.SetCustomField(CustomType.Application, 0, Constants.DisabledPackages, string.Join(",", disabled));
            }
        }

        [HttpPut("disable/{id}")]
        public void Disable(string id)
        {
            var disabled = Disabled();
            if (disabled == null)
            {
                _db.CustomFields.SetCustomField(CustomType.Application, 0, Constants.DisabledPackages, id);
            }
            else
            {
                if (!disabled.Contains(id))
                {
                    disabled.Add(id);
                    _db.CustomFields.SetCustomField(CustomType.Application, 0, Constants.DisabledPackages, string.Join(",", disabled));
                }
            }
        }

        List<string> Disabled()
        {
            var field = _db.CustomFields.GetValue(CustomType.Application, 0, Constants.DisabledPackages);
            return string.IsNullOrEmpty(field) ? null : field.Split(',').ToList();
        }

        Profile GetProfile()
        {
            try
            {
                return _db.Profiles.Single(p => p.IdentityName == User.Identity.Name);
            }
            catch
            {
                RedirectToAction("Login", "Account");
            }
            return null;
        }
    }
}