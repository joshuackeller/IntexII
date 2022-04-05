using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IntexII.Models
{
    public class Crash
    {
        [Key]
        [Required]
        public int crash_id { get; set; }
        public double route { get; set; }
        public double milepoint { get; set; }
        public double lat_utm_y { get; set; }
        public double long_utm_x { get; set; }
        public string main_road_name { get; set; }
        public string city { get; set; }
        public string county_name { get; set; }
        public double crash_severity_id { get; set; }
        public bool work_zone_related { get; set; }
        public bool pedestrian_involved { get; set; }
        public bool bicyclist_involved { get; set; }
        public bool motorcycle_involved { get; set; }
        public bool improper_restraint { get; set; }
        public bool unrestrained { get; set; }
        public bool dui { get; set; }
        public bool intersection_related { get; set; }
        public bool wild_animal_related { get; set; }
        public bool domestic_animal_related { get; set; }
        public bool overturn_rollover { get; set; }
        public bool commercial_motor_veh_involved { get; set; }
        public bool teenage_driver_involved { get; set; }
        public bool older_driver_involved { get; set; }
        public bool night_dark_condition { get; set; }
        public bool single_vehicle { get; set; }
        public bool distracted_driving { get; set; }
        public bool drowsy_driving { get; set; }
        public bool roadway_departure { get; set; }
        public bool outside_city_limits { get; set; }
        public DateTime crash_date { get; set; }
        // public DateTime crash_time { get; set; }



    }
}
