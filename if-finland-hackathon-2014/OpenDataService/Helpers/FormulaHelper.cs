using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenDataService.DataManagement;

namespace OpenDataService.Helpers {
    public static class FormulaHelper
    {
        public static double Measure(Location loc1, Location loc2)
        {
            // generally used geo measurement function
            var R = 6378.137; // Radius of earth in KM
            var dLat = (loc2.Latitude - loc1.Latitude)*Math.PI/180;
            var dLon = (loc2.Longitude - loc1.Longitude)*Math.PI/180;
            var a = Math.Sin(dLat/2)*Math.Sin(dLat/2) +
                    Math.Cos(loc1.Latitude*Math.PI/180)*Math.Cos(loc2.Latitude*Math.PI/180)*
                    Math.Sin(dLon/2)*Math.Sin(dLon/2);
            var c = 2*Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R*c;
            return d*1000; // meters
        }

    }
}