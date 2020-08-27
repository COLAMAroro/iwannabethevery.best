using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;
using Microsoft.Extensions.Logging;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using System.Web.Http;

namespace SilvaGunnerPokemon
{
    public static class rebuild_video_list
    {

        private static string YouTubeToken = Environment.GetEnvironmentVariable("YouTubeToken");

		public static List<string> GetVideos()
		{
			List<string> result = new List<string>();
			try
			{
				var yt = new YouTubeService(new BaseClientService.Initializer() { ApiKey = YouTubeToken });
				var channelsListRequest = yt.Channels.List("contentDetails");
				channelsListRequest.Id = "UC9ecwl3FTG66jIKA9JRDtmg";
				var channelsListResponse = channelsListRequest.Execute();
				foreach (var channel in channelsListResponse.Items)
				{
					// of videos uploaded to the authenticated user's channel.
					var uploadsListId = channel.ContentDetails.RelatedPlaylists.Uploads;
					var nextPageToken = "";
					while (nextPageToken != null)
					{
						var playlistItemsListRequest = yt.PlaylistItems.List("snippet");
						playlistItemsListRequest.PlaylistId = uploadsListId;
						playlistItemsListRequest.MaxResults = 50;
						playlistItemsListRequest.PageToken = nextPageToken;
						// Retrieve the list of videos uploaded to the authenticated user's channel.
						var playlistItemsListResponse = playlistItemsListRequest.Execute();
						var matching = (from i in playlistItemsListResponse.Items where i.Snippet.Title.ToLower().Contains("pokemon") || i.Snippet.Title.ToLower().Contains("pokémon") select i.Snippet.ResourceId.VideoId).ToList();
						if (matching.Count != 0)
							result.AddRange(matching);
						nextPageToken = playlistItemsListResponse.NextPageToken;
					}
				}
			}
			catch (Exception e)
			{
				throw e;
			}

			return result;
		}

		[FunctionName("rebuild_video_list")]
        public static void Run(
			[TimerTrigger("0 0 * * * *")]TimerInfo myTimer,
			[Blob("silvagunnerlist/items", FileAccess.Write)] Stream items,
			ILogger log
		)
        {
			try
			{
				List<string> items_list = GetVideos();
				items.Write(Encoding.ASCII.GetBytes(String.Join('\n', items_list)));
				log.LogInformation($"Updated the video list at {DateTime.Now}");
			} catch (Exception e)
            {
				log.LogError($"Error updating the database:\n{e}");
            }
        }
    }
}
