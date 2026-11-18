using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirebaseController : ControllerBase
    {
        private readonly UniversalDbContext _context;
        private readonly UserManager<IdentityUserData> _userManager;
        public FirebaseController(UniversalDbContext context, UserManager<IdentityUserData> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserToken(FirebaseToken model)
        {
            var user = await _userManager.FindByIdAsync(model.userId);

            if (user == null)
                return NotFound("User not found");

            user.FcmToken = model.token;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                message = "Token saved successfully"
            });
        }

    }
}
