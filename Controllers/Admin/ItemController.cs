using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;
using AinAlfahd.Data;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        UniversalDbContext db;
        MasterDBContext masterDBContext;
        private readonly IConfiguration configuration;
        private Cloudinary cloudinary;
        private readonly UserManager<IdentityUserData> _userManager;
        string cloudName;
        string apiKey;
        string apiSecret;
        public ItemController(UniversalDbContext db, IConfiguration configuration, MasterDBContext masterDBContext, UserManager<IdentityUserData> userManager)
        {
            this.db = db;
            this.configuration = configuration;
            cloudinary = new Cloudinary(new Account(
                configuration["Claoudinary:cloudName"],
                configuration["Claoudinary:apiKey"],
                configuration["Claoudinary:apiSecret"]
            ));
            this.masterDBContext = masterDBContext;
            _userManager = userManager;
        }

        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetAll(int businessId)
        {
            var business = await db.Businesses.FindAsync(businessId);

            if (business == null)
                return NotFound("Business not found");

            IQueryable<Item> query = db.Items
                .Include(i => i.Images)
                .Include(i => i.Platform)
                    .ThenInclude(p => p.BusinessPlatforms)
                    .ThenInclude(bp => bp.Business)
                .Include(i => i.Unit)
                .Include(i => i.Color)
                .Include(i => i.CarBrand)
                .Include(i => i.Category)
                .Include(i => i.Currency)
                .Include(i => i.BusinessItems);

            if (!business.Role.Contains("Manager"))
            {
                query = query
                    .Where(i => i.Platform.BusinessPlatforms
                        .Any(bp => bp.Business_id == businessId))
                    .Where(i => i.IsComplated == true);
            }

            var items = await query.Where(i => i.PlatformId != null).ToListAsync();

            var itemIds = items.Select(i => i.ItemId).ToList();

            var stockItemIds = await db.Inventories
                .Where(x => itemIds.Contains(x.ItemId ?? 0))
                .Select(x => x.ItemId)
                .Distinct()
                .ToListAsync();

            var response = items.Select(i => new ItemResponse
            {
                ItemId = i.ItemId,

                ItemDescription = i.ItemDescription,
                ItemDetails = i.ItemDetails,
                SKU = i.SKU,
                InternetId = i.InternetId,
                UPC = i.UPC,
                ShortCode = i.ShortCode,
                Model = i.Model,

                PlatformId = i.PlatformId,
                Platform = i.Platform,

                Height = i.Height,
                Width = i.Width,
                Length = i.Length,

                UnitId = i.UnitId,
                Unit = i.Unit,
                UnitValue = i.UnitValue,

                ColorId = i.ColorId,
                Color = i.Color,

                CarBrandId = i.CarBrandId,
                CarBrand = i.CarBrand,

                Description = i.Description,
                Weight = i.Weight,
                Internet = i.Internet,

                SizeId = i.SizeId,
                Size = i.Size,

                CategoryId = i.CategoryId,
                Category = i.Category,

                BasePrice = i.BasePrice,

                CurrencyId = i.CurrencyId,
                Currency = i.Currency,

                IsActive = i.IsActive,
                IsScraped = i.IsScraped,
                IsComplated = i.IsComplated,
                CanScrape = i.CanScrape,

                Images = i.Images,
                BusinessItems = i.BusinessItems,

                InsertBy = i.InsertBy,

                VinNumber = i.VinNumber,

                CarYearId = i.CarYearId,
                CarYear = i.CarYear,

                CarModelId = i.CarModelId,
                CarModel = i.CarModel,

                GlobalCustomerId = i.GlobalCustomerId,

                IsInStock = stockItemIds.Contains(i.ItemId)
            }).ToList();

            return Ok(response);
        }

        // ///////////////////////////////////////////

        [HttpGet("GetAllItems/{businessId}")]
        public async Task<IActionResult> GetAllItems(int businessId)
        {
            var business = await db.Businesses.FindAsync(businessId);

            if (business == null)
                return NotFound("Business not found");

            IQueryable<Item> query = db.Items
                .Include(i => i.Images)
                .Include(i => i.Platform)
                    .ThenInclude(p => p.BusinessPlatforms)
                    .ThenInclude(bp => bp.Business)
                .Include(i => i.Unit)
                .Include(i => i.Color)
                .Include(i => i.CarBrand)
                .Include(i => i.Category)
                .Include(i => i.Currency)
                .Include(i => i.BusinessItems);

            if (!business.Role.Contains("Manager"))
            {
                query = query
                    .Where(i => i.Platform.BusinessPlatforms
                        .Any(bp => bp.Business_id == businessId));
                  
            }

            var items = await query.Where(i => i.PlatformId != null).ToListAsync();

            var itemIds = items.Select(i => i.ItemId).ToList();

            var stockItemIds = await db.Inventories
                .Where(x => itemIds.Contains(x.ItemId ?? 0))
                .Select(x => x.ItemId)
                .Distinct()
                .ToListAsync();

            var response = items.Select(i => new ItemResponse
            {
                ItemId = i.ItemId,

                ItemDescription = i.ItemDescription,
                ItemDetails = i.ItemDetails,
                SKU = i.SKU,
                InternetId = i.InternetId,
                UPC = i.UPC,
                ShortCode = i.ShortCode,
                Model = i.Model,

                PlatformId = i.PlatformId,
                Platform = i.Platform,

                Height = i.Height,
                Width = i.Width,
                Length = i.Length,

                UnitId = i.UnitId,
                Unit = i.Unit,
                UnitValue = i.UnitValue,

                ColorId = i.ColorId,
                Color = i.Color,

                CarBrandId = i.CarBrandId,
                CarBrand = i.CarBrand,

                Description = i.Description,
                Weight = i.Weight,
                Internet = i.Internet,

                SizeId = i.SizeId,
                Size = i.Size,

                CategoryId = i.CategoryId,
                Category = i.Category,

                BasePrice = i.BasePrice,

                CurrencyId = i.CurrencyId,
                Currency = i.Currency,

                IsActive = i.IsActive,
                IsScraped = i.IsScraped,
                IsComplated = i.IsComplated,
                CanScrape = i.CanScrape,

                Images = i.Images,
                BusinessItems = i.BusinessItems,

                InsertBy = i.InsertBy,

                VinNumber = i.VinNumber,

                CarYearId = i.CarYearId,
                CarYear = i.CarYear,

                CarModelId = i.CarModelId,
                CarModel = i.CarModel,

                GlobalCustomerId = i.GlobalCustomerId,

                IsInStock = stockItemIds.Contains(i.ItemId)
            }).ToList();

            return Ok(response);
        }

        /// ////////////////////////////////////////////////

        [HttpGet("GetAllUndeProccessItems/{businessId}")]
        public async Task<IActionResult> GetAllUndeProccessItems(int businessId)
        {
            var business = await db.Businesses.FindAsync(businessId);

            if (business == null)
                return NotFound("Business not found");

            IQueryable<Item> query = db.Items
                .Include(i => i.Images)
                .Include(i => i.Platform)
                    .ThenInclude(p => p.BusinessPlatforms)
                    .ThenInclude(bp => bp.Business)
                .Include(i => i.Unit)
                .Include(i => i.Color)
                .Include(i => i.Category)
                .Include(i => i.Currency)
                .Include(i => i.CarBrand)
                .Include(i => i.BusinessItems);

            if (!business.Role.Contains("Manager"))
            {
                query = query
                    .Where(i => i.Platform.BusinessPlatforms
                        .Any(bp => bp.Business_id == businessId))
                    .Where(i => i.IsComplated == false);
            }

            var items = await query.Where(i => i.PlatformId != null).ToListAsync();

            // جميع الـ Items الموجودة في المخزن
            var itemIds = items.Select(i => i.ItemId).ToList();

            var stockItemIds = await db.Inventories
                .Where(x => itemIds.Contains(x.ItemId ?? 0))
                .Select(x => x.ItemId)
                .Distinct()
                .ToListAsync();

            // Response جديد يحتوي كل بيانات Item + IsInStock
            var response = items.Select(i => new ItemResponse
            {
                ItemId = i.ItemId,

                ItemDescription = i.ItemDescription,
                ItemDetails = i.ItemDetails,
                SKU = i.SKU,
                InternetId = i.InternetId,
                UPC = i.UPC,
                ShortCode = i.ShortCode,
                Model = i.Model,

                PlatformId = i.PlatformId,
                Platform = i.Platform,

                Height = i.Height,
                Width = i.Width,
                Length = i.Length,

                UnitId = i.UnitId,
                Unit = i.Unit,
                UnitValue = i.UnitValue,

                ColorId = i.ColorId,
                Color = i.Color,

                CarBrandId = i.CarBrandId,
                CarBrand = i.CarBrand,

                Description = i.Description,
                Weight = i.Weight,
                Internet = i.Internet,

                SizeId = i.SizeId,
                Size = i.Size,

                CategoryId = i.CategoryId,
                Category = i.Category,

                BasePrice = i.BasePrice,

                CurrencyId = i.CurrencyId,
                Currency = i.Currency,

                IsActive = i.IsActive,
                IsScraped = i.IsScraped,
                IsComplated = i.IsComplated,
                CanScrape = i.CanScrape,

                Images = i.Images,
                BusinessItems = i.BusinessItems,

                InsertBy = i.InsertBy,

                VinNumber = i.VinNumber,

                CarYearId = i.CarYearId,
                CarYear = i.CarYear,

                CarModelId = i.CarModelId,
                CarModel = i.CarModel,

                GlobalCustomerId = i.GlobalCustomerId,

                // الحقل الإضافي
                IsInStock = stockItemIds.Contains(i.ItemId)
            }).ToList();

            return Ok(response);
        }
        /// ////////////////////////////////////////////////

        [HttpGet("getItemFromAinAlfhdDB/{lastFourDigits}")]
        public async Task<IActionResult> getItemFromAinAlfhdDB(string lastFourDigits)
        {
            List<string> list = new List<string>();

            var items = await db.Items
               .Include(i => i.Images)
               .Where(i => !string.IsNullOrEmpty(i.SKU) && !EF.Functions.Like(i.SKU, "[0-9]%"))
               .Where(i => i.SKU.EndsWith(lastFourDigits))
               .ToListAsync();

            if (items.Count > 1)
            {
                list = items
              .SelectMany(i => i.Images)
              .Where(ii => !string.IsNullOrEmpty(ii.ImageUrl))
              .Select(ii => ii.ImageUrl)
              .ToList();
                return Ok(list);
            }

            else
            {
                var itemsFromAinAlfhd = await masterDBContext.AinAlfhdItem
                   .Include(i => i.ItemImages)
                   .Where(i => !string.IsNullOrEmpty(i.PCode) && !EF.Functions.Like(i.PCode, "[0-9]%"))
                   .Where(i => i.PCode.EndsWith(lastFourDigits))
                   .ToListAsync();

                foreach (var item in itemsFromAinAlfhd)
                {
                    list.Add(item.ImgUrl);
                }

                return Ok(list);
            }
        }

        [HttpGet("GetItemsFromItemTbl/{search}")]
        public async Task<IActionResult> GetItemsFromItemTbl(string search)
        {
            var items = await db.Items
                .Include(i => i.CarBrand)
                .Where(i =>
                   i.SKU.Contains(search)
                || i.UPC.Contains(search)
                || i.Model.Contains(search)
                || i.Internet.Contains(search)
                || i.ItemDescription.Contains(search)
                || i.CarBrand.CarBrandName.Contains(search))
                .Where(i => i.IsComplated == true)
                .ToListAsync();

            return Ok(items);
        }



        [HttpPost]
        public async Task<IActionResult> AddItem([FromForm] ItemDto1 model)
        {
            bool isItemComlated = false;
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if ((!model.sku.IsNullOrEmpty() || !model.upc.IsNullOrEmpty())
                && !model.ItemDescription.IsNullOrEmpty() && !model.ItemDetails.IsNullOrEmpty()
                && model.Images.Count() > 0 && model.CategoryId.HasValue && model.BasePrice != null)
            {
                isItemComlated = true;
            }
            var existItem = await db.Items.Where(i => i.SKU == model.sku || i.UPC == model.upc).FirstOrDefaultAsync();
            if (existItem != null)
            {
                var existBusinesItem = await db.BusinessItems
                    .Where(bi => bi.BusinessId == model.BusinessId)
                    .Where(bi => bi.ItemId == existItem.ItemId)
                    .FirstOrDefaultAsync();
                if (existBusinesItem == null)
                {
                    var businessItems1 = new BusinessItem
                    {
                        BusinessId = model.BusinessId,
                        ItemId = existItem.ItemId,
                    };

                    await db.BusinessItems.AddAsync(businessItems1);
                    await db.SaveChangesAsync();
                }

                return Ok(existItem);
            }

            var item = new Item
            {
                ItemDescription = model.ItemDescription,
                ItemDetails = model.ItemDetails,
                SKU = model.sku,
                UPC = model.upc,
                InternetId = model.InternetId,
                BasePrice = model.BasePrice,
                CategoryId = model.CategoryId,
                SizeId = model.SizeId,
                UnitId = model.UnitId,
                PlatformId = model.PlatformId,
                ColorId = model.ColorId,
                CurrencyId = model.CurrencyId,
                UnitValue = model.UnitValue,
                Model = model.Model,
                Height = model.Height,
                Width = model.Width,
                Length = model.Length,
                IsActive = true,
                IsScraped = true,
                CarBrandId = model.BrandId,
                IsComplated = isItemComlated,
                CanScrape = true
            };

            await db.Items.AddAsync(item);
            await db.SaveChangesAsync();

            if (model.Images != null && model.Images.Count > 0)
            {
                bool isFirst = true;

                foreach (var imageFile in model.Images)
                {
                    if (imageFile.Length == 0) continue;

                    string uploadedUrl = await UploadImageToCloudinary(imageFile);

                    var imagesItem = new ItemImage
                    {
                        ImageUrl = uploadedUrl,
                        ItemId = item.ItemId,
                        IsMain = isFirst,
                    };
                    await db.ItemImages.AddAsync(imagesItem);
                    isFirst = false;
                }

                await db.SaveChangesAsync();
            }

            var businessItems = new BusinessItem
            {
                BusinessId = model.BusinessId,
                ItemId = item.ItemId,
            };

            await db.BusinessItems.AddAsync(businessItems);
            await db.SaveChangesAsync();

            return Ok(item);
        }

        [HttpPost("AddItemJson/{id}")]
        public async Task<IActionResult> AddItemJson(int id, [FromBody] ItemJsonDto model)
        {
            bool isItemComlated = false;
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if ((!model.Sku.IsNullOrEmpty() || !model.Upc.IsNullOrEmpty())
                && !model.ItemDescription.IsNullOrEmpty() && !model.ItemDetails.IsNullOrEmpty()
                && model.Images.Count() > 0 && model.CategoryId.HasValue && model.BasePrice != null)
            {
                isItemComlated = true;
            }

            var existItem = await db.Items.FindAsync(id);
            if (existItem == null) return BadRequest();

            int? finalBrandId = null;
            if (model.Brand != null)
            {
                var brand = await db.CarBrands.Where(b => b.CarBrandName.Trim() == model.Brand.Trim()).FirstOrDefaultAsync();
                if (brand == null)
                {
                    if (model.Brand.Contains("Shop "))
                    {
                        model.Brand.Replace("Shop ", "");
                    }

                    var newBrand = new CarBrand
                    {
                        CarBrandName = model.Brand,
                    };
                    await db.CarBrands.AddAsync(newBrand);
                    await db.SaveChangesAsync();
                    finalBrandId = newBrand.CarBrandId;
                }
                else
                {
                    finalBrandId = brand.CarBrandId;
                }
            }

            existItem.ItemDescription = model.ItemDescription ?? existItem.ItemDescription;
            existItem.ItemDetails = model.ItemDetails;
            existItem.SKU = model.Sku ?? existItem.SKU;
            existItem.UPC = model.Upc ?? existItem.UPC;
            existItem.InternetId = model.InternetId;
            existItem.BasePrice = model.BasePrice ?? existItem.BasePrice;
            existItem.CategoryId = model.CategoryId ?? existItem.CategoryId;
            existItem.SizeId = model.SizeId;
            existItem.UnitId = model.UnitId;
            existItem.PlatformId = model.PlatformId;
            existItem.ColorId = model.ColorId;
            existItem.CurrencyId = model.CurrencyId;
            existItem.UnitValue = model.UnitValue;
            existItem.Model = model.Model ?? existItem.Model;
            existItem.Height = model.Height;
            existItem.Width = model.Width;
            existItem.CarBrandId = finalBrandId;
            existItem.Length = model.Length;
            existItem.IsActive = true;
            existItem.IsScraped = true;
            existItem.IsComplated = isItemComlated;
            existItem.IsScraped = true;
            existItem.CanScrape = true;
            await db.SaveChangesAsync();

            if (model.Images != null && model.Images.Count > 0)
            {
                bool isFirst = true;
                foreach (var imageUrl in model.Images)
                {
                    if (string.IsNullOrWhiteSpace(imageUrl)) continue;

                    var imagesItem = new ItemImage
                    {
                        ImageUrl = imageUrl,
                        ItemId = existItem.ItemId,
                        IsMain = isFirst,
                    };
                    await db.ItemImages.AddAsync(imagesItem);
                    isFirst = false;
                }

                await db.SaveChangesAsync();
            }

            var businessItems = new BusinessItem
            {
                BusinessId = model.BusinessId,
                ItemId = existItem.ItemId,
            };

            await db.BusinessItems.AddAsync(businessItems);
            await db.SaveChangesAsync();

            return Ok(existItem);
        }

        private async Task<string> UploadImageToCloudinary(IFormFile file)
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "items",
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error?.Message}");
            }

            return uploadResult.SecureUrl.ToString();
        }





        [HttpPost("AddItemFromMomile")]
        public async Task<IActionResult> AddItemFromMomile([FromBody] ItemDtoFromMobile model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var item = new Item
            {
                SKU = model.upc,
                PlatformId = model.PlatformId,
                IsActive = true,
            };

            await db.Items.AddAsync(item);
            await db.SaveChangesAsync();

            if (model.Images == null || model.Images.Count == 0)
                return BadRequest("No images uploaded");


            var uploadedImages = new List<UserImage>();

            foreach (var image in model.Images)
            {
                if (image.Length <= 0)
                    continue;

                using var stream = image.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(image.FileName, stream),
                    Folder = "Imgs"
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var imagesItem = new ItemImage
                    {
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        ItemId = item.ItemId,
                        IsMain = false,

                    };
                    await db.ItemImages.AddAsync(imagesItem);
                }
            }


            return Ok(item);
        }

        [HttpPost("AddItemFromWeb")]
        public async Task<IActionResult> AddItemFromWeb([FromBody] List<firstItemInputFromAngular> models)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            foreach (var model in models)
            {
                var itemCheckExist = await db.Items.Where(i => i.UPC == model.upc).FirstOrDefaultAsync();
                if (itemCheckExist != null)
                {
                    var inventory = new Inventory
                    {
                        ItemId = itemCheckExist.ItemId,
                    };

                    await db.Inventories.AddAsync(inventory);
                    await db.SaveChangesAsync();
                }
                else
                {
                    var item = new Item
                    {
                        SKU = model.upc,
                        PlatformId = model.PlatformId,
                        IsActive = true,
                    };
                    await db.Items.AddAsync(item);
                    await db.SaveChangesAsync();

                    var inventory = new Inventory
                    {
                        ItemId = item.ItemId,
                    };
                    await db.Inventories.AddAsync(inventory);
                    await db.SaveChangesAsync();
                }

            }
            return Ok(new
            {
                message = "items added successfuly"
            });
        }

        [HttpPost("addItemAndInv")]
        public async Task<IActionResult> addItemAndInv(ItemAndInventoryParams model)
        {
            int itemId;
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existItem = await db.Items.Where(i => i.SKU == model.sku).FirstOrDefaultAsync();
            if (existItem != null)
            {
                var existBusinesItem = await db.BusinessItems
                    .Where(bi => bi.BusinessId == model.businessId)
                    .Where(bi => bi.ItemId == existItem.ItemId)
                    .FirstOrDefaultAsync();
                if (existBusinesItem == null)
                {
                    var businessItems = new BusinessItem
                    {
                        BusinessId = model.businessId,
                        ItemId = existItem.ItemId,
                    };

                    await db.BusinessItems.AddAsync(businessItems);
                    await db.SaveChangesAsync();
                }


                existItem.PlatformId = model.platformId;
                existItem.CategoryId = model.categoryId;
                itemId = existItem.ItemId;
                if (model.addToInv == true)

                {
                    var existInventory = await db.Inventories.Where(i => i.ItemId == existItem.ItemId)
                    .Where(i => i.Size_id == model.sizeId)
                    .FirstOrDefaultAsync();

                    if (existInventory != null)
                    {
                        existInventory.QtyInInventory++;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        var inventory = new Inventory
                        {
                            ItemId = existItem.ItemId,
                            Size_id = model.sizeId,
                            Platform_id = model.platformId,
                            Business_id = model.businessId,
                            Insert_on = DateTime.UtcNow,
                            FolderImages = model.folderId,
                            QtyPublished = 1
                        };
                        await db.Inventories.AddAsync(inventory);
                        await db.SaveChangesAsync();
                    }
                }
            }

            else
            {
                var item = new Item
                {
                    SKU = model.sku,
                    PlatformId = model.platformId,
                    CategoryId = model.categoryId,
                    IsComplated = false,

                };

                await db.Items.AddAsync(item);
                await db.SaveChangesAsync();
                itemId = item.ItemId;

                foreach (var image in model.imageUrl)
                {
                    var itemImg = new ItemImage
                    {
                        ImageUrl = image,
                        ItemId = item.ItemId,
                    };

                    await db.ItemImages.AddAsync(itemImg);
                }

                var businessItems = new BusinessItem
                {
                    BusinessId = model.businessId,
                    ItemId = item.ItemId,
                };

                await db.BusinessItems.AddAsync(businessItems);
                await db.SaveChangesAsync();

                if (model.addToInv == true)

                {
                    var inventory = new Inventory
                    {
                        ItemId = item.ItemId,
                        Size_id = model.sizeId,
                        Platform_id = model.platformId,
                        Business_id = model.businessId,
                        FolderImages = model.folderId,
                        QtyInInventory = 1

                    };
                    await db.Inventories.AddAsync(inventory);
                    await db.SaveChangesAsync();
                }
            }
            var userImages = await db.UserImages.Where(i => i.FolderId == model.folderId).ToListAsync();
            foreach (var item in userImages)
            {
                item.IsProccessed = true;
                item.ItemId = itemId;
            }

            await db.SaveChangesAsync();
            return Ok(new
            {
                msg = "item and inventory added successfuly"
            });

        }


        [Authorize]
        [HttpPost("addDutyItemAndInv")]
        public async Task<IActionResult> addDutyItemAndInv(ItemAndInventoryParams model)
        {
            int itemId;
            var user = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existItem = await db.Items.Where(i => i.UPC == model.upc || i.Model == model.upc || i.SKU == model.upc || i.Internet == model.upc).FirstOrDefaultAsync();
            if (existItem != null)
            {

                var existBusinesItem = await db.BusinessItems
                    .Where(bi => bi.BusinessId == model.businessId)
                    .Where(bi => bi.ItemId == existItem.ItemId)
                    .FirstOrDefaultAsync();
                if (existBusinesItem == null)
                {
                    var businessItems = new BusinessItem
                    {
                        BusinessId = model.businessId,
                        ItemId = existItem.ItemId,
                    };

                    await db.BusinessItems.AddAsync(businessItems);
                    await db.SaveChangesAsync();
                }



                existItem.PlatformId = model.platformId;
                existItem.CategoryId = model.categoryId;
                itemId = existItem.ItemId;
                var existInventory = await db.Inventories.Where(i => i.ItemId == existItem.ItemId)
                    .Where(i => i.Size_id == model.sizeId)
                    .FirstOrDefaultAsync();

                if (model.addToInv == true)

                {
                    if (existInventory != null)
                    {
                        existInventory.QtyInInventory++;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        var inventory = new Inventory
                        {
                            ItemId = existItem.ItemId,
                            Size_id = model.sizeId,
                            Platform_id = model.platformId,
                            Business_id = model.businessId,
                            Insert_on = DateTime.UtcNow,
                            FolderImages = model.folderId,
                            IsProccessedInInventory = false,
                            QtyPublished = 1,
                            ItemConditionId = model.itemConditionId,
                            Category_id = model.categoryId,
                        };
                        await db.Inventories.AddAsync(inventory);
                        await db.SaveChangesAsync();
                    }
                }
            }

            else
            {
                var item = new Item
                {
                    UPC = model.upc,
                    PlatformId = model.platformId,
                    CategoryId = model.categoryId,
                    IsComplated = false,
                };
                await db.Items.AddAsync(item);
                await db.SaveChangesAsync();

                itemId = item.ItemId;

                foreach (var image in model.imageUrl)
                {
                    var itemImg = new ItemImage
                    {
                        ImageUrl = image,
                        ItemId = item.ItemId,
                        UserId = user.Id,
                    };

                    await db.ItemImages.AddAsync(itemImg);
                }

                var businessItems = new BusinessItem
                {
                    BusinessId = model.businessId,
                    ItemId = item.ItemId,
                };

                await db.BusinessItems.AddAsync(businessItems);
                await db.SaveChangesAsync();

                if (model.addToInv == true)

                {
                    var inventory = new Inventory
                    {
                        ItemId = item.ItemId,
                        Size_id = model.sizeId,
                        Platform_id = model.platformId,
                        Business_id = model.businessId,
                        FolderImages = model.folderId,
                        IsProccessedInInventory = false,
                        QtyInInventory = 1

                    };
                    await db.Inventories.AddAsync(inventory);
                    await db.SaveChangesAsync();
                }
            }
            var userImages = await db.UserImages.Where(i => i.FolderId == model.folderId).ToListAsync();
            foreach (var item in userImages)
            {
                item.IsProccessed = true;
                item.ItemId = itemId;
            }

            await db.SaveChangesAsync();
            return Ok(new
            {
                msg = "item and inventory added successfuly"
            });

        }

        [HttpPost("BindImagesWithItem")]
        public async Task<IActionResult> BindImagesWithItem([FromBody] BindImagesWithItem model)
        {
            var item = await db.Items.FindAsync(model.itemId);
            if (item == null) return NotFound();

            foreach (var url in model.ImagesUrls)
            {
                var newImgItem = new ItemImage
                {
                    ImageUrl = url,
                    ItemId = item.ItemId,
                };
                await db.ItemImages.AddAsync(newImgItem);
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                msg = "images binded with item successfuly!..."
            });
        }


        [HttpPost("UpdateUpc")]
        public async Task<IActionResult> UpdateUpc([FromBody] UpdateUpc model)
        {
            var item = await db.Items.FindAsync(model.itemId);
            if (item == null) return NotFound();

            item.UPC = model.upc;

            await db.SaveChangesAsync();

            return Ok(new
            {
                msg = "images binded with item successfuly!..."
            });
        }

        [Authorize]
        [HttpPost("BindImagesWithItemForDuty")]
        public async Task<IActionResult> BindImagesWithItemForDuty([FromBody] BindImagesWithItemForDuty model)
        {
            int? finalBrandId = null;
            if (model.brand != null)
            {
                var brand = await db.CarBrands.Where(b => b.CarBrandName.Trim() == model.brand.Trim()).FirstOrDefaultAsync();
                if (brand == null)
                {
                    if (model.brand.Contains("Shop "))
                    {
                        model.brand.Replace("Shop ", "");
                    }

                    var newBrand = new CarBrand
                    {
                        CarBrandName = model.brand,
                    };
                    await db.CarBrands.AddAsync(newBrand);
                    await db.SaveChangesAsync();
                    finalBrandId = newBrand.CarBrandId;
                }
                else
                {
                    finalBrandId = brand.CarBrandId;
                }
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var item = await db.Items.FindAsync(model.itemId);
            if (item == null) return NotFound();

            item.Height = model.height;
            item.UPC = model.upc;
            item.Width = model.width;
            item.Weight = model.weight;
            item.ItemDescription = model.title;
            item.Internet = model.internet;
            item.Model = model.model;
            item.BasePrice = model.price;
            item.CarBrandId = finalBrandId;
            item.ItemDetails = model.description;
            item.IsScraped = true;
            item.IsComplated = true;

            await db.SaveChangesAsync();

            //foreach (var url in model.ImagesUrls)
            //{
            //    var newImgItem = new ItemImage
            //    {
            //        ImageUrl = url,
            //        ItemId = item.ItemId,
            //        UserId = user.Id,
            //    };
            //    await db.ItemImages.AddAsync(newImgItem);
            //}

            ///////////////////////
            ///
            using var httpClient = new HttpClient();

            foreach (var url in model.ImagesUrls)
            {
                try
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(url);

                    using var stream = new MemoryStream(imageBytes);

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription("image.jpg", stream),
                        Folder = "Items"
                    };

                    var uploadResult = await cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.Error != null)
                    {
                        throw new Exception(uploadResult.Error.Message);
                    }

                    var newImgItem = new ItemImage
                    {
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        ItemId = item.ItemId,
                        UserId = user.Id
                    };

                    await db.ItemImages.AddAsync(newImgItem);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error uploading image: {ex.Message}");
                }
            }

            await db.SaveChangesAsync();

            ///////////////////////////////////////////////////////////////////////



            var invRec = await db.Inventories.FindAsync(model.currentInventoryId);
            if (invRec == null) return NotFound();

            invRec.SitePrice = model.price.ToString();
            invRec.Product_description = model.description;
            invRec.Product_name = model.title;
            invRec.IsProccessedInInventory = true;

            //invRec.Category_id = model.ca;
            await db.SaveChangesAsync();
            await db.SaveChangesAsync();

            return Ok(new
            {
                msg = "images binded with item successfuly!..."
            });
        }

        [HttpPut("DeleteImages")]
        public async Task<IActionResult> DeleteImages(DeleteImages ImagesIds)
        {
            var images = await db.ItemImages.Where(i => ImagesIds.ImagesIds.Contains(i.ItemImageId)).ToListAsync();
            if (images == null) return NotFound();
            db.ItemImages.RemoveRange(images);

            await db.SaveChangesAsync();
            return Ok(images);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromBody] ItemDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var item = await db.Items
                .Include(i => i.Images)
                .FirstOrDefaultAsync(i => i.ItemId == id);

            if (item == null) return NotFound("Item not found");

            item.ItemDescription = model.ItemDescription;
            item.ItemDetails = model.ItemDetails;
            item.SKU = model.sku;
            item.UPC = model.upc;
            item.InternetId = model.InternetId;
            item.BasePrice = model.BasePrice;
            item.CategoryId = model.CategoryId;
            item.SizeId = model.SizeId;
            item.UnitId = model.UnitId;
            item.PlatformId = model.PlatformId;
            item.ColorId = model.ColorId;
            item.CurrencyId = model.CurrencyId;
            item.UnitValue = model.UnitValue;
            item.Model = model.Model;
            item.Height = model.Height;
            item.Width = model.Width;
            item.Length = model.Length;
            item.CarBrandId = model.BrandId;

            if ((!item.SKU.IsNullOrEmpty() || !item.UPC.IsNullOrEmpty())
                    && !item.ItemDescription.IsNullOrEmpty() && !item.ItemDetails.IsNullOrEmpty()
                    && item.Images.Count() > 0 && item.CategoryId.HasValue && item.BasePrice != null)
            {
                item.IsComplated = true;
            }

            await db.SaveChangesAsync();

            // --- حذف الصور المطلوب حذفها ---
            if (model.DeletedImageIds != null && model.DeletedImageIds.Count > 0)
            {
                var imagesToDelete = item.Images
                    .Where(img => model.DeletedImageIds.Contains(img.ItemImageId))
                    .ToList();

                if (imagesToDelete.Any())
                {
                    db.ItemImages.RemoveRange(imagesToDelete);
                    await db.SaveChangesAsync();
                }
            }

            var inv = await db.Inventories.Where(i => i.ItemId == item.ItemId).FirstOrDefaultAsync();
            if (inv != null)
            {
                inv.ItemConditionId = model.ItemConditionId;
                inv.SitePrice = model.invPrice.ToString();
                inv.QtyPublished = model.Qty;
                inv.Business_id = model.BusinessId;
                inv.Insert_on = DateTime.UtcNow;
            }
            else
            {
                var newInv = new Inventory
                {
                    ItemId = item.ItemId,
                    SitePrice = model.invPrice.ToString(),
                    QtyPublished = model.Qty,
                    Business_id = model.BusinessId,
                    ItemConditionId = model.ItemConditionId
                };

                await db.Inventories.AddAsync(newInv);
            }
            await db.SaveChangesAsync();
            var updatedItem = await db.Items
                    .Include(i => i.Images)
                    .Include(i => i.Platform)
                    .Include(i => i.Unit)
                    .Include(i => i.Color)
                    .Include(i => i.Category)
                    .Include(i => i.Currency)
                    .FirstOrDefaultAsync(i => i.ItemId == id);

            return Ok(updatedItem);
        }

        [HttpPost("UploadImages")]
        public async Task<IActionResult> UploadImages([FromForm] int itemId, [FromForm] List<IFormFile> images)
        {
            var item = await db.Items
                        .Include(i => i.Images)
                        .FirstOrDefaultAsync(i => i.ItemId == itemId);

            if (item == null) return NotFound("Item not found");

            if (images == null || images.Count == 0)
                return BadRequest("No images provided");

            bool hasMainImage = await db.ItemImages
                .AnyAsync(img => img.ItemId == itemId && img.IsMain);

            bool isFirst = !hasMainImage;

            foreach (var imageFile in images)
            {
                if (imageFile.Length == 0) continue;

                string uploadedUrl = await UploadImageToCloudinary(imageFile);

                var imagesItem = new ItemImage
                {
                    ImageUrl = uploadedUrl,
                    ItemId = itemId,
                    IsMain = isFirst,
                };
                await db.ItemImages.AddAsync(imagesItem);
                isFirst = false;
            }

            if ((!item.SKU.IsNullOrEmpty() || !item.UPC.IsNullOrEmpty())
                && !item.ItemDescription.IsNullOrEmpty() && !item.ItemDetails.IsNullOrEmpty()
                && item.Images.Count() > 0 && item.CategoryId.HasValue && item.BasePrice != null)
            {
                item.IsComplated = true;
            }

            await db.SaveChangesAsync();

            var updatedImages = await db.ItemImages
                .Where(img => img.ItemId == itemId)
                .ToListAsync();

            return Ok(updatedImages);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteItem(int id)
        {
            var item = await db.Items.FindAsync(id);
            if (item == null) return NotFound("Item not found");

            var images = await db.ItemImages.Where(im => im.ItemId == id).ToListAsync();
            if (images != null)
            {
                db.ItemImages.RemoveRange(images);
                await db.SaveChangesAsync();
            }

            var UserImages = await db.UserImages.Where(im => im.ItemId == id).ToListAsync();
            if (UserImages != null)
            {
                db.UserImages.RemoveRange(UserImages);
                await db.SaveChangesAsync();
            }

            var Inventories = await db.Inventories.Where(im => im.ItemId == id).ToListAsync();
            if (Inventories != null)
            {
                db.Inventories.RemoveRange(Inventories);
                await db.SaveChangesAsync();
            }

            var bsit = await db.BusinessItems.Where(im => im.ItemId == id).ToListAsync();
            if (bsit != null)
            {
                db.BusinessItems.RemoveRange(bsit);
                await db.SaveChangesAsync();
            }

            db.Items.Remove(item);
            await db.SaveChangesAsync();

            return Ok(item);
        }

        [HttpGet("SearchAboutItem/{upc}")]
        public async Task<IActionResult> SearchAboutItem(string upc)
        {
            var item = await db.Items.Where(i => i.UPC == upc).FirstOrDefaultAsync();
            if(item != null)
            {
                var inventory = await db.Inventories.Where(i => i.ItemId == item.ItemId).FirstOrDefaultAsync();
                if (inventory != null) 
                    return Ok(new
                    {
                        msg = $"item already exist in item and inventory has {inventory.QtyPublished} piece",
                        itemId = item.ItemId,
                        inventoryId = inventory.Inventory_id,
                        qty = inventory.QtyPublished,
                        existInItem = true,
                        existInInv = true,
                    });
                return Ok(new
                {
                    msg = $"item already exist in item only",
                    itemId = item.ItemId,
                    existInItem = true,
                    existInInv = false,
                    qty = 0,
                });
            }
            return Ok(new
            {
                msg = $"item does not exist you can add it as a new item",
                existInItem = false,
                existInInv = false,
                qty = 0,
            });
        }

        [HttpPut("SetItemCannotScraped/{id}")]
        public async Task<IActionResult> SetItemCannotScraped(int id)
        {
            var item = await db.Items.FindAsync(id);
            if (item == null) return NotFound("Item not found");

            item.CanScrape = false;

            await db.SaveChangesAsync();

            return Ok(item);
        }


        [HttpPost("AddItemToInv/{itemId}/{invId}")]
        public async Task<IActionResult> AddItemToInv(int itemId, int invId)
        {
            var inv = await db.Inventories
                .FirstOrDefaultAsync(i => i.ItemId == itemId);

            if (inv == null)
            {
                inv = new Inventory
                {
                    ItemId = itemId,
                    QtyPublished = 1,
                    Business_id = invId,
                    Insert_on = DateTime.UtcNow
                };

                await db.Inventories.AddAsync(inv);
            }
            else
            {
                inv.QtyPublished = (inv.QtyPublished ?? 0) + 1;
                inv.Business_id = invId;
                inv.Insert_on = DateTime.UtcNow;
            }

            await db.SaveChangesAsync();

            return Ok(inv);
        }


        [HttpPut("UpdateItemDataFromScraping/{itemId}")]
        public async Task<IActionResult> UpdateItemDataFromScraping(int itemId, [FromBody] ScrapeItemModel model)
        {
            var checkItem = await db.Items.FindAsync(itemId);
            if (checkItem == null) return NotFound();

            checkItem.ItemDescription = model.description ?? checkItem.ItemDescription;

            if (!string.IsNullOrWhiteSpace(model.brand))
            {
                checkItem.CarBrandId = await GetBrandIdByName(model.brand);
            }


            checkItem.ItemDetails = model.details;
            checkItem.SKU = model.sku ?? checkItem.SKU;
            checkItem.Height = model.height ?? checkItem.Height;
            checkItem.Weight = model.weight ?? checkItem.Weight;
            checkItem.Width = model.width ?? checkItem.Width;
            checkItem.ShortCode = model.item_number_id ?? checkItem.ShortCode;
            checkItem.Model = model.model ?? checkItem.Model;
            checkItem.BasePrice = model.itemPrice ?? checkItem.BasePrice;
            checkItem.IsScraped = true;


            foreach (var image in model.images)
            {
                var newImgItem = new ItemImage
                {
                    ImageUrl = image,
                    ItemId = checkItem.ItemId,
                };
                await db.ItemImages.AddAsync(newImgItem);
            }

            await db.SaveChangesAsync();
            return Ok(checkItem);
        }


        private async Task<int> GetBrandIdByName(string name)
        {
            name = name.Trim();

            var brand = await db.CarBrands
                .FirstOrDefaultAsync(b => b.CarBrandName.Trim() == name);

            if (brand != null)
            {
                return brand.CarBrandId;
            }

            var newBrand = new CarBrand
            {
                CarBrandName = name
            };

            await db.CarBrands.AddAsync(newBrand);
            await db.SaveChangesAsync();

            return newBrand.CarBrandId;
        }


    }
}

