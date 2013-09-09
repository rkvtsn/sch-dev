using System.Collections.Generic;
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
			var oldList = List();
		    
            if (!oldList.Any())
				foreach (var time in lessons.Lessons.Select(x => x.Time).Distinct())
					_ctx.Lessons.Add(new Lesson {Time = time});
			else
				foreach (var time in lessons.Lessons.Select(x => x.Time).Distinct())
					if (oldList.All(x => x.Time != time))
						_ctx.Lessons.Add(new Lesson {Time = time});
		}
		
		public IList<Lesson> List()
		{
			List<Lesson> list = (from x in _ctx.Lessons orderby x.Time select x).ToList();
			return list;
		}

		public Lesson[] Array()
		{
			return (from x in _ctx.Lessons orderby x.Time select x).ToArray();
		}

        public void Edit(LessonsTime lesson)
		{
			var old = Get(lesson.LessonId);
			old.Time = lesson.Time;
		}

		public void Delete(int id)
		{
			_ctx.Lessons.Remove(Get(id));
		}
	}
}
