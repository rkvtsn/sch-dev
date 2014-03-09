using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            return (from x in _ctx.ScheduleTables
                    orderby x.Auditory
                    where x.Auditory.ToLower().StartsWith(firstLetters.ToLower())
                    select x.Auditory).Distinct().ToArray();
        }

        public string IsAvailableLector(int timeId, string value, bool week)
        {

            return _ctx.ScheduleTables
                    .Where(x => x.LessonId == timeId && x.LectorName == value && x.IsWeekOdd == week)
                    .Select(x => x.GroupId)
                    .FirstOrDefault()
                    .ToString(CultureInfo.InvariantCulture);

        }

        public string IsAvailableAuditory(int timeId, string value, bool week)
        {
            return _ctx.ScheduleTables
                    .Where(x => x.LessonId == timeId && x.Auditory == value && x.IsWeekOdd == week)
                    .Select(x => x.GroupId).FirstOrDefault().ToString(CultureInfo.InvariantCulture);
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
                          where x.GroupId == groupid
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
                                Group = g.OrderBy(x => x.IsWeekOdd)
                            }).OrderBy(x => x.Key.Time.Hour),
                            (k, g) => new
                            {
                                Key = k,
                                Group = g
                            }).OrderBy(x => x.Key.WeekdayId);


            return result.ToList();
        }
    }

    public class JsonLesson
    {
        public int LesssonId { get; set; }
        public DateTime Time { get; set; }
    }
}
