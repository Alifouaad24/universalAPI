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
    public class UserBusinessController : ControllerBase
    {
        UniversalDbContext db;
        public UserBusinessController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public async Task<IActionResult> bindBusinessToUser([FromBody] UserBusiness model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userBusinesses = db.UsersBusinesseses.Where(us => us.UserId == model.UserId);
            db.UsersBusinesseses.RemoveRange(userBusinesses);
            await db.SaveChangesAsync();

            var businessesToBind = await db.Businesses.Where(b => model.BusinessesIds.Contains(b.Business_id)).ToListAsync();

            var userBusinessesToAdd = businessesToBind.Select(b => new UsersBusinesses
            {
                UserId = model.UserId,
                Business_id = b.Business_id
            }).ToList();

            await db.UsersBusinesseses.AddRangeAsync(userBusinessesToAdd);
            await db.SaveChangesAsync();

            return Ok(new { msg = "businesses bind to user successfully" });
        }

    }
}