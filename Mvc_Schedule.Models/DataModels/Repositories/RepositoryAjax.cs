using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class RepositoryAjax : RepositoryBase<ConnectionContext>
    {
        public RepositoryAjax(ConnectionContext ctx) : base(ctx) { }

        public string[] ListSubjects(string firstLetters)
        {
            return (from x in _ctx.Subjects
                    orderby x.Title
                    where x.Title.ToLower().StartsWith(firstLetters.ToLower())
                    select x.Title).Distinct().ToArray();
        }

        public object ListSubjectsObjByLetter(string firstLetters)
        {
            return (from x in _ctx.Subjects
                    orderby x.Title
                    where x.Title.ToLower().StartsWith(firstLetters.ToLower())
                    select new { SubjectId = x.SubjectId, Title = x.Title }).Distinct();
        }

        public string[] ListLectors(string firstLetters)
        {
            var list = (from x in _ctx.Lectors
                        orderby x.SecondName
                        where
                            (x.SecondName.ToLower().StartsWith(firstLetters.ToLower())) ||
                            (x.Name.ToLower().StartsWith(firstLetters.ToLower()))
                        select x).Distinct().ToList();

            return list.Select(x => x.FullName).ToArray();
        }

        public string[] ListAuditory(string firstLetters)
        {
            return (from x in _ctx.Auditories
                    orderby x.Number
                    where x.Number.ToLower().StartsWith(firstLetters.ToLower())
                    select x.Number).Distinct().ToArray();
        }


        private int? IsAvailable(int groupId, int timeId, bool week, int weekdayId, Func<IQueryable<ScheduleTable>, IQueryable<ScheduleTable>> operation)
        {
            return operation(
                    _ctx.ScheduleTables
                    .Where(x => x.LessonId == timeId && x.IsWeekOdd == week && x.WeekdayId == weekdayId && x.GroupId != groupId))
                    .Select(t => t.GroupId)
                    .FirstOrDefault();
        }

        public int? IsAvailableLector(int groupId, int timeId, string value, bool week, int weekdayId)
        {
            return IsAvailable(groupId, timeId, week, weekdayId, x => x.Where(t => t.LectorName == value));
        }
        public object IsAvailableAuditory(int groupId, int timeId, string value, bool week, int weekdayId)
        {
            var isAvailable = IsAvailable(groupId, timeId, week, weekdayId, x => x.Where(t => t.Auditory == value));
            if (isAvailable != null)
            {
                var busyAuditories = (from x in _ctx.ScheduleTables
                                     where x.LessonId == timeId && x.IsWeekOdd == week && x.WeekdayId == weekdayId && x.GroupId != groupId
                                     select x.Auditory);
                var available = (from x in _ctx.Auditories select x.Number).FirstOrDefault(x => !busyAuditories.Contains(x));
                return new { Owner = isAvailable, Available = available };
            }
            return null;
        }


        public int IsAvailablePlan(int groupId, string value, int lessonType)
        {
            value = value.Trim();
            var q = (from x in _ctx.Plans
                     where x.GroupId == groupId && x.Subject.Title.ToLower() == value.ToLower()
                     select x).SingleOrDefault();
            if (q == null) return 0;
            if (lessonType == 1) return q.LectionH;
            if (lessonType == 2) return q.LabH;
            if (lessonType == 3) return q.PracticeH;
            return 0;
        }


        public object CheckListOnAvailability(int groupId)
        {
            var q = from x in _ctx.ScheduleTables
                    where x.GroupId == groupId
                    select
                        new
                        {
                            Id = x.ScheduleTableId,
                            Busy = (from t in _ctx.ScheduleTables
                                    join g in _ctx.StudGroups on t.GroupId equals g.GroupId
                                    where (t.ScheduleTableId != x.ScheduleTableId
                                           && t.IsWeekOdd == x.IsWeekOdd
                                           && t.WeekdayId == x.WeekdayId
                                           && t.LessonId == x.LessonId
                                           && t.GroupId != x.GroupId)
                                          && (t.Auditory == x.Auditory ||
                                              t.LectorName == x.LectorName)
                                    select new
                                    {
                                        Group = new
                                        {
                                            Id = t.ScheduleTableId,
                                            Title = g.Name
                                        },
                                        Auditory = t.Auditory == x.Auditory,
                                        Lector = t.LectorName == x.LectorName
                                    })
                        };

            return q;
        }



        public GroupsJson[] GroupsByFacult(int id)
        {
            return (from x in _ctx.StudGroups
                    where x.FacultId == id
                    orderby x.Name
                    select new GroupsJson
                    {
                        Id = x.GroupId,
                        Name = x.Name
                    }).ToArray();
        }


        public object SchSearch(string keyword)
        {
            var result = _ctx.Weekdays
                .Include(x => x.ScheduleTables)
                .GroupBy(
                    w => new
                    {
                        w.Name,
                        w.WeekdayId
                    },
                    w => (from x in w.ScheduleTables
                          where x.LectorName.Contains(keyword) 
                             || x.Auditory.Contains(keyword)
                             //|| x.StudGroup.Name.Contains(keyword)
                             || x.SubjectName.Contains(keyword)
                          join l in _ctx.Lessons on x.LessonId equals l.LessonId
                          //join sg in _ctx.StudGroups on x.GroupId equals sg.GroupId
                          join wd in _ctx.Weekdays on x.WeekdayId equals wd.WeekdayId
                          select new
                          {
                              x.Auditory,
                              x.Date,
                              x.GroupId,
                              x.IsWeekOdd,
                              x.LectorName,
                              Lesson = new
                              {
                                  x.Lesson.Time,
                                  x.Lesson.LessonId,
                              },
                              x.LessonId,
                              x.LessonType,
                              x.ScheduleTableId,
                              x.SubjectName,
                              x.GroupSub,
                              Weekday = new
                              {
                                  x.Weekday.Name,
                                  x.WeekdayId,
                                  x.IsWeekOdd
                              },
                              x.WeekdayId
                          })
                        .GroupBy(
                            x => x.Lesson,
                            x => x,
                            (k, g) => new
                            {
                                Key = new
                                {
                                    k.LessonId,
                                    k.Time,
                                    CountOdd = g.Count(x => x.IsWeekOdd),
                                    CountEven = g.Count(x => !x.IsWeekOdd),
                                },
                                Group = g.OrderBy(x => new { x.IsWeekOdd, x.GroupSub })
                            }).OrderBy(x => x.Key.Time.Hour),
                            (k, g) => new
                            {
                                Key = k,
                                Group = g
                            }).OrderBy(x => x.Key.WeekdayId);


            return result.ToList();
        }




        public object SchList(int groupid)
        {
            var result = _ctx.Weekdays
                .Include(x => x.ScheduleTables)
                .GroupBy(
                    w => new
                    {
                        w.Name,
                        w.WeekdayId
                    },
                    w => (from x in w.ScheduleTables
                          where x.GroupId == groupid // Условия поиска
                          join l in _ctx.Lessons on x.LessonId equals l.LessonId
                          join wd in _ctx.Weekdays on x.WeekdayId equals wd.WeekdayId
                          select new
                          {
                              x.Auditory,
                              x.Date,
                              x.GroupId,
                              x.IsWeekOdd,
                              x.LectorName,
                              Lesson = new
                              {
                                  x.Lesson.Time,
                                  x.Lesson.LessonId,
                              },
                              x.LessonId,
                              x.LessonType,
                              x.ScheduleTableId,
                              x.SubjectName,
                              x.GroupSub,
                              Weekday = new
                              {
                                  x.Weekday.Name,
                                  x.WeekdayId,
                                  x.IsWeekOdd
                              },
                              x.WeekdayId
                          })
                        .GroupBy(
                            x => x.Lesson,
                            x => x,
                            (k, g) => new
                            {
                                Key = new
                                {
                                    k.LessonId,
                                    k.Time,
                                    CountOdd = g.Count(x => x.IsWeekOdd),
                                    CountEven = g.Count(x => !x.IsWeekOdd),
                                },
                                Group = g.OrderBy(x => new { x.IsWeekOdd, x.GroupSub })//.ThenBy(x => x.IsWeekOdd)
                            }).OrderBy(x => x.Key.Time.Hour),
                            (k, g) => new
                            {
                                Key = k,
                                Group = g
                            }).OrderBy(x => x.Key.WeekdayId);


            return result.ToList();
        }


        public object SchGet(int id)
        {
            return (from x in _ctx.ScheduleTables
                    where x.ScheduleTableId == id
                    select new
                    {
                        x.LessonType,
                        x.LectorName,
                        x.Auditory,
                        x.GroupSub,
                        x.SubjectName
                    }).SingleOrDefault();
        }

        //public object Search(string keyword)
        //{
        //    //.Where(x => x.LectorName.Contains(keyword) || x.Auditory.Contains(keyword))
        //}
    }

    public class JsonLesson
    {
        public int LesssonId { get; set; }
        public DateTime Time { get; set; }
    }
}
