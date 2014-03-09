using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Controllers
{
	[Authorize]
	public class GroupsController : Controller
	{
		private readonly DomainContext _db = new DomainContext();

		public GroupsController()
		{
			ViewBag.Title = "Редактор групп";
		}

		public ActionResult Create()
		{
			ViewData["IsAdmin"] = Roles.IsUserInRole(StaticData.AdminRoleName);
			ViewBag.FacultId = new SelectList(_db.Facults.ListNames(), "FacultId", "Name");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(StudGroup studgroup)
		{
			if (ModelState.IsValid)
			{
				if (!Roles.IsUserInRole(studgroup.FacultId.ToString(CultureInfo.InvariantCulture)) && !Roles.IsUserInRole(StaticData.AdminRoleName))
					return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });

				_db.Groups.Add(studgroup);
				_db.SaveChanges();
				return RedirectToAction("Index", new { controller = "Facult" });
			}

			ViewBag.FacultId = new SelectList(_db.Facults.ListNames(), "FacultId", "Name", studgroup.FacultId);
			return View(studgroup);
		}

		public ActionResult Edit(int id)
		{
			StudGroup studgroup = _db.Groups.Get(id);
			if (studgroup == null || (!Roles.IsUserInRole(studgroup.FacultId.ToString(CultureInfo.InvariantCulture)) && !Roles.IsUserInRole(StaticData.AdminRoleName)))
				return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });
			ViewBag.FacultId = new SelectList(_db.Facults.ListNames(), "FacultId", "Name", studgroup.FacultId);
			return View(studgroup);
		}

		//
		// POST: /Groups/Edit/5

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(StudGroup studgroup)
		{
			if (ModelState.IsValid)
			{
				_db.Groups.Edit(studgroup);
				_db.SaveChanges();
				return RedirectToAction("Index", new { controller = "Facult" });
			}
			ViewBag.FacultId = new SelectList(_db.Facults.ListNames(), "FacultId", "Name", studgroup.FacultId);
			return View(studgroup);
		}

		//
		// GET: /Groups/Delete/5

		public ActionResult Delete(int id)
		{
			StudGroup studgroup = _db.Groups.Get(id);
			return View(studgroup);
		}

		//
		// POST: /Groups/Delete/5

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			_db.Groups.Delete(id);
			_db.SaveChanges();
			return RedirectToAction("Index", new { controller = "Facult" });
		}

		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}