using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Freeway.Classes
{
    public class Address
    {
        public Address()
        {
        }
        public Address(string _Address1, string _Address2, string _Address3, double _Longitude, double _Latitude, double _Altitude)
        {
            Address1 = _Address1;
            Address2 = _Address2;
            Address3 = _Address3;
            Longitude = _Longitude;
            Latitude = _Latitude;
            Altitude = _Altitude;
        }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }
    }
}
