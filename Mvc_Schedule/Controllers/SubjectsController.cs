using System.Web.Mvc;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels;
using Mvc_Schedule.Models.DataModels.Entities;


namespace Mvc_Schedule.Controllers
{
    [Authorize(Roles = StaticData.AdminRoleName)]
    public class SubjectsController : Controller
    {
        private readonly DomainContext _db = new DomainContext();

        public SubjectsController()
        {
            ViewBag.Title = "Справочник: Дисциплины";
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
                    var result = db.Subjects.AddListFromTxt(model.Txt);
                    db.SaveChanges();
                    ViewBag.Result = result;
                    return View();
                }
            }
            return View(model);
        }


        public ViewResult Index(int page = 1)
        {
            return View(_db.Subjects.ListWithPager(page));
        }

        public ActionResult Create()
        {
            ViewBag.IsDublicate = false;
            return View();
        } 

        [HttpPost]
        public ActionResult Create(Subject subject)
        {
            var dublicate = _db.Subjects.IsDublicate(subject);
            if (ModelState.IsValid && !dublicate)
            {
                _db.Subjects.Add(subject);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IsDublicate = dublicate;
            return View(subject);
        }
        

        public ActionResult Edit(int id)
        {
            var subject = _db.Subjects.Get(id);
            return View(subject);
        }

        [HttpPost]
        public ActionResult Edit(Subject subject)
        {
            var dublicate = _db.Subjects.IsDublicate(subject);
            if (ModelState.IsValid && !dublicate)
            {
                _db.Subjects.Edit(subject);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IsDublicate = dublicate;
            return View(subject);
        }

        public ActionResult Delete(int id)
        {
            var subject = _db.Subjects.Get(id);
            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            _db.Subjects.Remove(id);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}