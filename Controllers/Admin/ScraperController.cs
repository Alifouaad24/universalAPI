using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Network;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Universal_server.Data;
using Universal_server.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScraperController : ControllerBase
    {
        UniversalDbContext dBContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        UserManager<IdentityUserData> _userManeger;
        private Cloudinary cloudinary;
        //https://www.homedepot.com/s/045242556519
        string cloudName;
        string apiKey;
        string apiSecret;
        public ScraperController(UniversalDbContext dBContext, HttpClient httpClient, UserManager<IdentityUserData> userManeger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.dBContext = dBContext;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpClient = httpClient;
            _userManeger = userManeger;
            cloudinary = new Cloudinary(new Account(
                 configuration["Claoudinary:cloudName"],
                 configuration["Claoudinary:apiKey"],
                 configuration["Claoudinary:apiSecret"]
             ));
        }

        [HttpGet("ScrapeUrl")]
        public async Task<IActionResult> ScrapeUrl(
    [FromQuery] string url,
    CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest(new { success = false, message = "رابط غير صالح." });
            }

            try
            {
                using var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.All,
                    AllowAutoRedirect = true
                };
                using var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };

                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "Mozilla/5.0 (Linux; Android 14; Pixel 8) AppleWebKit/537.36 " +
                    "(KHTML, like Gecko) Chrome/128.0.0.0 Mobile Safari/537.36");
                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");

                using var res = await client.GetAsync(uri, cancellationToken);
                var body = await res.Content.ReadAsStringAsync(cancellationToken);
                var contentType = res.Content.Headers.ContentType?.MediaType ?? "text/plain";

                // إن كانت الاستجابة JSON نعيدها كما هي، وإلا نعيد النص (HTML مثلاً).
                return Content(body, contentType + "; charset=utf-8");
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, new { success = false, message = "انتهت المهلة." });
            }
            catch (Exception ex)
            {
                return StatusCode(502, new { success = false, message = "فشل جلب الرابط.", error = ex.Message });
            }
        }
        public class SheinProductInfo
        {
            public string ProductId { get; set; } = "";
            public string ProductName { get; set; } = "";
            public string ImageUrl { get; set; } = "";
        }


        // get basic data from encribted url from shein

        [HttpGet("extract")]
        public IActionResult Extract([FromQuery] string url, [FromQuery] int timeoutSeconds = 20)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest(
                
                    "يجب إرسال معلمة url."
                );
            }

            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--lang=en-US");

            // تعطيل JavaScript مهم جداً:
            // الصفحة تحتوي سكربت يعيد التوجيه تلقائياً بعد 100ms،
            // تعطيل JS يمنع الهروب من الصفحة ويتيح قراءة العناصر الثابتة بأمان.
            options.AddUserProfilePreference(
                "profile.managed_default_content_settings.javascript", 2);

            IWebDriver? driver = null;

            try
            {
                driver = new ChromeDriver(options);
                driver.Navigate().GoToUrl(url);

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(d =>
                {
                    try { return d.FindElement(By.Id("deeplink")) != null; }
                    catch (NoSuchElementException) { return false; }
                });

                // 1) اسم المنتج - من عنوان الصفحة (نفس محتوى og:title)
                string productName = driver.Title;

                // 2) رابط الصورة - من og:image (مع fallback على twitter:image)
                string imageUrl = "";
                try
                {
                    var ogImage = driver.FindElement(By.CssSelector("meta[property='og:image']"));
                    imageUrl = ogImage.GetAttribute("content") ?? "";
                }
                catch (NoSuchElementException)
                {
                    try
                    {
                        var twImage = driver.FindElement(By.CssSelector("meta[name='twitter:image']"));
                        imageUrl = twImage.GetAttribute("content") ?? "";
                    }
                    catch (NoSuchElementException) { }
                }

                // 3) رقم المنتج (goods_id) - من داخل قيمة input#deeplink
                //    القيمة تكون بالشكل:
                //    sheinlink://applink/goods?data=%7B%22goods_id%22%3A%22503234950%22...%7D
                string productId = "";
                try
                {
                    var deeplinkInput = driver.FindElement(By.Id("deeplink"));
                    string deeplinkValue = deeplinkInput.GetAttribute("value") ?? "";

                    const string marker = "data=";
                    int dataIndex = deeplinkValue.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                    if (dataIndex >= 0)
                    {
                        string encodedJson = deeplinkValue.Substring(dataIndex + marker.Length);
                        string decodedJson = HttpUtility.UrlDecode(encodedJson);

                        using var doc = JsonDocument.Parse(decodedJson);
                        if (doc.RootElement.TryGetProperty("goods_id", out var goodsIdEl))
                        {
                            productId = goodsIdEl.GetString() ?? "";
                        }
                    }
                }
                catch (NoSuchElementException) { /* لا يوجد عنصر deeplink */ }
                catch (JsonException) { /* فشل تحليل JSON */ }

                var result = new SheinProductInfo
                {
                    ProductId = productId,
                    ProductName = productName,
                    ImageUrl = imageUrl,
                };

 ;

                return Ok(result);
            }
            catch (WebDriverTimeoutException)
            {
                return StatusCode(500,
                
                    "انتهت مهلة تحميل الصفحة (Timeout)."
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, 
                
                     $"حدث خطأ غير متوقع: {ex.Message}"
                );
            }
            finally
            {
                driver?.Quit();
            }
        }


        /// <summary>
        /// /////////////////////////////////
        /// </summary>
        /// <param name="encodicUrl"></param>
        /// <returns></returns>


        [HttpPost("getDataFromSheInAidedClaude/{encodicUrl}")]
        public async Task<IActionResult> GetDataFromSheInAidedClaude(string encodicUrl)
        {
            try
            {
                var sheinUrl = encodicUrl;

                var apiKey = _configuration["Anthropic:ApiKey"]
                    ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");

                if (string.IsNullOrEmpty(apiKey))
                    return BadRequest(new { error = "Anthropic API key is not configured." });

                // 1) جلب HTML الصفحة بنفسك أولاً
                var pageClient = _httpClientFactory.CreateClient();
                pageClient.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");

                string pageHtml;
                try
                {
                    pageHtml = await pageClient.GetStringAsync("https://onelink.shein.com/44/5wtxu5kyqogg?shc=2_RuGtHcXrvNk");
                }
                catch (Exception fetchEx)
                {
                    return BadRequest(new
                    {
                        error = "فشل في جلب صفحة SHEIN مباشرة. الأرجح أن الصفحة تحتاج تصفح JS (headless browser مثل Puppeteer/Playwright).",
                        details = fetchEx.Message
                    });
                }

                // اختصار الـ HTML الطويل جدًا (Claude له حد على عدد التوكنز)
                if (pageHtml.Length > 100000)
                    pageHtml = pageHtml.Substring(0, 100000);

                // 2) إرسال الـ HTML الفعلي إلى Claude ليستخرج البيانات
                var http = _httpClientFactory.CreateClient();
                http.DefaultRequestHeaders.Add("x-api-key", apiKey);
                http.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var prompt = $@"أنت أداة استخراج بيانات دقيقة. لديك كود HTML الخام لصفحة منتج من موقع SHEIN.

                    المطلوب استخراج 4 عناصر فقط:
                    1. اسم المنتج (Product Name)
                    2. السعر (Price) — الرقم فقط مع رمز أو كود العملة
                    3. رابط الصورة الرئيسية للمنتج (Main Product Image URL) — أول/أكبر صورة تمثل المنتج، وليس صور بانرات أو إعلانات
                    4. رمز المنتج SKU — غالبًا يكون داخل data attributes مثل data-sku, data-goods-sn, أو ضمن JSON-LD/JSON مدمج بالصفحة بصيغة مثل ""goods_sn"" أو ""sku_code""

                    أماكن محتملة للبحث داخل الـ HTML:
                    - علامات <meta property=""og:title""> و <meta property=""og:image""> و <meta property=""product:price:amount"">
                    - بيانات JSON-LD داخل <script type=""application/ld+json"">
                    - أي سكريبت JavaScript embedded يحتوي على object باسم مثل productInfo أو goodsDetail أو __INITIAL_STATE__
                    - data attributes في عناصر HTML مثل data-price, data-sku, data-image

                    قواعد صارمة:
                    - إذا لم تجد قيمة لعنصر معين، ضع قيمته null (لا تخترع بيانات).
                    - لا تشرح، لا تعلّق، لا تستخدم Markdown code fences.
                    - الرد يجب أن يكون JSON صحيح 100% فقط، بدون أي نص قبله أو بعده.
                    - التزم بهذا الشكل بالضبط:

                    {{
                      ""sku"": null,
                      ""name"": null,
                      ""price"": null,
                      ""mainImage"": null
                    }}

                    كود HTML:
                    {pageHtml}";

                var claudeRequest = new
                {
                    model = "claude-sonnet-4-5",
                    max_tokens = 3000,
                    messages = new[] { new { role = "user", content = prompt } }
                };

                var response = await http.PostAsJsonAsync(
                    "https://api.anthropic.com/v1/messages",
                    claudeRequest);

                var rawJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, new { error = "فشل طلب Claude API", details = rawJson });

                using var doc = JsonDocument.Parse(rawJson);
                var textBuilder = new StringBuilder();
                if (doc.RootElement.TryGetProperty("content", out var contentArray))
                {
                    foreach (var block in contentArray.EnumerateArray())
                    {
                        if (block.TryGetProperty("type", out var t) && t.GetString() == "text"
                            && block.TryGetProperty("text", out var txt))
                        {
                            textBuilder.Append(txt.GetString());
                        }
                    }
                }

                var extractedText = textBuilder.ToString().Replace("```json", "").Replace("```", "").Trim();

                object? parsedProduct;
                try
                {
                    parsedProduct = JsonSerializer.Deserialize<object>(extractedText);
                }
                catch (JsonException)
                {
                    return Ok(new { encodicUrl, warning = "تعذر تحويل رد Claude إلى JSON", raw = extractedText });
                }

                return Ok(new {parsedProduct, pageHtml });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

      

        // المتطلبات (NuGet):
        //   Selenium.WebDriver
        //   Selenium.WebDriver.ChromeDriver   (أو WebDriverManager.Net لتحميل الدرايفر تلقائيًا)
        //
        // using OpenQA.Selenium;
        // using OpenQA.Selenium.Chrome;
        // using OpenQA.Selenium.Support.UI;
        // using System.Text.RegularExpressions;
        // using System.Text.Json;
        // using System.Linq;

        [HttpPost("getDataFromSheinn/{encodicUrl}")]
        public async Task<IActionResult> getDataFromSheinn(string encodicUrl)
        {
            try
            {
                var sheinUrl = encodicUrl;

                // 1) جلب HTML الصفحة بعد تنفيذ الجافاسكريبت عبر Selenium (Headless Chrome)
                string pageHtml;
                try
                {
                    pageHtml = await Task.Run(() => FetchRenderedHtml(sheinUrl));
                }
                catch (Exception fetchEx)
                {
                    return BadRequest(new
                    {
                        error = "فشل في جلب صفحة SHEIN عبر المتصفح (Selenium).",
                        details = fetchEx.Message
                    });
                }

                // 2) استخراج البيانات مباشرة من الـ HTML بدون أي API خارجي
                string? name = null;
                string? price = null;
                string? currency = null;
                string? mainImage = null;
                string? skuValue = null;

                // --- أ) من meta tags (Open Graph / product) ---
                name ??= ExtractMetaContent(pageHtml, "og:title");
                mainImage ??= ExtractMetaContent(pageHtml, "og:image");
                price ??= ExtractMetaContent(pageHtml, "product:price:amount");
                currency ??= ExtractMetaContent(pageHtml, "product:price:currency");

                // --- ب) من JSON-LD (<script type="application/ld+json">) ---
                var ldJsonBlocks = Regex.Matches(
                    pageHtml,
                    @"<script[^>]*type=[""']application/ld\+json[""'][^>]*>(.*?)</script>",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);

                foreach (Match m in ldJsonBlocks)
                {
                    var jsonText = m.Groups[1].Value.Trim();
                    try
                    {
                        using var ldDoc = JsonDocument.Parse(jsonText);
                        var root = ldDoc.RootElement;

                        var candidates = root.ValueKind == JsonValueKind.Array
                            ? root.EnumerateArray()
                            : new[] { root }.AsEnumerable();

                        foreach (var el in candidates)
                        {
                            if (el.TryGetProperty("@type", out var typeProp) &&
                                string.Equals(typeProp.GetString(), "Product", StringComparison.OrdinalIgnoreCase))
                            {
                                if (name is null && el.TryGetProperty("name", out var nameProp))
                                    name = nameProp.GetString();

                                if (mainImage is null && el.TryGetProperty("image", out var imgProp))
                                {
                                    mainImage = imgProp.ValueKind == JsonValueKind.Array
                                        ? imgProp.EnumerateArray().FirstOrDefault().GetString()
                                        : imgProp.GetString();
                                }

                                if (el.TryGetProperty("sku", out var skuProp))
                                    skuValue ??= skuProp.GetString();

                                if (el.TryGetProperty("offers", out var offersProp))
                                {
                                    var offer = offersProp.ValueKind == JsonValueKind.Array
                                        ? offersProp.EnumerateArray().FirstOrDefault()
                                        : offersProp;

                                    if (offer.ValueKind == JsonValueKind.Object)
                                    {
                                        if (price is null && offer.TryGetProperty("price", out var priceProp))
                                            price = priceProp.ValueKind == JsonValueKind.Number
                                                ? priceProp.GetRawText()
                                                : priceProp.GetString();

                                        if (currency is null && offer.TryGetProperty("priceCurrency", out var curProp))
                                            currency = curProp.GetString();
                                    }
                                }
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        continue;
                    }
                }

                // --- ج) من data attributes مباشرة في الـ HTML ---
                skuValue ??= ExtractAttr(pageHtml, "data-sku")
                             ?? ExtractAttr(pageHtml, "data-goods-sn")
                             ?? ExtractFromEmbeddedJson(pageHtml, "goods_sn")
                             ?? ExtractFromEmbeddedJson(pageHtml, "sku_code");

                price ??= ExtractAttr(pageHtml, "data-price")
                          ?? ExtractFromEmbeddedJson(pageHtml, "salePrice")
                          ?? ExtractFromEmbeddedJson(pageHtml, "price");

                mainImage ??= ExtractAttr(pageHtml, "data-image");

                var parsedProduct = new
                {
                    sku = skuValue,
                    name,
                    price,
                    currency,
                    mainImage
                };

                return Ok(new { encodicUrl, parsedProduct });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ---------- Selenium ----------

        private static string FetchRenderedHtml(string url)
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless=new");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument(
                "--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36");

            using var driver = new ChromeDriver(options);
            try
            {
                driver.Navigate().GoToUrl(url);

                // انتظار تحميل عنصر مهم في صفحة المنتج (يفضّل تعديل السيلكتور حسب بنية SHEIN الفعلية)
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                try
                {
                    wait.Until(d =>
                        d.FindElements(By.CssSelector("meta[property='og:title']")).Count > 0
                        || d.FindElements(By.CssSelector("script[type='application/ld+json']")).Count > 0);
                }
                catch (WebDriverTimeoutException)
                {
                    // نستمر بأي حال ونأخذ الـ HTML كما هو حتى لو لم تظهر العناصر المتوقعة
                }

                return driver.PageSource;
            }
            finally
            {
                driver.Quit();
            }
        }

        // ---------- Helpers (استخراج نصي) ----------

        private static string? ExtractMetaContent(string html, string propertyName)
        {
            var pattern =
                $@"<meta[^>]+(?:property|name)=[""']{Regex.Escape(propertyName)}[""'][^>]+content=[""']([^""']+)[""']" +
                $@"|<meta[^>]+content=[""']([^""']+)[""'][^>]+(?:property|name)=[""']{Regex.Escape(propertyName)}[""']";

            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (!match.Success) return null;

            return match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
        }

        private static string? ExtractAttr(string html, string attrName)
        {
            var pattern = $@"{Regex.Escape(attrName)}=[""']([^""']+)[""']";
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value : null;
        }

        private static string? ExtractFromEmbeddedJson(string html, string key)
        {
            var pattern = $@"[""']{Regex.Escape(key)}[""']\s*:\s*[""']?([^"",\}}\]]+)[""']?";
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : null;
        }



        [HttpPost("ScrapeByFergusonhomeHtml")]
        public async Task<IActionResult> ScrapeByFergusonhomeHtml(IFormFile htmlFile)
        {
            try
            {
                if (htmlFile == null || htmlFile.Length == 0)
                {
                    return BadRequest(new
                    {
                        message = "HTML file is required"
                    });
                }


                string html;

                using (var reader = new StreamReader(htmlFile.OpenReadStream()))
                {
                    html = await reader.ReadToEndAsync();
                }


                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);



                string title = "";
                string brand = "";
                string model = "";
                string item = "";
                string desc = "";
                decimal price = 0;

                List<string> images = new();



                // =========================
                // TITLE
                // =========================

                try
                {
                    title = doc.DocumentNode
                        .SelectSingleNode("//h1")
                        ?.InnerText
                        .CleanText()
                        ?? "";
                }
                catch { }



                // =========================
                // BRAND
                // =========================

                try
                {
                    brand = doc.DocumentNode
                        .SelectSingleNode(
                            "//nav[@aria-label='Breadcrumb']//li[3]//span[@itemprop='name']"
                        )
                        ?.InnerText
                        .CleanText()
                        ?? "";
                }
                catch { }



                // =========================
                // MODEL
                // =========================

                try
                {
                    model = doc.DocumentNode
                        .SelectSingleNode(
                            "//span[@data-automation='product-model-number']"
                        )
                        ?.InnerText
                        .CleanText()
                        ?? "";
                }
                catch { }



                // =========================
                // ITEM NUMBER
                // =========================

                try
                {
                    item = doc.DocumentNode
                        .SelectSingleNode(
                            "//span[@data-automation='product-item-number']"
                        )
                        ?.InnerText
                        .CleanText()
                        ?? "";
                }
                catch { }



                // =========================
                // PRICE
                // =========================

                try
                {
                    var priceText = doc.DocumentNode
                        .SelectSingleNode(
                            "//span[@data-automation='price']"
                        )
                        ?.InnerText
                        .Replace("$", "")
                        .Replace(",", "")
                        .Trim();


                    decimal.TryParse(priceText, out price);

                }
                catch { }



                // =========================
                // DESCRIPTION
                // =========================

                try
                {
                    desc = doc.DocumentNode
                        .SelectSingleNode(
                            "//div[contains(@class,'lh-copy') and contains(@class,'H_oFW')]"
                        )
                        ?.InnerText
                        .CleanText()
                        ?? "";
                }
                catch { }



                // =========================
                // IMAGES
                // =========================

                try
                {
                    var imageNodes = doc.DocumentNode
                        .SelectNodes("//div[@aria-label='main image']//img");


                    if (imageNodes != null)
                    {
                        foreach (var img in imageNodes)
                        {
                            // src
                            var src = img.GetAttributeValue("src", "");

                            if (!string.IsNullOrWhiteSpace(src))
                            {
                                images.Add(src);
                            }


                            // srcset
                            var srcSet = img.GetAttributeValue("srcset", "");

                            if (!string.IsNullOrWhiteSpace(srcSet))
                            {
                                var srcSetImages = srcSet
                                    .Split(',')
                                    .Select(x => x.Trim())
                                    .Select(x =>
                                    {
                                        // remove 330w / 450w
                                        return x.Split(' ')[0];
                                    })
                                    .ToList();


                                images.AddRange(srcSetImages);
                            }
                        }


                        images = images
                            .Where(x =>
                                x.Contains("/product/")
                                &&
                                !x.Contains("transparent")
                            )
                            .Select(x =>
                            {
                                // إزالة التكرار بين الأحجام
                                x = x.Replace(
                                    "t_base,c_pad,f_auto,dpr_auto,w_330,h_330",
                                    "t_base,c_pad,f_auto,dpr_2,w_450,h_450"
                                );

                                return x;
                            })
                            .Distinct()
                            .ToList();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return Ok(new
                {
                    images,
                    title,
                    brand,
                    model,
                    item,
                    price,
                    desc,
                    source = "Fergusonhome"

                });

            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        private static readonly HttpClient _fergusonClient = new(new HttpClientHandler
        {
            AutomaticDecompression = System.Net.DecompressionMethods.All,
            AllowAutoRedirect = true,
            UseCookies = true
        })
        { Timeout = TimeSpan.FromSeconds(30) };


        [HttpGet("ScrapeFergusonhome")]
        public async Task<IActionResult> ScrapeFergusonhome([FromQuery] string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                !(uri.Host.EndsWith("fergusonhome.com") || uri.Host.EndsWith("build.com")))
                return BadRequest(new { message = "A valid fergusonhome.com product URL is required" });

            try
            {
                // ========================= جلب الصفحة =========================
                var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, uri);
                request.Headers.TryAddWithoutValidation("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");
                request.Headers.TryAddWithoutValidation("Accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
                request.Headers.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.9");

                var response = await _fergusonClient.SendAsync(request);
                var html = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode(502, new { message = $"Fergusonhome blocked the request (HTTP {(int)response.StatusCode}). Use the HTML source option." });

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                string title = "", brand = "", model = "", item = "", desc = "", sku = "", upc = "", currency = "USD";
                decimal price = 0;
                var images = new List<string>();

                string First(params string?[] v) => v.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))?.Trim() ?? "";
                string Text(string xpath)
                {
                    var n = doc.DocumentNode.SelectSingleNode(xpath);
                    return n == null ? "" : HtmlAgilityPack.HtmlEntity.DeEntitize(n.InnerText).CleanText() ?? "";
                }
                decimal Price(string? t) =>
                    decimal.TryParse(System.Text.RegularExpressions.Regex.Replace(t ?? "", @"[^\d.]", ""),
                        System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : 0;

                // ========================= JSON-LD (الأدق) =========================
                var scripts = doc.DocumentNode.SelectNodes("//script[@type='application/ld+json']");
                if (scripts != null)
                {
                    foreach (var script in scripts)
                    {
                        System.Text.Json.JsonDocument json;
                        try { json = System.Text.Json.JsonDocument.Parse(HtmlAgilityPack.HtmlEntity.DeEntitize(script.InnerText)); }
                        catch { continue; }

                        using (json)
                        {
                            var stack = new Stack<System.Text.Json.JsonElement>();
                            stack.Push(json.RootElement);

                            while (stack.Count > 0)
                            {
                                var el = stack.Pop();
                                if (el.ValueKind == System.Text.Json.JsonValueKind.Array)
                                {
                                    foreach (var c in el.EnumerateArray()) stack.Push(c);
                                    continue;
                                }
                                if (el.ValueKind != System.Text.Json.JsonValueKind.Object) continue;
                                if (el.TryGetProperty("@graph", out var graph)) stack.Push(graph);

                                if (!el.TryGetProperty("@type", out var type) || !type.ToString().Contains("Product")) continue;

                                string S(System.Text.Json.JsonElement e, string name)
                                {
                                    if (e.ValueKind != System.Text.Json.JsonValueKind.Object || !e.TryGetProperty(name, out var v)) return "";
                                    return v.ValueKind switch
                                    {
                                        System.Text.Json.JsonValueKind.String => HtmlAgilityPack.HtmlEntity.DeEntitize(v.GetString() ?? ""),
                                        System.Text.Json.JsonValueKind.Number => v.GetRawText(),
                                        System.Text.Json.JsonValueKind.Object => S(v, "name"),
                                        _ => ""
                                    };
                                }

                                title = First(title, S(el, "name"));
                                brand = First(brand, S(el, "brand"));
                                desc = First(desc, S(el, "description"));
                                sku = First(sku, S(el, "sku"));
                                model = First(model, S(el, "mpn"));
                                upc = First(upc, S(el, "gtin12"), S(el, "gtin13"), S(el, "gtin14"), S(el, "gtin"));

                                if (el.TryGetProperty("image", out var img))
                                {
                                    if (img.ValueKind == System.Text.Json.JsonValueKind.String) images.Add(img.GetString()!);
                                    else if (img.ValueKind == System.Text.Json.JsonValueKind.Array)
                                        foreach (var i in img.EnumerateArray())
                                            images.Add(i.ValueKind == System.Text.Json.JsonValueKind.String
                                                ? i.GetString()!
                                                : First(S(i, "contentUrl"), S(i, "url")));
                                }

                                if (el.TryGetProperty("offers", out var offers))
                                {
                                    var offer = offers.ValueKind == System.Text.Json.JsonValueKind.Array && offers.GetArrayLength() > 0
                                        ? offers[0] : offers;
                                    if (price == 0) price = Price(First(S(offer, "price"), S(offer, "lowPrice")));
                                    currency = First(S(offer, "priceCurrency"), currency);
                                    upc = First(upc, S(offer, "gtin12"), S(offer, "gtin13"));
                                    sku = First(sku, S(offer, "sku"));
                                }
                            }
                        }
                    }
                }

                // ========================= الاحتياط: نفس الـ selectors القديمة =========================
                title = First(title, Text("//h1"));
                brand = First(brand, Text("//nav[@aria-label='Breadcrumb']//li[3]//span[@itemprop='name']"));
                model = First(model, Text("//span[@data-automation='product-model-number']"));
                item = Text("//span[@data-automation='product-item-number']");
                desc = First(desc, Text("//div[contains(@class,'lh-copy') and contains(@class,'H_oFW')]"));
                if (price == 0) price = Price(Text("//span[@data-automation='price']"));
                sku = First(sku, item, model);

                if (string.IsNullOrEmpty(upc))
                {
                    var m = System.Text.RegularExpressions.Regex.Match(
                        HtmlAgilityPack.HtmlEntity.DeEntitize(doc.DocumentNode.InnerText),
                        @"\bUPC\b[^0-9]{0,20}(\d{12,14})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (m.Success) upc = m.Groups[1].Value;
                }

                var imageNodes = doc.DocumentNode.SelectNodes("//div[@aria-label='main image']//img");
                if (imageNodes != null)
                {
                    foreach (var img in imageNodes)
                    {
                        images.Add(img.GetAttributeValue("src", ""));
                        var srcSet = img.GetAttributeValue("srcset", "");
                        if (!string.IsNullOrWhiteSpace(srcSet))
                            images.AddRange(srcSet.Split(',').Select(x => x.Trim().Split(' ')[0]));
                    }
                }

                // ========================= الصور بدقة عالية وبدون تكرار =========================
                images = images
                    .Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("data:") && !x.Contains("transparent"))
                    .Select(x =>
                    {
                        x = x.Trim();
                        if (x.StartsWith("//")) x = "https:" + x;
                        x = System.Text.RegularExpressions.Regex.Replace(x, @"\bw_\d+", "w_2000");
                        x = System.Text.RegularExpressions.Regex.Replace(x, @"\bh_\d+", "h_2000");
                        return x.Replace("dpr_auto", "dpr_1");
                    })
                    .Where(x => x.Contains("/product/"))
                    .DistinctBy(x => x.Contains("/product/") ? x[x.IndexOf("/product/")..].Split('?')[0] : x)
                    .ToList();

                // صفحة حظر/تحقق بدل صفحة المنتج
                if (string.IsNullOrEmpty(title) && images.Count == 0)
                    return StatusCode(502, new { message = "Fergusonhome returned a verification page. Use the HTML source option." });

                return Ok(new
                {
                    url = uri.ToString(),
                    title,
                    brand,
                    sku,
                    upc,
                    model,
                    item,
                    price,
                    currency,
                    desc,
                    images,
                    source = "Fergusonhome"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }


        }
        private async Task<string> GetFinalIherbProUrl(string wordSearch)
        {
            try
            {
                var options = new ChromeOptions();

                using (var driver = new ChromeDriver(options))
                {
                    var url = $"https://www.iherb.com/search?kw={Uri.EscapeDataString(wordSearch)}";

                    driver.Navigate().GoToUrl(url);

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                    var productElement = wait.Until(d =>
                    {
                        try
                        {
                            return d.FindElement(By.CssSelector("a.product-link"));
                        }
                        catch
                        {
                            return null;
                        }
                    });

                    if (productElement == null)
                    {
                        return "لم يتم العثور على أي منتج";
                        
                    }

                    string productUrl = productElement.GetAttribute("href");

                    return productUrl;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [HttpPost("ByIherb/{wordSearch}")]
        public async Task<IActionResult> ByIherb(string wordSearch)
        {
            try
            {
                var options = new ChromeOptions();
                // options.AddArgument("--headless");

                using (var driver = new ChromeDriver(options))
                {

                    var url1 = await GetFinalIherbProUrl(wordSearch);
                    driver.Navigate().GoToUrl(url1);

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                    wait.Until(d =>
                    {
                        try
                        {
                            return d.FindElement(
                                By.CssSelector("body > div.product-grouping-wrapper.defer-block > article > div:nth-child(2)")
                            ).Displayed;
                        }
                        catch
                        {
                            return false;
                        }
                    });


                    string pageContent = driver.PageSource;

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(pageContent);



                    var priceNode = doc.DocumentNode.SelectSingleNode(
                        "//section[contains(@class,'original-price-wrapper')]//span[contains(@class,'list-price')]"
                    );

                    string priceText = priceNode?.InnerText.Trim() ?? "0";

                    // 20.45 $ => 20.45
                    string numericPrice = Regex.Replace(priceText, @"[^\d.]", "");

                    decimal price = decimal.TryParse(
                        numericPrice,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out var result)
                        ? result
                        : 0m;




                    var overviewNode = doc.DocumentNode.SelectSingleNode("//h3/strong[contains(text(),'Overview')]/ancestor::div[contains(@class,'col-xs-24')]/div");
                    string brand = driver.FindElement(
                            By.CssSelector("#brand a[data-testid='product-brand-link']")
                        ).Text.Trim();

                    string productDescription = "";

                    if (overviewNode != null)
                    {
                        productDescription = HtmlEntity.DeEntitize(
                            overviewNode.InnerText.Trim()
                        );
                    }

                    var productName = driver.FindElement(By.XPath("//h1[@data-testid='product-name']")).Text;

                    var productImages = doc.DocumentNode
                        .SelectNodes("//div[contains(@class,'thumbnail-container')]//img[@data-large-img]")
                        ?.Select(img => img.GetAttributeValue("data-large-img", ""))
                        .Where(url => !string.IsNullOrWhiteSpace(url))
                        .Distinct()
                        .ToList()
                        ?? new List<string>();



                    // نحافظ على نفس البنية تمامًا
                    return Ok(new
                    {
                        title = productName,
                        desc = productDescription,
                        images = productImages,
                        price = price,
                        brand = brand
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }


        [HttpPost("ByIherbBulk/{wordSearch}")]
        public async Task<IActionResult> ByIherbBulk(string wordSearch)
        {
            try
            {
                var options = new ChromeOptions();
                // options.AddArgument("--headless");

                options.AddArgument("--disable-blink-features=AutomationControlled");

                using (var driver = new ChromeDriver(options))
                {
                    var url =
                        $"https://nl.iherb.com/search?kw={Uri.EscapeDataString(wordSearch)}";

                    driver.Navigate().GoToUrl(url);


                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

                    wait.Until(d =>
                    {
                        try
                        {
                            return d.FindElements(
                                By.CssSelector(".product-cell-container")
                            ).Count > 0;
                        }
                        catch
                        {
                            return false;
                        }
                    });


                    var doc = new HtmlAgilityPack.HtmlDocument();

                    doc.LoadHtml(driver.PageSource);


                    var products = new List<object>();


                    var productNodes = doc.DocumentNode
                        .SelectNodes(
                        "//div[contains(@class,'product-cell-container')]"
                        );


                    if (productNodes != null)
                    {
                        foreach (var product in productNodes)
                        {

                            // الاسم
                            var titleNode = product.SelectSingleNode(
                                ".//div[contains(@class,'product-title')]"
                            );

                            var title =
                                HtmlEntity.DeEntitize(
                                    titleNode?.InnerText.Trim() ?? ""
                                );


                            // الرابط
                            var linkNode = product.SelectSingleNode(
                                ".//a[contains(@class,'product-link')]"
                            );

                            var link =
                                linkNode?.GetAttributeValue("href", "");


                            // الصورة
                            var imageNode = product.SelectSingleNode(
                                ".//img"
                            );

                            var image =
                                imageNode?.GetAttributeValue("src", "");



                            // السعر
                            var priceNode = product.SelectSingleNode(
                                ".//span[contains(@class,'price')]"
                            );


                            var price =
                                HtmlEntity.DeEntitize(
                                    priceNode?.InnerText.Trim() ?? "0"
                                );


                            // product id
                            var productIdNode =
                                product.SelectSingleNode(
                                    ".//input[@name='pid']"
                                );

                            var productId =
                                productIdNode?.GetAttributeValue(
                                    "value",
                                    ""
                                );


                            // SKU
                            var skuNode =
                                product.SelectSingleNode(
                                    ".//*[@itemprop='sku']"
                                );

                            var sku =
                                skuNode?.GetAttributeValue(
                                    "content",
                                    ""
                                );



                            products.Add(new
                            {
                                id = productId,
                                sku = sku,
                                title = title,
                                url = link,
                                image = image,
                                price = price
                            });

                        }
                    }


                    return Ok(new
                    {
                        count = products.Count,
                        products = products
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    msg = ex.Message
                });
            }
        }









        [HttpGet("ByUPCItems/{upcCode}")]
        public async Task<IActionResult> ByUPCItems(string upcCode)
        {
            List<string> imgs = new();
            decimal price = 0;
            string upc = "Does not Apply";
            string brand = "RYOBI";
            string internet = string.Empty;
            string model = string.Empty;
            string productTitle = string.Empty;
            string source = "Ryobi Tools";
            string description = string.Empty;
            string storeSku = string.Empty;
            string desc = string.Empty;
            string length = "10";
            string h = "10";
            string w = "10";
            string dimensionsText = string.Empty;
            int qty = 0;
            string fullText = string.Empty;
            string url2 = $"https://api.upcitemdb.com/prod/trial/lookup?upc={upcCode}";
            HttpResponseMessage response = await _httpClient.GetAsync(url2);
            string responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var json = JObject.Parse(responseContent);
                var item = json["items"]?.FirstOrDefault();

                if (item != null)
                {
                    var imgs1 = item["images"]?.ToObject<List<string>>();
                    var offers = item["offers"]?.FirstOrDefault();
                    var price1 = offers?["price"]?.ToObject<decimal?>() ?? 0;
                    var model1 = item["model"]?.ToString();
                    var internet1 = item["asin"]?.ToString();
                    var storeSku1 = item["sku"]?.ToString();
                    var brand1 = item["brand"]?.ToString();
                    var productTitle1 = item["title"]?.ToString();
                    var description2 = item["description"]?.ToString();

                    // for upc item
                    return Ok(new
                    {
                        images = imgs1,
                        price = price1,
                        Model = model1,
                        Internet = internet1,
                        SKU = storeSku1,
                        Brand = brand1,
                        Title = productTitle1,
                        Source = "UPC Items DB",
                        desc = description2,
                        qty = 0,
                        height = h,
                        wedth = w,
                        length = length,
                    });
                }

                return NotFound(new { msg = "Item not found in response" });
            }
            return BadRequest();
        }

        // for universal from sheIn
        [HttpPost("getImagesFromShein")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> GetInfoFromSheIn(IFormFile htmlFile)
        {
            if (htmlFile == null || htmlFile.Length == 0)
                return BadRequest("الملف غير موجود أو فارغ");

            string htmlContent;
            string imageUrl = string.Empty;
            string price = string.Empty;
            List<string> images = new List<string>();

            using (var reader = new StreamReader(htmlFile.OpenReadStream(), Encoding.UTF8))
            {
                htmlContent = await reader.ReadToEndAsync();

            }

            htmlContent = htmlContent.Replace("\r\n", "").Replace("\n", "").Replace("\r", "");
            htmlContent = htmlContent.Replace("\\\"", "\"");
            // تحميل المحتوى في HtmlAgilityPack
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(htmlContent);



            var nameNode = document.DocumentNode.SelectSingleNode("//span[@class='fsp-element']");
            if (nameNode == null)
            {
                nameNode = document.DocumentNode.SelectSingleNode("//h1[contains(@class,'fsp-element')]");

            }
            var priceNode = document.DocumentNode.SelectSingleNode("//div[@id='productMainPriceId']");

            string productName = nameNode?.InnerText.Trim();
            price = priceNode?.InnerText.Trim();

            if (!price.Contains("$")) return BadRequest("Turn shein website to $ currency!");

            var imgNodes = document.DocumentNode.SelectNodes("//img[@data-src]");

            if (imgNodes != null)
            {
                foreach (var node in imgNodes)
                {
                    var src = node.GetAttributeValue("data-src", "");

                    if (!string.IsNullOrEmpty(src))
                    {
                        if (src.StartsWith("//"))
                            src = "https:" + src;

                        images.Add(src);
                    }
                }
            }

            // استخراج SKU
            var skuNode = document.DocumentNode.SelectSingleNode("//span[contains(@class,'product-intro__head-sku-text')]");
            string sku = skuNode?.InnerText?.Replace("SKU:", "").Trim() ?? "لم يتم العثور على SKU";

            var imageNode = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'crop-image-container')]");

            if (imageNode != null)
            {
                imageUrl = imageNode.GetAttributeValue("data-before-crop-src", "");

                if (imageUrl.StartsWith("//"))
                    imageUrl = "https:" + imageUrl;

            }

            if (images.Count > 8)
                images = images.Slice(0, 8);

            return Ok(new
            {
                images = images,
                price = price,
                name = productName
            });
        }


        // by Ryobi
        [HttpPost("ByRyobiTools/{wordSearch}")]
        public async Task<IActionResult> ByRyobiTools(string wordSearch)
        {
            List<string> imgs = new();
            decimal price = 0;
            string upc = "Does not Apply";
            string brand = "RYOBI";
            string internet = string.Empty;
            string model = string.Empty;
            string productTitle = string.Empty;
            string source = "Ryobi Tools";
            string description = string.Empty;
            string storeSku = string.Empty;
            string desc = string.Empty;
            string length = "10";
            string h = "10";
            string w = "10";
            string dimensionsText = string.Empty;
            int qty = 0;
            string fullText = string.Empty;

            try
            {
                var options = new ChromeOptions();
                //options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");

                using (var driver = new ChromeDriver(options))
                {
                    var url = $"https://www.ryobitools.com/products/{wordSearch}";
                    driver.Navigate().GoToUrl(url);
                    await Task.Delay(4000);

                    // انتظار تحميل الصفحة
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => d.FindElement(By.TagName("h1")));

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(driver.PageSource);

                    // العنوان
                    var titleNode = doc.DocumentNode.SelectSingleNode("//h1[contains(@class, 'headline-lg')]");
                    if (titleNode != null)
                        productTitle = HtmlEntity.DeEntitize(titleNode.InnerText.Trim());

                    // الموديل
                    var modelNode = doc.DocumentNode.SelectSingleNode("//h2[contains(@class, 'headline-sm')]");
                    if (modelNode != null)
                        model = HtmlEntity.DeEntitize(modelNode.InnerText.Trim());

                    // السعر
                    var priceNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'headline-md') or contains(@class, 'headline-lg')]");
                    if (priceNode != null)
                    {
                        var priceText = priceNode.InnerText.Replace("$", "").Trim();
                        if (decimal.TryParse(priceText, out var parsed))
                            price = parsed;
                    }

                    var scriptNode = doc.DocumentNode.SelectSingleNode("//script[contains(text(),'description')]");

                    if (scriptNode != null)
                    {
                        var scriptContent = scriptNode.InnerText;

                        JArray jsonArray = JArray.Parse(scriptContent);

                        JObject product = jsonArray.FirstOrDefault(obj => obj["@type"]?.ToString() == "Product") as JObject;

                        if (product != null)
                        {
                            string productName = product["name"]?.ToString() ?? "";
                            description = product["description"]?.ToString() ?? "";
                            string productModel = product["sku"]?.ToString() ?? "";
                            List<string> productImages = product["image"]?.Select(img => img.ToString()).ToList() ?? new List<string>();
                            decimal productPrice = product["offers"]?[0]?["price"]?.ToObject<decimal>() ?? 0;
                            string currency = product["offers"]?[0]?["priceCurrency"]?.ToString() ?? "";

                        }
                    }



                    // الصور
                    var imageNodes = doc.DocumentNode.SelectNodes("//div[contains(@class,'image-box')]//img | //img[contains(@src,'ryobitools.com')]");
                    if (imageNodes != null)
                    {
                        imgs = imageNodes
                            .Select(i => i.GetAttributeValue("src", ""))
                            .Where(src => !string.IsNullOrEmpty(src))
                            .Select(src => src.StartsWith("http") ? src : "https://www.ryobitools.com" + src)
                            .Distinct()
                            .ToList();
                    }
                }

                return Ok(new
                {
                    Title = productTitle,
                    Model = model,
                    Brand = brand,
                    Price = price,
                    Upc = upc,
                    Internet = internet,
                    images = imgs,
                    desc = description,
                    Source = source,
                    qty = 0,
                    height = h,
                    wedth = w,
                    length,
                    SKU = storeSku,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }



        //[HttpPost("ByLowesOxylabs/{productId}")]
        //public async Task<IActionResult> ByLowesOxylabs(string productId)
        //{
        //    string source = "Lowes";
        //    string storeSku = string.Empty;
        //    List<string> imgs = new List<string>();
        //    decimal price = 0;
        //    string model = string.Empty;
        //    string upc = string.Empty;
        //    string brand = string.Empty;
        //    string productTitle = string.Empty;
        //    List<string> description = new List<string>();

        //    try
        //    {
        //        using (var http = new HttpClient())
        //        {
        //            http.BaseAddress = new Uri("https://realtime.oxylabs.io/");
        //            var auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("saifaldin_k7eHA:FkK_FM+Gwi9_esg"));
        //            http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", auth);
        //            http.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        //            string url = $"https://www.lowes.com/pd/Moen-Georgene-Spot-Resist-Stainless-1-Handle-Deck-Mount-Pull-Down-Handle-Lever-Commercial-Residential-Kitchen-Faucet/1000957192";
        //            var payload = new
        //            {
        //                source = "universal",
        //                url = url
        //            };
        //            var json = System.Text.Json.JsonSerializer.Serialize(payload);
        //            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        //            var response = await http.PostAsync("v1/queries", content);
        //            response.EnsureSuccessStatusCode();
        //            var respBody = await response.Content.ReadAsStringAsync();

        //            return Ok(new { rawResult = respBody });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { error = ex.Message });
        //    }
        //}

        // lowes file
        [HttpPost("ParseLowesHtml")]
        public async Task<IActionResult> ParseLowesHtml(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            try
            {
                string htmlContent;
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    htmlContent = await reader.ReadToEndAsync();
                }

                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(htmlContent);

                var scriptNode = htmlDoc.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']");
                List<string> images = new List<string>();
                string productTitle = "not found";
                string description = "";
                decimal price = 0;
                string brand = "not found";
                string height = "";
                string width = "";
                string weight = "";
                string model = string.Empty;
                string sku = string.Empty;
                string item = string.Empty;
                string upc = "Does not Apply";
                int qty = 0;

                if (scriptNode != null)
                {
                    var jsonText = scriptNode.InnerText;
                    try
                    {
                        var jsonArray = JArray.Parse(jsonText);
                        var product = jsonArray.FirstOrDefault(obj => obj["@type"]?.ToString() == "Product");
                        if (product != null)
                        {
                            productTitle = product["name"]?.ToString() ?? productTitle;
                            description = product["description"]?.ToString() ?? description;
                            brand = product["brand"]?["name"]?.ToString() ?? brand;
                            decimal.TryParse(product["offers"]?["price"]?.ToString(), out price);
                            height = product["height"]?.ToString() ?? "10";
                            width = product["width"]?.ToString() ?? "10";
                            weight = product["weight"]?.ToString() ?? "10";
                            images = product["image"]?
                                .Select(i => i.ToString())
                                .Where(s => !string.IsNullOrEmpty(s) && s.Contains("lowes.com"))
                                .Distinct()
                                .ToList() ?? new List<string>();

                            sku = product["sku"]?.ToString();
                            var itemNode = htmlDoc.DocumentNode
                                .SelectSingleNode("//*[contains(text(),'Item #')]");

                            if (itemNode != null)
                            {
                                item = itemNode.InnerText.Trim().Replace("Item #", "").Replace(" |", "");
                            }

                            var modelNode = htmlDoc.DocumentNode
                                .SelectSingleNode("//*[contains(text(),'Model #')]");

                            if (modelNode != null)
                            {
                                model = modelNode.InnerText.Trim().Replace("Model #", "");
                            }
                        }
                    }
                    catch { }
                }

                if (images.Count == 0)
                {
                    var imgNodes = htmlDoc.DocumentNode.SelectNodes("//div[@data-testid='image-container']//img");
                    if (imgNodes != null)
                    {
                        images = imgNodes
                            .Select(i => i.GetAttributeValue("src", i.GetAttributeValue("data-src", "")))
                            .Where(s => !string.IsNullOrEmpty(s) && s.Contains("lowes.com"))
                            .Distinct()
                            .ToList();
                    }
                }

                return Ok(new
                {
                    images = images,
                    qty = qty,
                    price = price,
                    height = height,
                    wedth = width,
                    length = weight,
                    desc = description,
                    Model = model,
                    Internet = item,
                    SKU = sku,
                    Brand = brand,
                    Upc = upc,
                    Title = productTitle,
                    Source = "Lowes DB",
                });


            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        // By Build
        [HttpPost("ByBuild/{wordSearch}")]
        public async Task<IActionResult> ByBuild(string wordSearch)
        {
            List<string> imgs = new List<string>();
            decimal price = 0;
            string upc = string.Empty;
            string brand = string.Empty;
            string internet = string.Empty;
            string model = string.Empty;
            string storeSku = string.Empty;
            string productTitle = string.Empty;
            string source = string.Empty;
            string descc = string.Empty;
            string length = "10";
            string h = "10";
            string w = "10";
            List<string> description = new List<string>();
            try
            {
                var options = new ChromeOptions();
                //options.AddArgument("--headless");
                using (var driver = new ChromeDriver(options))
                {
                    var url1 = $"https://www.build.com/search?term={wordSearch}";
                    driver.Navigate().GoToUrl(url1);

                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

                    var firstCard = wait.Until(d => d.FindElement(By.CssSelector("div[data-automation^='product-card-']")));

                    var linkElement = firstCard.FindElement(By.CssSelector("a"));
                    var productUrl = linkElement.GetAttribute("href");

                    driver.Navigate().GoToUrl(productUrl);

                    wait.Until(d => d.FindElement(By.CssSelector("h1")));

                    var titleElement = driver.FindElement(By.CssSelector("h1"));
                    productTitle = titleElement?.Text.Trim() ?? "not found";


                    var specsElement = driver.FindElement(By.CssSelector("div.lh-copy.H_oFW"));



                    if (specsElement != null)
                    {
                        descc = specsElement.Text.Trim().Replace("\r\n", " ");
                        h = Regex.Match(descc, @"Height:\s*([\d\-\/]+)""").Groups[1].Value;
                        w = Regex.Match(descc, @"Width:\s*([\d\-\/]+)""").Groups[1].Value;
                        length = Regex.Match(descc, @"Extension:\s*([\d\-\/]+)""").Groups[1].Value;
                    }

                    model = "not found";
                    try
                    {
                        var modelElement = driver.FindElement(By.CssSelector("span[data-automation='product-model-number']"));
                        model = modelElement?.Text.Trim() ?? "not found";
                    }
                    catch
                    {
                        model = "not found";
                    }



                    price = 0;
                    try
                    {
                        var priceText = driver.FindElement(By.CssSelector("span[data-automation='price']")).Text;
                        priceText = priceText.Replace("$", "").Replace(",", "").Trim();
                        decimal.TryParse(priceText, out price);
                    }
                    catch
                    {
                        price = 0;
                    }


                    imgs = [];
                    try
                    {
                        var doc = new HtmlAgilityPack.HtmlDocument();
                        doc.LoadHtml(driver.PageSource);

                        var mainDiv = doc.DocumentNode.SelectSingleNode("//div[@class='zMWwe UqcrF']");

                        if (mainDiv != null)
                        {
                            imgs = mainDiv.Descendants("img")
                                .Select(img => img.GetAttributeValue("src", null))
                                .Where(src => !string.IsNullOrEmpty(src))
                                .ToList();
                        }
                    }
                    catch
                    {
                        imgs = new List<string>();
                    }
                    return Ok(new
                    {
                        images = imgs,
                        height = h,
                        wedth = w,
                        length = length,
                        details = descc,
                        price = price,
                        Model = model,
                        Internet = "",
                        SKU = storeSku,
                        Brand = brand,
                        Title = productTitle,
                        Source = "Build DB",
                        desc = description
                    });

                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }


        // by milwaukee
        [HttpPost("Bymilwaukeetool/{wordSearch}")]
        public async Task<IActionResult> Bymilwaukeetool(string wordSearch)
        {
            List<string> imgs = new();
            decimal price = 0;
            string upc = "Does not Apply";
            string brand = "Does not Apply";
            string internet = string.Empty;
            string model = "not found";
            string storeSku = string.Empty;
            string productTitle = string.Empty;
            string descc = string.Empty;
            string length = "10";
            string h = "10";
            string w = "10";
            string dimensionsText = string.Empty;
            int qty = 0;
            List<string> description = new();

            try
            {
                var options = new ChromeOptions();
                // options.AddArgument("--headless");

                using (var driver = new ChromeDriver(options))
                {

                    var url1 = $"https://www.milwaukeetool.com/search?q={wordSearch}&first=0";
                    driver.Navigate().GoToUrl(url1);

                    var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => ((IJavaScriptExecutor)d)
                        .ExecuteScript("return document.readyState").ToString() == "complete");

                    var container = driver.FindElement(By.CssSelector("div.flex.flex-wrap.gap-5.w-full.justify-center"));

                    IWebElement firstCard = null;

                    try
                    {
                        firstCard = container.FindElement(By.CssSelector("a")); ;

                    }
                    catch
                    {
                        return BadRequest(new { msg = "لم يتم إيجاد نتيجة بحث على Milwaukee" });
                    }

                    string productUrl = firstCard.GetAttribute("href");

                    driver.Navigate().GoToUrl(productUrl);

                    wait.Until(d => ((IJavaScriptExecutor)d)
                        .ExecuteScript("return document.readyState").ToString() == "complete");


                    string pageContent = driver.PageSource;

                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(pageContent);

                    try
                    {
                        var titleElement = driver.FindElement(By.XPath("//h1[contains(@class,'product-title')]/span"));
                        productTitle = titleElement.Text.Trim();

                        try
                        {
                            var modelElement = driver.FindElement(By.CssSelector("div.product-title > span"));
                            model = modelElement.Text.Trim();
                        }
                        catch
                        {
                            model = wordSearch;
                        }
                    }
                    catch
                    {
                        try
                        {
                            var titleElement2 = driver.FindElement(By.CssSelector("div.product-title h1 span"));
                            productTitle = titleElement2.Text.Trim();

                            try
                            {
                                var modelElement2 = driver.FindElement(By.CssSelector("div.product-title > span"));
                                model = modelElement2.Text.Trim();
                            }
                            catch { }
                        }
                        catch
                        {
                            // لم يتم العثور على اسم المنتج
                            productTitle = "";
                            model = "";
                        }
                    }

                    string productDescription = "";

                    // أولًا، البحث في HTML الظاهر
                    var descNode = doc.DocumentNode
                        .SelectSingleNode("//div[contains(@class, 'product-description')]") ??
                        doc.DocumentNode.SelectSingleNode("//section[contains(@class, 'product-overview')]") ??
                        doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'description')]") ??
                        doc.DocumentNode.SelectSingleNode("//div[@id='productDetails']") ??
                        doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'leading-normal')]");

                    if (descNode != null)
                    {
                        // إذا وجدنا الوصف في HTML
                        productDescription = HtmlEntity.DeEntitize(descNode.InnerText.Trim());
                    }
                    else
                    {
                        // إذا لم يوجد، البحث داخل سكربت self.__next_f.push
                        var scriptNode = doc.DocumentNode
                            .SelectSingleNode("//script[contains(text(), 'self.__next_f.push')]");

                        if (scriptNode != null)
                        {
                            var scriptText = scriptNode.InnerText;

                            // البحث عن pattern الوصف داخل النص
                            var match = Regex.Match(scriptText, "\"description\":\"([^\"]+)\"");
                            if (match.Success)
                            {
                                productDescription = System.Net.WebUtility.HtmlDecode(match.Groups[1].Value);
                            }
                        }
                    }

                    // الصور
                    var productImages = doc.DocumentNode
                        .SelectNodes("//img[contains(@src, '/web-images/sc/')]")
                        ?.Select(img => img.GetAttributeValue("src", null))
                        .Where(src => !string.IsNullOrEmpty(src))
                        .Distinct()
                        .Select(src => src.StartsWith("https") ? src : "https://www.milwaukeetool.com" + src)
                        .ToList() ?? new List<string>();

                    // نحافظ على نفس البنية تمامًا
                    return Ok(new
                    {
                        title = productTitle,
                        desc = productDescription,
                        images = productImages,
                        Model = model,
                        price = 0,
                        brand = "Does not Apply",
                        source = "Milwaukeetool",
                        Upc = upc,
                        height = h,
                        width = w,
                        length,
                        Internet = internet,
                        Dimention = dimensionsText,
                        SKU = storeSku,
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }




        // homeDepot file
        [HttpPost("HomeDepotFileHtmlAnalyse")]
        public async Task<IActionResult> HomeDepotFileHtmlAnalyse(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or missing.");

            string htmlContent;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                htmlContent = await reader.ReadToEndAsync();
            }

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            List<string> imgs = new();
            decimal price = 0;
            string upc = "Does not Apply";
            string brand = "Does not Apply";
            string internet = string.Empty;
            string model = "not found";
            string storeSku = string.Empty;
            string productTitle = string.Empty;
            string descc = string.Empty;
            string length = "10";
            string h = "10";
            string w = "10";
            string dimensionsText = string.Empty;
            int qty = 0;
            List<string> description = new();

            try
            {
                var imageNodes = document.DocumentNode
                 .SelectNodes("//img[contains(@src,'images.thdstatic.com/productImages/')]");

                if (imageNodes != null)
                {
                    imgs = imageNodes
                        .Select(x => x.GetAttributeValue("src", ""))
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x.Replace("_100.jpg", "_1000.jpg"))
                        .Distinct()
                        .ToList();
                }

                var imageNodesSmall = document.DocumentNode
                    .SelectNodes("//div[contains(@class,'mediagallery__thumbnailImageFit')]//img");

                if (imageNodesSmall != null)
                {
                    var final = imageNodesSmall.Select(img => img.GetAttributeValue("src", "")).ToList();
                    imgs.AddRange(final);
                    imgs = imgs.Select(img => img.Replace("100.jpg", "600.jpg")).ToList();
                }
                else
                {
                    var scriptNode0 = document.DocumentNode
                        .SelectSingleNode("//script[@type='application/ld+json' and contains(., 'Product')]");

                    if (scriptNode0 != null)
                    {
                        var jsonText = scriptNode0.InnerText;
                        var matches = Regex.Matches(jsonText, "\"https://images\\.thdstatic\\.com/productImages/[^\"]+?\\.jpg\"");
                        var final = matches.Cast<Match>().Select(m => m.Value.Trim('"')).ToList();
                        imgs.AddRange(final);
                        imgs = imgs.Select(img => img.Replace("100.jpg", "600.jpg")).ToList();
                    }
                }

                // storeSkuNumber
                var scriptNode = document.DocumentNode.SelectSingleNode("//script[contains(text(), 'storeSkuNumber')]");
                var scriptText = scriptNode?.InnerText ?? "not found";
                storeSku = Regex.Match(scriptText, "\"storeSkuNumber\"\\s*:\\s*\"(\\d+)\"").Groups[1].Value;
                if (string.IsNullOrEmpty(storeSku)) storeSku = "not found";
                if (storeSku == "not found")
                {
                    storeSku = document.DocumentNode
                        .SelectSingleNode("//h2[contains(text(),'Store SKU')]//span")
                        ?.InnerText
                        ?.Trim();
                }

                // العنوان
                var titleNode = document.DocumentNode.SelectSingleNode("//h1[contains(@class, 'sui-h4-bold')]");
                productTitle = titleNode?.InnerText.Trim() ?? "not found";

                // الوصف
                var descNode = document.DocumentNode.SelectNodes("//div[@data-fusion-slot='slotF8']//ul//li");
                if (descNode != null)
                    description.AddRange(descNode.Select(i => i.InnerText.Trim()));

                // JSON data
                var jsonNode = document.DocumentNode.SelectSingleNode("//script[@type='application/ld+json']");
                if (jsonNode != null)
                {
                    try
                    {
                        var jsonText = jsonNode.InnerText;
                        var data = JObject.Parse(jsonText);

                        descc = data["description"]?.ToString() ?? "";
                        h = data["height"]?.ToString()?.Replace(" in", "") ?? "";
                        w = data["width"]?.ToString()?.Replace(" in", "") ?? "";
                        length = data["length"]?.ToString()?.Replace(" in", "") ?? "";
                        dimensionsText = $"height: {h} width: {w} depth: {length}";
                    }
                    catch { /* تجاهل أي JSON غير صحيح */ }
                }
               
                // العلامة التجارية
                var brandNode = document.DocumentNode
                    .SelectSingleNode("//div[contains(@data-component,'ProductDetailsBrandCollection')]//h2");
                brand = brandNode?.InnerText.Trim() ?? "Does not Apply";

                descc = "";

                // الحالة الأولى
                var aboutNode = document.DocumentNode.SelectSingleNode(
                    "//h3[contains(.,'About This Product')]/following-sibling::p");

                if (aboutNode != null)
                {
                    descc = HtmlEntity.DeEntitize(aboutNode.InnerText.Trim());
                }

                // الحالة الثانية
                if (string.IsNullOrWhiteSpace(descc))
                {
                    var nodes = document.DocumentNode.SelectNodes(
                        "//div[@data-testid='SalientPoints']//li[not(descendant::a)]");

                    if (nodes != null)
                    {
                        descc = string.Join(" ",
                            nodes.Select(x => HtmlEntity.DeEntitize(x.InnerText.Trim())));
                    }
                }

                // استخراج الاسم من <title> كخيار بديل
                var productName = document.DocumentNode.SelectSingleNode("//title")?.InnerText.Trim() ?? "not found";
                if (brand == "Does not Apply" && productName.Contains(' '))
                    brand = productName.Split(' ')[0];

                // UPC
                var scriptNode2 = document.DocumentNode.SelectSingleNode("//script[contains(text(), 'upc')]");
                if (scriptNode2 != null)
                {
                    var match = Regex.Match(scriptNode2.InnerText, "\"upc\"\\s*:\\s*\"(\\d+)\"");
                    if (match.Success)
                        upc = match.Groups[1].Value ?? "Does not Apply";
                }

                // Internet #
                var internetNode = document.DocumentNode.SelectSingleNode("//h2[contains(text(), 'Internet #')]");
                if (internetNode != null)
                {
                    internet = internetNode.InnerText.Replace("Internet #", "").Trim();
                }
                else
                {
                    var scriptNode1 = document.DocumentNode.SelectSingleNode("//script[contains(text(), 'itemId')]");
                    if (scriptNode1 != null)
                    {
                        var match = Regex.Match(scriptNode1.InnerText, "\"itemId\"\\s*:\\s*\"(\\d+)\"");
                        if (match.Success)
                            internet = match.Groups[1].Value;
                    }
                }

                // الموديل
                var modelNode = document.DocumentNode.SelectSingleNode("//h2[contains(text(), 'Model')]/span");
                model = modelNode?.InnerText.Trim() ?? "not found";

                // السعر
                var priceNode = document.DocumentNode.SelectSingleNode("//span[contains(@class, 'sui-text-9xl')]");
                if (priceNode != null)
                {
                    var priceText = priceNode.InnerText.Trim();
                    if (decimal.TryParse(priceText.Replace("$", "").Replace(",", ""), out decimal parsedPrice))
                        price = parsedPrice;
                }

                var specRows = document.DocumentNode.SelectNodes("//section[@id='product-section-specifications']//tr");

                string height = "";
                string width = "";
                string depth = "";
                string weight = "";

                if (specRows != null)
                {
                    foreach (var row in specRows)
                    {
                        var keyNode = row.SelectSingleNode(".//th//p");
                        var valueNode = row.SelectSingleNode(".//td//p");

                        if (keyNode == null || valueNode == null)
                            continue;

                        var key = keyNode.InnerText.Trim().ToLower();
                        var value = valueNode.InnerText.Trim();

                        if (key.Contains("height"))
                            height = value.Replace(" in", "");

                        else if (key.Contains("width"))
                            width = value.Replace(" in", "");

                        else if (key.Contains("depth"))
                            depth = value.Replace(" in", "");

                        else if (key.Contains("weight"))
                            weight = value.Replace(" lb", "");
                    }
                }

                if (brand.Contains("Shop "))
                {
                    brand.Replace("Shop ", "");
                }

                // النتيجة
                return Ok(new
                {
                    images = imgs.Count > 12 ? imgs.Slice(0, 12).Distinct().ToList() : imgs.ToList(),
                    price,
                    Upc = upc,
                    height = h ?? height,
                    width = w ?? width,
                    length = length ?? depth,
                    weight = weight ?? weight,
                    desc = descc,
                    Model = model,
                    Internet = internet,
                    Dimention = dimensionsText,
                    SKU = storeSku,
                    Brand = brand.Replace("Shop ", ""),
                    Title = productTitle,
                    Source = "Home Depot File",
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error parsing HTML file", details = ex.Message });
            }
        }

        [HttpPost("UploadImages")]
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
                return BadRequest("لم يتم إرسال صورة.");

            var files = new List<string>();

            //var account = new Account(cloudName, apiKey, apiSecret);
            //var cloudinary = new Cloudinary(account);

            foreach (var item in images)
            {
                using var stream = item.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(item.FileName, stream),
                    Folder = "Scrape_uploads",
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                files.Add(uploadResult.SecureUrl?.ToString());
            }

            return Ok(files);
        }

    }
}

public static class StringExtensions
{
    public static string CleanText(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return "";

        return Regex
            .Replace(
                text,
                @"\s+",
                " "
            )
            .Trim();
    }
}
