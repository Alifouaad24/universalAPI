using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoScraperController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory; 
        UniversalDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://homedepot-scraper.omkar.cloud";
        private const string SheinBaseUrl = "https://shein-scraper.omkar.cloud";
        public AutoScraperController(IHttpClientFactory httpClientFactory, IConfiguration configuration, UniversalDbContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _dbContext = dbContext;
        }


        #region
        [HttpGet("HomeDepotSearch")]
        public async Task<IActionResult> HomeDepotSearch(
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] string? sortBy = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("search_term is required");

            //var checkItem = await _dbContext.Items.Where(i => i.UPC == searchTerm).FirstOrDefaultAsync();
            //if (checkItem != null)
            //{
            //    return Ok(new { message = "Item already exists in the database", itemId = checkItem.ItemId });
            //}

            var apiKey = _configuration["HomeDepotScraper:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { message = "API key is not configured on the server" });

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("API-Key", apiKey);

            // ================= 1) طلب البحث =================
            var searchUrl = $"{BaseUrl}/products/search?query={Uri.EscapeDataString(searchTerm)}";

            try
            {
                var searchResponse = await client.GetAsync(searchUrl, cancellationToken);
                var searchContent = await searchResponse.Content.ReadAsStringAsync(cancellationToken);

                if (!searchResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)searchResponse.StatusCode, new
                    {
                        message = "Failed to fetch search results from Home Depot scraper",
                        details = searchContent
                    });
                }

                // ================= 2) استخراج item_id لأول منتج =================
                string? itemId;
                using (var doc = JsonDocument.Parse(searchContent))
                {
                    if (!doc.RootElement.TryGetProperty("products", out var products) ||
                        products.ValueKind != JsonValueKind.Array ||
                        products.GetArrayLength() == 0)
                    {
                        return NotFound(new { message = "No products found for this search term" });
                    }

                    var first = products[0];
                    if (!first.TryGetProperty("id", out var itemIdElement))
                        return StatusCode(502, new { message = "First product has no item_id" });

                    // item_id يأتي كنص، لكن نتعامل مع الرقم أيضاً احتياطاً
                    itemId = itemIdElement.ValueKind == JsonValueKind.Number
                        ? itemIdElement.GetRawText()
                        : itemIdElement.GetString();
                }

                if (string.IsNullOrWhiteSpace(itemId))
                    return StatusCode(502, new { message = "First product has an empty item_id" });

                // ================= 3) طلب تفاصيل المنتج =================
                var productUrl = $"{BaseUrl}/products/details?product={Uri.EscapeDataString(itemId)}";

                var productResponse = await client.GetAsync(productUrl, cancellationToken);
                var productContent = await productResponse.Content.ReadAsStringAsync(cancellationToken);

                if (!productResponse.IsSuccessStatusCode)
                {
                    return StatusCode((int)productResponse.StatusCode, new
                    {
                        message = "Failed to fetch product details from Home Depot scraper",
                        itemId,
                        details = productContent
                    });
                }
                // ================= 4) أخذ أعلى دقة من كل مجموعة صور =================
                var root = JsonNode.Parse(productContent) as JsonObject;
                var productNode = root?["product"] as JsonObject;
                if (productNode is null)
                    return StatusCode(502, new { message = "Invalid product JSON" });

                // الصور هنا قائمة كائنات فيها link و template؛ نأخذ أعلى دقة من template
                var largeImages = new JsonArray();

                if (productNode["images"] is JsonArray imgs)
                {
                    foreach (var img in imgs)
                    {
                        if (img is not JsonObject io) continue;

                        // template فيه <SIZE>؛ نضع 1000 لأعلى دقة، وإلا نأخذ link كما هو
                        var template = io["template"]?.GetValue<string>();
                        var link = io["link"]?.GetValue<string>();

                        var best = !string.IsNullOrWhiteSpace(template)
                            ? template.Replace("<SIZE>", "1000")
                            : link;

                        if (!string.IsNullOrWhiteSpace(best))
                            largeImages.Add(best);
                    }
                }

                productNode["images"] = largeImages;

                return Content(productNode.ToJsonString(), "application/json");

            }
            catch (JsonException ex)
            {
                return StatusCode(502, new { message = "Invalid JSON returned from search API", error = ex.Message });
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

        [HttpGet("SheinSearchByShareLink")]
        public async Task<IActionResult> SheinSearchByShareLink([FromQuery] string shareLink)
        {
            if (string.IsNullOrWhiteSpace(shareLink))
                return BadRequest("shareLink is required");

            var apiKey = _configuration["SheinScraper:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { message = "API key is not configured on the server" });

            var client = _httpClientFactory.CreateClient();

            var queryParams = new List<string>
    {
        $"product={Uri.EscapeDataString(shareLink)}",
        "country=nl",
        "language=en",
        "currency=USD"
    };

            var url = $"{SheinBaseUrl}/products/details?{string.Join("&", queryParams)}";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("API-Key", apiKey);

                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        message = "Failed to fetch data from Shein scraper",
                        details = content
                    });
                }

                var product = MapSheinProduct(content);
                return Ok(product);
            }
            catch (JsonException)
            {
                return StatusCode(502, new { message = "Invalid response format from external API" });
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

        private static SheinProductDto MapSheinProduct(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var dto = new SheinProductDto
            {
                Name = GetString(root, "name"),
                Sku = GetString(root, "sku")
            };

            // الصور
            if (root.TryGetProperty("images", out var images) && images.ValueKind == JsonValueKind.Array)
            {
                dto.Images = images.EnumerateArray()
                    .Where(i => i.ValueKind == JsonValueKind.String)
                    .Select(i => i.GetString()!)
                    .ToList();
            }

            // السعر والعملة
            if (root.TryGetProperty("pricing", out var pricing) &&
                pricing.TryGetProperty("sale_price", out var salePrice))
            {
                if (salePrice.TryGetProperty("amount", out var amount) && amount.ValueKind == JsonValueKind.Number)
                    dto.Price = amount.GetDecimal();

                dto.Currency = GetString(salePrice, "currency");
            }

            // الفئة
            if (root.TryGetProperty("category", out var category))
                dto.Category = GetString(category, "name");

            return dto;
        }

        private static string GetString(JsonElement element, string property) =>
            element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
                ? value.GetString() ?? string.Empty
                : string.Empty;

        #endregion


        ///////////////////////////////////////// Apify /////////////////////////////////////////////

        [HttpGet("HomeDepotByUpcFromApify/{upc}")]
        public Task<IActionResult> HomeDepotByUpcFromApify(string upc, CancellationToken ct) => Handle(async () =>
        {
            if (!Regex.IsMatch(upc, @"^\d{8,14}$"))
                return BadRequest(new { success = false, message = "UPC غير صالح." });

            var items = await RunActor(
                _configuration["Apify:HomeDepotActor"]!,
                new
                {
                    keywords = new[] { upc },
                    scrapeMode = "overview",
                    inStoreOnly = false,
                    sort = "top_sellers",
                    storeId = "121",
                    maxResults = 1
                },
                ct);

            // نفضّل المنتج الذي يحتوي الـ UPC نفسه، وإلا أول نتيجة
            var product = items.FirstOrDefault(i => i!.ToJsonString().Contains(upc))
                          ?? items.FirstOrDefault();

            return product is null
                ? NotFound(new
                {
                    success = false,
                    message = "لم يُعثر على منتج بهذا الـ UPC.",
                    runId = _lastRun?["id"]?.ToString(),
                    status = _lastRun?["status"]?.ToString(),
                    statusMessage = _lastRun?["statusMessage"]?.ToString(),
                    datasetId = _lastRun?["defaultDatasetId"]?.ToString()
                })
                : Ok(new { success = true, upc, product });
        });

        // ========== 2. SHEIN بالـ SKU ==========
        [HttpGet("SheinBySkuFromApify/{sku}")]
        public Task<IActionResult> SheinBySkuFromApify(string sku, CancellationToken ct) => Handle(async () =>
        {
            if (!Regex.IsMatch(sku, @"^[A-Za-z0-9\-]{4,40}$"))
                return BadRequest(new { success = false, message = "SKU غير صالح." });

            var items = await RunActor(
                _configuration["Apify:SheinActor"]!,
                new { startUrls = new[] { new { url = $"https://us.shein.com/pdsearch/{sku}/" } } },
                ct);

            var product = items.FirstOrDefault(i =>
                              i!.ToJsonString().Contains(sku, StringComparison.OrdinalIgnoreCase))
                          ?? items.FirstOrDefault();

            return product is null
                ? NotFound(new { success = false, message = "لم يُعثر على منتج بهذا الـ SKU." })
                : Ok(new { success = true, sku, product });
        });

        // ========== 3. منتجات سلة SHEIN ==========
        [HttpPost("SheinCartFromApify/cart")]
        public Task<IActionResult> SheinCartFromApify([FromQuery] string url, CancellationToken ct) => Handle(async () =>
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || !uri.Host.Contains("shein"))
                return BadRequest(new { success = false, message = "رابط سلة SHEIN غير صالح." });

            var items = await RunActor(
                _configuration["Apify:SheinActor"]!,
                new { startUrls = new[] { new { url } } },
                ct);

            return Ok(new { success = items.Count > 0, count = items.Count, products = items });
        });

        // ================= مساعدات =================

        private JsonObject? _lastRun;   // معلومات آخر تشغيل، للتشخيص

        private async Task<JsonArray> RunActor(string actor, object input, CancellationToken ct)
        {
            var token = _configuration["Apify:Token"];
            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };

            // 1. تشغيل الـ Actor
            using var startRes = await client.PostAsJsonAsync(
                $"https://api.apify.com/v2/acts/{actor}/runs?token={token}&waitForFinish=60", input, ct);
            var startBody = await startRes.Content.ReadAsStringAsync(ct);

            if (!startRes.IsSuccessStatusCode)
                throw new ApifyException((int)startRes.StatusCode, startBody);

            var run = JsonNode.Parse(startBody)!["data"]!.AsObject();

            // 2. انتظار انتهاء التشغيل (Apify يسمح بانتظار 60 ثانية لكل طلب)
            for (int i = 0; i < 5; i++)
            {
                var status = run["status"]?.GetValue<string>();
                if (status is not ("READY" or "RUNNING")) break;

                var runId = run["id"]!.GetValue<string>();
                var pollBody = await client.GetStringAsync(
                    $"https://api.apify.com/v2/actor-runs/{runId}?token={token}&waitForFinish=60", ct);
                run = JsonNode.Parse(pollBody)!["data"]!.AsObject();
            }

            _lastRun = run;

            // 3. جلب النتائج
            var datasetId = run["defaultDatasetId"]!.GetValue<string>();
            var itemsBody = await client.GetStringAsync(
                $"https://api.apify.com/v2/datasets/{datasetId}/items?token={token}&clean=true", ct);

            return JsonNode.Parse(itemsBody) as JsonArray ?? new JsonArray();
        }

        private async Task<IActionResult> Handle(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (ApifyException ex)
            {
                return StatusCode(502, new { success = false, message = "فشل تشغيل Apify.", ex.Status, error = ex.Body });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, new { success = false, message = "انتهت المهلة." });
            }
        }

        private sealed class ApifyException(int status, string body) : Exception(body)
        {
            public int Status { get; } = status;
            public string Body { get; } = body;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
    }
}


public class SheinProductDto
{
    public string Name { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}