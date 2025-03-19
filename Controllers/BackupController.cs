using Microsoft.AspNetCore.Mvc;
using YouBrowse.Models;
using YouBrowse.Services;

namespace YouBrowse.Controllers
{
    public class BackupController : Controller
    {
        private readonly YouTubeApiService _youTubeApiService;

        public BackupController(YouTubeApiService youTubeApiService)
        {
            _youTubeApiService = youTubeApiService;
        }

        public IActionResult Index()
        {
            return View(new YouTubeSearchResult()); // Empty view initially
        }
        [HttpPost]
        public IActionResult Index(string query, string uploadDate, string videoDuration, string pageToken = "", int page = 1)
        {
            try
            {
                var result = _youTubeApiService.SearchVideosAsync(query, uploadDate, videoDuration, pageToken).Result;

                var model = new YouTubeSearchResult
                {
                    Videos = result.Videos,
                    NextPageToken = result.NextPageToken,
                    PrevPageToken = result.PrevPageToken,
                    CurrentPage = page
                };

                return View(model);
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // Check if it's a rate limit error
                if (ex.Error != null && ex.Error.Errors.Any(e => e.Reason == "quotaExceeded"))
                {
                    ViewBag.ErrorMessage = "YouTube API rate limit exceeded. Please try again later.";
                }
                else
                {
                    ViewBag.ErrorMessage = "An error occurred while fetching videos. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred: " + ex.Message;
            }

            return View(new YouTubeSearchResult()); // Return empty model with error message
        }
    }
}
