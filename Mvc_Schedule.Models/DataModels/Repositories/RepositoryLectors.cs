using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class RepositoryLectors : RepositoryBase<ConnectionContext, Lector>
    {
        public RepositoryLectors(ConnectionContext ctx)
            : base(ctx)
        {
            QueryPager = from x in _ctx.Lectors orderby x.SecondName select x;
        }

        public TxtUploaderResultModel AddListFromTxt(HttpPostedFileBase data)
        {
            var lectors = new List<Lector>();
            var result = TxtUploader.AddListFromTxt(data, 2, Encoding.GetEncoding(1251), fullName =>
            {
                var lector = new Lector { SecondName = fullName[0], Name = fullName[1] };
                if (fullName.Length > 2)
                    lector.ThirdName = fullName[2];
                lectors.Add(lector);
                return true;
            });

            result.Duplicates = this.AddList(lectors);
            return result;
        }

        private int AddList(List<Lector> lectors)
        {
            var failed = 0;
            var existLectors = (from x in _ctx.Lectors select x).ToList();
            foreach (var lector in lectors)
            {
                if (!existLectors.Contains(lector, new LectorEqualityComparer()))
                {
                    this.Add(lector);
                }
                else
                {
                    failed++;
                }
            }
            return failed;
        }

        public IList<Lector> List()
        {
            return _ctx.Lectors.ToList();
        }
        public bool Add(Lector lector)
        {
            lector.Name = lector.Name.Trim();
            lector.SecondName = lector.SecondName.Trim();
            lector.ThirdName = lector.ThirdName != null && lector.ThirdName.Trim() != string.Empty
                                   ? lector.ThirdName.Trim()
                                   : null;
            _ctx.Lectors.Add(lector);
            return true;
        }
        public void Edit(Lector lector)
        {
            var old = Get(lector.LectorId);
            old.Name = lector.Name;
            old.SecondName = lector.SecondName;
            old.ThirdName = lector.ThirdName;
        }
        public void Remove(int id)
        {
            var old = Get(id);
            _ctx.Lectors.Remove(old);
        }
        public Lector Get(int id)
        {
            return _ctx.Lectors.Find(id);
        }
    }
}