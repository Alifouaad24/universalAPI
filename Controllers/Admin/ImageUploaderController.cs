using System.Linq;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.BiDi.BrowsingContext;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;
using static System.Net.Mime.MediaTypeNames;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploaderController : ControllerBase
    {
        UniversalDbContext dbContext;
        private readonly UserManager<IdentityUserData> userManager;
        private readonly IConfiguration configuration;
        private Cloudinary cloudinary;
        string cloudName;
        string apiKey;
        string apiSecret;
        public ImageUploaderController(UniversalDbContext dbContext, UserManager<IdentityUserData> userManager, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.configuration = configuration;
            cloudinary = new Cloudinary(new Account(
                 configuration["Claoudinary:cloudName"],
                 configuration["Claoudinary:apiKey"],
                 configuration["Claoudinary:apiSecret"]
             ));
        }

        [HttpGet("{businessId}")]
        [Authorize]
        public async Task<IActionResult> GetAllUserImages(int businessId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (await userManager.IsInRoleAsync(user, "Super Admin"))
            {
                var imgs = await dbContext.UserImages
                    .Where(i => i.BusinessId == businessId)
                    .Where(i => i.UserId == user.Id)
                    .ToListAsync();

                return Ok(imgs);
            }

            var userImages = await dbContext.UserImages
                .Where(i => i.UserId == user.Id)
                .Where(i => i.BusinessId == businessId)
                .ToListAsync();

            return Ok(userImages);
        }

        [HttpGet("JustScraped/{businessId}")]
        [Authorize]
        public async Task<IActionResult> JustScraped(int businessId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            if (await userManager.IsInRoleAsync(user, "Super Admin"))
            {
                var imgs = await dbContext.UserImages
                    .Include(i => i.Item)
                    .Where(i => i.BusinessId == businessId)
                    .Where(i => i.UserId == user.Id)
                    .Where(i => i.Item.IsScraped == true)
                    .ToListAsync();

                return Ok(imgs);
            }

            var userImages = await dbContext.UserImages
                .Include(i => i.Item)
                .Where(i => i.UserId == user.Id)
                .Where(i => i.BusinessId == businessId)
                .Where(i => i.Item.IsScraped == true)
                .ToListAsync();

            return Ok(userImages);
        }

        public class CreateFolderDto
        {
            public bool AddToItemAndInventory { get; set; }
            public string? Upc { get; set; }
            public int? ItemId { get; set; }
        }
        [Authorize]
        [HttpPost("CreateFolder")]
        public async Task<IActionResult> CreateFolder([FromBody] CreateFolderDto dto)
        {
            var user = await userManager.GetUserAsync(User);
            string userId = user.Id;

            Inventory? inv = null;
            int? itemId = null;

            if (dto.AddToItemAndInventory)
            {
                if (dto.ItemId.HasValue)
                {
                    itemId = dto.ItemId;
                    inv = await dbContext.Inventories
                        .FirstOrDefaultAsync(i => i.ItemId == itemId);

                    if (inv == null)
                    {
                        inv = new Inventory { ItemId = itemId.Value };
                        await dbContext.Inventories.AddAsync(inv);
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    var item = new Item { UPC = dto.Upc, PlatformId = 3 };
                    await dbContext.Items.AddAsync(item);
                    await dbContext.SaveChangesAsync();

                    inv = new Inventory { ItemId = item.ItemId };
                    await dbContext.Inventories.AddAsync(inv);
                    await dbContext.SaveChangesAsync();

                    itemId = item.ItemId;
                }
            }

            UserFolder? folder = null;
            if (itemId.HasValue)
            {
                folder = await dbContext.UserFolders
                    .FirstOrDefaultAsync(f => f.ItemId == itemId && f.UserId == userId);
            }

            if (folder == null)
            {
                folder = new UserFolder
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    ItemId = itemId
                };

                await dbContext.UserFolders.AddAsync(folder);
                await dbContext.SaveChangesAsync();
            }

            var currentFolder = new
            {
                UserFolderId = folder.UserFolderId,
                UserId = userId,
                CreatedAt = folder.CreatedAt,
                ItemId = folder.ItemId,
                InvId = inv?.Inventory_id
            };

            return Ok(currentFolder);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAlbum([FromForm] UserImagesDto dto)
        {
            if (dto.Images == null || dto.Images.Count == 0)
                return BadRequest("No images uploaded");

            var user = await userManager.GetUserAsync(User);
            string userId = user.Id;


            var currentItem = await dbContext.Items.FindAsync(dto.ItemId);

            var folder = await dbContext.UserFolders
                .FirstOrDefaultAsync(f => f.ItemId == currentItem.ItemId && f.UserId == userId);



            if (folder == null)
                return BadRequest("جلسة تصوير غير صالحة");

            var uploadedImages = new List<UserImage>();
            var urlImages = new List<string>();

            foreach (var image in dto.Images)
            {
                if (image.Length <= 0) continue;

                using var stream = image.OpenReadStream();
                var uploadResult = await cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = new FileDescription(image.FileName, stream),
                    Folder = $"users/{userId}"
                });

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var url = uploadResult.SecureUrl.ToString();
                    uploadedImages.Add(new UserImage
                    {
                        UserId = userId,
                        BusinessId = dto.BusinessId,
                        ImageUrl = url,
                        FolderId = dto.FolderId,
                        ItemId = folder.ItemId,
                        InsertDate = DateTime.Now,
                        IsProccessed = folder.ItemId.HasValue
                    });
                    urlImages.Add(url);
                }
            }

            if (uploadedImages.Count == 0)
                return BadRequest("فشل رفع جميع الصور");

            if (folder.ItemId.HasValue)
            {
                var itemImages = urlImages.Select(url => new ItemImage
                {
                    ItemId = folder.ItemId.Value,
                    ImageUrl = url,
                    IsMain = false,
                    UserId = userId
                }).ToList();
                await dbContext.ItemImages.AddRangeAsync(itemImages);

                var item = await dbContext.Items.FindAsync(folder.ItemId.Value);
                var inv = await dbContext.Inventories.FindAsync(folder.ItemId.Value);

                if (item != null)
                {
                    // -------- UPC (الموجود عندك أصلاً) --------
                    if (!string.IsNullOrWhiteSpace(dto.upc))
                        item.UPC = dto.upc;

                    // 👇👇👇 التعديل الجديد: بيانات المنتج الإضافية 👇👇👇
                    if (dto.CategoryId.HasValue)
                        item.CategoryId = dto.CategoryId;

                    if (dto.ItemConditionId.HasValue)
                        inv.ItemConditionId = dto.ItemConditionId;

                    if (dto.PlatformId.HasValue)
                        item.PlatformId = dto.PlatformId;

                    if (!string.IsNullOrWhiteSpace(dto.Description))
                        item.Description = dto.Description; // عدّل اسم الخاصية لو مختلف بالـ Entity عندك

                    if (!string.IsNullOrWhiteSpace(dto.Details))
                        item.ItemDetails = dto.Details; // عدّل اسم الخاصية لو مختلف بالـ Entity عندك

                    if (dto.ItemPrice.HasValue)
                        item.BasePrice = dto.ItemPrice; // عدّل اسم الخاصية لو مختلف بالـ Entity عندك

                    if (dto.WarehousePrice.HasValue)
                        inv.SitePrice = dto.WarehousePrice.ToString(); // ⚠️ يحتاج عمود جديد لو مو موجود
                                                                       // 👆👆👆 نهاية التعديل 👆👆👆
                }
            }

            await dbContext.UserImages.AddRangeAsync(uploadedImages);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Photos uploaded successfully",
                remoteImageId = uploadedImages[0].UserImagesId,
                insetDate = uploadedImages[0].InsertDate.ToString(),
            });
        }

        [HttpDelete("{imageId}")]
        [Authorize]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var user = await userManager.GetUserAsync(User);

            var image = await dbContext.UserImages
                .FirstOrDefaultAsync(x => x.UserImagesId == imageId && x.UserId == user.Id);

            if (image == null)
                return NotFound("Image not found");

            var publicId = ExtractPublicId(image.ImageUrl);
            var deletionParams = new DeletionParams(publicId);
            var result = await cloudinary.DestroyAsync(deletionParams);

            if (result.Result != "ok")
                return BadRequest("Failed to delete image from Cloudinary");

            dbContext.UserImages.Remove(image);
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Image deleted successfully" });
        }

        [HttpDelete("DeleteImageByFolderId/{folderId}")]
        [Authorize]
        public async Task<IActionResult> DeleteImageByFolderId(int folderId)
        {
            var user = await userManager.GetUserAsync(User);

            var images = await dbContext.UserImages
                .Where(x => x.FolderId == folderId && x.UserId == user.Id)
                .ToListAsync();

            if (!images.Any())
                return NotFound("Images not found");

            foreach (var imgRec in images)
            {
                var publicId = ExtractPublicId(imgRec.ImageUrl);
                var deletionParams = new DeletionParams(publicId);
                var result = await cloudinary.DestroyAsync(deletionParams);

                if (result.Result != "ok")
                    return BadRequest("Failed to delete image from Cloudinary");
            }

            dbContext.UserImages.RemoveRange(images);
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Images deleted successfully" });
        }

        [HttpPost("UploadImageToCloudinary")]
        public async Task<IActionResult> UploadImageToCloudinary([FromForm] ImageForm model)
        {
            if (model == null || model.Image == null)
                return BadRequest("No image provided");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var stream = model.Image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(model.Image.FileName, stream),
                Folder = "CarcLogo"
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult == null || uploadResult.SecureUrl == null)
                return StatusCode(500, "Upload failed");

            return Ok(uploadResult.SecureUrl.ToString());
        }

        [HttpDelete("DeleteFolder/{id}")]
        public async Task<IActionResult> DeleteFolder(int id)
        {
            var folder = await dbContext.UserFolders.FindAsync(id);
            if (folder == null) return BadRequest();

            dbContext.UserFolders.RemoveRange(folder);
            await dbContext.SaveChangesAsync();
            return Ok(folder);

        }


        private string ExtractPublicId(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var uploadIndex = Array.IndexOf(segments, "upload");
            var publicIdSegments = segments.Skip(uploadIndex + 2);
            var publicId = string.Join("/", publicIdSegments);
            return Path.ChangeExtension(publicId, null);
        }







    }
}

public class ImageForm
{
    public IFormFile Image { get; set; }
}
