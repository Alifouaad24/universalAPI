using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Universal_server.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class EbayController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration configuration;
        private readonly UniversalDbContext _context;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string ruName = "https://dutyapi.somee.com/api/Ebay/callback";
        private readonly HttpClient _httpClient;
        public EbayController(IHttpClientFactory httpClientFactory, HttpClient httpClient, IMemoryCache cache, UniversalDbContext context, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _httpClient = httpClient;
            _context = context;
            this.configuration = configuration;
            clientId = configuration["Ebay:clientId"];
            clientSecret = configuration["Ebay:clientSecret"];
        }

        private string GetRequiredConfig(string key)
        {
            var value = configuration[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Missing required configuration value '{key}' in appsettings.json.");
            return value;
        }

        [HttpGet("login")]
        public IActionResult LoginToEbay()
        {
            var scopes = string.Join(" ",
                "https://api.ebay.com/oauth/api_scope/sell.inventory",
                "https://api.ebay.com/oauth/api_scope/sell.inventory.readonly",
                "https://api.ebay.com/oauth/api_scope/sell.account",
                "https://api.ebay.com/oauth/api_scope/sell.fulfillment",
                "https://api.ebay.com/oauth/api_scope/commerce.identity.readonly"

            );


            var encodedRedirectUri = Uri.EscapeDataString(ruName);

            //var url = $"https://auth.ebay.com/oauth2/authorize?client_id={clientId}&response_type=code&redirect_uri={encodedRedirectUri}&scope={Uri.EscapeDataString(scopes)}";
            var url = $"https://auth.ebay.com/oauth2/authorize?client_id={clientId}&response_type=code&redirect_uri={encodedRedirectUri}&scope={Uri.EscapeDataString(scopes)}&prompt=consent";


            return Redirect(url);
        }


        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
                return BadRequest(new { error = "Code is missing" });

            var token = await ExchangeCodeForToken(code);

            var tokenId = Guid.NewGuid().ToString();
            _cache.Set(tokenId + "_access", token.access_token, TimeSpan.FromMinutes(110));
            _cache.Set(tokenId + "_refresh", token.refresh_token);

            var angularUrl = $"https://duty.somee.com/MainScreen?tokenId={tokenId}";
            return Redirect(angularUrl);
        }

        private async Task<(string access_token, string refresh_token)> ExchangeCodeForToken(string code)
        {
            using var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/identity/v1/oauth2/token");
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", ruName }
        });

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Token exchange failed: {content}");

            var tokenData = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(content);
            return (tokenData.access_token, tokenData.refresh_token);
        }

        private string GetValidAccessToken(string tokenId)
        {
            if (_cache.TryGetValue(tokenId + "_access", out string token) && !string.IsNullOrEmpty(token))
                return token;

            throw new Exception("Access token not found. Please authenticate with eBay.");
        }

        /// /////////////////////////////////////////////
        [HttpPost("save-token")]
        public IActionResult SaveToken([FromBody] TokenInput input)
        {
            if (string.IsNullOrEmpty(input.AccessToken))
                return BadRequest(new { error = "Access token is required" });

            var tokenId = Guid.NewGuid().ToString();

            _cache.Set(tokenId + "_access", input.AccessToken, TimeSpan.FromMinutes(210));

            if (!string.IsNullOrEmpty(input.RefreshToken))
                _cache.Set(tokenId + "_refresh", input.RefreshToken);

            return Ok(new { tokenId });
        }


        /// ///////////////////
        /// 
        private async Task<string> GetBuyApiToken()
        {
            if (_cache.TryGetValue("buy_api_token", out string cachedToken) && !string.IsNullOrEmpty(cachedToken))
                return cachedToken;

            using var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/identity/v1/oauth2/token");

            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "grant_type", "client_credentials" },
        { "scope", "https://api.ebay.com/oauth/api_scope/buy.item.feed" }
    });

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to get Buy API token: {content}");

            using var json = JsonDocument.Parse(content);
            var token = json.RootElement.GetProperty("access_token").GetString();

            _cache.Set("buy_api_token", token, TimeSpan.FromHours(1));

            return token;
        }

        [HttpGet("search-ebay-product/{upc}")]
        public async Task<IActionResult> SearchEbayProduct(string upc)
        {
            string token;
            try
            {
                token = await GetBuyApiToken();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to get Buy API token", details = ex.Message });
            }

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.ebay.com/buy/browse/v1/item_summary/search?q={Uri.EscapeDataString(upc)}&limit=1"
            );

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY_US");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response;
            string content;
            try
            {
                response = await client.SendAsync(request);
                content = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error calling eBay API", details = ex.Message });
            }

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, new { message = "Failed to search product", details = content });

            try
            {
                var json = JsonDocument.Parse(content);
                if (!json.RootElement.TryGetProperty("itemSummaries", out var items) || !items.EnumerateArray().Any())
                    return Ok(new { message = "No products found" });

                var i = items.EnumerateArray().First();

                string categoryId = "N/A";
                if (i.TryGetProperty("categories", out var categories) && categories.ValueKind == JsonValueKind.Array)
                    categoryId = categories.EnumerateArray().First().GetProperty("categoryId").GetString();

                var product = new
                {
                    Title = i.GetProperty("title").GetString(),
                    Price = i.TryGetProperty("price", out var price) ? price.GetProperty("value").GetString() : "N/A",
                    Currency = i.TryGetProperty("price", out var price2) ? price2.GetProperty("currency").GetString() : "N/A",
                    Condition = i.TryGetProperty("condition", out var cond) ? cond.GetString() : "N/A",
                    ItemWebUrl = i.TryGetProperty("itemWebUrl", out var url) ? url.GetString() : "",
                    Image = i.TryGetProperty("image", out var img) ? img.GetProperty("imageUrl").GetString() : "",
                    CategoryId = categoryId
                };

                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to parse eBay response", details = ex.Message, raw = content });
            }
        }


        [HttpPost("SearchByImage/{tokenId}")]
        public async Task<IActionResult> SearchByImage(string tokenId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("لم يتم رفع أي صورة.");

            // الحصول على التوكن الصحيح
            string accessToken = GetValidAccessToken(tokenId);

            // تحويل الصورة إلى Base64
            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await imageFile.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }
            string base64Image = Convert.ToBase64String(imageBytes);

            var jsonBody = new
            {
                image = base64Image
            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY_US");
                httpClient.DefaultRequestHeaders.Add("X-EBAY-C-ENDUSERCTX", "affiliateCampaignId=<ePNCampaignId>,affiliateReferenceId=<referenceId>");
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(jsonBody), Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(
                    "https://api.ebay.com/buy/browse/v1/item_summary/search_by_image?limit=30&sort=-price",
                    content
                );
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, new { message = "فشل استدعاء eBay API", details = responseString });

                var data = JObject.Parse(responseString);

                var itemsList = data["itemSummaries"]
                                             .Select(item => new
                                             {
                                                 // ===== existing =====
                                                 itemId = item["itemId"]?.ToString(),
                                                 title = item["title"]?.ToString(),
                                                 imageUrl = item["image"]?["imageUrl"]?.ToString(),
                                                 thumbnailImages = item["thumbnailImages"]?.Select(img => img["imageUrl"]?.ToString()).ToList(),
                                                 additionalImages = item["additionalImages"]?.Select(img => img["imageUrl"]?.ToString()).ToList(),
                                                 price = item["price"]?["value"]?.ToString(),
                                                 currency = item["price"]?["currency"]?.ToString(),
                                                 itemWebUrl = item["itemWebUrl"]?.ToString(),
                                                 itemAffiliateWebUrl = item["itemAffiliateWebUrl"]?.ToString(),
                                                 condition = item["condition"]?.ToString(),
                                                 conditionId = item["conditionId"]?.ToString(),
                                                 buyingOptions = item["buyingOptions"]?.Select(b => b.ToString()).ToList(),
                                                 sellerUsername = item["seller"]?["username"]?.ToString(),
                                                 sellerFeedbackScore = item["seller"]?["feedbackScore"]?.ToObject<int?>(),
                                                 sellerFeedbackPercentage = item["seller"]?["feedbackPercentage"]?.ToObject<double?>(),
                                                 itemLocationCountry = item["itemLocation"]?["country"]?.ToString(),
                                                 itemLocationPostalCode = item["itemLocation"]?["postalCode"]?.ToString(),
                                                 topRatedBuyingExperience = item["topRatedBuyingExperience"]?.ToObject<bool>() ?? false,
                                                 priorityListing = item["priorityListing"]?.ToObject<bool>() ?? false,
                                                 legacyItemId = item["legacyItemId"]?.ToString(),
                                                 epid = item["epid"]?.ToString(),

                                                 // ===== NEW FIELDS YOU WANT =====

                                                 images = new List<string>
                                                 {
                                                    item["image"]?["imageUrl"]?.ToString()
                                                 }
                                                 .Concat(item["additionalImages"]?
                                                     .Select(img => img["imageUrl"]?.ToString()) ?? new List<string>())
                                                 .Where(x => !string.IsNullOrEmpty(x))
                                                 .ToList(),

                                                 upc = item["upc"]?.ToString(),
                                                 brand = item["brand"]?.ToString(),
                                                 model = item["model"]?.ToString(),
                                                 height = item["itemDimensions"]?["height"]?.ToString(),
                                                 width = item["itemDimensions"]?["width"]?.ToString(),
                                                 length = item["itemDimensions"]?["length"]?.ToString(),
                                                 dimention = item["itemDimensions"]?.ToString(),
                                                 weight = item["weight"]?.ToString(),
                                                 desc = item["shortDescription"]?.ToString(),
                                                 sku = item["sku"]?.ToString(),
                                                 internet = item["itemWebUrl"]?.ToString()
                                             })
                                             .ToList();

                return Ok(itemsList);
            }
        }

        [HttpPost("SearchByText/{tokenId}")]
        public async Task<IActionResult> SearchByText(string tokenId, [FromBody] SearchRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.SearchText))
                return BadRequest("لم يتم إدخال كلمة للبحث.");

            // الحصول على التوكن الصحيح
            string accessToken = GetValidAccessToken(tokenId);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                httpClient.DefaultRequestHeaders.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY_US");
                httpClient.DefaultRequestHeaders.Add("X-EBAY-C-ENDUSERCTX", "affiliateCampaignId=<ePNCampaignId>,affiliateReferenceId=<referenceId>");
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var url = $"https://api.ebay.com/buy/browse/v1/item_summary/search?q={Uri.EscapeDataString(model.SearchText)}&limit=30&sort=-price";

                var response = await httpClient.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, new { message = "فشل استدعاء eBay API", details = responseString });

                var data = JObject.Parse(responseString);

                var itemsList = data["itemSummaries"]
                                ?.Select(item => new
                                {
                                    itemId = item["itemId"]?.ToString(),
                                    title = item["title"]?.ToString(),
                                    imageUrl = item["image"]?["imageUrl"]?.ToString(),
                                    thumbnailImages = item["thumbnailImages"]?.Select(img => img["imageUrl"]?.ToString()).ToList(),
                                    additionalImages = item["additionalImages"]?.Select(img => img["imageUrl"]?.ToString()).ToList(),
                                    price = item["price"]?["value"]?.ToString(),
                                    currency = item["price"]?["currency"]?.ToString(),
                                    itemWebUrl = item["itemWebUrl"]?.ToString(),
                                    itemAffiliateWebUrl = item["itemAffiliateWebUrl"]?.ToString(),
                                    condition = item["condition"]?.ToString(),
                                    conditionId = item["conditionId"]?.ToString(),
                                    buyingOptions = item["buyingOptions"]?.Select(b => b.ToString()).ToList(),
                                    sellerUsername = item["seller"]?["username"]?.ToString(),
                                    sellerFeedbackScore = item["seller"]?["feedbackScore"]?.ToObject<int?>(),
                                    sellerFeedbackPercentage = item["seller"]?["feedbackPercentage"]?.ToObject<double?>(),
                                    itemLocationCountry = item["itemLocation"]?["country"]?.ToString(),
                                    itemLocationPostalCode = item["itemLocation"]?["postalCode"]?.ToString(),
                                    topRatedBuyingExperience = item["topRatedBuyingExperience"]?.ToObject<bool>() ?? false,
                                    priorityListing = item["priorityListing"]?.ToObject<bool>() ?? false,
                                    legacyItemId = item["legacyItemId"]?.ToString(),
                                    epid = item["epid"]?.ToString()
                                })
                                .Cast<dynamic>().ToList() ?? new List<dynamic>();

                return Ok(itemsList);
            }
        }


        private string GetTypeForProduct(ProductDto product)
        {
            if (product.Title.Contains("Angle Grinder", StringComparison.OrdinalIgnoreCase))
                return "Angle Grinder";
            if (product.Title.Contains("Hand Vacuum", StringComparison.OrdinalIgnoreCase))
                return "Hand Vacuum";
            return "Does not apply";
        }

        //// 3. نشر منتج كامل على eBay باستخدام tokenId
        //[HttpPost("publish-product/{tokenId}")]
        //public async Task<IActionResult> PublishProduct(string tokenId, [FromBody] ProductDto product)
        //{
        //    var inv0 = await _context.Inventories.Include(i => i.Item).Where(i => i.Item.Description == product.Title
        //                                                 && i.Item.ItemDetails == product.Description && i.Item.UPC == product.UPC)
        //                                                .FirstOrDefaultAsync();

        //    if (inv0 != null && (inv0.Item!.SKU == null || inv0.Item!.SKU == ""))
        //    {
        //        inv0.Item!.SKU = product.Sku;
        //        await _context.SaveChangesAsync();
        //    }


        //    string token;
        //    try
        //    {
        //        token = GetValidAccessToken(tokenId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Unauthorized(ex.Message);
        //    }

        //    var client = _httpClientFactory.CreateClient();

        //    var itemSpecifics = new Dictionary<string, string[]>();

        //    // Brand
        //    itemSpecifics.Add("Brand", new[] { string.IsNullOrWhiteSpace(product.Brand) ? "Does not apply" : product.Brand });

        //    // Type
        //    string typeValue = GetTypeForProduct(product);
        //    itemSpecifics.Add("Type", new[] { typeValue ?? "Does not apply" });

        //    // إضافة أي Item Specifics أخرى من metadata إذا متوفرة
        //    if (product.RequiredAspectsFromMetadata != null)
        //    {
        //        foreach (var aspect in product.RequiredAspectsFromMetadata)
        //        {
        //            if (!itemSpecifics.ContainsKey(aspect.Name))
        //                itemSpecifics.Add(aspect.Name, new[] { aspect.DefaultValue ?? "Does not apply" });
        //        }
        //    }


        //    // أ. إنشاء Inventory
        //    var inventoryPayload = new
        //    {
        //        product = new
        //        {
        //            title = product.Title,
        //            description = $"<p>{product.Description}</p>",
        //            aspects = itemSpecifics,

        //            upc = string.IsNullOrWhiteSpace(product.UPC) ? null : new[] { product.UPC },
        //            imageUrls = product.ImageUrls,

        //        },
        //        condition = product.Condition,
        //        packageWeightAndSize = new
        //        {
        //            dimensions = new { height = 10, length = 20, width = 15, unit = "INCH" },
        //            weight = new { value = 2, unit = "POUND" }
        //        },
        //        availability = new
        //        {
        //            shipToLocationAvailability = new { quantity = product.Quantity }
        //        }
        //    };

        //    var invRequest = new HttpRequestMessage(
        //        HttpMethod.Put,
        //        $"https://api.ebay.com/sell/inventory/v1/inventory_item/{product.Sku}")
        //    {
        //        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(inventoryPayload), Encoding.UTF8, "application/json")
        //    };
        //    invRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    invRequest.Content.Headers.ContentLanguage.Add("en-US");

        //    var invResponse = await client.SendAsync(invRequest);
        //    if (!invResponse.IsSuccessStatusCode)
        //        return StatusCode((int)invResponse.StatusCode, $"Inventory creation failed: {await invResponse.Content.ReadAsStringAsync()}");

        //    var offerPayload = new
        //    {
        //        sku = product.Sku,
        //        marketplaceId = "EBAY_US",
        //        format = "FIXED_PRICE",
        //        availableQuantity = product.Quantity,
        //        pricingSummary = new
        //        {
        //            price = new { value = product.Price.ToString("F2"), currency = product.Currency ?? "USD" }
        //        },
        //        listingPolicies = new
        //        {
        //            fulfillmentPolicyId = product.FulfillmentPolicyId,
        //            paymentPolicyId = product.PaymentPolicyId,
        //            returnPolicyId = product.ReturnPolicyId
        //        },
        //        categoryId = product.CategoryId,
        //        listingDescription = $"<h2>{product.Title}</h2><p>{product.Description}</p>",
        //        merchantLocationKey = "Store1"
        //    };



        //    var offerRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/sell/inventory/v1/offer")
        //    {
        //        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(offerPayload), Encoding.UTF8, "application/json")
        //    };
        //    offerRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    offerRequest.Content.Headers.ContentLanguage.Add("en-US");
        //    var offerResponse = await client.SendAsync(offerRequest);
        //    if (!offerResponse.IsSuccessStatusCode)
        //        return StatusCode((int)offerResponse.StatusCode, $"Offer creation failed: {await offerResponse.Content.ReadAsStringAsync()}");

        //    var jsonOffer = await offerResponse.Content.ReadAsStringAsync();
        //    var offerId = JsonDocument.Parse(jsonOffer).RootElement.GetProperty("offerId").GetString();


        //    var inv = await _context.Inventories.Include(i => i.Item).ThenInclude(ii => ii.Images)
        //       .Where(i => i.Item.UPC == product.UPC).FirstOrDefaultAsync();
        //    if (inv != null)
        //    {
        //        inv.EbayOfferID = offerId;
        //        inv.Status = "Offer Maked Only";
        //    }

        //    // ج. نشر Offer
        //    var publishRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.ebay.com/sell/inventory/v1/offer/{offerId}/publish");
        //    publishRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var publishResponse = await client.SendAsync(publishRequest);
        //    if (!publishResponse.IsSuccessStatusCode)
        //        return StatusCode((int)publishResponse.StatusCode, $"Publish failed: {await publishResponse.Content.ReadAsStringAsync()}");

        //    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //    // بعد النشر الناجح، استرجع معلومات الـ offer للحصول على listingId
        //    var getOfferRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.ebay.com/sell/inventory/v1/offer/{offerId}");
        //    getOfferRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    /////////////////////////////////////
        //    ///

        //    //await Task.Delay(10000);
        //    //var getOfferResponse = await client.SendAsync(getOfferRequest);
        //    //if (!getOfferResponse.IsSuccessStatusCode)
        //    //{
        //    //    return StatusCode((int)getOfferResponse.StatusCode, $"Failed to retrieve listing info: {await getOfferResponse.Content.ReadAsStringAsync()}");
        //    //}

        //    //var offerJson = await getOfferResponse.Content.ReadAsStringAsync();
        //    //var offerData = JsonDocument.Parse(offerJson).RootElement;

        //    //string listingId = offerData.TryGetProperty("listingId", out var idProp) ? idProp.GetString() : null;
        //    //string ebayUrl = listingId != null ? $"https://www.ebay.com/itm/{listingId}" : null;


        //    // here update ain alfhd inv tbl

        //    ///////////////////////

        //    await Task.Delay(5000);
        //    string? listingId = null;
        //    var getOfferResponse = await client.SendAsync(getOfferRequest);

        //    if (getOfferResponse.IsSuccessStatusCode)
        //    {
        //        var offerJson = await getOfferResponse.Content.ReadAsStringAsync();
        //        var offerData = JsonDocument.Parse(offerJson).RootElement;

        //        if (offerData.TryGetProperty("listingId", out var prop))
        //        {
        //            listingId = prop.GetString();

        //        }
        //    }




        //        inv.EbayInvID = product.Sku;
        //        inv.EbayOfferID = offerId;
        //        inv.Status = "Auto Published";
        //        inv.EbayListingId = listingId;

        //        foreach (var imageRec in inv.Item.Images)
        //        {
        //            imageRec.IsPublishedOnEbay = false;
        //        }

        //        foreach (var imageRec in inv.Item.Images)
        //        {
        //            if (product.ImageUrls.Contains(imageRec.ImageUrl))
        //            {
        //                imageRec.IsPublishedOnEbay = true;
        //            }
        //        }



        //    await _context.SaveChangesAsync();

        //    return Ok(new
        //    {
        //        message = "Product published successfully",
        //        offerId,
        //        listingId,
        //    });
        //}


        //// 3. نشر منتج كامل على eBay باستخدام tokenId
        //[HttpPost("publish-product/{tokenId}")]
        //public async Task<IActionResult> PublishProduct(string tokenId, [FromBody] ProductDto product)
        //{
        //    var inv0 = await _context.Inventories.Include(i => i.Item).Where(i => i.Item.Description == product.Title
        //                                                 && i.Item.ItemDetails == product.Description && i.Item.UPC == product.UPC)
        //                                                .FirstOrDefaultAsync();

        //    if (inv0 != null && (inv0.Item!.SKU == null || inv0.Item!.SKU == ""))
        //    {
        //        inv0.Item!.SKU = product.Sku;
        //        await _context.SaveChangesAsync();
        //    }


        //    string token;
        //    try
        //    {
        //        token = GetValidAccessToken(tokenId);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Unauthorized(ex.Message);
        //    }

        //    var client = _httpClientFactory.CreateClient();

        //    var itemSpecifics = new Dictionary<string, string[]>();

        //    // Brand
        //    itemSpecifics.Add("Brand", new[] { string.IsNullOrWhiteSpace(product.Brand) ? "Does not apply" : product.Brand });

        //    // Type
        //    string typeValue = GetTypeForProduct(product);
        //    itemSpecifics.Add("Type", new[] { typeValue ?? "Does not apply" });

        //    // إضافة أي Item Specifics أخرى من metadata إذا متوفرة
        //    if (product.RequiredAspectsFromMetadata != null)
        //    {
        //        foreach (var aspect in product.RequiredAspectsFromMetadata)
        //        {
        //            if (!itemSpecifics.ContainsKey(aspect.Name))
        //                itemSpecifics.Add(aspect.Name, new[] { aspect.DefaultValue ?? "Does not apply" });
        //        }
        //    }


        //    // أ. إنشاء Inventory
        //    var inventoryPayload = new
        //    {
        //        product = new
        //        {
        //            title = product.Title,
        //            description = $"<p>{product.Description}</p>",
        //            aspects = itemSpecifics,

        //            upc = string.IsNullOrWhiteSpace(product.UPC) ? null : new[] { product.UPC },
        //            imageUrls = product.ImageUrls,

        //        },
        //        condition = product.Condition,
        //        packageWeightAndSize = new
        //        {
        //            dimensions = new { height = 10, length = 20, width = 15, unit = "INCH" },
        //            weight = new { value = 2, unit = "POUND" }
        //        },
        //        availability = new
        //        {
        //            shipToLocationAvailability = new { quantity = product.Quantity }
        //        }
        //    };

        //    var invRequest = new HttpRequestMessage(
        //        HttpMethod.Put,
        //        $"https://api.ebay.com/sell/inventory/v1/inventory_item/{product.Sku}")
        //    {
        //        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(inventoryPayload), Encoding.UTF8, "application/json")
        //    };
        //    invRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    invRequest.Content.Headers.ContentLanguage.Add("en-US");

        //    var invResponse = await client.SendAsync(invRequest);
        //    if (!invResponse.IsSuccessStatusCode)
        //        return StatusCode((int)invResponse.StatusCode, $"Inventory creation failed: {await invResponse.Content.ReadAsStringAsync()}");

        //    var offerPayload = new
        //    {
        //        sku = product.Sku,
        //        marketplaceId = "EBAY_US",
        //        format = "FIXED_PRICE",
        //        availableQuantity = product.Quantity,
        //        pricingSummary = new
        //        {
        //            price = new { value = product.Price.ToString("F2"), currency = product.Currency ?? "USD" }
        //        },
        //        listingPolicies = new
        //        {
        //            fulfillmentPolicyId = product.FulfillmentPolicyId,
        //            paymentPolicyId = product.PaymentPolicyId,
        //            returnPolicyId = product.ReturnPolicyId
        //        },
        //        categoryId = product.CategoryId,
        //        listingDescription = $"<h2>{product.Title}</h2><p>{product.Description}</p>",
        //        merchantLocationKey = "Store1"
        //    };


        //    string offerId = product.EbayOfferID != null ? product.EbayOfferID : string.Empty;
        //    var offerRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/sell/inventory/v1/offer")
        //    {
        //        Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(offerPayload), Encoding.UTF8, "application/json")
        //    };
        //    offerRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    offerRequest.Content.Headers.ContentLanguage.Add("en-US");

        //    var inv = await _context.Inventories.Include(i => i.Item).ThenInclude(ii => ii.Images)
        //        .Where(i => i.Item.UPC == product.UPC).FirstOrDefaultAsync();

        //    if (string.IsNullOrEmpty(product.EbayOfferID))
        //    {
        //        var offerResponse = await client.SendAsync(offerRequest);
        //        if (!offerResponse.IsSuccessStatusCode)
        //            return StatusCode((int)offerResponse.StatusCode, $"Offer creation failed: {await offerResponse.Content.ReadAsStringAsync()}");

        //        var jsonOffer = await offerResponse.Content.ReadAsStringAsync();
        //        offerId = JsonDocument.Parse(jsonOffer).RootElement.GetProperty("offerId").GetString();

        //        if (inv != null)
        //        {
        //            inv.EbayOfferID = offerId;
        //            inv.Status = "Offer Made Only";
        //            await _context.SaveChangesAsync();
        //        }
        //    }
        //    else
        //    {
        //        var deleteResult = await DeleteEbayOffer(tokenId, inv.Inventory_id);

        //        if (deleteResult is ObjectResult objectResult &&
        //            objectResult.StatusCode != 200)
        //        {
        //            return BadRequest("Old eBay offer deletion failed");
        //        }

        //        var offerResponse = await client.SendAsync(offerRequest);
        //        if (!offerResponse.IsSuccessStatusCode)
        //            return StatusCode((int)offerResponse.StatusCode, $"Offer creation failed: {await offerResponse.Content.ReadAsStringAsync()}");

        //        var jsonOffer = await offerResponse.Content.ReadAsStringAsync();
        //        offerId = JsonDocument.Parse(jsonOffer).RootElement.GetProperty("offerId").GetString();

        //        if (inv != null)
        //        {
        //            inv.EbayOfferID = offerId;
        //            inv.Status = "Offer Made Only";
        //            await _context.SaveChangesAsync();
        //        }

        //    }


        //    // ج. نشر Offer
        //    var publishRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.ebay.com/sell/inventory/v1/offer/{offerId}/publish");
        //    publishRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var publishResponse = await client.SendAsync(publishRequest);
        //    if (!publishResponse.IsSuccessStatusCode)
        //    {
        //        inv.Notes = $"Publish failed: {await publishResponse.Content.ReadAsStringAsync()}";
        //        return StatusCode((int)publishResponse.StatusCode, $"Publish failed: {await publishResponse.Content.ReadAsStringAsync()}");
        //    }
                

        //    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //    // بعد النشر الناجح، استرجع معلومات الـ offer للحصول على listingId
        //    var getOfferRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.ebay.com/sell/inventory/v1/offer/{offerId}");
        //    getOfferRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    /////////////////////////////////////
        //    ///

        //    //await Task.Delay(10000);
        //    //var getOfferResponse = await client.SendAsync(getOfferRequest);
        //    //if (!getOfferResponse.IsSuccessStatusCode)
        //    //{
        //    //    return StatusCode((int)getOfferResponse.StatusCode, $"Failed to retrieve listing info: {await getOfferResponse.Content.ReadAsStringAsync()}");
        //    //}

        //    //var offerJson = await getOfferResponse.Content.ReadAsStringAsync();
        //    //var offerData = JsonDocument.Parse(offerJson).RootElement;

        //    //string listingId = offerData.TryGetProperty("listingId", out var idProp) ? idProp.GetString() : null;
        //    //string ebayUrl = listingId != null ? $"https://www.ebay.com/itm/{listingId}" : null;


        //    // here update ain alfhd inv tbl

        //    ///////////////////////

        //    await Task.Delay(5000);
        //    string? listingId = null;
        //    var getOfferResponse = await client.SendAsync(getOfferRequest);

        //    if (getOfferResponse.IsSuccessStatusCode)
        //    {
        //        var offerJson = await getOfferResponse.Content.ReadAsStringAsync();
        //        var offerData = JsonDocument.Parse(offerJson).RootElement;

        //        if (offerData.TryGetProperty("listingId", out var prop))
        //        {
        //            listingId = prop.GetString();

        //        }
        //    }

        //    if (inv != null)
        //    {
        //        inv.EbayInvID = product.Sku;
        //        inv.EbayOfferID = offerId;
        //        inv.Status = "Auto Published";
        //        inv.EbayListingId = listingId;
        //        inv.Notes = string.Empty;
        //        inv.QtyPublished -= product.Quantity;
        //        inv.QtyInInventory = product.Quantity;

        //        foreach (var imageRec in inv.Item.Images)
        //        {
        //            imageRec.IsPublishedOnEbay = false;
        //        }

        //        foreach (var imageRec in inv.Item.Images)
        //        {
        //            if (product.ImageUrls.Contains(imageRec.ImageUrl))
        //            {
        //                imageRec.IsPublishedOnEbay = true;
        //            }
        //        }

        //        await _context.SaveChangesAsync();
        //    }

        //    return Ok(new
        //    {
        //        message = "Product published successfully",
        //        offerId,
        //        listingId,
        //    });
        //}


        // 3. نشر منتج كامل على eBay باستخدام tokenId
        [HttpPost("publish-product/{tokenId}")]
        public async Task<IActionResult> PublishProduct(string tokenId, [FromBody] ProductDto product)
        {
            var inv0 = await _context.Inventories.Include(i => i.Item).Where(i => i.Item.Description == product.Title
                                                         && i.Item.ItemDetails == product.Description && i.Item.UPC == product.UPC)
                                                        .FirstOrDefaultAsync();

            if (inv0 != null && (inv0.Item!.SKU == null || inv0.Item!.SKU == ""))
            {
                inv0.Item!.SKU = product.Sku;
                await _context.SaveChangesAsync();
            }


            string token;
            try
            {
                token = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            var client = _httpClientFactory.CreateClient();

            var itemSpecifics = new Dictionary<string, string[]>();

            // Brand
            itemSpecifics.Add("Brand", new[] { string.IsNullOrWhiteSpace(product.Brand) ? "Does not apply" : product.Brand });

            // Type
            string typeValue = GetTypeForProduct(product);
            itemSpecifics.Add("Type", new[] { typeValue ?? "Does not apply" });

            // إضافة أي Item Specifics أخرى من metadata إذا متوفرة
            if (product.RequiredAspectsFromMetadata != null)
            {
                foreach (var aspect in product.RequiredAspectsFromMetadata)
                {
                    if (!itemSpecifics.ContainsKey(aspect.Name))
                        itemSpecifics.Add(aspect.Name, new[] { aspect.DefaultValue ?? "Does not apply" });
                }
            }


            // أ. إنشاء Inventory
            var inventoryPayload = new
            {
                product = new
                {
                    title = product.Title,
                    description = $"<p>{product.Description}</p>",
                    aspects = itemSpecifics,

                    upc = string.IsNullOrWhiteSpace(product.UPC) ? null : new[] { product.UPC },
                    imageUrls = product.ImageUrls,

                },
                condition = product.Condition,
                packageWeightAndSize = new
                {
                    dimensions = new { height = 10, length = 20, width = 15, unit = "INCH" },
                    weight = new { value = 2, unit = "POUND" }
                },
                availability = new
                {
                    shipToLocationAvailability = new { quantity = product.Quantity }
                }
            };

            var invRequest = new HttpRequestMessage(
                HttpMethod.Put,
                $"https://api.ebay.com/sell/inventory/v1/inventory_item/{product.Sku}")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(inventoryPayload), Encoding.UTF8, "application/json")
            };
            invRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            invRequest.Content.Headers.ContentLanguage.Add("en-US");

            var invResponse = await client.SendAsync(invRequest);
            if (!invResponse.IsSuccessStatusCode)
                return StatusCode((int)invResponse.StatusCode, $"Inventory creation failed: {await invResponse.Content.ReadAsStringAsync()}");

            var offerPayload = new
            {
                sku = product.Sku,
                marketplaceId = "EBAY_US",
                format = "FIXED_PRICE",
                availableQuantity = product.Quantity,
                pricingSummary = new
                {
                    price = new { value = product.Price.ToString("F2"), currency = product.Currency ?? "USD" }
                },
                listingPolicies = new
                {
                    fulfillmentPolicyId = product.FulfillmentPolicyId,
                    paymentPolicyId = product.PaymentPolicyId,
                    returnPolicyId = product.ReturnPolicyId
                },
                categoryId = product.CategoryId,
                listingDescription = $"<h2>{product.Title}</h2><p>{product.Description}</p>",
                merchantLocationKey = "Store1"
            };


            string offerId = product.EbayOfferID != null ? product.EbayOfferID : string.Empty;
            var offerRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/sell/inventory/v1/offer")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(offerPayload), Encoding.UTF8, "application/json")
            };
            offerRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            offerRequest.Content.Headers.ContentLanguage.Add("en-US");

            var inv = await _context.Inventories.Include(i => i.Item).ThenInclude(ii => ii.Images)
                .Where(i => i.Item.UPC == product.UPC).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(product.EbayOfferID))
            {
                var offerResponse = await client.SendAsync(offerRequest);
                if (!offerResponse.IsSuccessStatusCode)
                    return StatusCode((int)offerResponse.StatusCode, $"Offer creation failed: {await offerResponse.Content.ReadAsStringAsync()}");

                var jsonOffer = await offerResponse.Content.ReadAsStringAsync();
                offerId = JsonDocument.Parse(jsonOffer).RootElement.GetProperty("offerId").GetString();

                if (inv != null)
                {
                    inv.EbayOfferID = offerId;
                    inv.Status = "Offer Made Only";
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                var deleteResult = await DeleteEbayOffer(tokenId, inv.Inventory_id);

                if (deleteResult is ObjectResult objectResult &&
                    objectResult.StatusCode != 200)
                {
                    return BadRequest("Old eBay offer deletion failed");
                }

                var offerResponse = await client.SendAsync(offerRequest);
                if (!offerResponse.IsSuccessStatusCode)
                    return StatusCode((int)offerResponse.StatusCode, $"Offer creation failed: {await offerResponse.Content.ReadAsStringAsync()}");

                var jsonOffer = await offerResponse.Content.ReadAsStringAsync();
                offerId = JsonDocument.Parse(jsonOffer).RootElement.GetProperty("offerId").GetString();

                if (inv != null)
                {
                    inv.EbayOfferID = offerId;
                    inv.Status = "Offer Made Only";
                    await _context.SaveChangesAsync();
                }

            }


            // ج. نشر Offer
            var publishRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.ebay.com/sell/inventory/v1/offer/{offerId}/publish");
            publishRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var publishResponse = await client.SendAsync(publishRequest);
            if (!publishResponse.IsSuccessStatusCode)
            {
                inv.Notes = $"Publish failed: {await publishResponse.Content.ReadAsStringAsync()}";
                return StatusCode((int)publishResponse.StatusCode, $"Publish failed: {await publishResponse.Content.ReadAsStringAsync()}");
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // بعد النشر الناجح، استرجع معلومات الـ offer للحصول على listingId
            var getOfferRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.ebay.com/sell/inventory/v1/offer/{offerId}");
            getOfferRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            /////////////////////////////////////
            ///

            //await Task.Delay(10000);
            //var getOfferResponse = await client.SendAsync(getOfferRequest);
            //if (!getOfferResponse.IsSuccessStatusCode)
            //{
            //    return StatusCode((int)getOfferResponse.StatusCode, $"Failed to retrieve listing info: {await getOfferResponse.Content.ReadAsStringAsync()}");
            //}

            //var offerJson = await getOfferResponse.Content.ReadAsStringAsync();
            //var offerData = JsonDocument.Parse(offerJson).RootElement;

            //string listingId = offerData.TryGetProperty("listingId", out var idProp) ? idProp.GetString() : null;
            //string ebayUrl = listingId != null ? $"https://www.ebay.com/itm/{listingId}" : null;


            // here update ain alfhd inv tbl

            ///////////////////////

            await Task.Delay(5000);
            string? listingId = null;
            var getOfferResponse = await client.SendAsync(getOfferRequest);

            if (getOfferResponse.IsSuccessStatusCode)
            {
                var offerJson = await getOfferResponse.Content.ReadAsStringAsync();
                var offerData = JsonDocument.Parse(offerJson).RootElement;

                if (offerData.TryGetProperty("listingId", out var prop))
                {
                    listingId = prop.GetString();

                }
            }

            if (inv != null)
            {
                inv.EbayInvID = product.Sku;
                inv.EbayOfferID = offerId;
                inv.Status = "Auto Published";
                inv.EbayListingId = listingId;
                inv.Notes = string.Empty;
                inv.QtyPublished -= product.Quantity;
                inv.QtyInInventory = product.Quantity;

                foreach (var imageRec in inv.Item.Images)
                {
                    imageRec.IsPublishedOnEbay = false;
                }

                foreach (var imageRec in inv.Item.Images)
                {
                    if (product.ImageUrls.Contains(imageRec.ImageUrl))
                    {
                        imageRec.IsPublishedOnEbay = true;
                    }
                }

                await _context.SaveChangesAsync();
            }


            var currentPlatform = await _context.Platforms.Where(p => p.Description.Trim() == "ebay").FirstOrDefaultAsync();

            var newPublished = new Inventory_Published_Items
            {
                Inventory_id = inv.Inventory_id,
                Platform_id = currentPlatform.Platform_id,
                PublishedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            };

            await _context.Inventory_Published_Items.AddAsync(newPublished);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product published successfully",
                offerId,
                listingId,
            });
        }

        ///////////////////////////
        ///
        [HttpPost("PublishDropShippingProduct/{tokenId}")]
        public async Task<IActionResult> PublishDropShippingProduct(string tokenId, [FromBody] ProductDto product)
        {
            var item = await _context.Items.Where(i => i.ItemDescription == product.Title
                                                         && i.ItemDetails == product.Description && i.UPC == product.UPC)
                                                        .FirstOrDefaultAsync();

            if (item != null && (item!.SKU == null || item.SKU == ""))
            {
                item.SKU = product.Sku;
                await _context.SaveChangesAsync();
            }


            string token;
            try
            {
                token = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            var client = _httpClientFactory.CreateClient();

            var itemSpecifics = new Dictionary<string, string[]>();

            // Brand
            itemSpecifics.Add("Brand", new[] { string.IsNullOrWhiteSpace(product.Brand) ? "Does not apply" : product.Brand });

            // Type
            string typeValue = GetTypeForProduct(product);
            itemSpecifics.Add("Type", new[] { typeValue ?? "Does not apply" });

            // إضافة أي Item Specifics أخرى من metadata إذا متوفرة
            if (product.RequiredAspectsFromMetadata != null)
            {
                foreach (var aspect in product.RequiredAspectsFromMetadata)
                {
                    if (!itemSpecifics.ContainsKey(aspect.Name))
                        itemSpecifics.Add(aspect.Name, new[] { aspect.DefaultValue ?? "Does not apply" });
                }
            }


            // أ. إنشاء Inventory
            var inventoryPayload = new
            {
                product = new
                {
                    title = product.Title,
                    description = $"<p>{product.Description}</p>",
                    aspects = itemSpecifics,

                    upc = string.IsNullOrWhiteSpace(product.UPC) ? null : new[] { product.UPC },
                    imageUrls = product.ImageUrls,

                },
                condition = product.Condition,
                packageWeightAndSize = new
                {
                    dimensions = new { height = 10, length = 20, width = 15, unit = "INCH" },
                    weight = new { value = 2, unit = "POUND" }
                },
                availability = new
                {
                    shipToLocationAvailability = new { quantity = product.Quantity }
                }
            };

            var invRequest = new HttpRequestMessage(
                HttpMethod.Put,
                $"https://api.ebay.com/sell/inventory/v1/inventory_item/{product.Sku}")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(inventoryPayload), Encoding.UTF8, "application/json")
            };
            invRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            invRequest.Content.Headers.ContentLanguage.Add("en-US");

            var invResponse = await client.SendAsync(invRequest);
            if (!invResponse.IsSuccessStatusCode)
                return StatusCode((int)invResponse.StatusCode, $"Inventory creation failed: {await invResponse.Content.ReadAsStringAsync()}");

            var offerPayload = new
            {
                sku = product.Sku,
                marketplaceId = "EBAY_US",
                format = "FIXED_PRICE",
                availableQuantity = product.Quantity,
                pricingSummary = new
                {
                    price = new { value = product.Price.ToString("F2"), currency = product.Currency ?? "USD" }
                },
                listingPolicies = new
                {
                    fulfillmentPolicyId = product.FulfillmentPolicyId,
                    paymentPolicyId = product.PaymentPolicyId,
                    returnPolicyId = product.ReturnPolicyId
                },
                categoryId = product.CategoryId,
                listingDescription = $"<h2>{product.Title}</h2><p>{product.Description}</p>",
                merchantLocationKey = "Store1"
            };


            string offerId = product.EbayOfferID != null ? product.EbayOfferID : string.Empty;
            var offerRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.ebay.com/sell/inventory/v1/offer")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(offerPayload), Encoding.UTF8, "application/json")
            };
            offerRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            offerRequest.Content.Headers.ContentLanguage.Add("en-US");

            //var inv = await _context.Inventories.Include(i => i.Item).ThenInclude(ii => ii.Images)
            //    .Where(i => i.Item.UPC == product.UPC).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(product.EbayOfferID))
            {
                var offerResponse = await client.SendAsync(offerRequest);
                if (!offerResponse.IsSuccessStatusCode)
                    return StatusCode((int)offerResponse.StatusCode, $"Offer creation failed: {await offerResponse.Content.ReadAsStringAsync()}");

                var jsonOffer = await offerResponse.Content.ReadAsStringAsync();
                offerId = JsonDocument.Parse(jsonOffer).RootElement.GetProperty("offerId").GetString();

                //if (inv != null)
                //{
                //    inv.EbayOfferID = offerId;
                //    inv.Status = "Offer Made Only";
                //    await _context.SaveChangesAsync();
                //}
            }
            else
            {
                //var deleteResult = await DeleteEbayOffer(tokenId, inv.Inventory_id);

                //if (deleteResult is ObjectResult objectResult &&
                //    objectResult.StatusCode != 200)
                //{
                //    return BadRequest("Old eBay offer deletion failed");
                //}

                var offerResponse = await client.SendAsync(offerRequest);
                if (!offerResponse.IsSuccessStatusCode)
                    return StatusCode((int)offerResponse.StatusCode, $"Offer creation failed: {await offerResponse.Content.ReadAsStringAsync()}");

                var jsonOffer = await offerResponse.Content.ReadAsStringAsync();
                offerId = JsonDocument.Parse(jsonOffer).RootElement.GetProperty("offerId").GetString();

                //if (inv != null)
                //{
                //    inv.EbayOfferID = offerId;
                //    inv.Status = "Offer Made Only";
                //    await _context.SaveChangesAsync();
                //}

            }


            // ج. نشر Offer
            var publishRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.ebay.com/sell/inventory/v1/offer/{offerId}/publish");
            publishRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var publishResponse = await client.SendAsync(publishRequest);
            if (!publishResponse.IsSuccessStatusCode)
            {
                // inv.Notes = $"Publish failed: {await publishResponse.Content.ReadAsStringAsync()}";
                return StatusCode((int)publishResponse.StatusCode, $"Publish failed: {await publishResponse.Content.ReadAsStringAsync()}");
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // بعد النشر الناجح، استرجع معلومات الـ offer للحصول على listingId
            var getOfferRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.ebay.com/sell/inventory/v1/offer/{offerId}");
            getOfferRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            /////////////////////////////////////

            await Task.Delay(2000);
            string? listingId = null;
            var getOfferResponse = await client.SendAsync(getOfferRequest);

            if (getOfferResponse.IsSuccessStatusCode)
            {
                var offerJson = await getOfferResponse.Content.ReadAsStringAsync();
                var offerData = JsonDocument.Parse(offerJson).RootElement;

                if (offerData.TryGetProperty("listingId", out var prop))
                {
                    listingId = prop.GetString();

                }
            }

            //if (inv != null)
            //{
            //    inv.EbayInvID = product.Sku;
            //    inv.EbayOfferID = offerId;
            //    inv.Status = "Auto Published";
            //    inv.EbayListingId = listingId;
            //    inv.Notes = string.Empty;
            //    inv.QtyPublished -= product.Quantity;
            //    inv.QtyInInventory = product.Quantity;

            //    foreach (var imageRec in inv.Item.Images)
            //    {
            //        imageRec.IsPublishedOnEbay = false;
            //    }

            //    foreach (var imageRec in inv.Item.Images)
            //    {
            //        if (product.ImageUrls.Contains(imageRec.ImageUrl))
            //        {
            //            imageRec.IsPublishedOnEbay = true;
            //        }
            //    }

            //    await _context.SaveChangesAsync();
            //}

            return Ok(new
            {
                message = "Product published successfully",
                offerId,
                listingId,
            });
        }

        // //////////////////////////////



        /// 
        [HttpDelete("delete-ebay-offer/{tokenId}/{invId}")]
        public async Task<IActionResult> DeleteEbayOffer(string tokenId, int invId)
        {
            string token;

            try
            {
                token = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = "Invalid token",
                    details = ex.Message
                });
            }

            var inventory = await _context.Inventories.FindAsync(invId);

            var client = _httpClientFactory.CreateClient();

            var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"https://api.ebay.com/sell/inventory/v1/offer/{inventory.EbayOfferID}"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                inventory.Status = "";
                inventory.EbayOfferID = string.Empty;

                await _context.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    message = "Offer deleted successfully"
                });
            }

            var content = await response.Content.ReadAsStringAsync();

            return StatusCode((int)response.StatusCode, new
            {
                success = false,
                message = "Failed to delete offer",
                details = content
            });
        }






        /// 
        [HttpPut("update-product/{tokenId}")]
        public async Task<IActionResult> UpdateProduct(string tokenId, [FromBody] ProductDto product)
        {
            string token;
            try
            {
                token = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            var client = _httpClientFactory.CreateClient();

            var itemSpecifics = new Dictionary<string, string[]>();

            // Brand
            itemSpecifics.Add("Brand", new[] { string.IsNullOrWhiteSpace(product.Brand) ? "Does not apply" : product.Brand });

            // Type
            string typeValue = GetTypeForProduct(product);
            itemSpecifics.Add("Type", new[] { typeValue ?? "Does not apply" });

            if (product.RequiredAspectsFromMetadata != null)
            {
                foreach (var aspect in product.RequiredAspectsFromMetadata)
                {
                    if (!itemSpecifics.ContainsKey(aspect.Name))
                        itemSpecifics.Add(aspect.Name, new[] { aspect.DefaultValue ?? "Does not apply" });
                }
            }

            var inventoryPayload = new
            {
                product = new
                {
                    title = product.Title,
                    description = $"<p>{product.Description}</p>",
                    aspects = itemSpecifics,
                    upc = string.IsNullOrWhiteSpace(product.UPC) ? null : new[] { product.UPC },
                    imageUrls = product.ImageUrls,
                },
                condition = product.Condition,
                availability = new
                {
                    shipToLocationAvailability = new { quantity = product.Quantity }
                }
            };

            var invRequest = new HttpRequestMessage(
                HttpMethod.Put,
                $"https://api.ebay.com/sell/inventory/v1/inventory_item/{product.Sku}")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(inventoryPayload), Encoding.UTF8, "application/json")
            };
            invRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            invRequest.Content.Headers.ContentLanguage.Add("en-US");

            var invResponse = await client.SendAsync(invRequest);
            if (!invResponse.IsSuccessStatusCode)
                return StatusCode((int)invResponse.StatusCode, $"Inventory update failed: {await invResponse.Content.ReadAsStringAsync()}");

            var offerPayload = new
            {
                availableQuantity = product.Quantity,
                pricingSummary = new
                {
                    price = new { value = product.Price.ToString("F2"), currency = product.Currency ?? "USD" }
                },
                listingDescription = $"<h2>{product.Title}</h2><p>{product.Description}</p>"
            };

            var offerRequest = new HttpRequestMessage(
                HttpMethod.Put,
                $"https://api.ebay.com/sell/inventory/v1/offer/{product.EbayOfferID}")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(offerPayload), Encoding.UTF8, "application/json")
            };
            offerRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            offerRequest.Content.Headers.ContentLanguage.Add("en-US");

            var offerResponse = await client.SendAsync(offerRequest);
            if (!offerResponse.IsSuccessStatusCode)
                return StatusCode((int)offerResponse.StatusCode, $"Offer update failed: {await offerResponse.Content.ReadAsStringAsync()}");

            // 3. إعادة نشر الـ Offer
            var publishRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.ebay.com/sell/inventory/v1/offer/{product.EbayOfferID}/publish");
            publishRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var publishResponse = await client.SendAsync(publishRequest);
            if (!publishResponse.IsSuccessStatusCode)
                return StatusCode((int)publishResponse.StatusCode, $"Republish failed: {await publishResponse.Content.ReadAsStringAsync()}");

            return Ok(new { message = "Product updated and republished successfully" });
        }



        [HttpGet("my-visible-ebay-products/{tokenId}")]
        public async Task<IActionResult> GetVisibleEbayProducts(string tokenId)
        {
            string token;
            try
            {
                token = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://api.ebay.com/sell/inventory/v1/inventory_item"
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, new { message = "Failed to fetch products", details = content });
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(responseContent);

            var products = json.RootElement.GetProperty("inventoryItems").EnumerateArray();
            var detailedProducts = new List<object>();

            foreach (var item in products)
            {
                string sku = item.GetProperty("sku").GetString();

                var detailRequest = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"https://api.ebay.com/sell/inventory/v1/inventory_item/{sku}"
                );
                detailRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var detailResponse = await client.SendAsync(detailRequest);
                if (detailResponse.IsSuccessStatusCode)
                {
                    var detailContent = await detailResponse.Content.ReadAsStringAsync();
                    var detailJson = JsonDocument.Parse(detailContent).RootElement;

                    // جلب الحقول مع fallback للقيم الافتراضية
                    string condition = detailJson.TryGetProperty("condition", out var cond) ? cond.GetString() : "UNKNOWN";

                    string title = "No Title";
                    string description = "No Description";
                    List<string> images = new List<string>();

                    if (detailJson.TryGetProperty("product", out var prod))
                    {
                        if (prod.TryGetProperty("title", out var t)) title = t.GetString() ?? title;
                        if (prod.TryGetProperty("description", out var d)) description = d.GetString() ?? description;
                        if (prod.TryGetProperty("imageUrls", out var imgs))
                            images = imgs.EnumerateArray().Select(i => i.GetString() ?? "").Where(i => !string.IsNullOrEmpty(i)).ToList();
                    }

                    object packageWeightAndSize = detailJson.TryGetProperty("packageWeightAndSize", out var pkg) ? pkg.Clone() : null;
                    object availability = detailJson.TryGetProperty("availability", out var avail) ? avail.Clone() : null;

                    detailedProducts.Add(new
                    {
                        sku,
                        condition,
                        title,
                        description,
                        images,
                        packageWeightAndSize,
                        availability
                    });
                }
            }

            return Ok(new { products = detailedProducts });
        }



        [HttpGet("my-sold-ebay-products/{tokenId}")]
        public async Task<IActionResult> GetSoldEbayProducts(string tokenId)
        {
            string token;
            try
            {
                token = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Invalid token", details = ex.Message });
            }

            var client = _httpClientFactory.CreateClient();

            // رابط الـ Fulfillment API
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.ebay.com/sell/fulfillment/v1/order");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    message = "Failed to fetch sold products",
                    details = content
                });
            }

            // تحليل الرد JSON
            var json = JsonDocument.Parse(content);
            var orders = json.RootElement.TryGetProperty("orders", out var ordersArray)
                ? ordersArray.EnumerateArray().Select(o => new
                {
                    OrderId = o.GetProperty("orderId").GetString(),
                    Status = o.GetProperty("orderFulfillmentStatus").GetString(),
                    Total = o.GetProperty("pricingSummary").GetProperty("total").GetProperty("value").GetString(),
                    Currency = o.GetProperty("pricingSummary").GetProperty("total").GetProperty("currency").GetString(),
                    Buyer = o.GetProperty("buyer").GetProperty("username").GetString(),
                    LineItems = o.GetProperty("lineItems").EnumerateArray().Select(li => new
                    {
                        Title = li.GetProperty("title").GetString(),
                        Sku = li.GetProperty("sku").GetString(),
                        Quantity = li.GetProperty("quantity").GetInt32(),
                        Price = li.GetProperty("lineItemCost").GetProperty("value").GetString()
                    })
                })
                : Enumerable.Empty<object>();

            return Ok(new { orders });
        }


        [HttpGet("search-ebay-product/{tokenId}/{upc}")]
        public async Task<IActionResult> SearchEbayProduct(string tokenId, string upc)
        {
            string token;

            try
            {
                token = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = "Invalid token",
                    details = ex.Message
                });
            }

            var client = _httpClientFactory.CreateClient();

            //==========================
            // Search by UPC
            //==========================

            var searchRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.ebay.com/buy/browse/v1/item_summary/search?q={Uri.EscapeDataString(upc)}&limit=1");

            searchRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            searchRequest.Headers.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY_US");
            searchRequest.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var searchResponse = await client.SendAsync(searchRequest);

            var searchContent = await searchResponse.Content.ReadAsStringAsync();

            if (!searchResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)searchResponse.StatusCode, new
                {
                    message = "Failed to search product",
                    details = searchContent
                });
            }

            var searchJson = JsonDocument.Parse(searchContent);

            if (!searchJson.RootElement.TryGetProperty("itemSummaries", out var items) ||
                !items.EnumerateArray().Any())
            {
                return Ok(new
                {
                    message = "No products found"
                });
            }

            var first = items.EnumerateArray().First();

            var itemId = first.GetProperty("itemId").GetString();

            //==========================
            // Get Full Details
            //==========================

            var detailsRequest = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://api.ebay.com/buy/browse/v1/item/{itemId}");

            detailsRequest.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            detailsRequest.Headers.Add("X-EBAY-C-MARKETPLACE-ID", "EBAY_US");

            detailsRequest.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            var detailsResponse = await client.SendAsync(detailsRequest);

            var detailsContent = await detailsResponse.Content.ReadAsStringAsync();

            if (!detailsResponse.IsSuccessStatusCode)
            {
                return StatusCode((int)detailsResponse.StatusCode, new
                {
                    message = "Failed to get item details",
                    details = detailsContent
                });
            }

            var json = JsonDocument.Parse(detailsContent);
            var root = json.RootElement;

            //----------------------------------------------------
            // Images
            //----------------------------------------------------

            var images = new List<string>();

            if (root.TryGetProperty("image", out var image))
            {
                if (image.TryGetProperty("imageUrl", out var url))
                    images.Add(url.GetString());
            }

            if (root.TryGetProperty("additionalImages", out var additionalImages))
            {
                foreach (var img in additionalImages.EnumerateArray())
                {
                    if (img.TryGetProperty("imageUrl", out var u))
                        images.Add(u.GetString());
                }
            }

            //----------------------------------------------------
            // Specifications
            //----------------------------------------------------

            string description = "";

            if (root.TryGetProperty("description", out var desc))
            {
                description = desc.GetString() ?? "";

                // إزالة وسوم HTML
                description = Regex.Replace(description, "<.*?>", string.Empty);

                // تحويل HTML Entities مثل &nbsp; و &amp;
                description = WebUtility.HtmlDecode(description);

                // إزالة الأسطر الفارغة الزائدة
                description = Regex.Replace(description, @"(\r\n|\r|\n){2,}", "\n").Trim();
                description = Regex.Replace(description, "\n", "").Trim();
            }

            var specs = new List<object>();

            string model = "";
            string brand = "";
            string mpn = "";

            if (root.TryGetProperty("localizedAspects", out var aspects))
            {
                foreach (var aspect in aspects.EnumerateArray())
                {
                    var name = aspect.GetProperty("name").GetString();
                    var value = aspect.GetProperty("value").GetString();

                    specs.Add(new
                    {
                        Name = name,
                        Value = value
                    });

                    if (name.Equals("Model", StringComparison.OrdinalIgnoreCase))
                        model = value;

                    if (name.Equals("Brand", StringComparison.OrdinalIgnoreCase))
                        brand = value;

                    if (name.Equals("MPN", StringComparison.OrdinalIgnoreCase))
                        mpn = value;
                }
            }

            //----------------------------------------------------
            // Seller
            //----------------------------------------------------

            string sellerName = "";

            if (root.TryGetProperty("seller", out var seller))
            {
                if (seller.TryGetProperty("username", out var username))
                    sellerName = username.GetString();
            }

            //----------------------------------------------------
            // Category
            //----------------------------------------------------

            string categoryId = "";

            if (root.TryGetProperty("categoryId", out var cat))
                categoryId = cat.GetString();





            //----------------------------------------------------
            // Response
            //----------------------------------------------------

            return Ok(new
            {
                ItemId = itemId,

                SKU = (string?)null,

                Title = root.TryGetProperty("title", out var title)
                    ? title.GetString()
                    : "",

                Description = description,

                Brand = brand,

                Model = model,

                MPN = mpn,

                Price = root.TryGetProperty("price", out var price)
                    ? price.GetProperty("value").GetString()
                    : "",

                Currency = root.TryGetProperty("price", out var price2)
                    ? price2.GetProperty("currency").GetString()
                    : "",

                Condition = root.TryGetProperty("condition", out var condition)
                    ? condition.GetString()
                    : "",

                ItemWebUrl = root.TryGetProperty("itemWebUrl", out var itemUrl)
                    ? itemUrl.GetString()
                    : "",

                Store = sellerName,

                CategoryId = categoryId,

                Images = images,

                Specifications = specs
            });
        }



        [HttpGet("my-account-info/{tokenId}")]
        public async Task<IActionResult> GetEbayAccountInfo(string tokenId)
        {
            string accessToken;
            try
            {
                // التحقق من صحة الـ token
                accessToken = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://apiz.ebay.com/commerce/identity/v1/user"
            );

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, new
                {
                    message = "Failed to fetch account info",
                    details = responseContent
                });
            }

            return Content(responseContent, "application/json");
        }


        private async Task<string> IsEbayAuthenticated(string tokenId)
        {
            string accessToken;
            try
            {
                accessToken = GetValidAccessToken(tokenId);
            }
            catch
            {
                return "false";
            }
            return accessToken;

        }
        [HttpGet("check-auth/{tokenId}")]
        public async Task<IActionResult> CheckEbayAuth(string tokenId)
        {
            string isAuth = await IsEbayAuthenticated(tokenId);
            return Ok(new { authenticated = isAuth });
        }


        [HttpGet("all-listed-products/{tokenId}")]
        public async Task<IActionResult> GetSellerList(string tokenId)
        {
            string token;
            try
            {
                token = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }

            string endpoint = "https://api.ebay.com/ws/api.dll";

            string xmlRequest = $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <GetSellerListRequest xmlns=""urn:ebay:apis:eBLBaseComponents"">
                <RequesterCredentials>
                <eBayAuthToken>{token}</eBayAuthToken>
                </RequesterCredentials>
                <StartTimeFrom>{DateTime.UtcNow.AddMonths(-1):yyyy-MM-ddTHH:mm:ss.fffZ}</StartTimeFrom>
                <StartTimeTo>{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}</StartTimeTo>
                <Pagination>
                <EntriesPerPage>50</EntriesPerPage>
                <PageNumber>1</PageNumber>
                </Pagination>
                <IncludeWatchCount>true</IncludeWatchCount>
                <DetailLevel>ReturnAll</DetailLevel>
            </GetSellerListRequest>";

            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

            request.Headers.Add("X-EBAY-API-COMPATIBILITY-LEVEL", "967");
            request.Headers.Add("X-EBAY-API-DEV-NAME", GetRequiredConfig("Ebay:DEVNAME"));
            request.Headers.Add("X-EBAY-API-APP-NAME", GetRequiredConfig("Ebay:APPNAME"));
            request.Headers.Add("X-EBAY-API-CERT-NAME", GetRequiredConfig("Ebay:CERTNAME"));
            request.Headers.Add("X-EBAY-API-SITEID", "0");
            request.Headers.Add("X-EBAY-API-CALL-NAME", "GetSellerList");

            request.Content = new StringContent(xmlRequest, Encoding.UTF8, "text/xml");

            var response = await client.SendAsync(request);
            var responseXml = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, responseXml);

            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(responseXml);
            var itemNodes = xmlDoc.GetElementsByTagName("Item");

            var products = new List<object>();
            foreach (System.Xml.XmlNode item in itemNodes)
            {
                products.Add(new
                {
                    ItemID = item["ItemID"]?.InnerText,
                    Title = item["Title"]?.InnerText,
                    Price = item["SellingStatus"]?["CurrentPrice"]?.InnerText,
                    Currency = item["SellingStatus"]?["CurrentPrice"]?.Attributes?["currencyID"]?.Value,
                    Quantity = item["Quantity"]?.InnerText,
                    ListingStatus = item["SellingStatus"]?["ListingStatus"]?.InnerText,
                    StartTime = item["ListingDetails"]?["StartTime"]?.InnerText,
                    EndTime = item["ListingDetails"]?["EndTime"]?.InnerText
                });
            }

            return Ok(new { count = products.Count, products });
        }

        [HttpGet("GetSoldItemsAsync/{tokenId}")]
        public async Task<IActionResult> GetSoldItemsAsync(string tokenId)
        {

            string accessToken;
            try
            {
                accessToken = GetValidAccessToken(tokenId);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Invalid token", details = ex.Message });
            }

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "https://api.ebay.com/sell/fulfillment/v1/order"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            dynamic data = JsonConvert.DeserializeObject(json);

            var result = new List<EbaySoldItemDto>();

            foreach (var order in data.orders)
            {
                foreach (var item in order.lineItems)
                {
                    result.Add(new EbaySoldItemDto
                    {
                        OrderId = order.orderId,
                        CreationDate = order.creationDate,
                        BuyerUsername = order.buyer?.username,

                        Title = item.title,
                        Sku = item.sku,
                        Quantity = item.quantity,
                        Price = item.lineItemCost.value
                    });
                }
            }

            return Ok(result);
        }
    }
}

// DTOs
public class ProductDto
{
    public string Sku { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Brand { get; set; }
    public int Quantity { get; set; }
    public string Condition { get; set; }
    public List<string> ImageUrls { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USD";
    public string FulfillmentPolicyId { get; set; }
    public string PaymentPolicyId { get; set; }
    public string ReturnPolicyId { get; set; }
    public string CategoryId { get; set; }
    public string? EbayOfferID { get; set; }
    public string UPC { get; set; }
    public List<AspectDto> RequiredAspectsFromMetadata { get; set; } = new();

}

public class EbaySoldItemDto
{
    public string OrderId { get; set; }
    public DateTime CreationDate { get; set; }
    public string BuyerUsername { get; set; }

    public string Title { get; set; }
    public string Sku { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class TokenResponse
{
    public string access_token { get; set; }
    public string refresh_token { get; set; }
}
public class AspectDto
{
    public string Name { get; set; }
    public string DefaultValue { get; set; }
}

public class TokenInput
{
    public string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}

public class SearchRequest
{
    public string SearchText { get; set; }
}
