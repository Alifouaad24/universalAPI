using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemImagesController : ControllerBase
    {
        UniversalDbContext _context;
        public ItemImagesController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{imageUrl}")]
        public async Task<IActionResult> deleteItemImageRec(string imageUrl)
        {
            var rec = await _context.ItemImages.Where(ii => ii.ImageUrl.Contains(imageUrl)).FirstOrDefaultAsync();
            if (rec != null)
            {
                _context.ItemImages.Remove(rec);
                await _context.SaveChangesAsync();
            }
            return Ok(rec);
        }
    }
}
