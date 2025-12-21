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
    public class ActivityController : ControllerBase
    {
        UniversalDbContext db;
        public ActivityController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> getAllActivities()
        {
            var activities = await db.Activiities
                .Include(b => b.Activity_Services).ThenInclude(aS => aS.Service)
                .Include(b => b.business)
                .Where(b => b.visible == true).ToListAsync();
            return Ok(activities);
        }

        [HttpPost]
        public async Task<IActionResult> AddActivity([FromBody] ActivityDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var activity = new Activiity
            {
                Description = model.Description,
                Business_id = model.Business_id,
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
                Insert_by = ""
            };

            await db.Activiities.AddAsync(activity);
            await db.SaveChangesAsync();
            return Ok(activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditActivity(int id, [FromBody] ActivityDto model)
        {
            var act = await db.Activiities.FindAsync(id);
            if (act == null) return NotFound();

            act.Description = model.Description;
            act.Business_id = model.Business_id;

            await db.SaveChangesAsync();

            return Ok(act);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var act = await db.Activiities.FindAsync(id);
            if (act == null) return NotFound();

            act.visible = false;
            await db.SaveChangesAsync();
            return Ok(new
            {
                msg = "Activity deleted sauccessfully! "
            });
        }
    }
}
