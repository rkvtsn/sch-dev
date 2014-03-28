using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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
                var lector = new Lector { PreName = fullName[0], SecondName = fullName[1], Name = fullName[2] };
                if (fullName.Length > 3)
                    lector.ThirdName = fullName[3];
                lectors.Add(lector);
                return true;
            });

            result.Duplicates = this.AddList(lectors.Distinct());
            return result;
        }

        private int AddList(IEnumerable<Lector> lectors)
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
            lector.PreName = lector.PreName.Trim();
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
            old.PreName = lector.PreName;
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

        public void Add(string str)
        {
            var begin = (from ch in str.ToArray() where Char.IsUpper(ch) select str.IndexOf(ch)).FirstOrDefault();

            var prefix = str.Substring(0, begin);

            var name = str.Substring(begin);

            var lector = name.Replace(".", " ").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var newLector = new Lector { Name = lector[1], SecondName = lector[0], PreName = prefix};
            if (lector.Length > 2)
                newLector.ThirdName = lector[2];
            
            var q = _ctx.Lectors
                    .SingleOrDefault(x =>
                         x.Name == newLector.Name
                      && x.SecondName.StartsWith(newLector.SecondName)
                      && x.ThirdName.StartsWith(newLector.ThirdName));
            if (q != null) return;
            

            Add(newLector);
        }
    }
}