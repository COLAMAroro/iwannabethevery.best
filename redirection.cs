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
		public static IActionResult Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
			[Blob("silvagunnerlist/items", FileAccess.Read)] Stream items,
			ILogger log)
		{
			List<string> items_list = new StreamReader(items).ReadToEnd().Split('\n').ToList();
			string random = items_list[rng.Next(items_list.Count)];
			return new RedirectResult("https://www.youtube.com/watch?v=" + random, false, false);
		}
	}
}
