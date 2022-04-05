using System;
using System.Linq;

namespace IntexII.Models
{
    public interface IRDSRepo
    {
        IQueryable<Crash> crashes { get; }

        public void SaveCrash(Crash c);

        public void CreateCrash(Crash c);

        public void DeleteCrash(Crash c);

    }
}
