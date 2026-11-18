using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalSystemController : ControllerBase
    {
        UniversalDbContext _context;
        public GlobalSystemController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var systems = await _context.GlobalSystems
                .Include(s => s.BusinessGroup).ThenInclude(b => b.BusinessGroupBusinesses).ThenInclude(bsb => bsb.Business)
                .ToListAsync();
            return Ok(systems);
        }

        [HttpPost]
        public async Task<IActionResult> AddGlobalSystem([FromBody] GlobalSystemDto model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var newSysten = new GlobalSystem
            {
                GlobalSystemName = model.Name,
                GlobalSystemType = model.Type,
                GlobalSystemUrl = model.Url,
            };

            await _context.GlobalSystems.AddAsync(newSysten);
            await _context.SaveChangesAsync();

            return Ok(newSysten);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateGlobalSystem(int id, [FromBody] GlobalSystemDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existGlobalSystemD = await _context.GlobalSystems.FindAsync(id); 
            if(existGlobalSystemD == null) return NotFound("GlobalSystem not found...");

            existGlobalSystemD.GlobalSystemName = model.Name;
            existGlobalSystemD.GlobalSystemType = model.Type;
            existGlobalSystemD.GlobalSystemUrl = model.Url;
            existGlobalSystemD.BusinessGroupId = model.groupId;

            await _context.SaveChangesAsync();
            return Ok(existGlobalSystemD);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGlobalSystem(int id)
        {

            var existGlobalSystemD = await _context.GlobalSystems.FindAsync(id);
            if (existGlobalSystemD == null) return NotFound("GlobalSystem not found...");

            _context.GlobalSystems.Remove(existGlobalSystemD);
            await _context.SaveChangesAsync();

            return Ok(existGlobalSystemD);
        }


        [HttpPost("BindCustomerOrBusinessWithGlobalSystem")]
        public async Task<IActionResult> BindCustomerOrBusinessWithGlobalSystem([FromBody] BindCustomerOrBusinessWithGlobalSysDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            foreach(var sysId in model.sysIds)
            {
                var sysBusCust = new System_Business_customer
                {
                    GlobalSystemId = sysId,
                    Business_id = model.BusId,
                    GlobalCustomerId = model.CustId,
                };

                await _context.System_Business_customers.AddAsync(sysBusCust);
            }
           
            await _context.SaveChangesAsync();

            return Ok(new
            {
                msg = "Customer | Business Binded With Global System"
            });
        }

        [HttpDelete("RemoveSystemCustomerBusinessRelation/{relationId}")]
        public async Task<IActionResult> RemoveSystemCustomerBusinessRelation(int relationId)
        {
            var existRemoveSystemCustomerBusinessRelation = await _context.System_Business_customers.FindAsync(relationId);
            if (existRemoveSystemCustomerBusinessRelation == null) return NotFound(" relation record not found");

            _context.System_Business_customers.Remove(existRemoveSystemCustomerBusinessRelation);
            await _context.SaveChangesAsync();

            return Ok(existRemoveSystemCustomerBusinessRelation);
        }


    }


    public class GlobalSystemDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public int? groupId { get; set; }
    }

    public class BindCustomerOrBusinessWithGlobalSysDto
    {
        public List<int>? sysIds { get; set; }
        public int? BusId { get; set; }
        public int? CustId { get; set; }
    }
}
