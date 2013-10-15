using System.Data.Entity;
using System.Linq;
using Pager;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public abstract class RepositoryBase<TD> where TD : DbContext
    {
        protected TD _ctx;

        public RepositoryBase(TD ctx)
        {
            _ctx = ctx;
        }

        public void SaveChanges()
        {
            _ctx.SaveChanges();
        }
    }

    public abstract class RepositoryBase<TD, T> : RepositoryBase<TD> where TD : DbContext
    {
        public IQueryable<T> QueryPager;

        protected RepositoryBase(TD ctx) : base(ctx) { }

        public PagerView<T> ListWithPager(int page = 1, int count = 10)
        {
            var pager = new PagerViewBuilder<T>
            {
                Query = QueryPager,
                CurrentPage = page,
                ItemsOnPage = count,
            }.Build();

            return pager;
        }
    }
}
