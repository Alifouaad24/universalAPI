using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class StartInformationController : ControllerBase
    {
        UniversalDbContext dbContext;
        UserManager<IdentityUserData> _userManager;
        public StartInformationController(UniversalDbContext dbContext, UserManager<IdentityUserData> userManager)
        {
            this.dbContext = dbContext;
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetDutyInfo(int businessId)
        {
            var user = await _userManager.GetUserAsync(User);

            int CountUnProccessedImages = 0;
            int CountUnnScrapedItems = 0;
            int CountUnPublishedItems = 0;
            int CountUnPublishedOnMarket = 0;
            int CountOfNerTasks = 0;

            var unProccessedImages = await dbContext.UserImages
                .Where(ui => !ui.IsProccessed
                          && ui.BusinessId == businessId
                          && ui.UserId == user.Id)
                .GroupBy(ui => ui.FolderId)
                .Select(g => g.First())
                .ToListAsync();

            var unScrapedItems = await dbContext.Inventories.Where(i => !i.IsProccessedInInventory && i.Business_id == businessId).ToListAsync();

            var business = await dbContext.Businesses.FindAsync(businessId);
            if (!business.Role.IsNullOrEmpty() && business.Role.Contains("Manager"))
            {
                var allTasks = await dbContext.Features
                .Where(i => i.Status.Contains("New"))
                .Where(t => t.visible == true)
                .ToListAsync();

                CountOfNerTasks = allTasks.Count;
            }
            else
            {
                var Tasks = await dbContext.Features.Where(i => i.Business_id == businessId)
               .Where(i => i.Status.Contains("New"))
               .Where(t => t.visible == true)
               .ToListAsync();

                CountOfNerTasks = Tasks.Count;
            }

            var unPublishedItems = await dbContext.Inventories.Where(i => i.IsProccessedInInventory
            && !i.Status.Contains("Auto Published") && i.Business_id == businessId)
            .ToListAsync();

            var unPublishedItemsOnMarket = await dbContext.Inventories.Where(i => i.IsProccessedInInventory
                && !i.IsPublishedOnMarketPlace && i.Business_id == businessId)
                .ToListAsync();


            CountUnPublishedOnMarket = unPublishedItemsOnMarket.Count;
            CountUnProccessedImages = unProccessedImages.Count;
            CountUnnScrapedItems = unScrapedItems.Count;
            CountUnPublishedItems = unPublishedItems.Count;

            return Ok(new
            {
                CountUnProccessedImages = CountUnProccessedImages,
                CountUnnScrapedItems = CountUnnScrapedItems,
                CountUnPublishedItems = CountUnPublishedItems,
                countOfNerTasks = CountOfNerTasks,
                countUnPublishedOnMarket = CountUnPublishedOnMarket
            });


        }
    }
}
