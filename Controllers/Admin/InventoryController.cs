using System.IO.Compression;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        UniversalDbContext dbContext;
        private readonly IConfiguration configuration;
        private Cloudinary cloudinary;
        string cloudName;
        string apiKey;
        string apiSecret;

        public InventoryController(UniversalDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            cloudinary = new Cloudinary(new Account(
                 configuration["Claoudinary:cloudName"],
                 configuration["Claoudinary:apiKey"],
                 configuration["Claoudinary:apiSecret"]
             ));
        }

        [HttpGet("GetAllInventory/{businessId}")]
        public async Task<IActionResult> GetAllInventory(int businessId)
        {
            var business = await dbContext.Businesses.FindAsync(businessId);
            if (business == null)
                return NotFound();

            var query = dbContext.Inventories
                .Include(i => i.ItemCondition)
                .Include(i => i.platform)
                .Include(i => i.Size)
                .Include(i => i.Category)
                .Include(i => i.Item)
                    .ThenInclude(i => i.Images)
                .Where(i => !i.IsSold);

            if (!business.Role.IsNullOrEmpty() && !business.Role.Contains("Manager"))
                query = query.Where(i => i.Business_id == businessId);

            return Ok(await query.OrderByDescending(i => i.Insert_on).ToListAsync());
        }


        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetAll(int businessId)
        {
            var business = await dbContext.Businesses.FindAsync(businessId);
            if (business == null)
                return NotFound();

            var query = dbContext.Inventories
                .Include(i => i.ItemCondition)
                .Include(i => i.platform)
                .Include(i => i.Size)
                .Include(i => i.Category)
                .Include(i => i.Item)
                    .ThenInclude(i => i.Images)
                .Where(i => i.Item.CategoryId == null || i.ItemCondition == null || i.SitePrice == null)
                .Where(i => i.Status == null)
                .Where(i => !i.IsSold);

            if (!business.Role.IsNullOrEmpty() && !business.Role.Contains("Manager"))
                query = query.Where(i => i.Business_id == businessId);

            return Ok(await query.OrderByDescending(i => i.Insert_on).ToListAsync());
        }

        [HttpGet("GetAllForImagerApp/{businessId}")]
        public async Task<IActionResult> GetAllForImagerApp(int businessId)
        {
            var business = await dbContext.Businesses
                .FindAsync(businessId);

            if (business == null)
                return NotFound();

            var query = dbContext.Inventories
                .Include(i => i.ItemCondition)
                .Include(i => i.platform)
                .Include(i => i.Size)
                .Include(i => i.Category)
                .Include(i => i.Item)
                    .ThenInclude(i => i.Images)
                .Where(i => !i.IsSold);

            if (!string.IsNullOrEmpty(business.Role) &&
                !business.Role.Contains("Manager"))
            {
                query = query.Where(i => i.Business_id == businessId);
            }

            var inventories = await query
                .OrderByDescending(i => i.Insert_on)
                .ToListAsync();

            return Ok(inventories);
        }


        [HttpGet("GetAllWhereReadyToPublish/{businessId}")]
        public async Task<IActionResult> GetAllWhereReadyToPublish(int businessId)
        {
            var business = await dbContext.Businesses.FindAsync(businessId);
            if (business == null)
                return NotFound();

            var query = dbContext.Inventories
                .Include(i => i.ItemCondition)
                .Include(i => i.platform)
                .Include(i => i.Size)
                .Include(i => i.Category)
                .Include(i => i.Item)
                    .ThenInclude(i => i.Images)
                .Where(i => !i.IsSold)
                .Where(i => i.Item.CategoryId != null)
                .Where(i => i.ItemConditionId != null)
                .Where(i => i.SitePrice != null);

            if (!business.Role.IsNullOrEmpty() && !business.Role.Contains("Manager"))
                query = query.Where(i => i.Business_id == businessId);

            return Ok(await query.OrderByDescending(i => i.Insert_on).ToListAsync());
        }

        [HttpGet("GetAllActiveList/{businessId}")]
        public async Task<IActionResult> GetAllActiveList(int businessId)
        {
            var business = await dbContext.Businesses.FindAsync(businessId);
            if (business == null)
                return NotFound();

            var query = dbContext.Inventories
                .Include(i => i.ItemCondition)
                .Include(i => i.platform)
                .Include(i => i.Size)
                .Include(i => i.Category)
                .Include(i => i.Item).ThenInclude(i => i.Images)
                .Where(i => i.Status.Contains("published"))
                .Where(i => i.IsProccessedInInventory)
                .Where(i => !i.IsSold);

            if (!business.Role.IsNullOrEmpty() && !business.Role.Contains("Manager"))
                query = query.Where(i => i.Business_id == businessId);

            return Ok(await query.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAll1()
        {
            var inventory = await dbContext.Inventories
                .Include(i => i.platform).Include(i => i.Size).Include(i => i.Category)
                .Include(i => i.Item).ThenInclude(i => i.Images)
                .Where(i => i.Category_id != null)
                .ToListAsync();
            return Ok(inventory);
        }

        [HttpGet("GetProcesedItems/{businessId}")]
        public async Task<IActionResult> GetProcesedItems(int businessId)
        {
            var inventory = await dbContext.Inventories
                .Include(i => i.platform).Include(i => i.Size)
                .Include(i => i.Item).ThenInclude(i => i.Images)
                .Where(i => i.Business_id == businessId)
                .Where(i => i.Item.Images!.Count > 1)
                .ToListAsync();
            return Ok(inventory);
        }


        // ============================================================
        // 2) Endpoint رفع الصور - يرجع روابط الصور بعد الحفظ على السيرفر
        //    عدّل منطق الحفظ الفعلي (local disk / S3 / Azure Blob...) حسب مشروعك
        // ============================================================
        // ============================================================
        [HttpPost("UploadImagesToCloudinary")]
        public async Task<IActionResult> UploadImagesToCloudinary([FromForm] ImagesForm model)
        {
            if (model == null || model.Images == null || !model.Images.Any())
                return BadRequest("No images provided");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var urls = new List<string>();

            foreach (var image in model.Images)
            {
                using var stream = image.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(image.FileName, stream),
                    Folder = "Inventory" // عدّل اسم الفولدر حسب المشروع
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult == null || uploadResult.SecureUrl == null)
                    return StatusCode(500, "Upload failed");

                urls.Add(uploadResult.SecureUrl.ToString());
            }

            return Ok(urls);
        }

        // ============================================================
        // 3) Endpoint تحديث بيانات العنصر (Item + Inventory)
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInventoryItem(int id, [FromBody] UpdateInventoryDto dto)
        {
            var invRec = await dbContext.Inventories
                .Include(i => i.Item)
                    .ThenInclude(it => it.Images) // عدّل اسم الـ navigation property حسب الـ Entity الفعلي عندك
                .Where(i => i.Inventory_id == id)
                .FirstOrDefaultAsync();

            if (invRec == null) return NotFound();

            // -------- Category / Condition --------
            if (dto.CategoryId.HasValue)
                invRec.Item.CategoryId = dto.CategoryId;

            if (dto.ItemConditionId.HasValue)
                invRec.ItemConditionId = dto.ItemConditionId;

            // -------- الوصف / التفاصيل --------
            // عدّل أسماء الأعمدة هذي لو مختلفة بالـ Entity عندك
            if (dto.Description != null)
                invRec.Item.Description = dto.Description;

            if (dto.Details != null)
                invRec.Item.ItemDetails = dto.Details;

            // -------- الأسعار --------
            if (dto.ItemPrice.HasValue)
                invRec.Item.BasePrice = dto.ItemPrice.Value; // عدّل لو العمود decimal مو string

            if (dto.WarehousePrice.HasValue)
                invRec.SitePrice = dto.WarehousePrice.ToString(); // ⚠️ لازم تضيف هذا العمود لو مو موجود بالجدول

            if (dto.qty.HasValue)
                invRec.QtyPublished = dto.qty; 

            // -------- الصور الجديدة --------
            if (dto.ImageUrls != null && dto.ImageUrls.Any())
            {
                foreach (var url in dto.ImageUrls)
                {
                    invRec.Item.Images.Add(new ItemImage
                    {
                        ImageUrl = url,
                        IsMain = false,
                        IsPublishedOnEbay = false,
                    });
                }
            }

            await dbContext.SaveChangesAsync();

            return Ok(invRec);
        }


        [HttpPut("AddNewInvManual")]
        public async Task<IActionResult> AddNewInvManual(int id, [FromBody] ManualInventory model)
        {
            var existInventory = await dbContext.Inventories.Where(i => i.ItemId == model.ItemId).FirstOrDefaultAsync();

            if (existInventory != null)
            {
                existInventory.QtyInInventory += model.Qty;
                if (existInventory.Status == "Sold")
                {
                    existInventory.Status = existInventory.OldStatus;
                    existInventory.OldStatus = "";
                }

                await dbContext.SaveChangesAsync();
            }
            else
            {
                var inventory = new Inventory
                {
                    ItemId = model.ItemId,
                    Size_id = model.SizeId,
                    Platform_id = model.PlatformId,
                    Business_id = model.BusinessId,
                    Insert_on = DateTime.UtcNow,
                    QtyInInventory = model.Qty,
                    ItemConditionId = model.conditionId,
                    Category_id = model.CategoryId,
                };
                await dbContext.Inventories.AddAsync(inventory);
                await dbContext.SaveChangesAsync();
            }

            return Ok(new
            {
                msg = "Inv added successfuly ..."
            });

        }


        [HttpPut("AddPriceTitleToInv/{id}")]
        public async Task<IActionResult> AddPriceTitleToInv(int id, [FromBody] PriceTitle model)
        {
            var invRec = await dbContext.Inventories.FindAsync(id);
            if (invRec == null) return NotFound();

            invRec.SitePrice = model.Price;
            invRec.Product_description = model.Title;
            invRec.Category_id = model.CategoryId;
            invRec.SitePrice = model?.Price;
            await dbContext.SaveChangesAsync();
            return Ok(invRec);

        }

        [HttpPut("AddCategoryAndSizeToInv/{id}")]
        public async Task<IActionResult> AddCategoryAndSizeToInv(int id, [FromBody] CategoryAndSize model)
        {
            int? finalBrandId = null;
            if (model == null)
                return BadRequest("Invalid data.");

            if (!model.Brand.IsNullOrEmpty())
            {
                var brand = await dbContext.CarBrands.Where(b => b.CarBrandName.Trim() == model.Brand.Trim()).FirstOrDefaultAsync();
                if (brand == null)
                {
                    var newBrand = new CarBrand
                    {
                        CarBrandName = model.Brand,
                    };
                    await dbContext.CarBrands.AddAsync(newBrand);
                    await dbContext.SaveChangesAsync();
                    finalBrandId = newBrand.CarBrandId;
                }
                else
                {
                    finalBrandId = brand.CarBrandId;
                }
            }

            var invRec = await dbContext.Inventories
               .Include(i => i.Item)
               .FirstOrDefaultAsync(i => i.Inventory_id == id);

            if (invRec == null)
                return NotFound();

            if (invRec.Item == null)
                return BadRequest("Item not found for this inventory.");

            invRec.Category_id = model.CategoryId;
            invRec.Size_id = model.SizeId;
            invRec.Item.SKU = model.sKU;
            invRec.Item.UPC = model.upc;
            invRec.SitePrice = model?.price;
            invRec.Platform_id = model?.PlatformId;
            invRec.ItemConditionId = model?.ItemCondition;
            invRec.Product_name = model?.Title;
            invRec.Product_description = model?.Description;
            invRec.Item.CarBrandId = finalBrandId;
            invRec.QtyPublished = model?.qty;
            invRec.IsProccessedInInventory = model.IsProccessedInInventory;

            await dbContext.SaveChangesAsync();
            return Ok(invRec);
        }

        [HttpPut("SetRecordsSold")]
        public async Task<IActionResult> SetRecordsSold([FromBody] SoldItems model)
        {
            foreach (var inv in model.soldItems)
            {
                var inventory = await dbContext.Inventories.FindAsync(inv.invId);
                inventory.QtyInInventory -= inv.qty;

                if (inventory.QtyInInventory <= 0)
                {
                    inventory.QtyInInventory = 0;
                    inventory.IsSold = true;
                    inventory.OldStatus = inventory.Status;
                    inventory.Status = "Sold";
                }
                else
                {
                    inventory.QtyInInventory -= inv.qty;
                    inventory.IsSold = false;
                    inventory.OldStatus = inventory.Status;
                    inventory.Status = "Partially Sold";
                }

                await dbContext.SaveChangesAsync();
            }
            return Ok(new
            {
                msg = "inventory updated successfuly..."
            });
        }


        [HttpPut("SetPublishOnMarketPlase/{invId}")]
        public async Task<IActionResult> SetPublishOnMarketPlase(TogglePublishOnMarketPlase model)
        {
            var inventory = await dbContext.Inventories.FindAsync(model.id);
            if (inventory == null) return BadRequest("inv not found");

            inventory.IsProccessedInInventory = true;
            inventory.MarketPlaceOfferUrl = model.url;


            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                status = inventory.IsPublishedOnMarketPlace
            });
        }


        [HttpPut("ChangeQty/{invId}/{qty}")]
        public async Task<IActionResult> ChangeQty(int invId, int qty)
        {
            var inventory = await dbContext.Inventories.FindAsync(invId);
            if (inventory == null) return BadRequest("inv not found");

            inventory.QtyPublished += qty;

            await dbContext.SaveChangesAsync();

            return Ok(inventory);
        }

        [HttpPut("DecreaseQty/{invId}")]
        public async Task<IActionResult> DecreaseQty(int invId)
        {
            var inventory = await dbContext.Inventories.FindAsync(invId);
            if (inventory == null) return BadRequest("inv not found");

            if (inventory.QtyInInventory > 0)
                inventory.QtyInInventory--;

            else
            {

                inventory.OldStatus = inventory.Status;
                inventory.Status = "Sold";
            }

            await dbContext.SaveChangesAsync();

            return Ok(inventory);
        }

        [HttpDelete("{invId}")]
        public async Task<IActionResult> DeleteInventory(int invId)
        {
            var inventory = await dbContext.Inventories.FindAsync(invId);
            if (inventory == null) return BadRequest("inv not found");
            dbContext.Inventories.Remove(inventory);
            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                msg = "inv deleted successfuly..."
            });

        }

        [HttpPost("download-images")]
        public async Task<IActionResult> DownloadImages(
        [FromBody] DownloadImagesRequest request)
        {
            using var memoryStream = new MemoryStream();

            using (var archive =
                   new ZipArchive(
                       memoryStream,
                       ZipArchiveMode.Create,
                       true))
            {
                using var client = new HttpClient();

                for (int i = 0; i < request.Urls.Count; i++)
                {
                    try
                    {
                        var bytes =
                          await client.GetByteArrayAsync(
                              request.Urls[i]);

                        var extension =
                            Path.GetExtension(
                                new Uri(
                                   request.Urls[i]
                                ).AbsolutePath
                            );

                        if (string.IsNullOrEmpty(extension))
                        {
                            extension = ".jpg";
                        }

                        var entry =
                            archive.CreateEntry(
                                $"{i + 1}{extension}"
                            );

                        using var entryStream =
                            entry.Open();

                        await entryStream.WriteAsync(bytes);
                    }
                    catch
                    {

                    }
                }
            }

            memoryStream.Position = 0;

            return File(
              memoryStream.ToArray(),
              "application/zip",
              $"{request.FolderName}.zip"
            );
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] ToInventory request)
        {
            var inv = await dbContext.Inventories.Where(inv => inv.ItemId == request.ItemId && inv.ItemConditionId == request.ConditionId).FirstOrDefaultAsync();
            if(inv != null)
            {
                inv.QtyPublished = request.Qty;
                await dbContext.SaveChangesAsync();
                return Ok(inv);
            }

            var newInv = new Inventory
            {
                ItemId = request.ItemId,
                QtyInInventory = request.Qty,
                SitePrice = request.Price,
                ItemConditionId = request.ConditionId,
                Business_id = request.BusinessId,
                IsProccessedInInventory = true
            };

            await dbContext.Inventories.AddAsync(newInv);
            await dbContext.SaveChangesAsync();

            return Ok(newInv);
        }


        [HttpGet("PublishedPlatforms/{invId}")]
        public async Task<IActionResult> PublishedPlatforms(int invId)
        {
            var platforms = await dbContext.Inventory_Published_Items
                .Where(p => p.Inventory_id == invId)
                .GroupBy(p => p.Platform_id)
                .Select(g => new
                {
                    platformId = g.Key,
                    publishedOn = g.Max(x => x.PublishedOn)
                })
                .ToListAsync();

            return Ok(platforms);
        }





        [HttpGet("GetSellingPlatforms/{businessId}")]
        public async Task<IActionResult> GetSellingPlatforms(int businessId)
        {
            var platforms = await dbContext.BusinessPlatforms
                .Where(bp => bp.Business_id == businessId
                          && bp.ForSelling != false
                          && bp.Platform != null)
                .Select(bp => new
                {
                    platform_id = bp.Platform_id,
                    description = bp.Platform!.Description,
                    logoUrl = bp.Platform.LogoUrl
                })
                .Distinct()
                .ToListAsync();

            return Ok(platforms);
        }
        
    }
}

