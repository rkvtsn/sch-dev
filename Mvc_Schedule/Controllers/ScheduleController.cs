﻿using Mvc_Schedule.Models;
using System.Web.Mvc;

namespace Mvc_Schedule.Controllers
{
	public class ScheduleController : Controller
	{
		private readonly DomainContext _db = new DomainContext();

		public ScheduleController() { ViewBag.Title = "Редактор расписания"; }

        [HttpPost]
        public JsonResult GetList(string letter, string method)
        {
            if (method == "Lectors") { return Json(_db.Schedule.ListLectors(letter)); }
            else if (method == "Auditory") { return Json(_db.Schedule.ListAuditory(letter)); }
            else return Json(_db.Schedule.ListSubjects(letter));
        }

        [HttpGet]
		public ActionResult Index(int id = -1, int week = 1)
		{
            //var model = _db.Schedule.ListForIndex(id, 1 == week);
            //if (model.Group == null)
            //    return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });
            var group = _db.Groups.Get(id);

            if (group == null)
                return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });
            ViewBag.Title = group.Name;
            var model = _db.Schedule.GetWeekdaysWithSchedule(id);
			return View(model);
		}
        
        // Немного поиска
        [HttpGet]
        public ActionResult Search(string keyword, int searchType, int week = 1)
        {
            if (keyword == null || keyword.Trim() == string.Empty)
                return View();
            else
                return View(_db.Schedule.Search(keyword, searchType, 1 == week));
        }
        

		[Authorize]
		[HttpGet]
		public ActionResult Create(int id = -1, int week = 1)
		{
			var model = _db.Schedule.ListForCreate(id, 1 == week);
			if (model == null)
				return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });

			return View(model);
		}

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(FormCollection scheduleRows)
		{
			bool isValid;
			var scheduletable = _db.Schedule.FormToTable(scheduleRows, out isValid);

			if (isValid)
			{
				_db.Schedule.ListAdd(scheduletable);
				_db.SaveChanges();
				return RedirectToAction("Index", "Facult");
			}

			ViewBag.Error = "Ошибка ввода, заполните все поля";
			scheduletable.Lessons = _db.Lessons.List(); 
			scheduletable.Weekdays = _db.Weekdays.List();

			return View(scheduletable);
			//return View(new ScheduleTableCreate());
		}


		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}