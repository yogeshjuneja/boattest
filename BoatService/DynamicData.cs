using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace BoatService
{
    public class Info
    {
        public int Status { get; set; } = 200;
        public string Message { get; set; } = "Success";
    }
    public class DynamicData : Info
    {
        public dynamic data = new ExpandoObject();
        public Info info { get; set; } = new Info();

    }
}
