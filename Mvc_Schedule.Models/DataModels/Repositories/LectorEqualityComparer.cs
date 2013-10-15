using System.Collections.Generic;
using Mvc_Schedule.Models.DataModels.Entities;

namespace Mvc_Schedule.Models.DataModels.Repositories
{
    public class LectorEqualityComparer : IEqualityComparer<Lector>
    {
        public bool Equals(Lector x, Lector y)
        {
            return x.SecondName == y.SecondName && x.Name == y.Name && x.ThirdName == y.ThirdName;
        }

        public int GetHashCode(Lector obj)
        {
            return obj.GetHashCode();
        }
    }
}