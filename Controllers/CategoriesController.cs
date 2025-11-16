using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DBTest_BACK.Data;
using DBTest_BACK.Models;
using DBTest_BACK.DTOs;

namespace DBTest_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categories (Público)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Image = c.Image,
                    ProductCount = c.Products.Count,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(categories);
        }

        // GET: api/categories/5 (Público)
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryResponseDto>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = $"Categoría con Id {id} no encontrada" });
            }

            var categoryDto = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Image = category.Image,
                ProductCount = category.Products.Count,
                CreatedAt = category.CreatedAt
            };

            return Ok(categoryDto);
        }
    }
}
