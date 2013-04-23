using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Snip.Data;
using Snip.Models;

namespace Snip.Controllers
{
    public partial class StatsController : Controller
    {
        //
        // GET: /Stats/
        public virtual ActionResult Index()
        {
            using (var db = new SnipContext())
            {
                var stats = new Stats();
                stats.TotalSnipCount = db.Snippets.Count();
                stats.TotalUserCount = db.Snippets.Select(s => s.Creator).Distinct().Count();
                if (stats.TotalSnipCount > 0)
                {
                    stats.TotalViewCount = db.Snippets.Sum(s => s.ViewCount);
                    stats.AverageSnipsPerUser = stats.TotalSnipCount * 1.0 / stats.TotalUserCount;
                    stats.AverageViewsPerSnip = stats.TotalViewCount * 1.0 / stats.TotalSnipCount;
                }
                return View(stats);
            }
        }
    }
}
