using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class RepositoryAuditory : RepositoryBase<ConnectionContext, Auditory>
    {
        public RepositoryAuditory(ConnectionContext ctx)
            : base(ctx)
        {
            QueryPager = from x in _ctx.Auditories orderby x.Number select x;
        }

        public TxtUploaderResultModel AddListFromTxt(HttpPostedFileBase data)
        {
            return TxtUploader.AddListFromTxt(data, Encoding.GetEncoding(1251), x =>
            {
                var auditory = new Auditory { Number = x };

                this.Add(auditory);
                return true;
            });
        }

        public IList<Auditory> List()
        {
            return _ctx.Auditories.ToList();
        }

        public bool IsDublicate(Auditory auditory)
        {
            var first = (from x in _ctx.Auditories
                         where x.Number == auditory.Number
                         select x).FirstOrDefault();
            if (first == null)
            {
                return false;
            }
            return first.AuditoryId != auditory.AuditoryId;
        }

        public void Add(Auditory auditory)
        {
            auditory.Number = auditory.Number.Trim();
            _ctx.Auditories.AddOrUpdate(x => x.Number, auditory);
        }

        public Auditory Get(int id)
        {
            return _ctx.Auditories.Find(id);
        }
        public void Remove(int id)
        {
            var old = Get(id);
            if (old != null)
            {
                _ctx.Auditories.Remove(old);
            }
        }
        public void Edit(Auditory auditory)
        {
            var old = Get(auditory.AuditoryId);

            if (old != null)
            {
                old.Number = auditory.Number;
            }
        }

        public void Add(string number)
        {
            var auditory = new Auditory() { Number = number };
            this.Add(auditory);
        }
    }
}