public class ItemAndInventoryParams
{
    public int? categoryId { get; set; }
    public int? sizeId { get; set; }
    public int? platformId { get; set; }
    public string? upc { get; set; }
    public string? sku { get; set; }
    public List<string>? imageUrl { get; set; }
    public int? folderId { get; set; }
    public int? businessId { get; set; }
    public int? itemConditionId { get; set; }
    public int? BrandId { get; set; }
    public bool addToInv { get; set; }

}

public class BindImagesWithItem
{
    public int? itemId { get; set; }
    public List<string>? ImagesUrls { get; set; }

}

public class UpdateUpc
{
    public int? itemId { get; set; }
    public string upc { get; set; }

}

public class DeleteImages
{
    public List<int> ImagesIds { get; set; }

}


public class BindImagesWithItemForDuty
{
    public int? itemId { get; set; }
    public List<string>? ImagesUrls { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public string? brand { get; set; }
    public decimal? price { get; set; }
    public decimal? height { get; set; }
    public decimal? width { get; set; }
    public decimal? length { get; set; }
    public string? model { get; set; }
    public decimal? weight { get; set; }
    public string? internet { get; set; }
    public string? sKU { get; set; }
    public string? upc { get; set; }
    public int currentInventoryId { get; set; }


}

public class ScrapeItemModel
{
    public string? description { get; set; }
    public string? brand { get; set; }
    public string? model { get; set; }
    public decimal? itemPrice { get; set; }
    public List<string>? images { get; set; }

    /// /// exstra data // ///////////////////////////////////////
    public string? details { get; set; }
    public string? sku { get; set; }
    public string? upc { get; set; }
    public string? item_number_id { get; set; }
    public decimal? height { get; set; }
    public decimal? width { get; set; }
    public decimal? deepEqual { get; set; }
    public decimal? weight { get; set; }
}
