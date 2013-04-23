using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using Snip.Data;
using System.Threading;
using System.Linq;
using System;

namespace Snip
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        Timer expirationTimer;

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Add",
                "",
                new { controller = "Snippet", action = "Add" }
            );

            routes.MapRoute(
                "Delete",
                "delete/{id}",
                new { controller = "Snippet", action = "Delete" }
            );

            routes.MapRoute(
                "Edit",
                "edit/{id}",
                new { controller = "Snippet", action = "Edit" }
            );

            routes.MapRoute(
                "IsAvailable",
                "isavailable/{id}",
                new { controller = "Snippet", action = "IsAvailable", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                 "User",
                 "mysnips",
                 new { controller = "User", action = "Index" }
            );

            routes.MapRoute(
                 "UserArchive",
                 "mysnips/archive/{fileName}",
                 new { controller = "User", action = "Archive" }
            );

            routes.MapRoute(
                 "Stats",
                 "about",
                 new { controller = "Stats", action = "Index" }
            );

            // This must be last
            routes.MapRoute(
                 "Display",
                 "{id}",
                 new { controller = "Snippet", action = "Display" }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Database.SetInitializer<SnipContext>(new CreateDatabaseIfNotExists<SnipContext>());

            expirationTimer = new Timer(DeleteExpiredSnips, null, 0, 1000 * 60 * 5); // 5 minutes
        }

        private void DeleteExpiredSnips(object state)
        {
            using (var db = new SnipContext())
            {
                foreach (var snip in db.Snippets.Where(s => s.Expiration < DateTime.UtcNow))
                {
                    db.Snippets.Remove(snip);
                }
                db.SaveChanges();
            }
        }
    }
}