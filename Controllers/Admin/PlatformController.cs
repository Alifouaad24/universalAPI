using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformController : ControllerBase
    {
        UniversalDbContext db;
        public PlatformController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetAllData(int businessId)
        {
            var business = await db.Businesses.FindAsync(businessId);

            if (business == null)
                return NotFound("Business not found");

            IQueryable<Universal_server.Models.Platform> query = db.Platforms.Include(p => p.BusinessPlatforms);

            if (!business.Role.Contains("Manager"))
            {
                query = query.Where(p => p.BusinessPlatforms.Any(bp => bp.Business_id == businessId));
            }

            var data = await query.ToListAsync();

            return Ok(data);
        }


        [HttpGet("GetSupplierPlatForms/{busSerId}")]
        public async Task<IActionResult> GetSupplierPlatForms(int busSerId)
        {
            var bsp = await db.Business_Service_Platforms
                .Include(bsp => bsp.Platform)
                .Where(bsps => bsps.Business_ServiceId == busSerId).ToListAsync();
            return Ok(bsp);
        }

        [HttpPost("bindSupplierBusinessWithPlatform")]
        public async Task<IActionResult> bindSupplierBusinessWithPlatform(bindSupplierBusinessWithPlatform model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            foreach (int i in model.platformsIds)
            {
                var bsp = new Business_Service_Platform
                {
                    Business_ServiceId = model.bsId,
                    Platform_id = i,
                };

                await db.Business_Service_Platforms.AddAsync(bsp);
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                msg = "binded successfult"
            });

        }


        [HttpPost]
        public async Task<IActionResult> addPlatform(PlatformDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var platform = new Universal_server.Models.Platform
            {
                Description = model.Description,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                visible = true,
            };

            await db.Platforms.AddAsync(platform);
            await db.SaveChangesAsync();

            if (model.PlatformBus != null && model.PlatformBus.Count > 0)
            {
                foreach (var i in model.PlatformBus)
                {
                    var busPlatform = new BusinessPlatforms
                    {
                        Platform_id = platform.Platform_id,
                        Business_id = i.businessId,
                        ForSelling = i.isForSell,
                        ForScraping = i.isForScrape
                    };

                    await db.BusinessPlatforms.AddAsync(busPlatform);
                }
                await db.SaveChangesAsync();
            }
            return Ok(platform);
        }

        [HttpPut("{platformId}")]
        public async Task<IActionResult> UpdatePlatform(int platformId, PlatformDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var platform = await db.Platforms.FindAsync(platformId);
            if (platform == null) return NotFound("Platform not found");

            // تحديث بيانات الـ Platform نفسها
            platform.Description = model.Description;
            db.Platforms.Update(platform);

            // جلب كل الروابط الحالية لهذه الـ Platform مع البزنسات
            var existingLinks = await db.BusinessPlatforms
                .Where(bp => bp.Platform_id == platformId)
                .ToListAsync();

            var incomingBusinessIds = model.PlatformBus?.Select(b => b.businessId).ToHashSet() ?? new HashSet<int>();

            // 1) احذف الروابط الموجودة سابقاً والتي لم تعد موجودة بالطلب الجديد
            var linksToRemove = existingLinks
                .Where(link => !incomingBusinessIds.Contains(link.Business_id ?? -1))
                .ToList();

            if (linksToRemove.Any())
            {
                db.BusinessPlatforms.RemoveRange(linksToRemove);
            }

            // 2) أضف أو حدّث كل عنصر بالطلب الجديد
            if (model.PlatformBus != null)
            {
                foreach (var incoming in model.PlatformBus)
                {
                    var existingLink = existingLinks
                        .FirstOrDefault(link => link.Business_id == incoming.businessId);

                    if (existingLink != null)
                    {
                        // موجود مسبقاً → حدّث القيم فقط
                        existingLink.ForSelling = incoming.isForSell;
                        existingLink.ForScraping = incoming.isForScrape;
                        db.BusinessPlatforms.Update(existingLink);
                    }
                    else
                    {
                        // غير موجود مسبقاً → أضف رابط جديد
                        var newLink = new BusinessPlatforms
                        {
                            Platform_id = platformId,
                            Business_id = incoming.businessId,
                            ForSelling = incoming.isForSell,
                            ForScraping = incoming.isForScrape
                        };
                        await db.BusinessPlatforms.AddAsync(newLink);
                    }
                }
            }

            await db.SaveChangesAsync();

            // إرجاع الـ Platform مع الروابط المحدّثة
            var updatedPlatform = await db.Platforms
                .Include(p => p.BusinessPlatforms)
                .FirstOrDefaultAsync(p => p.Platform_id == platformId);

            return Ok(updatedPlatform);
        }
    }
}

public class bindSupplierBusinessWithPlatform
{
    public int bsId { get; set; }
    public List<int> platformsIds { get; set; }
}
