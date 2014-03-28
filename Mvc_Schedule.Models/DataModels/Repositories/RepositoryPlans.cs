using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.InteropServices;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class RepositoryPlans : RepositoryBase<ConnectionContext>
    {
        public RepositoryPlans(ConnectionContext ctx) : base(ctx) { }


        public object List(int id)
        {
            return (from x in _ctx.Plans
                    where x.GroupId == id
                    join s in _ctx.Subjects on x.SubjectId equals s.SubjectId
                    select new { planId = x.PlanId, title = s.Title, pr = x.PracticeH, lab = x.LabH, lec = x.LectionH }).ToList();
        }


        public object Get(int id)
        {
            return (from x in _ctx.Plans
                    join s in _ctx.Subjects on x.SubjectId equals s.SubjectId
                    where x.PlanId == id
                    select new
                    {
                        SubjectName = x.Subject.Title,
                        PlanId = x.PlanId,
                        LecH = x.LectionH,
                        PrH = x.PracticeH,
                        LabH = x.LabH,

                    }).SingleOrDefault();
        }

        private PlanCreate PreparePlanCreate(PlanCreate planCreate)
        {
            planCreate.SubjectName = planCreate.SubjectName.Trim();
            if (_ctx.Plans.Include(x => x.Subject).SingleOrDefault(x => x.Subject.Title.ToLower() == planCreate.SubjectName.ToLower() && planCreate.Plan.GroupId == x.GroupId && planCreate.Plan.PlanId != x.PlanId) != null) return null;
            planCreate.Plan.Subject = _ctx.Subjects.SingleOrDefault(x => x.Title.ToLower() == planCreate.SubjectName.ToLower()) ?? new Subject { Title = planCreate.SubjectName };

            return planCreate;
        }

        public string Add(PlanCreate planCreate)
        {
            var p = PreparePlanCreate(planCreate);
            if (p == null) return null;

            _ctx.Plans.Add(planCreate.Plan);
            return planCreate.SubjectName;
        }


        public string Edit(PlanCreate planCreate)
        {
            var p = PreparePlanCreate(planCreate);
            if (p == null) return null;

            var plan = _ctx.Plans.Find(planCreate.Plan.PlanId);
            plan.LabH = p.Plan.LabH;
            plan.PracticeH = p.Plan.PracticeH;
            plan.LectionH = p.Plan.LectionH;
            plan.Subject = p.Plan.Subject;

            return planCreate.SubjectName;
        }

        public string Delete(int planId)
        {
            var p = _ctx.Plans.Include(x => x.Subject).SingleOrDefault(x => x.PlanId == planId);

            if (p == null) return null;
            var result = p.Subject.Title;
            _ctx.Plans.Remove(p);
            return result;
        }

        
    }
}
