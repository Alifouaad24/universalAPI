using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Universal_server.Models;
using Universal_server.Models.Helper_models;
using Universal_server.Services;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<IdentityUserData> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        GenerateToken GenerateToken;
        public AccountController(UserManager<IdentityUserData> userManager, RoleManager<IdentityRole> roleManager, GenerateToken generateToken)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            GenerateToken = generateToken;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new IdentityUserData
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            //await _userManager.AddToRoleAsync(user, "User");

            var token = GenerateToken.GenerateJwtToken(
                user.Id,
                user.Email,
                "User"
            );

            return Ok(new
            {
                message = "User registered successfully",
                token = token
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] loginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
                return Unauthorized(new { message = "Invalid email or password" });

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User"; 

            var token = GenerateToken.GenerateJwtToken(
                user.Id,
                user.Email,
                role
            );

            return Ok(new
            {
                message = "Login successful",
                token = token
            });
        }

        [HttpPost("CreateRole/{roleName}")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role name cannot be empty");

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
                return BadRequest("Role already exists");

            var role = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"Role '{roleName}' created successfully");
        }

        [HttpPost("AssignRoleToUser")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignUserToRole model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserEmail);
            if (user == null)
                return NotFound("User not found");

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists)
                return BadRequest("Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"User assigned to role successfully");
        }




    }
}
