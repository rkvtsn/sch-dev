using System.Collections.Generic;
using System.Linq;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class RepositoryLectors : RepositoryBase<ConnectionContext>
    {
        public RepositoryLectors(ConnectionContext ctx)
            : base(ctx)
        {
        }

        public IList<Lector> List()
        {
            return _ctx.Lectors.ToList();
        }
        public void Add(Lector lector)
        {
            _ctx.Lectors.Add(lector);
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