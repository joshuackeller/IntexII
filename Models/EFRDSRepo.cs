using System;
using System.Linq;

namespace IntexII.Models
{
    public class EFRDSRepo : IRDSRepo
    {
        private RDSContext context { get; set; }

        public EFRDSRepo (RDSContext temp)
        {
            context = temp;
        }

        public IQueryable<Crash> crashes => context.crashes;



        public void CreateCrash(Crash c)
        {
            context.Add(c);
            context.SaveChanges();
        }

        public void DeleteCrash(Crash c)
        {
            context.Remove(c);
            context.SaveChanges();
        }

        public void SaveCrash(Crash c)
        {
            context.Update(c);
            context.SaveChanges();
        }
    }
}
