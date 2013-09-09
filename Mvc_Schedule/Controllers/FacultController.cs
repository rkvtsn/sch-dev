using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;


namespace Mvc_Schedule.Controllers
{
	[Authorize]
	public class FacultController : Controller
	{
		private readonly DomainContext _db = new DomainContext();
		public FacultController()
		{
			ViewBag.Title = "Редактор факультетов";
		}
		public ViewResult Index()
		{
			ViewData["isAccessable"] = User.IsInRole(StaticData.AdminRoleName);
			var model = _db.Facults.ListNames();
			return View(model);
		}

		[Authorize(Roles = StaticData.AdminRoleName)]
		public ActionResult Create()
		{
			return View();
		}
		[Authorize(Roles = StaticData.AdminRoleName)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(FacultCreate model)
		{
			if (ModelState.IsValid)
			{
				_db.Facults.Add(model);
				_db.SaveChanges();

				if (!Roles.RoleExists(model.FacultInstance.FacultId.ToString(CultureInfo.InvariantCulture)))
					Roles.CreateRole(model.FacultInstance.FacultId.ToString(CultureInfo.InvariantCulture));
				
				return RedirectToAction("Index");
			}

			return View(model);
		}
		[Authorize(Roles = StaticData.AdminRoleName)]
		public ActionResult Edit(int id)
		{
			var facult = _db.Facults.Get(id);
			if (facult == null)
				return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });

			return View(facult);
		}
		[Authorize(Roles = StaticData.AdminRoleName)]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(Facult facult)
		{
			if (ModelState.IsValid)
			{
				_db.Facults.Edit(facult);
				_db.SaveChanges();
				
				return RedirectToAction("Index");
			}
			return View(facult);
		}
		[Authorize(Roles = StaticData.AdminRoleName)]
		public ActionResult Delete(int id)
		{
			var facult = _db.Facults.Get(id);
			if (facult == null)
				return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });
			return View(facult);
		}
		[Authorize(Roles = StaticData.AdminRoleName)]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			if (!_db.Facults.Delete(id)) 
				return RedirectToRoute(new { controller = "Default", action = "Error", id = 400 });
			_db.SaveChanges();
			if (!Roles.RoleExists(id.ToString(CultureInfo.InvariantCulture)))
				Roles.DeleteRole(id.ToString(CultureInfo.InvariantCulture));
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}