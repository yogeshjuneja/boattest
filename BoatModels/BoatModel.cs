using System;
using System.Collections.Generic;
using System.Text;

namespace BoatModels
{
    public class BoatRegisterModel
    {
        public string BoatName { get; set; }
        public int HourlyRate { get; set; }    
    }

    public class BoatRentedModel
    {
        public int BoatNumber { get; set; }
        public  string CustomerName { get; set; }
    }
}
