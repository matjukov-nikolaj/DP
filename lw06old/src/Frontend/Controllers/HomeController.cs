using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;
using Newtonsoft.Json;
using Core;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        private static Dictionary<string, string> properties = Configuration.GetParameters();

        private static string API_VALUES_ROUTE = "/api/values/";
        private static string TEXT_DETAILS_ROUTE ="/Home/TextDetails/";
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        public IActionResult TextDetails(string id)
		{
			string details = SendGetRequest(properties["BACKEND_HOST"] + API_VALUES_ROUTE + id).Result;
			ViewData["Message"] = details;
			return View();
		}

        [HttpPost]
        public IActionResult Upload(string message, string location)
        {
            string id = null; 
            //TODO: send data in POST request to backend and read returned id value from response
            if (message == null || location == null) {
                return Ok("Empty request.");
            }
            string url = properties["BACKEND_HOST"] + API_VALUES_ROUTE;
            HttpClient client = new HttpClient();
            Console.WriteLine("User entered data: " + message + " Location: " + location);
            Message messageObj = new Message(message, location);
            string data = JsonConvert.SerializeObject(messageObj);
            var response = client.PostAsJsonAsync(url, data);
            id = response.Result.Content.ReadAsStringAsync().Result;

            return new RedirectResult(properties["BACKEND_HOST"] + TEXT_DETAILS_ROUTE + id);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> SendGetRequest(string requestUri)
		{
            HttpClient client = new HttpClient();
			var response = await client.GetAsync(requestUri);
			string value = await response.Content.ReadAsStringAsync();
			if (response.IsSuccessStatusCode && value != null)
			{
				return value;
			}
			return response.StatusCode.ToString();
		}


    }
}
