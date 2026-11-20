using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models.Helper_models;
using Universal_server.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Universal_server.firebase_options;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        UniversalDbContext db;
        IFirebaseNotificationService firebase;
        UserManager<IdentityUserData> _userManager;
        public FeatureController(UniversalDbContext db, UserManager<IdentityUserData> userManager, IFirebaseNotificationService firebase)
        {
            this.db = db;
            _userManager = userManager;
            this.firebase = firebase;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices(string status)
        {
            var query = await db.Features
                .Include(f => f.GlobalSystem)
                .Include(f => f.Business)
                .Include(f => f.Comments)
                .Include(f => f.Service)
                .Where(f => f.visible)
                .Where(f => f.Status == status)
                .ToListAsync();

            return Ok(query);
        }


        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetAllServices(int businessId)
        {
            var business = await db.Businesses.FindAsync(businessId);

            if (business == null)
                return NotFound("Business not found");

            IQueryable<Feature> query = db.Features
                .Include(f => f.GlobalSystem)
                .Include(f => f.Business)
                .Include(f => f.Comments)
                .Include(f => f.Service)
                .Where(f => f.visible);

            if (!business.Role.Contains("Manager"))
            {
                query = query.Where(f => f.Business_id == businessId);
            }

            var features = await query
                .OrderByDescending(f => f.Status.Contains("New"))
                .ThenByDescending(f => f.Status.Contains("On progress"))
                .ThenByDescending(f => f.Status.Contains("Waiting response"))
                .ThenByDescending(f => f.Status.Contains("Testing"))
                .ThenByDescending(f => f.Status.Contains("Completed"))
                .ToListAsync();

            return Ok(features);
        }


        [HttpPost]
        public async Task<IActionResult> addService([FromBody] FeatureDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var feature = new Feature
            {
                GlobalSystemId = model.GlobalSystemId,
                Body = model.Body,
                Status = model.Status,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
                Business_id = model.BusinessId,
                Service_id = model.ServiceId,

            };

            await db.Features.AddAsync(feature);
            await db.SaveChangesAsync();

            var allUsers = await _userManager.Users.Where(u => u.FcmToken != null).ToListAsync();

            _ = Task.Run(async () =>
            {
                foreach (var u in allUsers)
                {
                    try
                    {
                        await firebase.SendAsync(
                            u.FcmToken,
                            $"New Task",
                            $""
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }
            });


            return Ok(feature);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateService(int id, [FromBody] FeatureDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var feature = await db.Features.FindAsync(id);

            if (feature == null) return BadRequest("Feature not found");

            feature.GlobalSystemId = model.GlobalSystemId;
            feature.Body = model.Body;
            feature.Status = model.Status;
            feature.Business_id = model.BusinessId;
            feature.Service_id = model.ServiceId;

            await db.SaveChangesAsync();

            return Ok(feature);
        }

        [HttpPut("SetTaskStatusClosed/{id}")]
        public async Task<IActionResult> SetTaskStatusClosed(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var feature = await db.Features.FindAsync(id);

            if (feature == null) return BadRequest("Feature not found");

            feature.Status = "Closed";
            await db.SaveChangesAsync();

            return Ok(feature);
        }

        [HttpPut("SetStatusComleted/{id}")]
        public async Task<IActionResult> SetStatusComleted(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var feature = await db.Features.FindAsync(id);

            if (feature == null) return BadRequest("Feature not found");

            feature.Status = "Completed";


            await db.SaveChangesAsync();

            return Ok(feature);
        }

        [HttpPut("ChangeStatus/{id}")]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var feature = await db.Features.FindAsync(id);

            if (feature == null) return BadRequest("Feature not found");
            feature.Status = status;
            await db.SaveChangesAsync();

            var featureTitle = feature.Body.Length > 25
                ? feature.Body.Substring(0, 25)
                : feature.Body;

            var allUsers = await _userManager.Users.Where(u => u.FcmToken != null).ToListAsync();

            foreach (var u in allUsers)
            {
                try
                {
                    await firebase.SendAsync(
                        u.FcmToken,
                        $"Task {featureTitle}",
                        $"changed to {status}"
                    );
                }
                catch (Exception ex)
                {

                }
            }

            return Ok(feature);
        }


        [Authorize]
        [HttpPut("AddComment/{id}")]
        public async Task<IActionResult> AddNewComment(int id, CommentBody model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
                return Unauthorized();

            var feature = await db.Features.FindAsync(id);

            if (feature == null)
                return NotFound("Feature not found");

            var comment = new Comment
            {
                CommentContent = model.comment,
                AddedBy = user.UserName,
                IsRead = false,
                FeatureId = id,
                AddedOn = DateTime.Now,
            };

            await db.Comments.AddAsync(comment);
            await db.SaveChangesAsync();
            var featureTitle = feature.Body.Length > 25
                ? feature.Body.Substring(0, 25)
                : feature.Body;

            var allUsers = await _userManager.Users.Where(u => u.FcmToken != null && u.Id != user.Id).ToListAsync();

            foreach (var u in allUsers)
            {
                try
                {
                    await firebase.SendAsync(
                        u.FcmToken,
                        $"{user.UserName} added new comment on {featureTitle}",
                        model.comment
                    );
                }
                catch (Exception ex)
                {

                }
            }

            return Ok(new
            {
                Message = "Comment added successfully"
            });
        }

        [HttpDelete("DeleteComment/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var comment = await db.Comments.FindAsync(id);

            if (comment == null) return BadRequest("comment not found");

            db.Comments.Remove(comment);
            await db.SaveChangesAsync();

            return Ok(comment);
        }

        [Authorize]
        [HttpPut("makeAllCommentsRead/{featureId}")]
        public async Task<IActionResult> makeAllCommentsRead(int featureId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var comments = await db.Comments.Where(c => c.FeatureId == featureId).Where(c => !c.IsRead).ToListAsync();

            foreach (var comment in comments)
            {
                if (comment.AddedBy.Trim().ToLower() == currentUser.UserName.Trim().ToLower()) continue;
                comment.IsRead = true;
            }

            await db.SaveChangesAsync();
            return Ok(comments);

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeature(int id)
        {
            var feature = await db.Features.FindAsync(id);
            if (feature == null) return NotFound();

            feature.visible = false;
            await db.SaveChangesAsync();
            return Ok(feature);
        }
    }
}
