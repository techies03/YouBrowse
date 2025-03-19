using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using YouBrowse.Models;

namespace YouBrowse.Services
{
    public class YouTubeApiService
    {
        private readonly YouTubeService _youtubeService;

        public YouTubeApiService(IConfiguration configuration)
        {
            var apiKey = configuration["YouTube:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("YouTube API key is missing from configuration.");

            _youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = apiKey,
                ApplicationName = "YouBrowse"
            });
        }

        public async Task<YouTubeSearchResult> SearchVideosAsync(
            string query, string uploadDate, string videoDuration,
            string pageToken = "", int maxResults = 9)
        {
            var searchRequest = _youtubeService.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.MaxResults = maxResults;
            searchRequest.Type = "video";
            searchRequest.PageToken = pageToken;

            // Apply upload date filter
            if (!string.IsNullOrEmpty(uploadDate))
            {
                searchRequest.PublishedAfterDateTimeOffset = uploadDate switch
                {
                    "today" => DateTimeOffset.UtcNow.AddDays(-1),
                    "this_week" => DateTimeOffset.UtcNow.AddDays(-7),
                    "this_month" => DateTimeOffset.UtcNow.AddMonths(-1),
                    _ => searchRequest.PublishedAfterDateTimeOffset
                };
            }

            // Apply video duration filter
            if (!string.IsNullOrEmpty(videoDuration))
            {
                searchRequest.VideoDuration = videoDuration switch
                {
                    "short" => SearchResource.ListRequest.VideoDurationEnum.Short__,
                    "medium" => SearchResource.ListRequest.VideoDurationEnum.Medium,
                    "long" => SearchResource.ListRequest.VideoDurationEnum.Long__,
                    _ => searchRequest.VideoDuration
                };
            }

            var searchResponse = await searchRequest.ExecuteAsync();

            return new YouTubeSearchResult
            {
                Videos = searchResponse.Items.Select(item => new YouTubeVideo
                {
                    VideoId = item.Id.VideoId,
                    Title = item.Snippet.Title,
                    ChannelName = item.Snippet.ChannelTitle,
                    PublishedAt = item.Snippet.PublishedAtDateTimeOffset?.UtcDateTime.ToString("yyyy-MM-dd") ?? "Unknown",
                    ThumbnailUrl = item.Snippet.Thumbnails.Medium.Url
                }).ToList(),
                NextPageToken = searchResponse.NextPageToken,
                PrevPageToken = searchResponse.PrevPageToken
            };
        }
    }
}
