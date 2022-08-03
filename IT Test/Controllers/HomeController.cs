using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Mvc;
using System.Data;
using System.ComponentModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using IT_Test.Models;
using Microsoft.AspNetCore.Mvc;

namespace IT_Test.Controllers
{
    public class HomeController : Controller
    {
        /*
         * Part 1 – 1.5 hours 

        Create a .Net MVC app where 

        Step 1 - the user enters a city name. 

        Step 2- The app makes a call to the Open Weather API service and get latitude and longitude for the city we specified.
                API - http://api.openweathermap.org/geo/1.0/direct?q={city name},{state code},{country code}&limit={limit}&appid={API key} 

        Step 3- The app uses the lat/long in step 2 and makes a call to get the current weather for this city using a second 
                API - http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key} 

        Step 4 – The app shows the current weather info to the user.

         * 
         */


        const string APIKey = "c11e0c7cb21bbbe31f6b0b7421502508";

        public ActionResult Index()
        {
            //Read the City data from /App_Data/AustraliaCity.json

            return View();
        }

        public ActionResult About()
        {


            return View();
        }

        public ActionResult Contact()
        {


            return View();
        }
        public static DataTable ReadFileToDataTable(string json)
        {
            DataTable table = new DataTable();
            //code goes to here
            return table;
        }

        /// <summary>
        /// Get weather data 
        /// Example path : https://localhost:44396/Home/search?cityName=london
        /// </summary>
        /// <param name="SearchDTO"></param>
        /// <returns>WeatherDTO</returns>

        public async Task<ActionResult> Search(string cityName)
        {
            if (string.IsNullOrEmpty(cityName))
                return Json(new { errorDescrption = "City name is empty", statusCode = 400 }, JsonRequestBehavior.AllowGet);

            var weatherDirect = await _GetCityLatitudeLongitude(cityName);
            if(weatherDirect == null)
                return Json(new { errorDescrption = "Please enter valid city name ", statusCode = 400 }, JsonRequestBehavior.AllowGet);
            var weather = await _GetCityWeatherByLatitudeLongitude(weatherDirect.lat, weatherDirect.lon);
            return Json(weather, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns latitude , longitude,local names, state, country with given city name
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns>WeatherDirectModel</returns>
        private async Task<WeatherDirectDTO> _GetCityLatitudeLongitude(string cityName)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var baseURL = "http://api.openweathermap.org/geo/1.0/direct";
                    var query = "?q=" + cityName + "&limit=" + 1 + "&appid=" + APIKey;
                    var url = baseURL + query;
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var weatherDirectModel = JsonConvert.DeserializeObject<List<WeatherDirectDTO>>(responseBody);
                    return weatherDirectModel.FirstOrDefault();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
            return null;

        }

        /// <summary>
        /// Returns weather details with given latitude and longitude
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns>WeatherModel</returns>
        private async Task<WeatherDTO> _GetCityWeatherByLatitudeLongitude(double lat, double lon)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var baseURL = "http://api.openweathermap.org/data/2.5/weather";
                    var query = "?lat=" + lat + "&lon=" + lon + "&appid=" + APIKey;
                    var url = baseURL + query;
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    var weather = JsonConvert.DeserializeObject<WeatherDTO>(responseBody);
                    return weather;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("\nException Caught!");
                    Console.WriteLine("Message :{0} ", e.Message);
                }
            }
            return null;
        }
    }
}
