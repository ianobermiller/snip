using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Snip.Controllers
{
    public abstract partial class BaseController : Controller
    {
        public string GetUserName()
        {
            return StringUtils.GetAliasFromQualifiedName(User.Identity.Name);
        }
    }
}