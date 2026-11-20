using AinAlfahd.BL;
using AinAlfahd.Data;
using AinAlfahd.Models;
using AinAlfahd.ModelsDTO;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Controllers.Admin;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AinAlfahd.Areas.Admin.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        MasterDBContext db;
        UniversalDbContext dbContext;
        UserManager<IdentityUserData> userManager;
        ICustomer icustomer;
        private readonly IConfiguration configuration;
        private Cloudinary cloudinary;
        string cloudName;
        string apiKey;
        string apiSecret;
        public CustomersController(MasterDBContext db, ICustomer icustomer1, UniversalDbContext dbContext, IConfiguration configuration, UserManager<IdentityUserData> userManager)
        {
            this.db = db;
            icustomer = icustomer1;
            this.dbContext = dbContext;
            cloudName = configuration["Claoudinary:cloudName"];
            apiKey = configuration["Claoudinary:apiKey"];
            apiSecret = configuration["Claoudinary:apiSecret"];

            cloudinary = new Cloudinary(new Account(
                cloudName,
                apiKey,
                apiSecret
            ));
            this.userManager = userManager;
        }

        [HttpGet("{businessId}")]
        [Authorize]
        public async Task<IActionResult> GetAllCustomers(int businessId)
        {
            var user = await userManager.GetUserAsync(User);
            bool isAdmin = await userManager.IsInRoleAsync(user, "Super Admin");

            var businessById = await dbContext.Businesses.FindAsync(businessId);


            if (businessById == null) return BadRequest("Business not found...");


            if (isAdmin && businessById!.Business_name.Contains("APX"))
            {
                var allCustomers = await dbContext.GlobalCustomers
                .Include(c => c.Address).ThenInclude(a => a.City)
                .Include(c => c.Address).ThenInclude(a => a.Area)
                .Include(c => c.Buseness_Customers).ThenInclude(b => b.Business)
                    .ToListAsync();
                return Ok(allCustomers);
            }

            var customers = await dbContext.GlobalCustomers
                .Include(c => c.Address)
                                .Include(c => c.Address).ThenInclude(a => a.City)
                .Include(c => c.Address).ThenInclude(a => a.Area)
                .Include(c => c.Buseness_Customers)
                    .ThenInclude(b => b.Business)
                .Where(c => c.Buseness_Customers
                    .Any(bc => bc.Business_id == businessId))
                .ToListAsync();

            return Ok(customers);
        }


        [HttpGet("GetAllCustomersForApp/{businessId}")]
        public async Task<IActionResult> GetAllCustomersForApp(int businessId)
        {
            var customers = await dbContext.GlobalCustomers
                .Include(c => c.Address)
                .Include(c => c.Buseness_Customers)
                    .ThenInclude(b => b.Business)
                .Where(c => c.Buseness_Customers
                    .Any(bc => bc.Business_id == businessId))
                .ToListAsync();

            return Ok(customers);
        }


        [HttpGet("SearchAboutCustomers/{wordSearch}")]
        [HttpGet("SearchAboutCustomers/{wordSearch}/{businessId}")]
        public async Task<IActionResult> SearchAboutCustomers(string wordSearch, int? businessId)
        {
            var query = dbContext.GlobalCustomers
                .Include(c => c.Country)
                .Include(c => c.Address)
                .Include(c => c.Buseness_Customers)
                    .ThenInclude(b => b.Business)
                .Where(c =>
                    c.CustomerMobile.Contains(wordSearch) ||
                    c.CustomerName.Contains(wordSearch));

            if (businessId.HasValue)
            {
                query = query.Where(c =>
                    c.Buseness_Customers.Any(b => b.Business_id == businessId));
            }

            var customers = await query.ToListAsync();

            return Ok(new
            {
                customers
            });
        }


        [HttpGet("SearchAboutDetectedCustomer/{wordSearch}")]
        [HttpGet("SearchAboutDetectedCustomer/{wordSearch}/{businessId}")]
        public async Task<IActionResult> SearchAboutDetectedCustomer(string wordSearch, int? businessId)
        {
            var query = dbContext.GlobalCustomers
                .Include(c => c.Country)
                .Include(c => c.Address).ThenInclude(a => a.City)
                .Include(c => c.Address).ThenInclude(a => a.Area)
                .Include(c => c.Buseness_Customers)
                    .ThenInclude(b => b.Business)
                .Where(c =>
                    c.CustomerMobile.Contains(wordSearch) ||
                    c.CustomerName.Contains(wordSearch));

            if (businessId.HasValue)
            {
                query = query.Where(c =>
                    c.Buseness_Customers.Any(b => b.Business_id == businessId));
            }

            var customer = await query.FirstOrDefaultAsync();

            return Ok(new
            {
                customer
            });
        }


        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] GlobalCustomerDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existCustomer = await dbContext.GlobalCustomers.Where(c => c.CustomerMobile == model.CustomerMobile).FirstOrDefaultAsync();

            if (existCustomer != null)
            {
                var bus_cust = await dbContext.Buseness_Customers
                    .Where(bc => bc.GlobalCustomerId == existCustomer.GlobalCustomerId)
                    .ToListAsync();

                if (bus_cust != null)
                {
                    dbContext.Buseness_Customers.RemoveRange(bus_cust);
                    await dbContext.SaveChangesAsync();
                }

                if (model.BusinessesIds != null && model.BusinessesIds.Count > 0)
                {
                    foreach (var middleId in model.BusinessesIds)
                    {
                        var businessCustomer = new Buseness_Customer
                        {
                            Business_id = middleId,
                            GlobalCustomerId = existCustomer.GlobalCustomerId,
                            IsOwner = false,
                        };

                        await dbContext.Buseness_Customers.AddAsync(businessCustomer);
                    }
                }

                await dbContext.SaveChangesAsync();

                return Ok(new
                {
                    msg = "Customer Binded With Business Successfuly ..."
                });
            }


            var customer = new GlobalCustomer
            {
                CustomerName = model.CustomerName,
                CustomerMobile = model.CustomerMobile,
                CustomerEmail = model.CustomerEmail,
                Country_id = model.Country_id,
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                Insert_by = "",
                visible = true
            };

            await dbContext.GlobalCustomers.AddAsync(customer);
            await dbContext.SaveChangesAsync();

            if (model.Address != null)
            {
                var address = new Address
                {
                    Line_1 = model.Address.Line_1,
                    Line_2 = model.Address.Line_2,
                    StateId = model.Address.StateId,
                    Post_code = model.Address.Post_code,
                    CityId = model.Address.CityId,
                    AreaId = model.Address.AreaId,
                    CountryId = model.Address.CountryId,
                    Land_Mark = model.Address.LandMark,
                    Us_City = model.Address.Us_City,
                    Insert_on = DateOnly.FromDateTime(DateTime.Now),
                    visible = true,
                    GlobalCustomerId = customer.GlobalCustomerId,
                    Insert_by = ""
                };
                await dbContext.Addresses.AddAsync(address);
                await dbContext.SaveChangesAsync();
            }

            if (model.BusinessesIds != null && model.BusinessesIds.Count > 0)
            {
                foreach (var middleId in model.BusinessesIds)
                {
                    var businessCustomer = new Buseness_Customer
                    {
                        Business_id = middleId,
                        GlobalCustomerId = customer.GlobalCustomerId,
                        IsOwner = false,
                    };

                    await dbContext.Buseness_Customers.AddAsync(businessCustomer);
                }
            }


            await dbContext.SaveChangesAsync();

            return Ok(customer);
        }



        [HttpPut("BindImagesWithCustomer/{id}")]
        public async Task<IActionResult> BindImagesWithCase(int id, [FromForm] ImagesCustomerRequests model)
        {

            var customer = await dbContext.GlobalCustomers
                .Where(c => c.GlobalCustomerId == id).FirstOrDefaultAsync();

            if (customer?.CustomerImage != null)
            {
                var publicId = customer.CustomerImage.Split('/').Last().Split('.').First();
                var deletionParams = new DeletionParams(publicId);
                await cloudinary.DestroyAsync(deletionParams);
            }


            using var stream = model.Image.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(model.Image.FileName, stream),
                Folder = "Customers"
            };
            var uploadResult = await cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {

                customer.CustomerImage = uploadResult.SecureUrl.ToString();

            }

            await dbContext.SaveChangesAsync();


            return Ok(customer);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomer(int id, [FromBody] GlobalCustomerDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var customer = await dbContext.GlobalCustomers
                .Include(c => c.Address)
                .Where(c => c.GlobalCustomerId == id)
                .FirstOrDefaultAsync();

            if (customer == null) return BadRequest("customer not found");


            customer.CustomerName = model.CustomerName;
            customer.CustomerMobile = model.CustomerMobile;
            customer.CustomerEmail = model.CustomerEmail;
            customer.customerCode = model.customerCode;

            var bus_cust = await dbContext.Buseness_Customers.Where(bc => bc.GlobalCustomerId == id).ToListAsync();
            if (bus_cust.Any())
            {
                dbContext.Buseness_Customers.RemoveRange(bus_cust);
                await dbContext.SaveChangesAsync();
            }

            foreach (var bus in model.BusinessesIds)
            {
                var bc = new Buseness_Customer
                {
                    GlobalCustomerId = id,
                    Business_id = bus,
                    IsOwner = false,
                };

                await dbContext.Buseness_Customers.AddAsync(bc);
            }

            await dbContext.SaveChangesAsync();

            return Ok(customer);
        }

        [HttpPut("AddNewAddressForCustomer/{customerId}")]
        public async Task<IActionResult> AddNewAddressForCustomer(int customerId, [FromBody] Universal_server.Models.Helper_models.AddressDto model)
        {
            var address = new Address
            {
                Line_1 = model.Line_1,
                Line_2 = model.Line_2,
                StateId = model.StateId,
                Post_code = model.Post_code,
                CityId = model.CityId,
                AreaId = model.AreaId,
                CountryId = model.CountryId,
                Land_Mark = model.LandMark,
                Us_City = model.Us_City,
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
                GlobalCustomerId = customerId,
                Insert_by = ""
            };
            await dbContext.Addresses.AddAsync(address);
            await dbContext.SaveChangesAsync();

            return Ok(address);
        }

        [HttpPut("UpdateAddressForCustomer/{addressId}")]
        public async Task<IActionResult> UpdateAddressForCustomer(int addressId, [FromBody] Universal_server.Models.Helper_models.AddressDto model)
        {
            var existAddress = await dbContext.Addresses.FindAsync(addressId);

            existAddress.Line_1 = model.Line_1;
            existAddress.Line_2 = model.Line_2;
            existAddress.StateId = model.StateId;
            existAddress.Post_code = model.Post_code;
            existAddress.CityId = model.CityId;
            existAddress.AreaId = model.AreaId;
            existAddress.CountryId = model.CountryId;
            existAddress.Land_Mark = model.LandMark;
            existAddress.Us_City = model.Us_City;

            await dbContext.SaveChangesAsync();

            return Ok(existAddress);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await dbContext.GlobalCustomers.FindAsync(id);
            if (result == null) return BadRequest("customer not found");

            var bs_cs = await dbContext.Buseness_Customers.Where(bc => bc.GlobalCustomerId == id).ToListAsync();
            var bs_Address = await dbContext.Addresses.Where(a => a.GlobalCustomerId == id).ToListAsync();

            if (bs_cs != null && bs_cs.Count > 0)
            {
                dbContext.Buseness_Customers.RemoveRange(bs_cs);
                await dbContext.SaveChangesAsync();
            }

            if (bs_Address != null && bs_Address.Count > 0)
            {
                dbContext.Addresses.RemoveRange(bs_Address);
                await dbContext.SaveChangesAsync();
            }

            dbContext.GlobalCustomers.Remove(result);
            await dbContext.SaveChangesAsync();
            return Ok(result);
        }

        [HttpDelete("DeleteAddress/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await dbContext.Addresses.FindAsync(id);
            if (result == null) return BadRequest("Address not found");
            dbContext.Addresses.Remove(result);
            await dbContext.SaveChangesAsync();
            return Ok(result);

        }

        [HttpGet("SearchAboutCustomersByCode/{code}")]
        public async Task<IActionResult> SearchAboutCustomersByCode(string code)
        {
            var query = await dbContext.GlobalCustomers
                .Include(c => c.Country)
                .Include(c => c.Address).ThenInclude(a => a.City)
                .Include(c => c.Address).ThenInclude(a => a.Area)
                .Where(c => c.customerCode == code).FirstOrDefaultAsync();

            return Ok(query);
        }

        [HttpGet("ReturnFirstCharacter")]
        public IActionResult ReturnFirstCharacter()
        {
            var character = 'B';
            return Ok(character);
        }



        ///  ///////////////////////////////////////////////////////////////////////////
        private async Task<IActionResult> SearchAboutCustomerApi(string wordSearch)
        {
            var wordSearch1 = Uri.UnescapeDataString(wordSearch);

            var customers = await db.Customers
                .Where(c => c.CustName.Contains(wordSearch1) || c.CustMob.Contains(wordSearch1))
                .ToListAsync();
            return Ok(customers);
        }

        private async Task<IActionResult> SearchAboutDetectedCustomerApi(string wordSearch1)
        {
            var wordSearch = Uri.UnescapeDataString(wordSearch1);

            var customer = await db.Customers.Include(c => c.City).Include(c => c.Area)
                .Where(c => c.CustName.Contains(wordSearch) || c.CustMob.Contains(wordSearch))
                .FirstOrDefaultAsync();


            return Ok(customer);
        }
    }
}
public class ImagesCustomerRequests
{
    public IFormFile Image { get; set; }
}