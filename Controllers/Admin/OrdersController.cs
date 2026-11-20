using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.BL;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Universal_server.firebase_options;
using System.Linq;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        IIOrders _iOrders;
        private readonly UserManager<IdentityUserData> _userManager;
        private Cloudinary cloudinary;
        private readonly IConfiguration configuration;
        IFirebaseNotificationService firebase;
        string cloudName;
        string apiKey;
        string apiSecret;
        UniversalDbContext dbContext;
        public OrdersController(IIOrders iOrders, UniversalDbContext dbContext, IConfiguration configuration, UserManager<IdentityUserData> userManager, IFirebaseNotificationService firebase)
        {
            _iOrders = iOrders;
            this.dbContext = dbContext;
            cloudName = configuration["Claoudinary:cloudName"];
            apiKey = configuration["Claoudinary:apiKey"];
            apiSecret = configuration["Claoudinary:apiSecret"];

            cloudinary = new Cloudinary(new Account(
                cloudName,
                apiKey,
                apiSecret
            ));
            _userManager = userManager;
            this.firebase = firebase;
        }

        [HttpGet("GetAllAssignTypes")]
        public async Task<IActionResult> GetAllAssignTypes()
        {
            var Assign_types = await dbContext.Assign_types.ToListAsync();
            return Ok(Assign_types);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderRequest model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _iOrders.AddOrder(model);
            return Ok(result);

        }


        [HttpGet("{busId}")]
        [HttpGet("{busId}/{serviceId}")]
        public async Task<IActionResult> GetAllOrders(int busId, int? serviceId)
        {
            var query = dbContext.GlobalOrders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.City)
                                    .Include(o => o.Customer)
                    .ThenInclude(c => c.Address).ThenInclude(a => a.Area)
                .Include(o => o.Item)
                    .ThenInclude(c => c.CarBrand)
                .Include(o => o.Item)
                    .ThenInclude(c => c.CarModel)
                .Include(o => o.Item)
                    .ThenInclude(c => c.CarYear)
                .Include(o => o.GlobalOrderDetail)
                    .ThenInclude(od => od.Item)
                .Include(o => o.GlobalOrderDetail)
                    .ThenInclude(od => od.Service)
                .Include(o => o.GlobalOrderDetail)
                    .ThenInclude(od => od.Case_Service_Notes)
                .Include(o => o.GlobalOrderDetail)
                    .ThenInclude(od => od.Complaint)
                .Include(o => o.GlobalOrderDetail)
                    .ThenInclude(od => od.Cause)
                .Include(o => o.GlobalOrderDetail)
                    .ThenInclude(od => od.Unit)
                .Include(o => o.Service)
                .Include(o => o.OrderStatus)
                .Include(o => o.orderImages)
                .Include(o => o.AssignerType)
                .Include(o => o.AssigneeType)
                .Where(o => o.Business_id == busId);

            if (serviceId.HasValue)
            {
                query = query.Where(o => o.Service_id == serviceId.Value);
            }

            var orders = await query
                .OrderByDescending(o => o.InsertDate)
                .AsSplitQuery()
                .ToListAsync();

            // ------------------------------------------------------------------
            // اعتماد الاسم النصي (Type) بدل الرقم الثابت — يشتغل بغض النظر
            // عن ترتيب الـ Assign_typeId الفعلي بقاعدة البيانات
            // ------------------------------------------------------------------
            var businessIds = new HashSet<int>();
            var systemIds = new HashSet<int>();
            var customerIds = new HashSet<int>();
            var userIds = new HashSet<string>();

            void CollectId(Assign_type? type, string? id)
            {
                if (type == null || string.IsNullOrWhiteSpace(id)) return;

                switch (type.Type?.Trim().ToLower())
                {
                    case "business":
                        if (int.TryParse(id, out var bId)) businessIds.Add(bId);
                        break;
                    case "system":
                        if (int.TryParse(id, out var sId)) systemIds.Add(sId);
                        break;
                    case "customer":
                        if (int.TryParse(id, out var cId)) customerIds.Add(cId);
                        break;
                    case "user":
                        userIds.Add(id);   // GUID، ما في تحويل
                        break;
                }
            }

            foreach (var o in orders)
            {
                CollectId(o.AssignerType, o.AssignerId);
                CollectId(o.AssigneeType, o.AssigneeId);
            }

            var businessNames = businessIds.Count > 0
                ? await dbContext.Businesses
                    .Where(b => businessIds.Contains(b.Business_id))
                    .ToDictionaryAsync(b => b.Business_id, b => b.Business_name)
                : new Dictionary<int, string>();

            var systemNames = systemIds.Count > 0
                ? await dbContext.GlobalSystems
                    .Where(s => systemIds.Contains(s.GlobalSystemId))
                    .ToDictionaryAsync(s => s.GlobalSystemId, s => s.GlobalSystemName)
                : new Dictionary<int, string>();

            var customerNames = customerIds.Count > 0
                ? await dbContext.GlobalCustomers
                    .Where(c => customerIds.Contains(c.GlobalCustomerId))
                    .ToDictionaryAsync(c => c.GlobalCustomerId, c => c.CustomerName)
                : new Dictionary<int, string>();

            var userNames = userIds.Count > 0
                    ? await _userManager.Users
                        .Where(u => userIds.Contains(u.Id))
                        .ToDictionaryAsync(u => u.Id, u => u.UserName)
                    : new Dictionary<string, string>();

            string? ResolveName(Assign_type? type, string? id)
            {

                if (type == null || string.IsNullOrWhiteSpace(id)) return null;

                return type.Type?.Trim().ToLower() switch
                {
                    "business" when int.TryParse(id, out var bId) => businessNames.GetValueOrDefault(bId),
                    "system" when int.TryParse(id, out var sId) => systemNames.GetValueOrDefault(sId),
                    "customer" when int.TryParse(id, out var cId) => customerNames.GetValueOrDefault(cId),
                    "user" => userNames.GetValueOrDefault(id),
                    _ => null
                };
            }

            var result = orders.Select(o => new
            {
                o.GlobalOrderId,
                o.Business_id,
                o.Service_id,
                o.Service,
                o.OrderStatus,
                o.Customer,
                o.Item,
                o.GlobalOrderDetail,
                o.orderImages,
                o.Notes,
                o.Schedule_dt,
                o.Schedule_time,
                o.InsertDate,
                o.CreatedDate,
                o.Cart_value,

                AssignerTypeId = o.AssignerTypeId,
                AssignerType = o.AssignerType,
                AssignerId = o.AssignerId,
                AssignerName = ResolveName(o.AssignerType, o.AssignerId),

                AssigneeTypeId = o.AssigneeTypeId,
                AssigneeType = o.AssigneeType,
                AssigneeId = o.AssigneeId,
                AssigneeName = ResolveName(o.AssigneeType, o.AssigneeId),
            });

            return Ok(result);
        }

        [HttpPost("AddGlobalOrder")]
        public async Task<IActionResult> AddGlobalOrder(GlobalOrderDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var assignerError = await ValidateAssignIdAsync(model.AssignerTypeId, model.AssignerId);
            if (assignerError != null) return BadRequest(assignerError);

            var assigneeError = await ValidateAssignIdAsync(model.AssigneeTypeId, model.AssigneeId);
            if (assigneeError != null) return BadRequest(assigneeError);

            if (model.packages != null && model.packages.Any())
            {
                var unitIds = model.packages.Select(p => p.unitId).Distinct().ToList();
                var existingUnitIds = await dbContext.Units
                    .Where(u => unitIds.Contains(u.UnitId))
                    .Select(u => u.UnitId)
                    .ToListAsync();

                var missingUnitIds = unitIds.Except(existingUnitIds).ToList();
                if (missingUnitIds.Any())
                    return BadRequest($"الوحدات التالية غير موجودة: {string.Join(", ", missingUnitIds)}");
            }

            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var checkExistOrder = await dbContext.GlobalOrders
                    .Where(o => o.GlobalCustomerId == model.GlobalCustomerId)
                    .Where(o => o.Business_id == 61)
                    .Where(o => o.OrderStatusId == model.OrderStatusId)
                    .FirstOrDefaultAsync();

                if (checkExistOrder == null)
                {
                    var order = new GlobalOrder
                    {
                        Business_id = model.Business_id,
                        GlobalCustomerId = model.GlobalCustomerId,
                        Schedule_dt = model.Schedule_dt,
                        Schedule_time = model.Schedule_time,
                        OrderStatusId = model.OrderStatusId,
                        Notes = model.Notes,
                        InsertDate = DateOnly.FromDateTime(DateTime.Now),
                        CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                        Service_id = model.Service_id,
                        AssigneeId = model.AssigneeId,
                        AssignerId = model.AssignerId,
                        CreatedBy = User?.Identity?.Name ?? "",
                        AssignerTypeId = model.AssignerTypeId,
                        AssigneeTypeId = model.AssigneeTypeId
                    };


                    await dbContext.GlobalOrders.AddAsync(order);
                    await dbContext.SaveChangesAsync();

                    if (model.packages != null && model.packages.Any())
                    {
                        var details = model.packages.Select(item => new GlobalOrderDetail
                        {
                            GlobalOrderId = order.GlobalOrderId,
                            UnitId = item.unitId,
                            UnitValue = item.value,
                            ImageUrl = item.imageUrl
                        }).ToList();
                        await dbContext.GlobalOrderDetails.AddRangeAsync(details);
                        await dbContext.SaveChangesAsync();
                    }
                }
                else
                {
                    if (model.packages != null && model.packages.Any())
                    {
                        var details = model.packages.Select(item => new GlobalOrderDetail
                        {
                            GlobalOrderId = checkExistOrder.GlobalOrderId,
                            UnitId = item.unitId,
                            UnitValue = item.value,
                            ImageUrl = item.imageUrl
                        }).ToList();
                        await dbContext.GlobalOrderDetails.AddRangeAsync(details);
                        await dbContext.SaveChangesAsync();
                    }
                }


                await transaction.CommitAsync();

                return Ok(new
                {
                    msg = "order added successfully"
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "حدث خطأ أثناء حفظ الطلب. حاول مرة أخرى.");
            }
        }

        // -----------------------------------------------------------------
        // تحقق من صحة الـ Assigner/Assignee id حسب نوعه قبل الحفظ
        // -----------------------------------------------------------------
        private async Task<string?> ValidateAssignIdAsync(int? typeId, string? id)
        {
            if (typeId == null || string.IsNullOrWhiteSpace(id)) return null; // اختياري، مسموح يكون فاضي

            var type = await dbContext.Assign_types.FindAsync(typeId.Value);
            if (type == null) return $"Assign type {typeId} not found.";

            switch (type.Type?.Trim().ToLower())
            {
                case "user":
                    var userExists = await _userManager.Users.AnyAsync(u => u.Id == id);
                    if (!userExists) return $"User with id '{id}' not found.";
                    break;

                case "business":
                    if (!int.TryParse(id, out var businessId) || !await dbContext.Businesses.AnyAsync(b => b.Business_id == businessId))
                        return $"Business with id '{id}' not found or invalid.";
                    break;

                case "customer":
                    if (!int.TryParse(id, out var customerId) || !await dbContext.GlobalCustomers.AnyAsync(c => c.GlobalCustomerId == customerId))
                        return $"Customer with id '{id}' not found or invalid.";
                    break;

                case "system":
                    if (!int.TryParse(id, out var systemId) || !await dbContext.GlobalSystems.AnyAsync(s => s.GlobalSystemId == systemId))
                        return $"System with id '{id}' not found or invalid.";
                    break;

                default:
                    return $"Unknown assign type: '{type.Type}'.";
            }

            return null; // valid
        }

        [HttpPost("AddCarGlobalOrder")]
        public async Task<IActionResult> AddCarGlobalOrder(CarRequests model)
        {
            int brandId = 0;
            int ModelId = 0;
            int YearId = 0;

            if (!ModelState.IsValid) return BadRequest(ModelState);


            var carBrandRec = await dbContext.CarBrands.Where(c => c.CarBrandName.ToLower().Contains(model.Brand.ToLower())).FirstOrDefaultAsync();
            if (carBrandRec == null)
            {
                var newCarBrandRec = new CarBrand
                {
                    CarBrandName = model.Brand.ToLower().Trim(),
                };

                await dbContext.CarBrands.AddAsync(newCarBrandRec);
                await dbContext.SaveChangesAsync();
                brandId = newCarBrandRec.CarBrandId;
            }
            else
            {
                brandId = carBrandRec.CarBrandId;
            }
            var carYearRec = await dbContext.CarYears.Where(cy => cy.CarYearNumber.Trim() == model.Year.Trim()).FirstOrDefaultAsync();
            if (carYearRec == null)
            {
                var newCarYearRec = new CarYear
                {
                    CarYearNumber = model.Year.Trim(),
                };

                await dbContext.CarYears.AddAsync(newCarYearRec);
                await dbContext.SaveChangesAsync();

                YearId = newCarYearRec.CarYearId;
            }
            else
            {
                YearId = carYearRec.CarYearId;
            }

            var carModelRec = await dbContext.CarModels.Where(cy => cy.CarModelName.Trim() == model.Model.Trim()).FirstOrDefaultAsync();
            if (carModelRec == null)
            {
                var newCarModelRec = new CarModel
                {
                    CarModelName = model.Model.Trim(),
                    CarBrandId = brandId,
                };

                await dbContext.CarModels.AddAsync(newCarModelRec);
                await dbContext.SaveChangesAsync();
                ModelId = newCarModelRec.CarModelId;
            }
            else
            {
                ModelId = carModelRec.CarModelId;
            }

            int carId;
            Item finalRecord = new Item();



            var car = await dbContext.Items
                .Where(c => c.CarBrandId == brandId)
                .Where(c => c.CarModelId == ModelId)
                .Where(c => c.CarYearId == YearId)
                .Where(c => c.VinNumber == model.VinNumber)
                .FirstOrDefaultAsync();

            if (car == null)
            {
                var carInfo = new Item
                {
                    VinNumber = model.VinNumber,
                    CarBrandId = brandId,
                    CarModelId = ModelId,
                    CarYearId = YearId,
                };

                await dbContext.Items.AddAsync(carInfo);
                await dbContext.SaveChangesAsync();

                carId = carInfo.ItemId;
                finalRecord = carInfo;
            }
            else
            {
                carId = car.ItemId;
                finalRecord = car;
            }



            var existOrder = await dbContext.GlobalOrders.FindAsync(model.orderId);

            existOrder.ItemId = carId;
            await dbContext.SaveChangesAsync();

            return Ok(finalRecord);
        }

        [HttpPost("AddDetailForOrder")]
        public async Task<IActionResult> AddServiceToGlobalOrder(int orderId, DetailReq model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newDetail = new GlobalOrderDetail
            {
                Cost = model.Cost,
                Discount = model.Discount,
                GlobalOrderId = orderId,
                Notes = model.Notes,
                Service_id = model.ServiceId,
                paid = model.paid,
                ItemId = model.ItemId,
                CauseId = model.CauseId,
                ComplaintId = model.ComplaintId,
            };

            await dbContext.GlobalOrderDetails.AddAsync(newDetail);
            await dbContext.SaveChangesAsync();

            var createdDetail = await dbContext.GlobalOrderDetails
                .Include(d => d.Service)
                .Include(d => d.Cause)
                .Include(d => d.Complaint)
                    .ThenInclude(c => c.Complaint_Value)
                        .ThenInclude(cv => cv.ComplaintType)
                .Include(d => d.OrderStatus)
                .FirstOrDefaultAsync(d => d.GlobalOrderDetailId == newDetail.GlobalOrderDetailId);

            return Ok(createdDetail);
        }

        [HttpPut("{orderId}")]
        public async Task<IActionResult> EditOrder(int orderId, GlobalOrderDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);

            var existOrder = await dbContext.GlobalOrders.FindAsync(orderId);
            if (existOrder == null) return BadRequest(ModelState);

            var assignerError = await ValidateAssignIdAsync(model.AssignerTypeId, model.AssignerId);
            if (assignerError != null) return BadRequest(assignerError);

            var assigneeError = await ValidateAssignIdAsync(model.AssigneeTypeId, model.AssigneeId);
            if (assigneeError != null) return BadRequest(assigneeError);

            existOrder.GlobalCustomerId = model.GlobalCustomerId;
            existOrder.Notes = model.Notes;
            existOrder.Schedule_dt = model.Schedule_dt;
            existOrder.Schedule_time = model.Schedule_time;
            existOrder.Business_id = model.Business_id;
            existOrder.OrderStatusId = model.OrderStatusId;
            existOrder.Modified_by = user?.UserName;
            existOrder.Service_id = model.Service_id ;
            existOrder.AssignerId = model.AssignerId ??  existOrder.AssignerId;
            existOrder.AssigneeId = model.AssigneeId ?? existOrder.AssigneeId;
            existOrder.AssigneeTypeId = model.AssigneeTypeId ?? existOrder.AssigneeTypeId;
            existOrder.AssignerTypeId = model.AssignerTypeId ?? existOrder.AssignerTypeId;

            await dbContext.SaveChangesAsync();

            return Ok(existOrder);
        }
        /////////////////////////
        ///
        [HttpPut("EditDetailForOrder")]
        public async Task<IActionResult> EditDetailForOrder(int orderDetailId, DetailReq request)
        {
            var orderDetail = await dbContext.GlobalOrderDetails.FindAsync(orderDetailId);
            if (orderDetail == null) return NotFound("Case not found");


            orderDetail.Service_id = request.ServiceId ?? orderDetail.Service_id;
            orderDetail.Notes = request.Notes ?? orderDetail.Notes;
            orderDetail.Cost = request.Cost ?? orderDetail.Cost;
            orderDetail.Discount = request.Discount ?? orderDetail.Discount;
            orderDetail.paid = request.paid ?? orderDetail.paid;
            orderDetail.ItemId = request.ItemId ?? orderDetail.ItemId;
            orderDetail.CauseId = request.CauseId ?? orderDetail.CauseId;
            orderDetail.ComplaintId = request.ComplaintId ?? orderDetail.ComplaintId;

            await dbContext.SaveChangesAsync();
            return Ok(orderDetail);
        }

        [HttpPut("UpdateETaskOrder/{id}")]
        public async Task<IActionResult> UpdateETaskOrder(int id, [FromBody] eTaskModel model)
        {
            var order = await dbContext.GlobalOrders.FindAsync(id);
            if (order == null) return NotFound("not found");

            order.Notes = model.Notes ?? order.Notes;
            order.OrderStatusId = model.StatusId;

            await dbContext.SaveChangesAsync();
            return Ok(order);
        }

        [HttpPut("changeStatusOrderService")]
        public async Task<IActionResult> changeStatusCaseService(int caseServicId, CaseServiceStatusRequest request)
        {
            var CaseService = await dbContext.OredesServices.FindAsync(caseServicId);
            if (CaseService == null) return NotFound("order service not found");


            CaseService.Resolved = request.resulved;

            await dbContext.SaveChangesAsync();
            return Ok(CaseService);
        }

        [HttpPut("UpdateOrderStatus/{orderId}/{statusId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, int statusId)
        {
            var order = await dbContext.GlobalOrders.FindAsync(orderId);
            if (order == null) return NotFound("not found");

            order.OrderStatusId = statusId;

            await dbContext.SaveChangesAsync();
            return Ok(order);
        }

        [HttpDelete("DeleteOrderDetail")]
        public async Task<IActionResult> DeleteOrderDetail(int orderDetailId)
        {
            var GlobalOrderDetails = await dbContext.GlobalOrderDetails.FindAsync(orderDetailId);
            if (GlobalOrderDetails == null) return NotFound("order detail not found");

            var notes = await dbContext.Case_Service_Notes.Where(n => n.GlobalOrderDetailId == orderDetailId).ToListAsync();

            if (notes.Count > 0)
            {
                dbContext.Case_Service_Notes.RemoveRange(notes);
            }

            dbContext.GlobalOrderDetails.Remove(GlobalOrderDetails);

            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                msg = "order detail deleted successfully"
            });
        }

        [HttpPost("AddServiceCaseNote")]
        public async Task<IActionResult> AddServiceCaseNote(int caseServicId, CaseServiceNotesRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var CaseService = await dbContext.GlobalOrderDetails.FindAsync(caseServicId);
            if (CaseService == null) return NotFound("Case service not found");

            foreach (var note in request.notes)
            {
                var caseNote = new Case_Service_Notes
                {
                    GlobalOrderDetailId = caseServicId,
                    Notes = note
                };
                await dbContext.Case_Service_Notes.AddAsync(caseNote);
            }

            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                msg = "Notes added successfully"
            });
        }

        [HttpDelete("DeleteCaseServiceNote")]
        public async Task<IActionResult> DeleteCaseServiceNote(int caseServicNoteId)
        {

            var CaseServiceNote = await dbContext.Case_Service_Notes.FindAsync(caseServicNoteId);
            if (CaseServiceNote == null) return NotFound("Case service note not found");

            dbContext.Case_Service_Notes.Remove(CaseServiceNote);

            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                msg = "Notes added successfully"
            });
        }

        [HttpPut("BindImagesWithOrder/{id}")]
        public async Task<IActionResult> BindImagesWithOrder(int id, [FromForm] ImagesCaseRequests model)
        {
            var caseImgs = new List<caseImage>();

            var order = await dbContext.GlobalOrders.FindAsync(id);

            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var image in model.Images)
                {
                    if (image.Length <= 0)
                        continue;

                    using var stream = image.OpenReadStream();

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(image.FileName, stream),
                        Folder = "Orders"
                    };
                    var uploadResult = await cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        caseImgs.Add(new caseImage
                        {
                            GlobalOrderId = order.GlobalOrderId,
                            ImageUrl = uploadResult.SecureUrl.ToString(),
                        });
                    }
                }
                await dbContext.caseImages.AddRangeAsync(caseImgs);
                await dbContext.SaveChangesAsync();
            }

            return Ok(caseImgs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await dbContext.GlobalOrders.FindAsync(id);
            if (order == null) return BadRequest();

            var order_detail = await dbContext.GlobalOrderDetails.Where(os => os.GlobalOrderId == id).ToListAsync();
            if (order_detail.Count > 0)
            {
                foreach (var item in order_detail)
                {
                    var notes = await dbContext.Case_Service_Notes.Where(csn => csn.GlobalOrderDetailId == item.GlobalOrderDetailId).ToListAsync();
                    dbContext.RemoveRange(notes);
                }
                dbContext.GlobalOrderDetails.RemoveRange(order_detail);
                await dbContext.SaveChangesAsync();
            }
            dbContext.GlobalOrders.Remove(order);

            await dbContext.SaveChangesAsync();

            return Ok(order);
        }

        ////////////////////////////////Universal Orders/////////////////////////////////////////
        ///

        [HttpGet("GetUniversalOrders")]
        public async Task<IActionResult> GetUniversalOrders(int businessId, int platformId)
        {
            var orders = await dbContext.GlobalOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderStatus)
                .Include(o => o.ShippingType)
                .Include(o => o.Supplier).ThenInclude(s=> s.ProviderBusiness)
                .Include(o => o.Platform)
                .Include(o => o.OrderItems)
                .Where(o => o.Business_id == businessId)
                .Where(o => o.Platform_id == platformId)
                .ToListAsync();
            return Ok(orders);
        }

        [Authorize]
        [HttpPost("AddUniversalOrder")]
        public async Task<IActionResult> AddUniversalOrder(UniversalOrderModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var user = await _userManager.GetUserAsync(User);

            var order = new GlobalOrder
            {
                GlobalCustomerId = model.GlobalCustomerId,
                Platform_id = model.Platform_id,
                Business_id = model.Business_id,
                ShippingTypeId = model.ShippingTypeId,
                Cart_value = model.CartValue,
                OrderStatusId = model.OrderStatusId,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                CreatedBy = user.UserName,
                SupplierId = model.providerId
            };

            await dbContext.GlobalOrders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            if (model.ItemInfos != null && model.ItemInfos.Count > 0)
            {
                foreach (var info in model.ItemInfos)
                {
                    var itemId = 0;
                    var item = await dbContext.Items.Where(i => i.ShortCode == info.ProductId).FirstOrDefaultAsync();
                    if (item != null) itemId = item.ItemId;
                    else
                    {
                        var newItem = new Item
                        {
                            Description = info.ProductName,
                            ShortCode = info.ProductId,
                        };

                        await dbContext.Items.AddAsync(newItem);
                        await dbContext.SaveChangesAsync();

                        var itemImage = new ItemImage
                        {
                            ImageUrl = info.ImageUrl,
                            IsMain = false,
                            UserId = user.Id,
                            ItemId = newItem.ItemId,
                        };
                        await dbContext.ItemImages.AddAsync(itemImage);
                        await dbContext.SaveChangesAsync();

                        itemId = newItem.ItemId;
                    }

                    var orderItem = new OrderItems
                    {
                        ItemsId = itemId,
                        GlobalOrderId = order.GlobalOrderId,
                        Size_id = info.size_id,
                        Price = info.price,
                        Service_id = model.Service_id,
                    };
                    await dbContext.OrderItems.AddAsync(orderItem);
                }
                await dbContext.SaveChangesAsync();
            }
            return Ok(order);
        }


        [Authorize]
        [HttpPut("AddComment/{id}")]
        public async Task<IActionResult> AddNewComment(int id, CommentBody model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var feature = await dbContext.GlobalOrders.FindAsync(id);

            if (feature == null)
                return NotFound("Feature not found");

            var comment = new Comment
            {
                CommentContent = model.comment,
                AddedBy = user.UserName,
                IsRead = false,
                GlobalOrderId = id,
                AddedOn = DateTime.Now,
            };

            await dbContext.Comments.AddAsync(comment);
            await dbContext.SaveChangesAsync();
            var featureTitle = feature.Notes.Length > 25
                ? feature.Notes.Substring(0, 25)
                : feature.Notes;

            var allUsers = await _userManager.Users.Where(u => u.FcmToken != null && u.Id != user.Id).ToListAsync();

            foreach (var u in allUsers)
            {
                try
                {
                    await firebase.SendAsync(
                        u.FcmToken,
                        $"{user.UserName} added new comment on {featureTitle}",
                        model.comment
                    );
                }
                catch (Exception ex)
                {

                }
            }

            return Ok(new
            {
                Message = "Comment added successfully"
            });
        }


        [Authorize]
        [HttpPut("makeAllCommentsRead/{orderId}")]
        public async Task<IActionResult> makeAllCommentsRead(int orderId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var comments = await dbContext.Comments.Where(c => c.GlobalOrderId == orderId).Where(c => !c.IsRead).ToListAsync();

            foreach (var comment in comments)
            {
                if (comment.AddedBy.Trim().ToLower() == currentUser.UserName.Trim().ToLower()) continue;
                comment.IsRead = true;
            }

            await dbContext.SaveChangesAsync();
            return Ok(comments);

        }

    }

    public class CarRequests
    {
        public int? CustomerId { get; set; }
        public int orderId { get; set; }
        public string? VinNumber { get; set; }
        public string? Year { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
    }

    public class DetailReq
    {
        public int? ServiceId { get; set; }
        public int? ItemId { get; set; }
        public int? CauseId { get; set; }
        public int? ComplaintId { get; set; }
        public int? GlobalOrderId { get; set; }
        public string? Notes { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Discount { get; set; }
        public decimal? paid { get; set; }
    }

    public class UniversalOrderModel
    {
        public int? Platform_id { get; set; }
        public int Business_id { get; set; }
        public int? Service_id { get; set; }
        public int? GlobalCustomerId { get; set; }
        public int? ShippingTypeId { get; set; }
        public int? OrderStatusId { get; set; }
        public string CartValue { get; set; }
    public int? providerId { get; set; }
    public List<ItemInfo>? ItemInfos { get; set; }

    }

    public class ItemInfo
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int size_id { get; set; }
        public decimal price { get; set; }
    }

    public class eTaskModel
    {
        public string Notes { get; set; }
        public int StatusId { get; set; }
    }


}

