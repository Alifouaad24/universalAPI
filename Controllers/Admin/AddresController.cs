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
            var addresses = await db.Addresses.ToListAsync();
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
                City = model.City,
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
                Insert_by = ""
            };

            await db.Addresses.AddAsync(address);
            await db.SaveChangesAsync();
            return Ok(address);
        }
    }
}
