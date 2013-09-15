using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Controllers
{
    [Authorize(Roles = StaticData.AdminRoleName)]
    public class LectorsController : Controller
    {
        public LectorsController()
        {
            ViewBag.Title = "Справочник: Преподаватели";
        }
        public ViewResult Index()
        {
            using (var db = new DomainContext())
            {
                return View(db.Lectors.List());
            }
        }
        
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(Lector lector)
        {
            if (ModelState.IsValid)
            {
                using (var db = new DomainContext())
                {
                    db.Lectors.Add(lector);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(lector);
        }

        public ActionResult Edit(int id)
        {
            using (var db = new DomainContext())
            {
                Lector lector = db.Lectors.Get(id);
                return View(lector);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(Lector lector)
        {
            if (ModelState.IsValid)
            {
                using (var db = new DomainContext())
                {
                    db.Lectors.Edit(lector);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(lector);
        }

        public ActionResult Delete(int id)
        {
            using (var db = new DomainContext())
            {
                Lector lector = db.Lectors.Get(id);
                return View(lector);
            }
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (var db = new DomainContext())
            {
                db.Lectors.Remove(id);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        }
    }
}