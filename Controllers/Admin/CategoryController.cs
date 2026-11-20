using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        UniversalDbContext db;
        public CategoryController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories(int businessId)
        {
            var data = await db.Categories.Include(c => c.Business).ToListAsync();
            return Ok(data);
        }

        [HttpGet("{businessId}")]
        public async Task<IActionResult> GetAllData(int businessId)
        {
            var business = await db.Businesses.FindAsync(businessId);

            if (business == null) return NotFound("Business not found");

            if (business.Role.Contains("Manager"))
            {
                var allData = await db.Categories.OrderBy(c => c.Name.ToLower()).ToListAsync();
                return Ok(allData);
            }

            var data = await db.Categories.Where(c => c.Business_id == businessId).OrderBy(c => c.Name.ToLower()).ToListAsync();
            return Ok(data);

        }


        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = await db.Categories.Where(c => c.Name.Trim() == model.Name.Trim()).FirstOrDefaultAsync();
            if (category != null) return Ok(category);

            var newCategory = new Category
            {
                Name = model.Name,
                Business_id = model.Business_id,
                EbayCategoryId = model.EbayCategoryId,
                LinkedCategoryId = model.LinkedCategoryId,
                ParentId = model.ParentId
            };

            await db.Categories.AddAsync(newCategory);
            await db.SaveChangesAsync();
            return Ok(newCategory);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = await db.Categories.FindAsync(id);
            if (category == null) return BadRequest("Category not found");

            category.Name = model.Name;
            category.Business_id = model.Business_id;
            category.EbayCategoryId = model.EbayCategoryId;
            category.ParentId  = model.ParentId;
            category.LinkedCategoryId = model.LinkedCategoryId;

            await db.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var category = await db.Categories.FindAsync(id);
            if (category == null) return BadRequest("Category not found");

            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return Ok(category);
        }
    }
}

public class CategoryDto
{

    public string Name { get; set; }
    public string? EbayCategoryId { get; set; }
    public int? Business_id { get; set; }
    public int? LinkedCategoryId { get; set; }
    public int? ParentId { get; set; }
}
