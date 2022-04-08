using System;
using System.Linq;

namespace IntexII.Models.ViewModels
{
    public class CrashesViewModel
    {
        public IQueryable<Crash> crashes { get; set; }

        public PageInfo PageInfo { get; set; }

        public FilterModel Filter { get; set; }
    }
}
