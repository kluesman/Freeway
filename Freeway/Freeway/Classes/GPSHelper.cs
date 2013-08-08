using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freeway.Classes
{
    public class GPSHelper
    {
        public double MilesPerHour(double MetersPerSecond)
        {
            double MetersPerHour = MetersPerSecond * 60 * 60;
            double MilesPerHour = MetersPerHour * 0.000621371; // 1 Meter = 0.000621371 Mile
            return MilesPerHour;
        }
    }
}
