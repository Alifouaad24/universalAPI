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


        [HttpGet("SheinCartProducts")]
        public async Task<IActionResult> SheinCartProducts(
        [FromQuery] string link,
         [FromQuery] string language = "ar",
    [FromQuery] string currency = "USD",
        CancellationToken cancellationToken = default)
        {
            var inddex = link.IndexOf("https");
            link = link.Substring(inddex);
            if (string.IsNullOrWhiteSpace(link))
                return BadRequest(new { message = "link is required" });

            // التحقق من أن الرابط صحيح ويعود لـ Shein
            if (!Uri.TryCreate(link.Trim(), UriKind.Absolute, out var cartUri) ||
                (cartUri.Scheme != Uri.UriSchemeHttps && cartUri.Scheme != Uri.UriSchemeHttp) ||
                !cartUri.Host.Contains("shein", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "link must be a valid Shein cart link" });
            }

            var apiKey = _configuration["SheinScraper:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { message = "API key is not configured on the server" });

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("API-Key", apiKey);
            client.Timeout = TimeSpan.FromSeconds(500); // جلب السلة قد يأخذ وقتاً أطول من البحث

            // var url = $"{SheinBaseUrl}/cart/products?link={Uri.EscapeDataString(cartUri.ToString())}&language=ar&currency=USD";
            var url = $"{SheinBaseUrl}/cart/products" +
                      $"?link={Uri.EscapeDataString(cartUri.AbsoluteUri)}" +
                      $"&language={Uri.EscapeDataString(language)}" +
                      $"&currency={Uri.EscapeDataString(currency.ToUpperInvariant())}";
            try
            {
                var response = await client.GetAsync(url, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        message = "Failed to fetch cart products from Shein scraper",
                        details = content
                    });
                }

                return Content(content, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, new { message = "Error contacting external API", error = ex.Message });
            }
            catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                return StatusCode(504, new { message = "Request timed out" });
            }
        }

        //[HttpGet("SheinSearchBYSKU")]
        //public async Task<IActionResult> SheinSearchBYSKU(
        //    [FromQuery] string query,
        //    [FromQuery] int page = 1,
        //    [FromQuery] string? sort = null,
        //    [FromQuery] decimal? minPrice = null,
        //    [FromQuery] decimal? maxPrice = null)
        //{
        //    if (string.IsNullOrWhiteSpace(query))
        //        return BadRequest("query is required");

        //    var apiKey = _configuration["SheinScraper:ApiKey"];
        //    if (string.IsNullOrWhiteSpace(apiKey))
        //        return StatusCode(500, new { message = "API key is not configured on the server" });

        //    var client = _httpClientFactory.CreateClient();
        //    client.DefaultRequestHeaders.Add("API-Key", apiKey);

        //    var queryParams = new List<string>
        //    {
        //            $"product={Uri.EscapeDataString(query)}",
        //    };

        //    if (!string.IsNullOrWhiteSpace(sort))
        //        queryParams.Add($"sort={Uri.EscapeDataString(sort)}");

        //    if (minPrice.HasValue)
        //        queryParams.Add($"min_price={minPrice.Value}");

        //    if (maxPrice.HasValue)
        //        queryParams.Add($"max_price={maxPrice.Value}");

        //    var url = $"{SheinBaseUrl}/products/details?{string.Join("&", queryParams)}";

        //    try
        //    {
        //        var response = await client.GetAsync(url);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var errorContent = await response.Content.ReadAsStringAsync();
        //            return StatusCode((int)response.StatusCode, new
        //            {
        //                message = "Failed to fetch data from Shein scraper",
        //                details = errorContent
        //            });
        //        }

        //        var content = await response.Content.ReadAsStringAsync();
        //        return Content(content, "application/json");
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        return StatusCode(502, new { message = "Error contacting external API", error = ex.Message });
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        return StatusCode(504, new { message = "Request timed out" });
        //    }
        //}


        [HttpGet("SheinSearchBYSKU")]
        public async Task<IActionResult> SheinSearchBYSKU(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] string? sort = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string country = "us",
            [FromQuery] string language = "en",
            [FromQuery] string currency = "usd")
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("query is required");

            var apiKey = _configuration["SheinScraper:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { message = "API key is not configured on the server" });

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromMinutes(3);

            try
            {
                // ===== 1) البحث بالـ SKU =====
                var searchParams = new List<string>
        {
            $"query={Uri.EscapeDataString(query.Trim())}",
            $"page={page}"
        };
                if (!string.IsNullOrWhiteSpace(sort))
                    searchParams.Add($"sort={Uri.EscapeDataString(sort)}");
                if (minPrice.HasValue)
                    searchParams.Add($"min_price={minPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
                if (maxPrice.HasValue)
                    searchParams.Add($"max_price={maxPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");

                var searchUrl = $"{SheinBaseUrl}/search/products?{string.Join("&", searchParams)}";
                var (searchOk, searchStatus, searchBody) = await SendSheinRequest(client, searchUrl, apiKey);

                if (!searchOk)
                    return StatusCode(searchStatus, new
                    {
                        message = "Failed to search Shein scraper",
                        details = searchBody
                    });

                // ===== 2) استخراج goods_id من أول نتيجة =====
                string? goodsId = null;
                using (var doc = JsonDocument.Parse(searchBody))
                {
                    if (doc.RootElement.TryGetProperty("results", out var results)
                        && results.ValueKind == JsonValueKind.Array
                        && results.GetArrayLength() > 0
                        && results[0].TryGetProperty("goods_id", out var idProp))
                    {
                        goodsId = idProp.ValueKind == JsonValueKind.Number
                            ? idProp.GetRawText()
                            : idProp.GetString();
                    }
                }

                if (string.IsNullOrWhiteSpace(goodsId))
                    return NotFound(new { message = $"No product found for '{query}'" });

                // ===== 3) طلب التفاصيل =====
                var detailsParams = new List<string>
                {
                    $"product={Uri.EscapeDataString(goodsId)}",
                    $"country={Uri.EscapeDataString(country)}",
                    $"language={Uri.EscapeDataString(language)}",
                    $"currency={Uri.EscapeDataString(currency)}"
                };

                var detailsUrl = $"{SheinBaseUrl}/products/details?{string.Join("&", detailsParams)}";
                var (detailsOk, detailsStatus, detailsBody) = await SendSheinRequest(client, detailsUrl, apiKey);

                if (!detailsOk)
                    return StatusCode(detailsStatus, new
                    {
                        message = "Failed to fetch product details from Shein scraper",
                        goodsId,
                        details = detailsBody
                    });

                return Content(detailsBody, "application/json");
            }
            catch (JsonException)
            {
                return StatusCode(502, new { message = "Invalid response from Shein scraper" });
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

        private static async Task<(bool ok, int status, string body)> SendSheinRequest(
            HttpClient client, string url, string apiKey)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("API-Key", apiKey);

            using var response = await client.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();
            return (response.IsSuccessStatusCode, (int)response.StatusCode, body);
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
