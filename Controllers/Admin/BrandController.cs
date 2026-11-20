using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly UniversalDbContext context;
        public BrandController(UniversalDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetAllBrandsAsync(int businessId)
        {
            var brands = await context.CarBrands.Where(b => b.BusinessId == businessId).ToListAsync();
            return Ok(brands);
        }
    }
}
