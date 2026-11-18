using AinAlfahd.Data;
using AinAlfahd.Models;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.AinAlFHDModels;

namespace Universal_server.BL
{
    public interface IIOrders
    {
        Task<OrderResponse> GetOrdersAsync();
        Task<OrderResponse> AddOrder(OrderRequest model);
    }

    public class IOrders : IIOrders
    {
        private readonly MasterDBContext _context;
        public IOrders(MasterDBContext context)
        {
            _context = context;
        }
        public async Task<OrderResponse> GetOrdersAsync()
        {
            var orders = await _context.AinAlFhdOrders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .Where(o => o.MerchantId == 2482).ToListAsync();
            return new OrderResponse {
                Code = 200,
                IsSuccess = true,
                data = new
                {
                    Orders = orders,
                }
            };
        }

        public async Task<OrderResponse> AddOrder(OrderRequest model)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.CustMob == model.CustomerMobile);

                int customerId;
                if (customer == null)
                {
                    var city = await _context.Cities
                        .FirstOrDefaultAsync(c => c.Description == model.CustomerCity);

                    if (city == null)
                    {
                        return new OrderResponse
                        {
                            IsSuccess = false,
                            Code = 400,
                            Message = "City not match",
                        };
                    }

                    var area = await _context.Areas
                        .FirstOrDefaultAsync(a =>
                            a.Description == model.CustomerArea &&
                            a.CityId == city.Id);

                    if (area == null)
                    {
                        return new OrderResponse
                        {
                            IsSuccess = false,
                            Code = 400,
                            Message = "Area not match",

                        };
                    }

                    customer = new Customer
                    {
                        CustName = model.CustomerName,
                        CustMob = model.CustomerMobile,
                        CustCity = city.Id,
                        CustArea = area.Id,
                        CustLandmark = model.CustomerLandMark

                    };

                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                customerId = customer.Id;

                var order = new AinAlFhdOrder
                {
                    OrderDt = DateOnly.FromDateTime(DateTime.Now),
                    OrderOwner = customerId,
                    WithDelivery = 1,
                    Notes = model.Notes,
                    OrderStatus = 132,
                    ServiceCharges = 0,
                    TaskStatusId = 2,
                    Totalqty = model.TotalQty,
                    NetAmount = model.NetAmount,
                    Source = 3,
                    MerchantId = 2482
                };

                _context.AinAlFhdOrders.Add(order);
                await _context.SaveChangesAsync();

                var orderDetail = new Models.AinAlFHDModels.AinAlFhdOrderDetail
                {
                    OrderNo = order.Id,
                    ItemCode = 13313,
                    MerchantId = 2482,
                    Qty = model.Qty,
                    OriginalAmount = model.OriginalAmount,
                    Size_id = 11
                };

                _context.AinAlFhdOrderDetails.Add(orderDetail);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrderResponse
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Order Added successfuly",
                    data = new
                    {
                        orderNumber = order.Id,
                    }
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}

public class OrderRequest
{
    public string? Notes { get; set; }
    public int? TotalQty { get; set; }
    public int? Qty { get; set; }
    public int? NetAmount { get; set; }
    public int? OriginalAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerMobile { get; set; }
    public string? CustomerCity { get; set; }
    public string? CustomerArea { get; set; }
    public string? CustomerLandMark { get; set; }
}

public class OrderResponse
{
    public bool IsSuccess { get; set; }
    public int Code { get; set; }
    public string Message { get; set; }
    public object data { get; set; }

}
