using Microsoft.AspNetCore.Mvc;
using YouTubeConvertor2._0.Models;
using YoutubeExplode;
using YoutubeExplode.Converter;

namespace YouTubeConvertor2._0.Controllers
{
    public class YoutubeController : Controller
    {
        private readonly YoutubeClient _ytClient;
        private readonly string _path = Path.GetTempPath() + @"\YTConverterFiles\";

        public YoutubeController()
        {
            _ytClient = new YoutubeClient();
        }

        [HttpPost]
        public Task<IActionResult> ConvertAudio(string URL)
        {
            CleanPath();

            if (URL.Contains("playlist"))
            {
                return DownloadPlaylist(URL);
            }

            return DownloadSingleAudio(URL);
        }

        public void CleanPath()
        {
            if (Directory.Exists(_path))
            {
                Directory.Delete(_path, true);
                Directory.CreateDirectory(_path);
            }
            else
            {
                Directory.CreateDirectory(_path);
            }
        }

        public async Task<IActionResult> DownloadSingleAudio(string URL)
        {
            try
            {
                var video = await _ytClient.Videos.GetAsync(URL);
                string title = string.Join("I", video.Title.Split(Path.GetInvalidFileNameChars()));

                await _ytClient.Videos.DownloadAsync(URL, _path + $@"{title}.mp3");

                VideoItem item = new VideoItem
                {
                    Title = title,
                    Artist = video.Author.ToString(),
                    Duriation = video.Duration,
                    ThumbnailUrl = video.Thumbnails[4].Url
                };

                return View("~/Views/Home/Index.cshtml", item);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error! {e.Message}");
            }
        }

        public IActionResult DownloadFile(string title)
        {
            if (Directory.GetFiles(_path, "*", SearchOption.AllDirectories).Length == 1)
            {
                var fileBytes = System.IO.File.ReadAllBytes(_path + $"{title}.mp3");
                return File(fileBytes, "audio/mpeg", $"{title}.mp3");
            }

            return StatusCode(500);
        }
    }
}
