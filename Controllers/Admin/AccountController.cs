using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
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
        UniversalDbContext db;
        GenerateToken GenerateToken;
        public AccountController(UserManager<IdentityUserData> userManager, RoleManager<IdentityRole> roleManager, GenerateToken generateToken, UniversalDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            GenerateToken = generateToken;
            this.db = db;
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
                ["User"]
            );

            return Ok(new
            {
                message = "User registered successfully",
                token = token
            });
        }

        [HttpPost("AddUsers")]
        public async Task<IActionResult> AddUsers([FromBody] RegisterUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
                return BadRequest("Username already exists");

            var user = new IdentityUserData
            {
                UserName = model.UserName,
                Email = model.Email
            };

            string password = await GeneratePassword();

            using var transaction = await db.Database.BeginTransactionAsync();

            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                user.UserPassword = password;
                await _userManager.UpdateAsync(user);

                if (model.Roles?.Any() == true)
                {
                    foreach (var role in model.Roles)
                    {
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            await _userManager.AddToRoleAsync(user, role);
                        }
                    }
                }

                if (model.BusinessIds?.Any() == true)
                {
                    var validBusinessIds = await db.Businesses
                        .Where(b => model.BusinessIds.Contains(b.Business_id))
                        .Select(b => b.Business_id)
                        .ToListAsync();

                    foreach (var businessId in validBusinessIds)
                    {
                        await db.UsersBusinesseses.AddAsync(new UsersBusinesses
                        {
                            UserId = user.Id,
                            Business_id = businessId
                        });
                    }

                    await db.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "User added successfully",
                    password
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    message = "An error occurred while creating the user",
                    error = ex.Message
                });
            }
        }

        [HttpPut("updateUser/{id}")]
        public async Task<IActionResult> updateUser(string id, [FromBody] RegisterUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(existingUser);

            if (userRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(existingUser, userRoles);
            }

            var userBuss = await db.UsersBusinesseses.Where(ub => ub.UserId == existingUser.Id).ToListAsync();
            if (userBuss.Any())
            {
                db.UsersBusinesseses.RemoveRange(userBuss);
                await db.SaveChangesAsync();
            }


            if (model.Roles?.Any() == true)
            {
                foreach (var role in model.Roles)
                {
                    if (await _roleManager.RoleExistsAsync(role))
                    {
                        await _userManager.AddToRoleAsync(existingUser, role);
                    }
                }
            }

            if (model.BusinessIds?.Any() == true)
            {
                var validBusinessIds = await db.Businesses
                    .Where(b => model.BusinessIds.Contains(b.Business_id))
                    .Select(b => b.Business_id)
                    .ToListAsync();

                foreach (var businessId in validBusinessIds)
                {
                    await db.UsersBusinesseses.AddAsync(new UsersBusinesses
                    {
                        UserId = existingUser.Id,
                        Business_id = businessId
                    });
                }

                await db.SaveChangesAsync();
            }


            return Ok(new
            {
                UserId = existingUser.Id,
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
            var businesses = await db.Businesses.Include(b => b.Business_Services).ThenInclude(bs => bs.Service)
                 .Include(b => b.BusinessTypes).ThenInclude(bbt => bbt.BusinessType)
                 .Include(b => b.BusinessAddresses).ThenInclude(ba => ba.Address)
                 .Include(b => b.Activities)
                .Where(b => b.UsersBusinesses.Any(ub => ub.UserId == user.Id))
                .ToListAsync();


            var token = GenerateToken.GenerateJwtToken(
                user.Id,    
                user.Email,
                roles.ToList()
            );

            return Ok(new
            {
                message = "Login successful",
                user = user,
                token = token,
                businesses = businesses
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

            return Ok(new
            {
                msg = $"Role '{roleName}' created successfully"
            });
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

        [HttpGet("getAllRoles")]
        public async Task<IActionResult> getAllRoles()
        {
           var roles = await _roleManager.Roles.ToListAsync();
            return Ok(roles);
        }

        [HttpGet("getAllUsers")]
        public async Task<IActionResult> getAllUsers()
        {
            var users = await _userManager.Users.Include(u => u.UsersBusinesses).ThenInclude(ub => ub.Business).ToListAsync();

            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    Roles = roles,
                    Businesses = user.UsersBusinesses.Select(ub => new
                    {
                        ub.Business.Business_id,
                        ub.Business.Business_name
                    })
                });
            }

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            return Ok(user);
        }

        [HttpDelete("deleteRole/{id}")]
        public async Task<IActionResult> deleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            await _roleManager.DeleteAsync(role);
            return Ok(role);
        }

        [HttpPut("UpdateRole/{id}/{roleName}")]
        public async Task<IActionResult> UpdateRole(string id, string roleName)
        {
            var role = await _roleManager.FindByIdAsync(id);
            role.Name = roleName;
            await _roleManager.UpdateAsync(role);
            return Ok(role);
        }



        private Task<string> GeneratePassword()
        {
            var random = new Random();
            return Task.FromResult(random.Next(0, 100000).ToString("D5"));
        }






    }
}
