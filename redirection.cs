using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using Azure.Storage.Blobs;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;

namespace SilvaGunnerPokemon
{
	class itemsclass
	{
		public List<string> items { get; set; }
	};

	public static class redirection
	{
		private static Random rng = new Random();
		[FunctionName("redirection")]
		public static HttpResponseMessage Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
			[Blob("silvagunnerlist/items", FileAccess.Read)] Stream items,
			ILogger log)
		{
			List<string> items_list = new StreamReader(items).ReadToEnd().Split('\n').ToList();
			string random = items_list[rng.Next(items_list.Count)];
			string html = $"<head><meta http-equiv=\"refresh\" content=\"0; URL = https://www.youtube.com/watch?v={random}\"/></head>";
			var response = new HttpResponseMessage(HttpStatusCode.OK);
			response.Content = new StringContent(html);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return response;
		}
	}
}
