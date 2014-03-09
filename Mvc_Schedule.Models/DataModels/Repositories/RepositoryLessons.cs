using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Mvc_Schedule.Models.DataModels.Entities;
using Mvc_Schedule.Models.DataModels.ModelViews;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class RepositoryLessons : RepositoryBase<ConnectionContext>
    {
        public RepositoryLessons(ConnectionContext ctx) : base(ctx) { }

        public Lesson Get(int id)
        {
            return _ctx.Lessons.Find(id);
        }

        public LessonsTime GetForEdit(int id)
        {
            return (from x in _ctx.Lessons
                    where x.LessonId == id
                    select new LessonsTime
                    {
                        Minutes = x.Time.Minute,
                        Hours = x.Time.Hour,
                        LessonId = x.LessonId
                    }).SingleOrDefault();
        }

        public void Add(LessonsCreate lessons)
        {
            Add(lessons.Lessons);
        }

        public void Add(LessonsTime[] lessons)
        {
            var oldList = List();

            if (!oldList.Any())
                foreach (var time in lessons.Select(x => x.Time).Distinct())
                    Add(new Lesson { Time = time });
            else
                foreach (var time in lessons.Select(x => x.Time).Distinct())
                    if (oldList.All(x => x.Time != time))
                        Add(new Lesson { Time = time });
        }

        public bool Add(LessonsTime lesson)
        {
            return Add(new Lesson { Time = lesson.Time });
        }

        public bool Add(Lesson lesson)
        {
            var old = _ctx.Lessons.FirstOrDefault(x => x.Time == lesson.Time);
            if (old != null) return false;
            _ctx.Lessons.Add(lesson);
            return true;
        }

        public IList<Lesson> List()
        {
            List<Lesson> list = (from x in _ctx.Lessons orderby x.Time select x).ToList();
            return list;
        }

        public LessonsTime[] Array()
        {
            var result = (from x in _ctx.Lessons
                          orderby x.Time.Hour, x.Time.Minute
                          select new LessonsTime
                              {
                                  Hours = x.Time.Hour,
                                  LessonId = x.LessonId,
                                  Minutes = x.Time.Minute
                              }).ToArray();
            return result;
        }

        public void Edit(LessonsTime lesson)
        {
            var old = Get(lesson.LessonId);
            var same = _ctx.Lessons.FirstOrDefault(x => lesson.LessonId != x.LessonId && (x.Time.Hour == lesson.Hours && x.Time.Minute == lesson.Minutes));
            if (same != null) 
                _ctx.Lessons.Remove(old);
            else
                old.Time = lesson.Time;
        }

        public Lesson Delete(int id)
        {
            var old = Get(id);
            _ctx.Lessons.Remove(old);
            return old;
        }
    }
}
