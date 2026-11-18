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
    public class ComplaintController : ControllerBase
    {
        private readonly UniversalDbContext _context;
        public ComplaintController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllComplaints()
        {
            var complaints = await _context.Complaints
                .Include(c => c.Complaint_Value)
                .ThenInclude(cv => cv.ComplaintType)
                .ToListAsync();
            return Ok(complaints);
        }

        [HttpGet("GetAllComplaintsTypes")]
        public async Task<IActionResult> GetAllComplaintsTypes()
        {
            var types = await _context.ComplaintTypes.ToListAsync();
            return Ok(types);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewComplaint(AddNewComplaintModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var complimaintValue = new Complaint_Value
            {
                ComplaintTypeId = model.ComplaintTypeId,
                Value = model.Value,
                Description = model.Description,
            };

            await _context.Complaint_Values.AddAsync(complimaintValue);
            await _context.SaveChangesAsync();

            var Complimaint = new Complaint
            {
                Complaint_ValueId = complimaintValue.Complaint_ValueId,
            };

            await _context.Complaints.AddAsync(Complimaint);
            await _context.SaveChangesAsync();

            return Ok(Complimaint);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComplaiment(int id, [FromBody] AddNewComplaintModel model)
        {
            var complaimentValue = await _context.Complaint_Values.FindAsync(id);
            if (complaimentValue == null) return BadRequest("complaimentValue not found");
            complaimentValue.Value = model.Value;
            complaimentValue.Description = model.Description;
            complaimentValue.ComplaintTypeId = model.ComplaintTypeId;

            await _context.SaveChangesAsync();
            return Ok(complaimentValue);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplaiment(int id)
        {
            var complaiment = await _context.Complaints.FindAsync(id);
            if (complaiment == null) return BadRequest("complaiment not found");

            var complaimentValue = await _context.Complaint_Values.FindAsync(complaiment.Complaint_ValueId);
            if (complaimentValue != null) _context.Complaint_Values.Remove(complaimentValue);

            _context.Complaints.Remove(complaiment);

            await _context.SaveChangesAsync();
            return Ok(complaiment);
        }
    }
}
