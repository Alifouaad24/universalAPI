using Microsoft.AspNetCore.Authorization;
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
    public class AddressController : ControllerBase
    {
        UniversalDbContext db;
        public AddressController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAddresses()
        {
            var addresses = await db.Addresses.Where(b => b.visible == true).ToListAsync();
            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] AddressDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var address = new Address
            {
                Line_1 = model.Line_1,
                Line_2 = model.Line_2,
                State = model.State,
                Post_code = model.Post_code,
                City = model.City,
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
                Insert_by = ""
            };

            await db.Addresses.AddAsync(address);
            await db.SaveChangesAsync();
            return Ok(address);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditAddress(int id, [FromBody] AddressDto model)
        {
            var address = await db.Addresses.FindAsync(id);
            if (address == null) return NotFound();

            address.Line_1 = model.Line_1;
            address.Line_2 = model.Line_2;
            address.State = model.State;
            address.Post_code = model.Post_code;
            address.City = model.City;

            await db.SaveChangesAsync();
            return Ok(address);

        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await db.Addresses.FindAsync(id);
            if (address == null) return NotFound();

            address.visible = false;
            await db.SaveChangesAsync();
            return Ok(new
            {
                msg = "address deleted successfully! "
            });
        }
    }
}
