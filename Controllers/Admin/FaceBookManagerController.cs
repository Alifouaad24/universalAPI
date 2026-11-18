using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceBookManagerController : ControllerBase
    {
        private readonly UniversalDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _client;

        private readonly string PageId;
        private readonly string PageAccessToken;

        public FaceBookManagerController(
            UniversalDbContext db,
            HttpClient client,
            IConfiguration configuration)
        {
            _db = db;
            _client = client;
            _configuration = configuration;
            PageId = _configuration["Meta:PageId"]!;
            PageAccessToken = _configuration["Meta:AccessToken"]!;
        }

        #region Get Posts

        [HttpGet("posts")]
        public async Task<IActionResult> GetPosts()
        {
            var url = $"https://graph.facebook.com/v23.0/{PageId}/posts" + $"?fields=id,message,created_time" + $"&access_token={PageAccessToken}";

            var response = await _client.GetAsync(url);

            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        #endregion

        #region Create Post

        [HttpPost("PublishProduct")]
        public async Task<IActionResult> PublishProduct(string imageUrl, string title, string description, string productUrl)
        {
            var message = $"""
                🛍️ {title}

                {description}

                🔗 {productUrl}
                """;

            var content = new FormUrlEncodedContent(
            [
                new("url", imageUrl),
                new("caption", message),
                new("access_token", PageAccessToken)
            ]);

            var response = await _client.PostAsync($"https://graph.facebook.com/v23.0/{PageId}/photos", content);

            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        [HttpPost("publish-text")]
        public async Task<IActionResult> PublishText()
        {
            var message = "هذا منشور تجريبي من نظامي 🚀";

            var url = $"https://graph.facebook.com/v21.0/{PageId}/feed";

            var data = new Dictionary<string, string>
    {
        { "message", message },
        { "access_token", PageAccessToken }
    };

            var content = new FormUrlEncodedContent(data);

            var response = await _client.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(result);
            }

            return Ok(new
            {
                success = true,
                response = result
            });
        }

        #endregion

        #region Delete Post

        [HttpDelete("posts/{postId}")]
        public async Task<IActionResult> DeletePost(string postId)
        {
            var response = await _client.DeleteAsync($"https://graph.facebook.com/v23.0/{postId}" + $"?access_token={PageAccessToken}");

            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        #endregion

        #region Get Comments

        [HttpGet("posts/{postId}/comments")]
        public async Task<IActionResult> GetComments(string postId)
        {
            var response = await _client.GetAsync($"https://graph.facebook.com/v23.0/{postId}/comments" + $"?fields=id,message,from,created_time" +
                $"&access_token={PageAccessToken}");

            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        #endregion

        #region Reply To Comment

        [HttpPost("comments/{commentId}/reply")]
        public async Task<IActionResult> ReplyToComment(
            string commentId,
            [FromBody] string reply)
        {
            var content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                { "message", reply },
                { "access_token", PageAccessToken }
                });

            var response = await _client.PostAsync($"https://graph.facebook.com/v23.0/{commentId}/comments", content);

            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        #endregion

        #region Get Single Post Details

        [HttpGet("posts/details/{postId}")]
        public async Task<IActionResult> GetPostDetails(string postId)
        {
            var response = await _client.GetAsync(
                $"https://graph.facebook.com/v23.0/{postId}" + $"?fields=id,message,created_time,comments.summary(true),likes.summary(true)" +
                $"&access_token={PageAccessToken}");

            var result = await response.Content.ReadAsStringAsync();

            return Content(result, "application/json");
        }

        #endregion
    }
}
