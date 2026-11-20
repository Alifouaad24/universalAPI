using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingCartController : ControllerBase
    {
        private readonly UniversalDbContext _context;

        public PricingCartController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddNewPricing([FromBody] List<CartPricingItems> model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model == null || model.Count == 0) return BadRequest("Cart is empty");

            if (model.Any(i => string.IsNullOrWhiteSpace(i.Sku)))
                return BadRequest("Every item must have a SKU");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var pricingCart = new PricingCart();
                await _context.PricingCarts.AddAsync(pricingCart);
                await _context.SaveChangesAsync();

                foreach (var item in model)
                {
                    var sku = item.Sku!.Trim();
                    var existingItem = await _context.Items.FirstOrDefaultAsync(i => i.SKU == sku);

                    int itemId;
                    if (existingItem == null)
                    {
                        var newItem = new Item
                        {
                            SKU = sku,
                            BasePrice = item.Price,
                            ItemDescription = item.Name,
                            SizeId = await GetOrCreateSizeId(item.Size),
                            ColorId = await GetOrCreateColorId(item.Color),
                        };
                        await _context.Items.AddAsync(newItem);
                        await _context.SaveChangesAsync();

                        if (!string.IsNullOrWhiteSpace(item.ImageUrl))
                        {
                            await _context.ItemImages.AddAsync(new ItemImage
                            {
                                ImageUrl = item.ImageUrl,
                                ItemId = newItem.ItemId,
                            });
                        }
                        itemId = newItem.ItemId;
                    }
                    else
                    {
                        itemId = existingItem.ItemId;
                    }

                    await _context.PricingCartItems.AddAsync(new PricingCartItem
                    {
                        PricingCartId = pricingCart.PricingCartId,
                        ItemId = itemId,
                        // Price = item.Price,  // بعد إضافة الحقل للموديل
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(pricingCart.PricingCartId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<int?> GetOrCreateColorId(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            var normalized = name.Trim().ToLower();

            var color = await _context.Colors
                .FirstOrDefaultAsync(c => c.Name.Trim().ToLower() == normalized);
            if (color != null) return color.ColorId;

            color = new Color { Name = name.Trim() };
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();
            return color.ColorId;
        }

        private async Task<int?> GetOrCreateSizeId(string? description)
        {
            if (string.IsNullOrWhiteSpace(description)) return null;
            var normalized = description.Trim().ToLower();

            var size = await _context.Sizes
                .FirstOrDefaultAsync(s => s.Description.Trim().ToLower() == normalized);
            if (size != null) return size.Size_id;

            size = new Size { Description = description.Trim() };
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            return size.Size_id;
        }
    }
}
public class CartPricingItems
{
    public string? Name { get; set; }
    public string? Sku { get; set; }
    public string? ImageUrl { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public decimal? Price { get; set; }
}
