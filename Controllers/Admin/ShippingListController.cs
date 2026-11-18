using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;
using AinAlfahd.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingListController : ControllerBase
    {
        private UniversalDbContext db;
        public ShippingListController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShippingList()
        {
            var list = await db.ShippingLists
                .Include(s => s.FromCountry)
                .Include(s => s.ToCountry)
                .Include(s => s.ShippingType)
                .Include(s => s.Business)
                .ToListAsync();
            return Ok(list);
        }

        [HttpGet("{businessId}")]
        [HttpGet("{shippingType:int}/{businessId:int}")]
        public async Task<IActionResult> GetAllShippingListByShippingType(
            int businessId,
            int? shippingType = null)
        {
            var query = db.ShippingLists
                .Include(s => s.FromCountry)
                .Include(s => s.ToCountry)
                .Include(s => s.ShippingType)
                .Include(s => s.Business)
                .Where(l => l.Business_id == businessId);

            if (shippingType.HasValue)
            {
                query = query.Where(l => l.ShippingTypeId == shippingType.Value);
            }

            var list = await query.ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> AddShippingList(AddEditShippingList model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var list = new ShippingList
            {
                ToCountryId = model.ToCountryId,
                FromCountryId = model.FromCountryId,
                ShippingTypeId = model.ShippingTypeId,
                Business_id = model.businessId,
                NackName = model.Nackname
            };

            await db.ShippingLists.AddAsync(list);
            await db.SaveChangesAsync();

            return Ok(list);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditShippingList(int id, AddEditShippingList model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var list = await db.ShippingLists.FindAsync(id);
            if (list == null) return BadRequest(model);

            list.ToCountryId = model.ToCountryId;
            list.FromCountryId = model.FromCountryId;
            list.ShippingTypeId = model.ShippingTypeId;
            list.Business_id  = model.businessId;
            list.NackName = model.Nackname;

            await db.SaveChangesAsync();
            return Ok(list);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingList(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var tarrifs = await db.TariffDetails.Where(td => td.ShippingListId == id).ToListAsync();
            if(tarrifs != null && tarrifs.Count > 0)
            {
                db.TariffDetails.RemoveRange(tarrifs);
                await db.SaveChangesAsync();
            }

            var list = await db.ShippingLists.FindAsync(id);
            if (list == null) return BadRequest();

            db.ShippingLists.Remove(list);
            await db.SaveChangesAsync();

            return Ok(list);
        }
    }
}

public class AddEditShippingList
{
    public int? FromCountryId { get; set; }
    public string? Nackname { get; set; }
    public int? ToCountryId { get; set; }
    public int? ShippingTypeId { get; set; }
    public int? businessId { get; set; }

}
