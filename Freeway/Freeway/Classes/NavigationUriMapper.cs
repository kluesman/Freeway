﻿
using System;
using System.Windows.Navigation;
using System.Net;

namespace Freeway.Classes
{
    public class NavigationUriMapper : UriMapperBase
    {
        private static string TargetPageName = "MapPage.xaml";
        private string tempUri;

        public override Uri MapUri(Uri uri)
        {
            tempUri = uri.ToString();

            // Parse URI when launched by App Connect from Search
            if (tempUri.Contains("/SearchExtras"))
            {
                // Decode all characters in the URI.
                tempUri = HttpUtility.UrlDecode(tempUri);

                // Create a new URI for place cards.
                if (tempUri.Contains("Bing_Places"))
                {
                    return GetPlaceCardUri(tempUri);
                }

            }

            // Immediately return the URI when it is not related to App Connect for Search.
            return uri;
        }
        // This method extracts the string values that correspond to parameters in an App Connect URI.
        private string GetURIParameterValue(string parameteridentifier, string uri)
        {
            string tempValue = "";

            // If the parameter exists in the string, extract the corresonding parameter value.
            if (uri.Contains(parameteridentifier))
            {
                string subUri;

                // Extract the characters that contain and follow the parameter identifier.
                subUri = uri.Substring(uri.LastIndexOf(parameteridentifier));

                // Remove the parameter identifier from the substring.
                subUri = subUri.Replace(parameteridentifier, "");

                // Obtain the position of the next parameter in the substring.
                int nextParameterPosition = FindNextParameter(subUri);


                if (nextParameterPosition < int.MaxValue)
                {
                    // Remove the characters that contain and follow the next parameter.
                    tempValue = subUri.Substring(0, nextParameterPosition);
                }
                else
                {
                    // No more parameters follow in the string. 
                    tempValue = subUri;
                }

                // Encode the parameter values to help prevent issues in the URI.
                tempValue = HttpUtility.UrlEncode(tempValue);
            }

            return tempValue;
        }
        /// Returns the string position of the next App Connect URI parameter, if applicable.
        private int FindNextParameter(string subUri)
        {
            int lowestPosition = int.MaxValue;
            int tempPosition;

            tempPosition = subUri.IndexOf("&ProductName");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&Category");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&PlaceName");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("?PlaceName");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&PlaceLatitude");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&PlaceLongitude");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&PlaceAddress");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            tempPosition = subUri.IndexOf("&MovieName");
            if ((tempPosition > -1) && (tempPosition < lowestPosition)) lowestPosition = tempPosition;

            return lowestPosition;
        }
        private Uri GetPlaceCardUri(string uri)
        {
            // Extract parameter values from URI.
            string PlaceNameValue = GetURIParameterValue("PlaceName=", uri);
            string PlaceLatitudeValue = GetURIParameterValue("PlaceLatitude=", uri);
            string PlaceLongitudeValue = GetURIParameterValue("PlaceLongitude=", uri);
            string PlaceAddressValue = GetURIParameterValue("PlaceAddress=", uri);
            string CategoryValue = GetURIParameterValue("Category=", uri);

            // Create new URI.
            string NewURI = String.Format("/{0}?PlaceName={1}&PlaceLatitude={2}&PlaceLongitude={3}&PlaceAddress={4}&Category={5}",
                                   TargetPageName, PlaceNameValue, PlaceLatitudeValue, PlaceLongitudeValue, PlaceAddressValue, CategoryValue);

            return new Uri(NewURI, UriKind.Relative);
        }
    }
}
