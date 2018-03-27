﻿using System.Threading.Tasks;
using CSULB.GetUsGrub.Models;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace CSULB.GetUsGrub.BusinessLogic
{
    /// <summary>
    /// Service for accessing Google's Geocoding API to handle geocoding.
    /// 
    /// @Author: Brian Fann
    /// @Last Updated: 3/22/18
    /// </summary>
    public class GoogleGeocodeService : IGeocodeServiceAsync
    {
        private string BuildUrl(IAddress address, string key)
        {
            var url = "https://maps.googleapis.com/maps/api/geocode/json?";
            url += $"address={address.Street1} {address.Street2}, {address.City}, {address.State} {address.Zip}";
            url.Replace(' ', '+');
            url += $"&key={key}";

            return url;
        }

        /// <summary>
        /// Converts an address into geocoordinates using Google's Geocoding API.
        /// </summary>
        /// <param name="address">Address to geocode.</param>
        /// <returns>Coordinates of address.</returns>
        public async Task<ResponseDto<IGeoCoordinates>> GeocodeAsync(IAddress address)
        {
            try
            {
                // Retrieve key from configuration and build url for the get request.
                var key = ConfigurationManager.AppSettings["GoogleGeocodingApi"];
                var url = BuildUrl(address, key);

                // Send get request and parse the response
                var responseJson = await new GoogleBackoffGetRequest(url, "OVER_QUERY_LIMIT").TryExecute();
                var responseObj = JObject.Parse(responseJson);

                // Retrieve status code from the response
                var status = (string)responseObj.SelectToken("status");

                // Exit early if status is not OK.
                if (!status.Equals("OK"))
                {
                    if (status.Equals("ZERO_RESULTS"))
                    {
                        status = "Invalid Input";
                    }
                    else
                    {
                        status = "Unexpected Error";
                    }

                    return new ResponseDto<IGeoCoordinates>()
                    {
                        Error = status
                    };
                }

                // Retrieve latitude and longitude data from response and return coordinates.
                var lat = (float)responseObj.SelectToken("results[0].geometry.location.lat");
                var lng = (float)responseObj.SelectToken("results[0].geometry.location.lng");
                
                return new ResponseDto<IGeoCoordinates>()
                {
                    Data = new GeoCoordinates()
                    {
                        Latitude = lat,
                        Longitude = lng
                    }
                };
            }
            // If API Key not found, throw error.
            catch (ConfigurationErrorsException)
            {
                throw new System.Exception("Google Geocoding API Key not found.");
            }
        }
    }
}