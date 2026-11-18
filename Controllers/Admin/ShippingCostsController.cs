using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingCostsController : ControllerBase
    {
        private readonly UniversalDbContext _context;
        public ShippingCostsController(UniversalDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetShippCost(int businessId, int ShippTypeId)
        {
            var costs = await _context.ShippingCosts
                .Include(sc => sc.Business)
                .Include(sc => sc.Weight_Unit)

                .Include(sc => sc.Logistic).ThenInclude(l => l.Logistic_shippingType)
                .Include(sc => sc.Logistic).ThenInclude(l => l.Business)
                .Include(sc => sc.Logistic).ThenInclude(l => l.Currency)
                .Where(sc => sc.Business_id == businessId)
                .Where(sc => sc.Logistic.Logistic_shippingType
                .Any(x => x.ShippingTypeId == ShippTypeId)).ToListAsync();

            return Ok(costs);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewLogistic(LogisticModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newShipp = new ShippingCost
            {
                Business_id = model.Business_id,
                CostAmount = model.CostAmount,
                LogisticId = model.LogisticId,
                Weight_UnitId = model.Weight_UnitId,
                SaleAmount = model.SaleAmount,
                Weight_Value = model.Weight_Value,
            };

            await _context.ShippingCosts.AddAsync(newShipp);
            await _context.SaveChangesAsync();
            return Ok(newShipp);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditLogistic(int id, [FromBody] LogisticModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var logistic = await _context.ShippingCosts.FindAsync();

            if (logistic == null) return BadRequest("not found logistic");

            logistic.Business_id = model.Business_id;
            logistic.CostAmount = model.CostAmount;
            logistic.LogisticId = model.LogisticId;
            logistic.Weight_UnitId = model.Weight_UnitId;
            logistic.SaleAmount = model.SaleAmount;
            logistic.Weight_Value = model.Weight_Value;

            await _context.SaveChangesAsync();
            return Ok(logistic);
        }
    }
}

public class LogisticModel
{

    public int? Business_id { get; set; }
    public int? Weight_UnitId { get; set; }
    public decimal? Weight_Value { get; set; }
    public int? LogisticId { get; set; }
    public decimal? CostAmount { get; set; }
    public decimal? SaleAmount { get; set; }
}