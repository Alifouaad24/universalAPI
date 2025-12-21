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
            var businesses = await db.Businesses
                .Include(b => b.Activities)
                .Include(b => b.Country)
                .Include(b => b.BusinessAddresses).ThenInclude(ba => ba.Address)
                .Include(b => b.BusinessTypes).ThenInclude(ba => ba.BusinessType)
                .Where(b => b.visible == true).ToListAsync();
            return Ok(businesses);
        }

        [HttpPost]
        public async Task<IActionResult> AddBusiness([FromBody] BusinessDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (model.address != null &&
               !string.IsNullOrWhiteSpace(model.address.Post_code) &&
               !string.IsNullOrWhiteSpace(model.address.City) &&
               !string.IsNullOrWhiteSpace(model.address.State) &&
               !string.IsNullOrWhiteSpace(model.address.Line_2) &&
               !string.IsNullOrWhiteSpace(model.address.Line_1))
            {
                var address = new Address
                {
                    Line_1 = model.address.Line_1,
                    City = model.address.City,
                    State = model.address.State,
                    Line_2 = model.address.Line_2,
                    Post_code = model.address.Post_code,
                };

                await db.Addresses.AddAsync(address);
                await db.SaveChangesAsync();
                model.AddressId!.Add(address.Address_id);
            }

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
                Is_active = model.Is_active,
                Business_whatsapp = model.Business_whatsapp,
                Business_email = model.Business_email,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                visible = true,
            };

            await db.Businesses.AddAsync(business);
            await db.SaveChangesAsync();

            if (model?.BusinessTypeId?.Count > 0)
            {
                var businessTypes = model.BusinessTypeId.Select(id =>
                    new Business_BusinessType
                    {
                        Business_id = business.Business_id,
                        Business_type_id = id,
                        Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                        visible = true
                    }).ToList();

                await db.Business_BusinessTypes.AddRangeAsync(businessTypes);
            }


            if (model?.AddressId?.Count > 0)
            {
                var businessAddresses = model.AddressId.Select(id =>
                    new Business_Address
                    {
                        Business_id = business.Business_id,
                        Address_id = id,
                        Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                        visible = true
                    }).ToList();

                await db.Business_Addresses.AddRangeAsync(businessAddresses);
            }

            await db.SaveChangesAsync();

            return Ok(business);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusiness(int id, [FromBody] BusinessDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var business = await db.Businesses
                .Include(b => b.BusinessTypes)
                .Include(b => b.BusinessAddresses)
                .FirstOrDefaultAsync(b => b.Business_id == id);

            if (business == null)
                return NotFound();


            business.Business_name = model.Business_name;
            business.CountryId = model.CountryId;
            business.Business_phone = model.Business_phone;
            business.Business_webSite = model.Business_webSite;
            business.Business_fb = model.Business_fb;
            business.Business_instgram = model.Business_instgram;
            business.Business_tiktok = model.Business_tiktok;
            business.Business_google = model.Business_google;
            business.Business_youtube = model.Business_youtube;
            business.Business_whatsapp = model.Business_whatsapp;
            business.Business_email = model.Business_email;


            if (model?.BusinessTypeId != null)
            {
                db.Business_BusinessTypes.RemoveRange(business.BusinessTypes);

                var newTypes = model.BusinessTypeId.Select(idType =>
                    new Business_BusinessType
                    {
                        Business_id = business.Business_id,
                        Business_type_id = idType,
                        Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                        visible = true
                    }).ToList();

                await db.Business_BusinessTypes.AddRangeAsync(newTypes);
            }


            if (model?.AddressId != null)
            {
                db.Business_Addresses.RemoveRange(business.BusinessAddresses);

                var newAddresses = model.AddressId.Select(addrId =>
                    new Business_Address
                    {
                        Business_id = business.Business_id,
                        Address_id = addrId,
                        Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                        visible = true
                    }).ToList();

                await db.Business_Addresses.AddRangeAsync(newAddresses);
            }

            await db.SaveChangesAsync();
            return Ok(business);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBusiness(int id)
        {
            var business = await db.Businesses.FindAsync(id);
            if (business == null) return NotFound();

            business.visible = false;
            await db.SaveChangesAsync();
            return Ok(business);
        }

    }
}
