using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversalOrderController : ControllerBase
    {
        private readonly UniversalDbContext db;
        public UniversalOrderController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetAllOrderStatus(int businessId)
        {
            var orderStatus = await db.UniversalOrderStatus
                .Where(os => os.Business_id.Equals(businessId))
                .ToListAsync();
            return Ok(orderStatus);
        }

        [HttpGet("GetAllOrderStatusByService/{serviceId}")]
        public async Task<IActionResult> GetAllOrderStatusByService(int serviceId)
        {
            var orderStatus = await db.UniversalOrderStatus
                .Where(os => os.Service_id.Equals(serviceId))
                .ToListAsync();
            return Ok(orderStatus);
        }
    }
}
