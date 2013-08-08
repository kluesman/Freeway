using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Freeway.Classes
{
    class DirectionsRequestUriMapper : UriMapperBase
    {

        public override Uri MapUri(Uri uri)
        {
            string tempUri = Uri.UnescapeDataString(uri.ToString());

            // Does the Uri contain a request for driving directions or walking directions?
            if (tempUri.Contains("ms-drive-to") || tempUri.Contains("ms-walk-to"))
            {
                // Parse the Uri.
                char[] uriDelimiters = { '?', '=', '&' };
                string[] uriParameters = tempUri.Split(uriDelimiters);
                string destLatitude = uriParameters[4];
                string destLongitude = uriParameters[6];
                string destName = uriParameters[8];

                // Map the request for directions to the ShowDestination page of the app.
                return new Uri(
                    "/MapPage.xaml?" +
                    "Latitude=" + destLatitude + "&" +
                    "Longitude=" + destLongitude + "&" +
                    "Name=" + destName,
                    UriKind.Relative);
            }
            // Otherwise, handle the Uri normally.
            return uri;
        }
    }
}
