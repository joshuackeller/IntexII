using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntexII.Models
{
    public class ModelData
    {


        public float county_name_utah { get; set; }
        public float county_name_saltlake { get; set; }
        public float county_name_other { get; set; }
        public float city_OUTSIDE_CITY_LIMITS { get; set; }
        public float roadway_departure { get; set; }
        public float single_vehicle { get; set; }
        public float night_dark_condition { get; set; }
        public float older_driver_involved { get; set; }
        public float teenage_driver_involved { get; set; }
        public float intersection_related { get; set; }


        public Tensor<float> AsTensor()
        {
            float[] data = new float[]
            {
                county_name_utah, county_name_saltlake, county_name_other, city_OUTSIDE_CITY_LIMITS,
               roadway_departure, single_vehicle, night_dark_condition, older_driver_involved,
                teenage_driver_involved, intersection_related
            };
            int[] dimensions = new int[] { 1, 10 };
            return new DenseTensor<float>(data, dimensions);
        }
    }
}