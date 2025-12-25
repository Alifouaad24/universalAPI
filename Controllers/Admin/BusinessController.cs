using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly string cloudName;
        private readonly string apiKey;
        private readonly string apiSecret;
        IConfiguration configuration;
        public BusinessController(UniversalDbContext db, IConfiguration configuration)
        {
            this.db = db;
            cloudName = configuration!["Claoudinary:cloudName"]!;
            apiKey = configuration["Claoudinary:apiKey"]!;
            apiSecret = configuration["Claoudinary:apiSecret"]!;
            this.configuration = configuration;
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
        public async Task<IActionResult> AddBusiness([FromForm] BusinessDTO model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (Request.Form.ContainsKey("Services"))
            {
                var servicesJson = Request.Form["Services"].ToString();
                model.Services = JsonConvert.DeserializeObject<List<ServiceDto>>(servicesJson);
            }

            if (Request.Form.ContainsKey("BusinessTypeId"))
            {
                var typesJson = Request.Form["BusinessTypeId"].ToString();
                model.BusinessTypeId = JsonConvert.DeserializeObject<List<int>>(typesJson);
            }

            if (Request.Form.ContainsKey("AddressId"))
            {
                var addressesJson = Request.Form["AddressId"].ToString();
                model.AddressId = JsonConvert.DeserializeObject<List<int>>(addressesJson);
            }

            if (Request.Form.ContainsKey("address"))
            {
                var addressJson = Request.Form["address"].ToString();
                model.address = JsonConvert.DeserializeObject<List<AddressDto>>(addressJson);
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            var cloudinary = new Cloudinary(account);
            string imgUrl = "";

            if (model.BusinessLogo != null && model.BusinessLogo.Length > 0)
            {
                var fileName = $"Logo_{Guid.NewGuid()}{Path.GetExtension(model.BusinessLogo.FileName)}";
                using var stream = model.BusinessLogo.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, stream),
                    Folder = "Inventory_uploads",
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                imgUrl = uploadResult.SecureUrl?.ToString() ?? "";
            }

            if (model.address != null && model.address.Count > 0)
            {
                foreach(AddressDto addressDto in model.address)
                {
                    var address = new Address
                    {
                        Line_1 = addressDto.Line_1,
                        Line_2 = addressDto.Line_2,
                        StateId = addressDto.StateId,
                        Post_code = addressDto.Post_code,
                        CityId = addressDto.CityId,
                        AreaId = addressDto.AreaId,
                        CountryId = addressDto.CountryId,
                        Land_Mark = addressDto.LandMark,
                        Insert_on = DateOnly.FromDateTime(DateTime.Now),
                        visible = true,
                        Insert_by = ""
                    };

                    await db.Addresses.AddAsync(address);
                    await db.SaveChangesAsync();
                    model.AddressId ??= new List<int>();
                    model.AddressId.Add(address.Address_id);
                }
               
            }

            var business = new Business
            {
                Business_name = model.Business_name,
                CountryId = model.CountryId,
                Business_phone = model.Business_phone,
                Business_webSite = model.Business_webSite,
                Business_fb = model.Business_fb,
                Business_LogoUrl = imgUrl,
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

            if (model.Services?.Count > 0)
            {
                foreach (var ser in model.Services)
                {
                    var service = new Service
                    {
                        Description = ser.Description,
                        IsPublic = ser.IsPublic,
                        Service_icon = ser.Service_icon,
                        Service_Route = $"/Home/{ser.Description.ToLower()}"
                    };
                    await db.Services.AddAsync(service);
                    await db.SaveChangesAsync();

                    await db.Business_Services.AddAsync(new Business_Service
                    {
                        Business_id = business.Business_id,
                        Service_id = service.Service_id
                    });
                }
            }

            if (model.BusinessTypeId?.Count > 0)
            {
                var businessTypes = model.BusinessTypeId.Select(id => new Business_BusinessType
                {
                    Business_id = business.Business_id,
                    Business_type_id = id,
                    Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                    visible = true
                }).ToList();
                await db.Business_BusinessTypes.AddRangeAsync(businessTypes);
            }

            if (model.AddressId?.Count > 0)
            {
                var businessAddresses = model.AddressId.Select(id => new Business_Address
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


            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (Request.Form.ContainsKey("Services"))
            {
                var servicesJson = Request.Form["Services"].ToString();
                model.Services = JsonConvert.DeserializeObject<List<ServiceDto>>(servicesJson);
            }

            if (Request.Form.ContainsKey("BusinessTypeId"))
            {
                var typesJson = Request.Form["BusinessTypeId"].ToString();
                model.BusinessTypeId = JsonConvert.DeserializeObject<List<int>>(typesJson);
            }

            if (Request.Form.ContainsKey("AddressId"))
            {
                var addressesJson = Request.Form["AddressId"].ToString();
                model.AddressId = JsonConvert.DeserializeObject<List<int>>(addressesJson);
            }

            if (Request.Form.ContainsKey("address"))
            {
                var addressJson = Request.Form["address"].ToString();
                model.address = JsonConvert.DeserializeObject<List<AddressDto>>(addressJson);
            }

            var account = new Account(cloudName, apiKey, apiSecret);
            var cloudinary = new Cloudinary(account);
            string imgUrl = "";

            if (model.BusinessLogo != null && model.BusinessLogo.Length > 0)
            {
                var fileName = $"Logo_{Guid.NewGuid()}{Path.GetExtension(model.BusinessLogo.FileName)}";
                using var stream = model.BusinessLogo.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(fileName, stream),
                    Folder = "Inventory_uploads",
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                imgUrl = uploadResult.SecureUrl?.ToString() ?? "";
            }

            if (model.address != null && model.address.Count > 0)
            {
                foreach (AddressDto addressDto in model.address)
                {
                    var address = new Address
                    {
                        Line_1 = addressDto.Line_1,
                        Line_2 = addressDto.Line_2,
                        StateId = addressDto.StateId,
                        Post_code = addressDto.Post_code,
                        CityId = addressDto.CityId,
                        AreaId = addressDto.AreaId,
                        CountryId = addressDto.CountryId,
                        Land_Mark = addressDto.LandMark,
                        Insert_on = DateOnly.FromDateTime(DateTime.Now),
                        visible = true,
                        Insert_by = ""
                    };
                    await db.Addresses.AddAsync(address);
                    await db.SaveChangesAsync();
                    model.AddressId ??= new List<int>();
                    model.AddressId.Add(address.Address_id);
                }

            }

            business.Business_name = model.Business_name;
            business.CountryId = model.CountryId;
            business.Business_phone = model.Business_phone;
            business.Business_webSite = model.Business_webSite;
            business.Business_fb = model.Business_fb;
            business.Business_LogoUrl = imgUrl;
            business.Business_instgram = model.Business_instgram;
            business.Business_tiktok = model.Business_tiktok;
            business.Business_google = model.Business_google;
            business.Business_youtube = model.Business_youtube;
            business.Is_active = model.Is_active;
            business.Business_whatsapp = model.Business_whatsapp;
            business.Business_email = model.Business_email;
            business.Insert_by = "";
            business.Insert_on = DateOnly.FromDateTime(DateTime.UtcNow);
            business.visible = true;
            
            await db.SaveChangesAsync();

            if (model.Services?.Count > 0)
            {
                foreach (var ser in model.Services)
                {
                    var service = new Service
                    {
                        Description = ser.Description,
                        IsPublic = ser.IsPublic,
                        Service_icon = ser.Service_icon,
                        Service_Route = $"Home/{ser.Description}"
                    };
                    await db.Services.AddAsync(service);
                    await db.SaveChangesAsync();

                    await db.Business_Services.AddAsync(new Business_Service
                    {
                        Business_id = business.Business_id,
                        Service_id = service.Service_id
                    });
                }
            }

            if (model.BusinessTypeId?.Count > 0)
            {
                var businessTypes = model.BusinessTypeId.Select(id => new Business_BusinessType
                {
                    Business_id = business.Business_id,
                    Business_type_id = id,
                    Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                    visible = true
                }).ToList();
                await db.Business_BusinessTypes.AddRangeAsync(businessTypes);
            }

            if (model.AddressId?.Count > 0)
            {
                var businessAddresses = model.AddressId.Select(id => new Business_Address
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
