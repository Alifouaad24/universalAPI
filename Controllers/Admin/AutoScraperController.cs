using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoScraperController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://homedepot-scraper.omkar.cloud";
        private const string SheinBaseUrl = "https://shein-scraper.omkar.cloud";
        public AutoScraperController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("HomeDepotSearch")]
        public async Task<IActionResult> HomeDepotSearch(
        [FromQuery] string searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] string? sortBy = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("search_term is required");

            var apiKey = _configuration["HomeDepotScraper:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { message = "API key is not configured on the server" });

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("API-Key", apiKey); 

            var queryParams = new List<string>
        {
            $"search_term={Uri.EscapeDataString(searchTerm)}",
            $"page={page}"
        };

            if (!string.IsNullOrWhiteSpace(sortBy))
                queryParams.Add($"sort_by={Uri.EscapeDataString(sortBy)}");

            if (minPrice.HasValue)
                queryParams.Add($"min_price={minPrice.Value}");

            if (maxPrice.HasValue)
                queryParams.Add($"max_price={maxPrice.Value}");

            var url = $"{BaseUrl}/homedepot/search?{string.Join("&", queryParams)}";

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new
                    {
                        message = "Failed to fetch data from Home Depot scraper",
                        details = errorContent
                    });
                }

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = "Error contacting external API", error = ex.Message });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, new { message = "Request timed out" });
            }
        }

        [HttpGet("SheinSearchBYSKU")]
        public async Task<IActionResult> SheinSearchBYSKU(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] string? sort = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("query is required");

            var apiKey = _configuration["SheinScraper:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { message = "API key is not configured on the server" });

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("API-Key", apiKey);

            var queryParams = new List<string>
            {
                    $"product={Uri.EscapeDataString(query)}",
            };

            if (!string.IsNullOrWhiteSpace(sort))
                queryParams.Add($"sort={Uri.EscapeDataString(sort)}");

            if (minPrice.HasValue)
                queryParams.Add($"min_price={minPrice.Value}");

            if (maxPrice.HasValue)
                queryParams.Add($"max_price={maxPrice.Value}");

            var url = $"{SheinBaseUrl}/products/details?{string.Join("&", queryParams)}";

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new
                    {
                        message = "Failed to fetch data from Shein scraper",
                        details = errorContent
                    });
                }

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = "Error contacting external API", error = ex.Message });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, new { message = "Request timed out" });
            }
        }

        [HttpGet("SheinProductDetailsByURL")]
        public async Task<IActionResult> SheinProductDetailsByURL([FromQuery] string product)
        {
            if (string.IsNullOrWhiteSpace(product))
                return BadRequest("product is required");

            var apiKey = _configuration["SheinScraper:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { message = "API key is not configured on the server" });

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("API-Key", apiKey);

            var url = $"{SheinBaseUrl}/products/details?product={Uri.EscapeDataString(product)}";

            try
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new
                    {
                        message = "Failed to fetch data from Shein scraper",
                        details = errorContent
                    });
                }

                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = "Error contacting external API", error = ex.Message });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, new { message = "Request timed out" });
            }
        }
    }
}