public class SoldItems
{
    public List<OneSoldItems> soldItems { get; set; }
}

public class OneSoldItems
{
    public int invId { get; set; }
    public int qty { get; set; }
}

public class PriceTitle
{
    public string Price { get; set; }
    public string Title { get; set; }
    public int? CategoryId { get; set; }
}

public class CategoryAndSize
{
    public int? CategoryId { get; set; }
    public int? SizeId { get; set; }
    public string? sKU { get; set; }
    public string? upc { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? price { get; set; }
    public string? Brand { get; set; }
    public int? PlatformId { get; set; }
    public int? ItemCondition { get; set; }
    public int? qty { get; set; }
    public bool IsProccessedInInventory { get; set; }


}

public class DownloadImagesRequest
{
    public List<string> Urls { get; set; } = [];

    public string FolderName { get; set; } = "images";
}

public class TogglePublishOnMarketPlase
{
    public int id { get; set; }
    public string url { get; set; }
}

public class ManualInventory
{
    public int ItemId { get; set; }
    public int? SizeId { get; set; }
    public int? conditionId { get; set; }
    public int? PlatformId { get; set; }
    public int? BusinessId { get; set; }
    public int? CategoryId { get; set; }
    public int? Qty { get; set; }
    public decimal? InvPrice { get; set; }
}

public class ToInventory
{
    public int? ItemId { get; set; }
    public int? ConditionId { get; set; }
    public string? Price { get; set; }
    public int? Qty { get; set; }
    public int? BusinessId { get; set; }
}

public class UpdateInventoryDto
{
    public int? CategoryId { get; set; }
    public int? ItemConditionId { get; set; }
    public int? qty { get; set; }
    public string? Description { get; set; }
    public string? Details { get; set; }

    public decimal? ItemPrice { get; set; }       // سعر الايتم
    public decimal? WarehousePrice { get; set; }  // سعر المخزن
    public List<string>? ImageUrls { get; set; }  // روابط الصور بعد الرفع
}

public class ImagesForm
{
    public List<IFormFile> Images { get; set; }
}