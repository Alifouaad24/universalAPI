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
    public class BusinessController : ControllerBase
    {
        UniversalDbContext db;
        public BusinessController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBusiness()
        {
            var businesses = await db.Businesses.ToListAsync();
            return Ok(businesses);
        }

        [HttpPost]
        public async Task<IActionResult> AddBusiness([FromBody] BusinessDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var business = new Business
            {
                Business_name = model.Business_name,
                CountryId = model.CountryId,
                Business_phone = model.Business_phone,
                Business_webSite = model.Business_webSite,
                Business_fb = model.Business_fb,
                Business_instgram = model.Business_instgram,
                Business_tiktok = model.Business_tiktok,
                Business_google = model.Business_google,
                Business_youtube = model.Business_youtube,
                Business_whatsapp = model.Business_whatsapp,
                Business_email = model.Business_email,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
            };

            await db.Businesses.AddAsync(business);

            var bus_busType = new Business_BusinessType
            {
                Business_id = business.Business_id,
                Business_type_id = model.BusinessTypeId,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
            };
            await db.Business_BusinessTypes.AddAsync(bus_busType);


            var busines_Address = new Business_Address
            {
                Business_id = business.Business_id,
                Address_id = model.AddressId,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
            };
            await db.Business_Addresses.AddAsync(busines_Address);
            await db.SaveChangesAsync();

            return Ok(business);
        }
    }
}
