using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Frontend.Models;
using System.Net.Http;

namespace Frontend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(string data)
        {
            string id = null; 
            //TODO: send data in POST request to backend and read returned id value from response
            if (data == null) {
                return Ok("Empty request.");
            }
            string url = "http://127.0.0.1:5000/api/values";
            HttpClient client = new HttpClient();
            Console.WriteLine("User entered data: " + data);
            var response = client.PostAsJsonAsync(url, data);
            id = response.Result.Content.ReadAsStringAsync().Result;

            return new RedirectResult("http://127.0.0.1:5001/Home/TextDetails/" + result);
            return Ok(id);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult TextDetails(string id)
		{
			string details = SendGetRequest("http://127.0.0.1:5000/api/values/" + id).Result;
			ViewData["Message"] = details;
			return View();
		}

        private async Task<string> SendGetRequest(string requestUri)
		{
			var response = await httpClient.GetAsync(requestUri);
			string value = await response.Content.ReadAsStringAsync();
			if (response.IsSuccessStatusCode && value != null)
			{
				return value;
			}
			return response.StatusCode.ToString();
		}


    }
}
