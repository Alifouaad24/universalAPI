using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models.AliModels;
using Universal_server.Models.Helper_models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        UniversalDbContext db;
        public DeviceController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDevices()
        {
            var devices = await db.Devices.ToListAsync();
            return Ok(devices);
        }

        [HttpGet("{deviceId}")]
        public async Task<IActionResult> GetDevice(string deviceId)
        {
            var device = await db.Devices.Where(d => d.DeviceName == deviceId).FirstOrDefaultAsync();
            return Ok(device);
        }

        [HttpGet("ReturnDevicesNeedApprovement")]
        public async Task<IActionResult> ReturnDevicesNeedApprovement()
        {
            var devices = await db.Devices.Where(d => d.IsApproved == false).ToListAsync();
            return Ok(devices);
        }

        [HttpPost]
        public async Task<IActionResult> AddDevice([FromBody] DeviceDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var device = new Device
            {
                DeviceName = model.DeviceName,
                IsApproved = false,
                IsAdmin = false,
                RequestData = DateOnly.FromDateTime(DateTime.UtcNow),
            };

            await db.Devices.AddAsync(device);
            await db.SaveChangesAsync();

            return Ok(device);
        }

        [HttpPut("ApproveDevice/{DeviceName}")]
        public async Task<IActionResult> ApproveDevice(string DeviceName)
        {
            var device = await db.Devices.FirstOrDefaultAsync(d => d.DeviceName == DeviceName);
            if (device == null) return BadRequest(ModelState);
            device.IsApproved = true;
            await db.SaveChangesAsync();
            return Ok(device);
        }

        [HttpPut("MakeAdmin/{DeviceName}")]
        public async Task<IActionResult> MakeAdmin(string DeviceName)
        {
            var device = await db.Devices.FirstOrDefaultAsync(d => d.DeviceName == DeviceName);
            if (device == null) return BadRequest(ModelState);
            device.IsAdmin = true;
            await db.SaveChangesAsync();
            return Ok(device);
        }

        [HttpDelete("{DeviceName}")]
        public async Task<IActionResult> DeleteDevice(string DeviceName)
        {
            var device = await db.Devices.FirstOrDefaultAsync(d => d.DeviceName == DeviceName);
            if (device == null) return NoContent();
            db.Devices.Remove(device);
            await db.SaveChangesAsync();
            return Ok(device);
        }

    }
}
