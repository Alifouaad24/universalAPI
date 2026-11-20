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
    public class ShippingTaarefController : ControllerBase
    {
        private UniversalDbContext context;

        public ShippingTaarefController(UniversalDbContext context)
        {
            this.context = context;
        }


        [HttpGet("GetAllTaarefs/{businessId}")]
        public async Task<IActionResult> GetAllTaarefs(int businessId)
        {
            var shippingTaarefs = await context.Taarefs
                .Include(t => t.Platform)
                .Include(t => t.TariffDetails).ThenInclude(td => td.Currency)
                .Include(t => t.TariffDetails).ThenInclude(td => td.ShippingList).ThenInclude(sl =>sl.FromCountry)
                .Include(t => t.TariffDetails).ThenInclude(td => td.ShippingList).ThenInclude(sl =>sl.ToCountry)
                .Include(t => t.TariffDetails).ThenInclude(td => td.ShippingList).ThenInclude(sl =>sl.ShippingType)
                .Include(t => t.TariffDetails).ThenInclude(td => td.Currency)
                .Where(t => t.IsActive)
                .Where(t => t.TariffDetails.Any(
                    td => td.ShippingList.Business_id == businessId))
                .ToListAsync();

            return Ok(shippingTaarefs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllTaarefById(int id)
        {
            var shippingTaarefs = await context.Taarefs
                .Include(bpc => bpc.Platform)
                .Where(bpc => bpc.IsActive == true)
                .Where(bpc => bpc.TaarefId == id)
                .FirstOrDefaultAsync();
            return Ok(shippingTaarefs);
        }

        [HttpGet("GetTaarefDetails/{id}")]
        public async Task<IActionResult> GetTaarefDetails(int id)
        {
            var shippingTaarefs = await context.TariffDetails
                .Include(bpc => bpc.Taaref)
                .Include(bpc => bpc.Currency)
                .Include(bpc => bpc.Unit)
                .Include(bpc => bpc.Service)
                .Include(bpc => bpc.ShippingList).ThenInclude(sh => sh.FromCountry)
                .Include(bpc => bpc.ShippingList).ThenInclude(sh => sh.ToCountry)
                .Include(bpc => bpc.ShippingList).ThenInclude(sh => sh.ShippingType)
                .Where(bpc => bpc.IsActive == true)
                .Where(bpc => bpc.TaarefId == id)
                .FirstOrDefaultAsync();
            return Ok(shippingTaarefs);
        }

        [HttpPost]
        public async Task<IActionResult> AddTaaref([FromBody] TaareffDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var tariff = new Taaref
            {
                IsActive = model.IsActive,
                Platform_id = model.PlatformId,
                TariffType = model.TariffType,
            };

            await context.Taarefs.AddAsync(tariff);
            await context.SaveChangesAsync();

            return Ok(tariff);

        }

        [HttpPost("AddTariffDetails")]
        public async Task<IActionResult> AddTariffDetails([FromBody] TariffDetails model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Invalid tariff details data");

                // التأكد من وجود التعرفة الرئيسية
                var tariff = await context.Taarefs
                    .FindAsync(model.TaarefId);

                if (tariff == null)
                    return NotFound("Tariff not found");

                // التحقق من نوع التعرفة
                if (tariff.TariffType == 1)
                {
                    if (model.Service_id == null || model.ShippingListId == null)
                        return BadRequest("Service and ShippingList are required");

                }
                else if (tariff.TariffType == 2)
                {
                    if (model.Service_id != null || model.ShippingListId != null)
                        return BadRequest("Service and ShippingList are not allowed for Product tariff");
                }
                else
                {
                    return BadRequest("Invalid tariff type");
                }

                if (model.CurrencyId == null || model.UnitId == null)
                    return BadRequest("Currency and Unit are required");

                if (model.Price < 0)
                    return BadRequest("Price cannot be negative");

                // إضافة التفاصيل
                model.TariffDetailsId = 0;

                await context.TariffDetails.AddAsync(model);

                await context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Tariff details added successfully",
                    tariffDetailsId = model.TariffDetailsId,
                    taarefId = model.TaarefId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error adding tariff details",
                    error = ex.Message
                });
            }
        }
    }
}
