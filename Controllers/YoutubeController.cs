using AngleSharp.Dom;
using Microsoft.AspNetCore.Mvc;
using YouTubeConvertor2._0.Models;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace YouTubeConvertor2._0.Controllers
{
    public class YoutubeController : Controller
    {
        private readonly YoutubeClient _youtubeClient;

        public YoutubeController()
        {
            _youtubeClient = new YoutubeClient();
        }

        [HttpPost]
        public async Task<IActionResult> ConvertAudio(string url)
        {
            try
            {
                var video = await _youtubeClient.Videos.GetAsync(url);
                await _youtubeClient.Videos.DownloadAsync(url, "temp.mp3");

                var videoItem = new VideoItem
                {
                    Title = video.Title,
                    Artist = video.Author.ToString(),
                    Duriation = video.Duration,
                    ThumbnailUrl = video.Thumbnails[4].Url
                };

                return View("~/Views/Home/Index.cshtml", videoItem);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error! {e.Message}");
            }
        }

        public IActionResult DownloadFile(string title)
        {
            var fileBytes = System.IO.File.ReadAllBytes("temp.mp3");
            return File(fileBytes, "audio/mpeg", $"{title}.mp3");
        }
    }
}
