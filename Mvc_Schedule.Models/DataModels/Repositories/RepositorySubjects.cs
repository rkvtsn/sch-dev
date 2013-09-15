using System.Collections.Generic;
using System.Linq;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class RepositorySubjects : RepositoryBase<ConnectionContext>
    {
        public RepositorySubjects(ConnectionContext ctx)
            : base(ctx)
        {
        }

        public IList<Subject> List()
        {
            return _ctx.Subjects.ToList();
        }

        public bool IsDublicate(Subject subject)
        {
            var first = (from x in _ctx.Subjects
                         where x.Title == subject.Title
                         select x).FirstOrDefault();
            if (first == null)
            {
                return false;
            }
            return first.SubjectId != subject.SubjectId;
        }
        
        public void Add(Subject subject)
        {
            subject.Title = subject.Title.Trim();
            _ctx.Subjects.Add(subject);
        }

        public Subject Get(int id)
        {
            return _ctx.Subjects.Find(id);
        }
        public void Remove(int id)
        {
            var old = Get(id);
            if (old != null)
            {
                _ctx.Subjects.Remove(old);
            }
        }
        public void Edit(Subject subject)
        {
            var old = Get(subject.SubjectId);

            if (old != null)
            {
                old.Title = subject.Title;
            }
        }

    }
}