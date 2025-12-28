using System.Diagnostics;
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
    public class ServiceController : ControllerBase
    {
        UniversalDbContext db;
        public ServiceController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices() 
        {
            var services = await db.Services
                .Include(b => b.Activity_Services).ThenInclude(aS => aS.Activiity)
                .Include(s => s.Business_Services).ThenInclude(bs => bs.Business)
                .Where(b => b.visible == true).ToListAsync();
            return Ok(services);
        }

        [HttpGet("GetAllServicesbus")]
        public async Task<IActionResult> GetAllServicesbus()
        {
            var services = await db.Business_Services
                .Include(b => b.Service)
                .Where(b => b.Business.UsersBusinesses.Any(x => x.UserId == "9ed6b37b-1694-4478-9f0f-9a99163fd716")).ToListAsync();
            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> addService([FromBody] ServiceDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var service = new Service
            {
                Description = model.Description,
                IsPublic = model.IsPublic,
                Service_icon = model.Service_icon,
                Service_Route = $"Home/{model.Description}",
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
            };

            await db.Services.AddAsync(service);
            await db.SaveChangesAsync();

            if (model.BusinessesId.Count > 0)
            {
                var businesses = await db.Businesses.Where(b => model.BusinessesId.Contains(b.Business_id)).ToListAsync();
                foreach (var bus in businesses) {
                
                    await db.Business_Services.AddAsync(new Business_Service
                    {
                        Business_id = bus.Business_id,
                        Service_id = service.Service_id,
                    });
                }
                await db.SaveChangesAsync();
            }

            if (model.ActivitiesId.Count > 0)
            {
                var activities = await db.Activiities.Where(a => model.ActivitiesId.Contains(a.Activity_id)).ToListAsync();
                foreach (var act in activities)
                {

                    await db.Activity_Services.AddAsync(new Activity_Service
                    {
                        Activity_id = act.Activity_id,
                        Service_id = service.Service_id,
                    });
                }
                await db.SaveChangesAsync();
            }


            await db.SaveChangesAsync();
            return Ok(service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditService(int id, [FromBody] ServiceDto model)
        {
            var service = await db.Services.FindAsync(id);
            if (service == null) return NotFound();


            var busServs = await db.Business_Services.Where(bs => bs.Service_id == service.Service_id).ToListAsync();
            if (busServs.Count > 0) 
            { 
                db.Business_Services.RemoveRange(busServs);
                await db.SaveChangesAsync();
            }

            var ActServs = await db.Activity_Services.Where(bs => bs.Service_id == service.Service_id).ToListAsync();
            if (busServs.Count > 0)
            {
                db.Activity_Services.RemoveRange(ActServs);
                await db.SaveChangesAsync();
            }


            if (model.BusinessesId.Count > 0)
            {
                var businesses = await db.Businesses.Where(b => model.BusinessesId.Contains(b.Business_id)).ToListAsync();
                foreach (var bus in businesses)
                {

                    await db.Business_Services.AddAsync(new Business_Service
                    {
                        Business_id = bus.Business_id,
                        Service_id = service.Service_id,
                    });
                }
                await db.SaveChangesAsync();
            }

            if (model.ActivitiesId.Count > 0)
            {
                var activities = await db.Activiities.Where(a => model.ActivitiesId.Contains(a.Activity_id)).ToListAsync();
                foreach (var act in activities)
                {

                    await db.Activity_Services.AddAsync(new Activity_Service
                    {
                        Activity_id = act.Activity_id,
                        Service_id = service.Service_id,
                    });
                }
                await db.SaveChangesAsync();
            }

            service.Description = model.Description;
            service.Service_icon = model.Service_icon;
            service.Insert_by = "";

            await db.SaveChangesAsync();
            return Ok(service);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await db.Services.FindAsync(id);
            if (service == null) return NotFound();

            service.visible = false;
            await db.SaveChangesAsync();
            return Ok(service);
        }
    }
}
