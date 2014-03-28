using System.Web.Mvc;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Controllers
{
    [Authorize(Roles = StaticData.AdminRole)]
    public class LectorsController : Controller
    {
        public LectorsController()
        {
            ViewBag.Title = "Справочник: Преподаватели";
        }
        
        [HttpGet, Authorize]
        public ViewResult AddFromTxt()
        {
            return View();
        }

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public ActionResult AddFromTxt(TxtFile model)
        {
            if (ModelState.IsValid)
            {
                using (var db = new DomainContext())
                {
                    var result = db.Lectors.AddListFromTxt(model.Txt);
                    db.SaveChanges();
                    ViewBag.Result = result;
                    return View();
                }
            }
            return View(model);
        }

        public ActionResult Index(int page = 1)
        {
            using (var db = new DomainContext())
            {
                var model = db.Lectors.ListWithPager(page);
                if (!model.IsValid)
                    return RedirectToAction("Error", "Default", new { id = 404 });
                return View(model);
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
                if (lector == null) return RedirectToAction("Error", "Default", new { id = 404 });
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



        #region
        [HttpPost]
        public JsonResult GetListByLetter(string letter)
        {
            using (var db= new DomainContext())
            {
                return Json(db.Ajax.ListAuditory(letter));
            }
        }
        #endregion
    }
}