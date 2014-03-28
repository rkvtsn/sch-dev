using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Web;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class RepositorySubjects : RepositoryBase<ConnectionContext, Subject>
    {
        public RepositorySubjects(ConnectionContext ctx)
            : base(ctx)
        {
            QueryPager = from x in _ctx.Subjects orderby x.Title select x;
        }

        public TxtUploaderResultModel AddListFromTxt(HttpPostedFileBase data)
        {
            return TxtUploader.AddListFromTxt(data, Encoding.GetEncoding(1251), subjectName =>
            {
                var subject = new Subject { Title = subjectName };

                this.Add(subject);
                return true;
            });
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
            _ctx.Subjects.AddOrUpdate(x => x.Title, subject);
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

        public void Add(string subjectName)
        {
            var subject = new Subject { Title = subjectName };
            this.Add(subject);
        }
    }
}