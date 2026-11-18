using System.Linq;
using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;
using Universal_server.Services;
using System.Text.RegularExpressions;

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
        private readonly UserManager<IdentityUserData> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public BusinessController(UniversalDbContext db, IConfiguration configuration, UserManager<IdentityUserData> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.db = db;
            cloudName = configuration!["Claoudinary:cloudName"]!;
            apiKey = configuration["Claoudinary:apiKey"]!;
            apiSecret = configuration["Claoudinary:apiSecret"]!;
            this.configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllBusiness()
        {
            var businesses = await db.Businesses.Include(b => b.Country)
                .Include(b => b.Business_Services).ThenInclude(bs => bs.Service)
                .Include(b => b.BusinessAddresses).ThenInclude(ba => ba.Address)
                .Include(b => b.Buseness_Customers).ThenInclude(bc => bc.GlobalCustomer)
                .Include(b => b.Business_Activitiy).ThenInclude(bc => bc.Activiity)
                .Include(b => b.BusinessTypes).ThenInclude(ba => ba.BusinessType)
                .Include(b => b.ProviderBusinessRelations)
                .Include(b => b.ConsumerBusinessRelations)
                .Include(b => b.UsersBusinesses).ThenInclude(ub => ub.UserData)
                .Include(b => b.Business_Assets).ThenInclude(ba => ba.Asset)
                .Include(b => b.System_Business_customers).ThenInclude(ub => ub.GlobalSystem)
                .Where(b => b.visible == true).ToListAsync();
            return Ok(businesses);
        }

        [HttpGet("GetAllBusinessForTask/{id}")]
        public async Task<IActionResult> GetAllBusinessForTask(int id)
        {
            var businerss = await db.Businesses.FindAsync(id);
            if (businerss.Role.Contains("Manager"))
            {
                var allBusinesses = await db.Businesses.Include(b => b.Country)
                .Where(b => b.visible == true).ToListAsync();
                return Ok(allBusinesses);
            }

            var businesses = await db.Businesses.Include(b => b.Country)
                .Where(b => b.Business_id == id)
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
                        Us_City = addressDto.Us_City,
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
                Role = "Normal"
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
                        Service_Route = $"/Home/{ser.Description.Replace(" ", "-").ToLower()}"
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

            if(model.SetItConsumer == true)
            {
                var supolier = new Supplier
                {
                    ConsumerId = business.Business_id,
                    ProviderId = model.ProviderBusiness,
                };
                await db.Suppliers.AddAsync(supolier);
                await db.SaveChangesAsync();
            }

            await db.SaveChangesAsync();
            return Ok(business);
        }

        [HttpPut("AddActivitiesToBusiness")]
        public async Task<IActionResult> AddActivitiesToBusiness([FromBody] AddActivitiesToBusiness model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentBusiness = await db.Businesses.FindAsync(model.Business_id);
            if (currentBusiness == null) return BadRequest("Business not found");

            var businessAct = await db.Business_Activitiies.Where(ba => ba.Business_id == model.Business_id && model.ActivitiesIds.Contains(ba.Activity_id)).ToListAsync();
            if (businessAct != null)
            {
                db.Business_Activitiies.RemoveRange(businessAct);
                await db.SaveChangesAsync();
            }

            foreach (var id in model.ActivitiesIds)
            {
                var newActivityBus = new Business_Activitiy
                {
                    Activity_id = id,
                    Business_id = currentBusiness.Business_id,

                };
                await db.Business_Activitiies.AddAsync(newActivityBus);
            }
            await db.SaveChangesAsync();
            return Ok(currentBusiness);
        }



        [HttpPut("SetServingToBusiness")]
        public async Task<IActionResult> SetServingToBusiness([FromBody] SetServingToBusiness model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentBusiness = await db.Businesses.FindAsync(model.Business_id);
            if (currentBusiness == null) return BadRequest("Business not found");

            foreach (var id in model.consumerBusinessIds)
            {
                var newRelBus = new BusinessRelation
                {
                    ProviderBusinessId = model.Business_id,
                    ConsumerBusinessId = id,
                };
                try
                {
                    await db.BusinessRelations.AddAsync(newRelBus);
                }
                catch (Exception ex) { }

            }
            await db.SaveChangesAsync();
            return Ok(currentBusiness);
        }




        [HttpPut("AddCustomesToBusiness")]
        public async Task<IActionResult> AddCustomesToBusiness([FromBody] AddCustomeToBusiness model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentBusiness = await db.Businesses.FindAsync(model.Business_id);
            if (currentBusiness == null) return BadRequest("Business not found");

            var customer = await db.GlobalCustomers
                .Where(c => c.CustomerMobile == model.GlobalCustomerDto.CustomerMobile)
                .FirstOrDefaultAsync();

            if (customer != null)
            {
                var businessCustomer = new Buseness_Customer
                {
                    Business_id = model.Business_id,
                    GlobalCustomerId = customer.GlobalCustomerId
                };
                await db.Buseness_Customers.AddAsync(businessCustomer);
            }
            else
            {



                var newCustomer = new GlobalCustomer
                {
                    CustomerName = model.GlobalCustomerDto.CustomerName,
                    CustomerMobile = model.GlobalCustomerDto.CustomerMobile,
                    CustomerEmail = model.GlobalCustomerDto.CustomerEmail,
                    Country_id = model.GlobalCustomerDto.Country_id,
                    Insert_on = DateOnly.FromDateTime(DateTime.Now),
                    Insert_by = "",
                    visible = true
                };

                await db.GlobalCustomers.AddAsync(newCustomer);
                await db.SaveChangesAsync();

                var address = new Address
                {
                    Line_1 = model.GlobalCustomerDto.Address.Line_1,
                    Line_2 = model.GlobalCustomerDto.Address.Line_2,
                    StateId = model.GlobalCustomerDto.Address.StateId,
                    Post_code = model.GlobalCustomerDto.Address.Post_code,
                    CityId = model.GlobalCustomerDto.Address.CityId,
                    AreaId = model.GlobalCustomerDto.Address.AreaId,
                    CountryId = model.GlobalCustomerDto.Address.CountryId,
                    Land_Mark = model.GlobalCustomerDto.Address.LandMark,
                    Us_City = model.GlobalCustomerDto.Address.Us_City,
                    Insert_on = DateOnly.FromDateTime(DateTime.Now),
                    GlobalCustomerId = newCustomer.GlobalCustomerId,
                    visible = true,
                    Insert_by = ""
                };
                await db.Addresses.AddAsync(address);
                await db.SaveChangesAsync();

                var businessCustomer = new Buseness_Customer
                {
                    Business_id = model.Business_id,
                    GlobalCustomerId = newCustomer.GlobalCustomerId,
                    IsOwner = false
                };
                await db.Buseness_Customers.AddAsync(businessCustomer);
            }

            await db.SaveChangesAsync();
            return Ok(currentBusiness);
        }


        [HttpPut("SetCustomerAsBusinessOwner")]
        public async Task<IActionResult> SetCustomerAsBusinessOwner([FromBody] SetCustomerAsOwner model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var currentBusiness = await db.Businesses.FindAsync(model.Business_id);
            if (currentBusiness == null) return BadRequest("Business not found");

            var customer = await db.GlobalCustomers.FindAsync(model.GlobalCustomerId);

            if (customer == null) return BadRequest();

            var businessCustomer = await db.Buseness_Customers
                .Where(bc => bc.Business_id == model.Business_id && bc.GlobalCustomerId == model.GlobalCustomerId)
                .FirstOrDefaultAsync();

            if (businessCustomer != null)
                businessCustomer.IsOwner = true;

            await db.SaveChangesAsync();
            return Ok(currentBusiness);
        }

        [HttpDelete("RemoveActivityFromBusiness/{activityId}/{businessId}")]
        public async Task<IActionResult> RemoveActivityFromBusiness(int activityId, int businessId)
        {
            var bus_activity = await db.Business_Activitiies.Where(ba => ba.Activity_id == activityId && ba.Business_id == businessId).FirstOrDefaultAsync();
            if (bus_activity == null) return BadRequest("Activity not found");

            db.Business_Activitiies.Remove(bus_activity);

            await db.SaveChangesAsync();
            return Ok(bus_activity);
        }


        [HttpDelete("RemoveCustomerFromBusiness/{busCusId}")]
        public async Task<IActionResult> RemoveCustomerFromBusiness(int busCusId)
        {
            var businessCust = await db.Buseness_Customers.FindAsync(busCusId);
            if (businessCust == null) return BadRequest("businessOwner not found");

            db.Buseness_Customers.Remove(businessCust);
            await db.SaveChangesAsync();
            return Ok(new
            {
                msg = "businessOwner removed successfuly"
            });
        }

        [HttpDelete("RemoveCustomerFromBusinessAsOwner/{busCusId}")]
        public async Task<IActionResult> RemoveCustomerFromBusinessAsOwner(int busCusId)
        {
            var businessOwner = await db.Buseness_Customers.FindAsync(busCusId);
            if (businessOwner == null) return BadRequest("businessOwner not found");

            businessOwner.IsOwner = false;
            await db.SaveChangesAsync();
            return Ok(new
            {
                msg = "businessOwner removed successfuly"
            });
        }

        [HttpDelete("RemoveServiceFromBusiness/{serviceId}/{businessId}")]
        public async Task<IActionResult> RemoveServiceFromBusiness(int serviceId, int businessId)
        {
            var businessService = await db.Business_Services.SingleOrDefaultAsync(bs => bs.Service_id == serviceId && bs.Business_id == businessId);
            if (businessService == null) return BadRequest("service not found");

            db.Business_Services.Remove(businessService);
            await db.SaveChangesAsync();

            return Ok(new
            {
                msg = "businessService removed successfuly"
            });
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBusiness(int id, [FromForm] BusinessDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var business = await db.Businesses
                .Include(b => b.BusinessTypes)
                .Include(b => b.BusinessAddresses)
                .FirstOrDefaultAsync(b => b.Business_id == id);

            if (business == null)
                return NotFound();

            // ===================== Parse JSON =====================
            if (Request.Form.ContainsKey("Services"))
                model.Services = JsonConvert.DeserializeObject<List<ServiceDto>>(Request.Form["Services"]);

            if (Request.Form.ContainsKey("BusinessTypeId"))
                model.BusinessTypeId = JsonConvert.DeserializeObject<List<int>>(Request.Form["BusinessTypeId"]);

            if (Request.Form.ContainsKey("AddressId"))
                model.AddressId = JsonConvert.DeserializeObject<List<int>>(Request.Form["AddressId"]);

            if (Request.Form.ContainsKey("address"))
                model.address = JsonConvert.DeserializeObject<List<AddressDto>>(Request.Form["address"]);

            // ===================== Image Upload =====================
            if (model.BusinessLogo != null && model.BusinessLogo.Length > 0)
            {
                var account = new Account(cloudName, apiKey, apiSecret);
                var cloudinary = new Cloudinary(account);

                var fileName = $"Logo_{Guid.NewGuid()}{Path.GetExtension(model.BusinessLogo.FileName)}";
                using var stream = model.BusinessLogo.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = "_uploads"
                };

                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                business.Business_LogoUrl = uploadResult.SecureUrl?.ToString();
            }

            // ===================== Update Business =====================
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
            business.Is_active = model.Is_active;
            business.visible = true;

            await db.SaveChangesAsync();

            // ===================== Clear Old Relations =====================
            db.Business_BusinessTypes.RemoveRange(
                db.Business_BusinessTypes.Where(x => x.Business_id == id));

            db.Business_Addresses.RemoveRange(
                db.Business_Addresses.Where(x => x.Business_id == id));

            await db.SaveChangesAsync();

            // ===================== Services =====================
            if (model.Services?.Any() == true)
            {
                foreach (var ser in model.Services)
                {
                    var service = new Service
                    {
                        Description = ser.Description,
                        IsPublic = ser.IsPublic,
                        Service_icon = ser.Service_icon,
                        Service_Route = $"Home/{ser.Description.Replace(" ", "-").ToLower()}"
                    };

                    db.Services.Add(service);
                    await db.SaveChangesAsync();

                    db.Business_Services.Add(new Business_Service
                    {
                        Business_id = id,
                        Service_id = service.Service_id
                    });
                }
            }

            // ===================== Business Types =====================
            if (model.BusinessTypeId?.Any() == true)
            {
                db.Business_BusinessTypes.AddRange(
                    model.BusinessTypeId.Select(typeId => new Business_BusinessType
                    {
                        Business_id = id,
                        Business_type_id = typeId,
                        visible = true,
                        Insert_on = DateOnly.FromDateTime(DateTime.UtcNow)
                    })
                );
            }

            // ===================== Addresses =====================
            if (model.address?.Any() == true)
            {
                foreach (var dto in model.address)
                {
                    var address = new Address
                    {
                        Line_1 = dto.Line_1,
                        Line_2 = dto.Line_2,
                        CityId = dto.CityId,
                        StateId = dto.StateId,
                        AreaId = dto.AreaId,
                        CountryId = dto.CountryId,
                        Post_code = dto.Post_code,
                        Us_City = dto.Us_City,
                        Land_Mark = dto.LandMark,
                        Insert_on = DateOnly.FromDateTime(DateTime.UtcNow),
                        visible = true
                    };

                    db.Addresses.Add(address);
                    await db.SaveChangesAsync();

                    db.Business_Addresses.Add(new Business_Address
                    {
                        Business_id = id,
                        Address_id = address.Address_id,
                        visible = true,
                        Insert_on = DateOnly.FromDateTime(DateTime.UtcNow)
                    });
                }
            }
            else
            {
                if (model.AddressId != null && model.AddressId.Count > 0)
                {
                    foreach (var idd in model.AddressId)
                    {
                        db.Business_Addresses.Add(new Business_Address
                        {
                            Business_id = id,
                            Address_id = idd,
                            visible = true,
                            Insert_on = DateOnly.FromDateTime(DateTime.UtcNow)
                        });
                    }
                }
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



        [HttpDelete("RemoveBusinessRelation/{BusinessRelationId}")]
        public async Task<IActionResult> RemoveBusinessRelation(int BusinessRelationId)
        {
            var busRelation = await db.BusinessRelations.FindAsync(BusinessRelationId);
            if (busRelation == null) return BadRequest("not found");

            db.BusinessRelations.Remove(busRelation);
            await db.SaveChangesAsync();
            return Ok(new
            {
                msg = "busRelation deleted succesfuly! ..."
            });
        }

        [HttpPost("AddUsers")]
        public async Task<IActionResult> AddUsers([FromBody] RegisterUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
                return BadRequest("Username already exists");

            var user = new IdentityUserData
            {
                UserName = model.UserName,
                Email = model.Email
            };

            string password = await GenerateRandomPassword.GeneratePassword();

            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                user.UserPassword = password;
                await _userManager.UpdateAsync(user);

                if (model.Roles?.Any() == true)
                {
                    foreach (var role in model.Roles)
                    {
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                }

                if (model.BusinessIds?.Any() == true)
                {
                    var validBusinessIds = await db.Businesses
                        .Where(b => model.BusinessIds.Contains(b.Business_id))
                        .Select(b => b.Business_id)
                        .ToListAsync();

                    foreach (var businessId in validBusinessIds)
                    {
                        await db.UsersBusinesseses.AddAsync(new UsersBusinesses
                        {
                            UserId = user.Id,
                            Business_id = businessId
                        });
                    }

                    await db.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "User added successfully",
                    password
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the user",
                    error = ex.Message
                });
            }
        }

        [HttpGet("GetUserGroups/{userId}")]
        public async Task<IActionResult> GetUserGroups(string userId)
        {
            var userGroups = await db.BusinessGroups
                .Include(b => b.BusinessGroupBusinesses)
                    .ThenInclude(bg => bg.Business)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return Ok(userGroups);
        }

        [HttpPost("addNewGroup")]
        public async Task<IActionResult> addNewGroup(AddNewGroup model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.businessesIds == null || !model.businessesIds.Any())
                return BadRequest("Please select at least one business.");

            var newGroup = new BusinessGroup
            {
                GroupName = model.name,
                UserId = model.userId,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            };

            await db.BusinessGroups.AddAsync(newGroup);
            await db.SaveChangesAsync();

            foreach (var businessId in model.businessesIds.Distinct())
            {

                var bgs = new BusinessGroupBusinesses
                {
                    Business_id = businessId,
                    BusinessGroupsId = newGroup.BusinessGroupId
                };

                await db.BusinessGroupBusinesses.AddAsync(bgs);
                
            }

            await db.SaveChangesAsync();

            return Ok(newGroup);
        }


        [HttpPost("editGroup/{groupId}")]
        public async Task<IActionResult> addNewGroup(int groupId, AddNewGroup model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.businessesIds == null || !model.businessesIds.Any())
                return BadRequest("Please select at least one business.");

            var currentGroup = await db.BusinessGroups.FindAsync(groupId);

            if (currentGroup == null) return NotFound(" Group not founf");


            currentGroup.GroupName = model.name;
            currentGroup.UserId = model.userId;
            currentGroup.CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

            var businessesGroup = await db.BusinessGroupBusinesses.Where(bbgb => bbgb.BusinessGroupsId == groupId).ToListAsync();

            if(businessesGroup != null && businessesGroup.Count > 0)
            {
                db.BusinessGroupBusinesses.RemoveRange(businessesGroup);
            }
            

            await db.SaveChangesAsync();

            foreach (var businessId in model.businessesIds.Distinct())
            {

                var bgs = new BusinessGroupBusinesses
                {
                    Business_id = businessId,
                    BusinessGroupsId = currentGroup.BusinessGroupId
                };

                await db.BusinessGroupBusinesses.AddAsync(bgs);

            }

            await db.SaveChangesAsync();

            return Ok(currentGroup);
        }


        [HttpGet("getAllMedia/{businessId}")]
        public async Task<IActionResult> getAllMedia(int businessId)
        {
            var media = await db.Multimedias
                .Where(m => m.Business_id == businessId)
                .OrderByDescending(m => m.MultimediaId)
                .Select(m => new { m.MultimediaId, m.Business_id, m.fileUrl })
                .ToListAsync();

            return Ok(media);
        }

        [HttpPost("UpdateMedia")]
        [RequestSizeLimit(25 * 1024 * 1024)]
        public async Task<IActionResult> UpdateMedia([FromForm] UploadMedia model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model.file == null || model.file.Length == 0) return BadRequest("No file selected");

            var business = await db.Businesses.FindAsync(model.businessId);
            if (business == null) return NotFound("Business not found");

            var cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
            var fileName = Path.GetFileName(model.file.FileName);
            using var stream = model.file.OpenReadStream();
            var description = new FileDescription(fileName, stream);

            UploadResult result;
            if ((model.file.ContentType ?? "").StartsWith("video/"))
            {
                result = await cloudinary.UploadAsync(new VideoUploadParams
                {
                    File = description,
                    Folder = business.Business_name,
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                });
            }
            else
            {
                // الصور وملفات PDF
                result = await cloudinary.UploadAsync(new ImageUploadParams
                {
                    File = description,
                    Folder = business.Business_name,
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = false
                });
            }

            if (result.Error != null || result.SecureUrl == null)
                return StatusCode(502, result.Error?.Message ?? "Upload to Cloudinary failed");

            var media = new Multimedia
            {
                Business_id = business.Business_id,
                fileUrl = result.SecureUrl.ToString(),
            };

            await db.Multimedias.AddAsync(media);
            await db.SaveChangesAsync();

            return Ok(new { media.MultimediaId, media.Business_id, media.fileUrl });
        }

        [HttpDelete("DeleteMedia/{id}")]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var media = await db.Multimedias.FindAsync(id);
            if (media == null) return NotFound();

            var (publicId, type) = ParseCloudinaryUrl(media.fileUrl);
            if (publicId != null)
            {
                var cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
                var result = await cloudinary.DestroyAsync(
                    new DeletionParams(publicId) { ResourceType = type, Invalidate = true });

                if (result.Result != "ok" && result.Result != "not found")
                    return StatusCode(502, "Failed to delete file from Cloudinary");
            }

            db.Multimedias.Remove(media);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("deleteGroup/{id}")]
        public async Task<IActionResult> deleteGroup(int id)
        {
            var group = await db.BusinessGroups.FindAsync(id);
            if(group == null) return NotFound();

            db.BusinessGroups.Remove(group);

            await db.SaveChangesAsync();
            return Ok();
        }

        /// يستخرج publicId ونوع الملف من رابط Cloudinary
        private static (string? publicId, ResourceType type) ParseCloudinaryUrl(string? url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return (null, ResourceType.Image);

            var segments = uri.AbsolutePath.Trim('/').Split('/').ToList();
            var uploadIndex = segments.IndexOf("upload");
            if (uploadIndex < 1) return (null, ResourceType.Image);

            var type = segments[uploadIndex - 1] switch
            {
                "video" => ResourceType.Video,
                "raw" => ResourceType.Raw,
                _ => ResourceType.Image
            };

            var rest = segments.Skip(uploadIndex + 1).ToList();
            if (rest.Count > 0 && Regex.IsMatch(rest[0], @"^v\d+$")) rest.RemoveAt(0); // رقم النسخة

            var path = Uri.UnescapeDataString(string.Join("/", rest));

            // publicId للصور والفيديو يكون بدون الامتداد
            if (type != ResourceType.Raw)
            {
                var dot = path.LastIndexOf('.');
                if (dot > path.LastIndexOf('/')) path = path[..dot];
            }

            return (path, type);
        }

        //[HttpDelete("DeleteMedia/{id}")]
        //public async Task<IActionResult> DeleteMedia(int id)
        //{
        //    var media = await db.Multimedias.FindAsync(id);
        //    if (media == null) return NotFound();

        //    // السجلات القديمة قبل إضافة PublicId لا يمكن حذفها من Cloudinary
        //    if (!string.IsNullOrEmpty(media.PublicId))
        //    {
        //        var cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
        //        var type = media.ResourceType switch
        //        {
        //            "video" => ResourceType.Video,
        //            "raw" => ResourceType.Raw,
        //            _ => ResourceType.Image
        //        };

        //        var result = await cloudinary.DestroyAsync(
        //            new DeletionParams(media.PublicId) { ResourceType = type, Invalidate = true });

        //        if (result.Result != "ok" && result.Result != "not found")
        //            return StatusCode(502, "Failed to delete file from Cloudinary");
        //    }

        //    db.Multimedias.Remove(media);
        //    await db.SaveChangesAsync();
        //    return Ok();
        //}

        //[HttpPut("BindBusinessWithPlatforms")]
        //public async Task<IActionResult> BindBusinessWithPlatforms(BindBusinessWithPlatforms model)
        //{
        //    foreach (var platformId in model.PlatformsIds)
        //    {
        //        var platform = await db.Platforms.FindAsync(platformId);
        //        if (platform != null)
        //            platform.BusinessId = model.Business_id;
        //    }

        //    await db.SaveChangesAsync();

        //    return Ok(model);
        //}

        //[HttpPut("UnBindBusinessWithPlatforms/{platformId}")]
        //public async Task<IActionResult> UnBindBusinessWithPlatforms(int platformId)
        //{
        //    var platform = await db.Platforms.FindAsync(platformId);
        //    if (platform != null) return NotFound();

        //    platform.BusinessId = null;

        //    await db.SaveChangesAsync();

        //    return Ok(platform);
        //}
    }

    public class AddActivitiesToBusiness
    {
        public int Business_id { get; set; }
        public List<int> ActivitiesIds { get; set; }
    }

    public class AddCustomeToBusiness
    {
        public int Business_id { get; set; }
        public GlobalCustomerDto GlobalCustomerDto { get; set; }
    }

    public class SetCustomerAsOwner
    {
        public int Business_id { get; set; }
        public int GlobalCustomerId { get; set; }
    }

    public class SetServingToBusiness
    {
        public int Business_id { get; set; }
        public List<int> consumerBusinessIds { get; set; }

    }

    public class BindBusinessWithPlatforms
    {
        public int Business_id { get; set; }
        public List<int> PlatformsIds { get; set; }
    }

    public class AddNewGroup
    {
        public string name { get; set; }
        public string userId { get; set; }
        public List<int> businessesIds { get; set; }
    }

    public class UploadMedia
    {
        public int businessId { get; set; }
        public IFormFile file { get; set; }
    }

}


