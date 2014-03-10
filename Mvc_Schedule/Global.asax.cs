using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;

using Mvc_Schedule.Models.DataModels;

namespace Mvc_Schedule
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            #region @styles
            // @app
            bundles.Add(new StyleBundle("~/styles/main").Include(
                "~/Content/app/font-cleanvertising.css"
                ,"~/Content/app/style.css"
                , "~/Content/app/font-awesome.min.css"
                ));
            bundles.Add(new StyleBundle("~/styles/lesson-index").Include("~/Content/app/lesson-index.css"));
            bundles.Add(new StyleBundle("~/styles/facult-index").Include("~/Content/app/facult-index.css"));
            
            // @addons
            bundles.Add(new StyleBundle("~/styles/validation").Include("~/Content/validation.css"));
            bundles.Add(new StyleBundle("~/styles/dlghelper").Include("~/Content/dlghelper.css"));
            bundles.Add(new StyleBundle("~/styles/jquery-ui").Include("~/Content/jquery.ui.autocomplete.css"));
            #endregion

            #region @scripts
            // @app
            bundles.Add(new ScriptBundle("~/scripts/default-index").Include("~/Scripts/app/default-index.js"));
            bundles.Add(new ScriptBundle("~/scripts/lesson-index").Include("~/Scripts/app/lesson-index.js"));
            bundles.Add(new ScriptBundle("~/scripts/schedule-create").Include("~/Scripts/app/schedule-create.js"));
            bundles.Add(new ScriptBundle("~/scripts/facult-index").Include("~/Scripts/app/facult-index.js"));
            bundles.Add(new ScriptBundle("~/scripts/plan-index").Include("~/Scripts/app/plan-index.js"));
            bundles.Add(new ScriptBundle("~/scripts/plan-create").Include("~/Scripts/app/plan-create.js"));
            bundles.Add(new ScriptBundle("~/scripts/sch-index").Include("~/Scripts/app/sch-index.js"));
            bundles.Add(new ScriptBundle("~/scripts/sch-create")
                .Include("~/Scripts/app/sch-index.js", "~/Scripts/app/sch-create.js"));

            // @addons
            bundles.Add(new ScriptBundle("~/scripts/jquery", "//ajax.googleapis.com/ajax/libs/jquery/1.11.0/jquery.min.js")
                .Include("~/Scripts/jquery-{version}.js"));
            bundles.Add(new ScriptBundle("~/scripts/validation")
                .Include("~/Scripts/jquery.validate.min.js",
                         "~/Scripts/jquery.validate.unobtrusive.min.js"));
            bundles.Add(new ScriptBundle("~/scripts/dlghelper").Include("~/Scripts/dlghelper.js"));
            bundles.Add(new ScriptBundle("~/scripts/jquery-ui").Include("~/Scripts/jquery-ui-{version}.custom.min.js"));
            #endregion
            
            //<script src="@Url.Content("~/Scripts/jquery-ui-1.8.23.custom.min.js")" type="text/javascript"></script>
            //<link href="@Url.Content("~/Content/jquery.ui.autocomplete.css")" rel="stylesheet" type="text/css" />

            BundleTable.EnableOptimizations = true;
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters) { filters.Add(new HandleErrorAttribute()); }
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Plans", "Plans/{id}",
                new { controller = "Plans", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute("Errors", "{action}/{id}",
                new { controller = "Default", id = UrlParameter.Optional }, new { action = "Error" });

            routes.MapRoute("Account", "{action}",
                new { controller = "Auth" }, new { action = "LogOn|LogOff" });

            routes.MapRoute("Schedule", "Schedule/{action}/{id}/{week}",
                new { controller = "Schedule", action = "Index", id = UrlParameter.Optional, week = UrlParameter.Optional });

            routes.MapRoute("Default", "{controller}/{action}/{id}",
                new { controller = "Default", action = "Index", id = UrlParameter.Optional });

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            RegisterBundles(BundleTable.Bundles);

            //Database.SetInitializer(new DbMigrate(DbMigrate.MigrateStrategy.ClearDb)); // не использовать
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ConnectionContext, Models.Migrations.Configuration>()); //
        }
    }
}