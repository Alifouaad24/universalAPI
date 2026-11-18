using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : ControllerBase
    {
        UniversalDbContext context;
        public AssetController(UniversalDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAssetsAsync()
        {
            var assets = await context.Assets.ToListAsync();
            return Ok(assets);
        }


        [HttpPost]
        public async Task<IActionResult> AddAssetToBusiness(AssetBusinessDto model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var businessAsset = new Business_Asset
            {
                Account_Id = model.Account_Id,
                Business_id = model.Business_id,
                AssetId = model.AssetId,
            };

            await context.AddAsync(businessAsset);
            await context.SaveChangesAsync();

            return Ok(businessAsset);
        }

        [HttpDelete("{busAssetId}")]
        public async Task<IActionResult> DeleteAssetFromBusiness(int busAssetId)
        {
          var busAsset = await context.Business_Assets.FindAsync(busAssetId);
            if(busAsset == null) return BadRequest("Asset not found");

            context.Remove(busAsset);
            await context.SaveChangesAsync();

            return Ok(busAsset);
        }
    }
}
