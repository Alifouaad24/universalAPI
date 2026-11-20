using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly UniversalDbContext _context;
        public CarController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarsBrandsModelsYears()
        {
            var brands = await _context.CarBrands
             .OrderBy(b => b.CarBrandName)
             .ToListAsync();

            foreach (var brand in brands)
            {
                if (!string.IsNullOrWhiteSpace(brand.CarBrandName))
                {
                    brand.CarBrandName =
                        char.ToUpper(brand.CarBrandName[0]) +
                        brand.CarBrandName.Substring(1).ToLower();
                }
            }

            var models = await _context.CarModels
                .OrderBy(m => m.CarModelName)
                .ToListAsync();

            foreach (var model in models)
            {
                if (!string.IsNullOrWhiteSpace(model.CarModelName))
                {
                    model.CarModelName =
                        char.ToUpper(model.CarModelName[0]) +
                        model.CarModelName.Substring(1).ToLower();
                }
            }
            var years = await _context.CarYears.OrderBy(y => y.CarYearNumber).ToListAsync();

            return Ok(new
            {
                brands = brands,
                models = models,
                years = years
            });

        }
    }
}
