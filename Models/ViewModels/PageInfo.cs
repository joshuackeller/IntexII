using System;
namespace IntexII.Models.ViewModels
{
    public class PageInfo
    {
        public double TotalNumCrashes { get; set; }

        public double CrashesPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int Page2 {get; set;}


        public int Page4 { get; set; }


        // Calculate how many total pages are needed
        public int TotalPages => (int)Math.Ceiling((double)TotalNumCrashes / CrashesPerPage);

        
    }
}
