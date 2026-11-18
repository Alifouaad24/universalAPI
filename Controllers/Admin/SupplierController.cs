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
    public class SupplierController : ControllerBase
    {
        private readonly UniversalDbContext _context;
        public SupplierController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpGet("{consumerId}")]
        public async Task<IActionResult> GetSuppliers(int consumerId)
        {
            var suppliers = await _context.Suppliers
                .Include(s => s.ConsumerBusiness)
                .Include(s => s.ProviderBusiness)
                .Include(s => s.Service)
                .Include(s => s.Platform)
                .Where(s => s.ConsumerId == consumerId).ToListAsync();
            return Ok(suppliers);
        }

        [HttpGet("GetConsumers/{ProviderId}")]
        public async Task<IActionResult> GetConsumers(int ProviderId)
        {
            var suppliers = await _context.Suppliers
                .Include(s => s.ConsumerBusiness)
                .Include(s => s.ProviderBusiness)
                .Include(s => s.Service)
                .Include(s => s.Platform)
                .Where(s => s.ProviderId == ProviderId).ToListAsync();
            return Ok(suppliers);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSupplier([FromBody] SupplierDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existRecord = await _context.Suppliers
                .Where(s => s.ConsumerId== model.ConsumerId &&
                s.ProviderId == model.ProviderId &&
                s.Platform_id == model.Platform_id &&
                s.ServiceId == model.ServiceId)
                .FirstOrDefaultAsync();
            if (existRecord != null) return Ok(existRecord);

            var supolier = new Supplier
            {
                ConsumerId = model.ConsumerId,
                ProviderId = model.ProviderId,
                ServiceId = model.ServiceId,
                Platform_id = model.Platform_id,
            };
            await _context.Suppliers.AddAsync(supolier);
            await _context.SaveChangesAsync();
            return Ok(supolier);
        }
    }
}
