using System;
using System.Web;
using System.Web.Mvc;
using Mvc_Schedule.Models;

namespace Mvc_Schedule.Controllers
{
	public class DefaultController : Controller
	{
		public DefaultController()
		{
			ViewBag.Title = "Главная";
		}

		public ActionResult Index()
		{
			using (var db = new DomainContext())
			{
				var model = db.Facults.ListNames(true);
				return View(model);
			}
		}

		#region ajax-запросы
		[HttpPost]
		public JsonResult GetGroups(string id)
		{
			int facultId;
			if (int.TryParse(id, out facultId))
				using (var db = new DomainContext())
				{
					var data = db.Groups.ByFacult(facultId);
					return Json(data);
				}
			return Json(null);
		}

		public JsonResult ChangeTheme()
		{
			var cookie = System.Web.HttpContext.Current.Request.Cookies["Theme"];
			if (cookie == null)
			{
				System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie("Theme", "True") { Expires = DateTime.Now.AddDays(2) });
			}
			else
				if (cookie.Value == "False")
				{
					System.Web.HttpContext.Current.Response.Cookies["Theme"].Value = "True";
					System.Web.HttpContext.Current.Response.Cookies["Theme"].Expires = DateTime.Now.AddDays(2);
				}
				else
				{
					System.Web.HttpContext.Current.Response.Cookies["Theme"].Value = "False";
					System.Web.HttpContext.Current.Response.Cookies["Theme"].Expires = DateTime.Now.AddDays(2);
					return Json("False");
				}

			return Json("True");
		}
		#endregion


		public ViewResult Error(int id = 101)
		{
			switch (id)
			{
				case 404:
					ViewBag.Message = "Ошибка 404. Запрашиваемой страницы не существует.";
					break;
				case 400:
					ViewBag.Message = "Ошибка 400. Ой, что-то не то...";
					break;
				case 500:
					ViewBag.Message = "Ошибка 500. Сервер отдыхает.";
					break;
				default:
					id = 101;
					ViewBag.Message = "Ошибка 101. Непонятно... Попробуйте ещё.";
					break;
			}
			Response.StatusCode = id;
			return View();
		}
	}
}
