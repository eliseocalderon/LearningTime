using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace LearningTime.Services
{
    public class CoordService
    {
        private ILogger _logger;

        public CoordService(ILogger<CoordService> logger)
        {
            _logger = logger;
        }

        public async Task<CoordServiceResult> Lookup(string location)
        {
            var result = new CoordServiceResult
            {
                Success = false,
                Message = "Undetermined failure while looking up coordinates"
            };

            //Lookup coordinates
            var encodedName = WebUtility.UrlEncode(location);
            var bingMapsKey = Startup.Configuration["AppSettings:BingKey"];
            var url = $"http://dev.virtualearth.net/REST/v1/Locations?query={encodedName}&key={bingMapsKey}";

            var client = new HttpClient();

            var json = await client.GetStringAsync(url);

            var bingMapsResponse = JsonConvert.DeserializeObject<BingMapsResponse>(json);

            if (bingMapsResponse.resourceSets[0].resources[0].confidence != "High")
            {
                result.Message = $"Could not find a confident match for '{location}' as destination.";
            }
            else
            {
                var coords = bingMapsResponse.resourceSets[0].resources[0].geocodePoints[0].coordinates;
                result.Latitude = coords[0];
                result.Longitude = coords[1];
                result.Success = true;
                result.Message = "Success";
            }
            return result;
        }
    }
}
