using System.Web.Mvc;
using System.Web.Routing;

using System.Data.Entity;
using Mvc_Schedule.Models.DataModels;

namespace Mvc_Schedule
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters) { filters.Add(new HandleErrorAttribute()); }
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute("Theme", "ChangeTheme",
				new { controller = "Default" }, new { action = "ChangeTheme" });

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
			//Database.SetInitializer(new DbMigrate(DbMigrate.MigrateStrategy.ClearDb)); // Миграция БД
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<ConnectionContext, Models.Migrations.Configuration>());
		}
	}
}