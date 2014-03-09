using System;
using System.Web.Mvc;
using Mvc_Schedule.Models;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Controllers
{
    public class PlansController : Controller
    {
        public PlansController() { ViewBag.Title = "Планы"; }

        private DomainContext db = new DomainContext();

        [HttpGet, Authorize]
        public ActionResult Index(int id = -1)
        {
            var group = db.IsAccessableFor(id);
            if (group == null)
                return RedirectToRoute(new { controller = "Default", action = "Error", id = 404 });

            return View(group);

        }


        #region Ajax

        [HttpPost]
        public JsonResult GetSubjects(string letter)
        {
            return Json(db.Ajax.ListSubjectsObjByLetter(letter));
        }

        [HttpPost]
        public JsonResult List(int id)
        {
            var list = db.Plans.List(id);
            return Json(list);
        }

        [HttpPost]
        public JsonResult Get(int planId)
        {
            return Json(db.Plans.Get(planId));
        }

        private PlanCreate ParseForm(FormCollection form, bool isEdit)
        {
            int groupId, lab, pr, lec;

            if (
                int.TryParse(form.GetValue("group-id").AttemptedValue, out groupId)
                && int.TryParse(form.GetValue("lab").AttemptedValue, out lab)
                && int.TryParse(form.GetValue("pr").AttemptedValue, out pr)
                && int.TryParse(form.GetValue("lec").AttemptedValue, out lec)
                )
            {
                var planId = 0;
                if (isEdit) int.TryParse(form.GetValue("plan-id").AttemptedValue, out planId);
                return new PlanCreate
                {
                    Plan = new Plan
                    {
                        GroupId = groupId,
                        LabH = lab,
                        LectionH = lec,
                        PracticeH = pr,
                        PlanId = planId,
                    },
                    SubjectName = form.GetValue("subject-title").AttemptedValue
                };
            }

            return null;
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Add(FormCollection form)
        {
            var planCreate = ParseForm(form, isEdit: false);
            string result = null;
            if (planCreate != null)
            {
                result = db.Plans.Add(planCreate);
                if (result != null)
                    db.SaveChanges();
            }
            return Json(result);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Edit(FormCollection form)
        {
            string result = null;
            var planCreate = ParseForm(form, isEdit: true);
            if (planCreate != null)
            {
                result = db.Plans.Edit(planCreate);
                if (result != null)
                    db.SaveChanges();
            }

            return Json(result);
        }


        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Drop(FormCollection form)
        {
            var planId = 0;
            if (!int.TryParse(form.GetValue("plan-id").AttemptedValue, out planId)) return Json(null);
            var result = db.Plans.Delete(planId);
            db.SaveChanges();
            return Json(result);
        }



        #endregion

    }
}
