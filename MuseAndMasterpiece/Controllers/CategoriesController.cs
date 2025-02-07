using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseAndMasterpiece.Data;
using MuseAndMasterpiece.Data.Migrations;
using MuseAndMasterpiece.Models;
using NuGet.DependencyResolver;

namespace MuseAndMasterpiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Categories including the total number of Artworks it holds and the list of all Artwork Titles it holds.
        /// </summary>
        /// <returns>
        /// 200 Ok
        /// List of Categories including ID, Name of Category, Total Artworks it holds and all the Artwork Titles.
        /// </returns>
        /// <example>
        /// GET: api/Categories/List -> [{CategoryId: 1, CName: "Calligraphy", TotalArtworks: 2, ArtworkTitles: ["Elegant Script","Modern Calligraphy"]},{....},{....}]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> ListCategories()
        {
            List<Category> Categories = await _context.Categories
                .Include(c => c.Artworks)
                .ToListAsync();

            List<CategoryDto> CategoryDtos = new List<CategoryDto>();

            foreach (Category Category in Categories)
            {
                CategoryDtos.Add(new CategoryDto()
                {
                    CategoryId = Category.CategoryId,
                    CName = Category.CName,
                    DateCreated = Category.DateCreated,
                    TotalArtworks = Category.Artworks.Count(),
                    ArtworksTitle = Category.Artworks != null ? Category.Artworks.Select(a => a.Title).ToList() : new List<string>()

                });

            }
            // return 200 OK with CategoryDtos
            return Ok(CategoryDtos);
        }


        /// <summary>
        /// Return a Category specified by it's {id}
        /// </summary>
        /// /// <param name="id">Category's id</param>
        /// <returns>
        /// 200 Ok
        /// CategoryDto : It includes ID, Name of Category, Total Artworks it holds and all the Artwork Titles.
        /// or
        /// 404 Not Found when there is no Category for that {id}
        /// </returns>
        /// <example>
        /// GET: api/Categories/Find/1 -> {CategoryId: 1, CName: "Calligraphy", TotalArtworks: 2, ArtworkTitles: ["Elegant Script","Modern Calligraphy"]}
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<CategoryDto>> FindCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Artworks)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound();
            }

            CategoryDto categoryDto = new CategoryDto()
            {
                CategoryId = category.CategoryId,
                CName = category.CName,
                DateCreated = category.DateCreated,
                TotalArtworks = category.Artworks.Count(),
                ArtworksTitle = category.Artworks != null ? category.Artworks.Select(a => a.Title).ToList() : new List<string>()

            };


            return Ok(categoryDto);
        }

        /// <summary>
        /// It updates an Category
        /// </summary>
        /// <param name="id">The ID of Category which we want to update</param>
        /// <param name="CategoryDto">The required information to update the Category</param>
        /// <returns>
        /// 400 Bad Request or 404 Not Found or 204 No Content
        /// </returns>       
        [HttpPut(template: "Update/{id}")]
        public async Task<IActionResult> AddUpdCategory(int id, UpdCategoryDto updatecategoryDto)
        {
            if (id != updatecategoryDto.CategoryId)
            {
                return BadRequest();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.CategoryId = updatecategoryDto.CategoryId;
            category.CName = updatecategoryDto.CName;   
            category.DateCreated = updatecategoryDto.DateCreated;

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        /// <summary>
        /// Adds an Category 
        /// </summary>
        /// <remarks>
        /// We add an Category with the necessary fields in an AddUpdCategoryDto
        /// </remarks>
        /// <param name="AddUpdCategoryDto">The required information to add the Category</param
        /// <returns>
        /// 201 Created or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Categories/Add -> Add the Category
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<Category>> AddCategory(AddCategoryDto addcategoryDto)
        {
            Category category = new Category()
            {
                CName = addcategoryDto.CName,
                DateCreated = addcategoryDto.DateCreated

            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();


            AddCategoryDto categoryDto = new AddCategoryDto()
            {
                CName = category.CName,
                DateCreated = category.DateCreated
            };

            return CreatedAtAction("FindCategory", new { id = category.CategoryId }, categoryDto);
        }

        /// <summary>
        /// Delete a Category specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Category we want to delete</param>
        /// <returns>
        /// 201 No Content or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Categories/Delete/{id} -> Deletes the Category associated with {id}
        /// </example>
        [HttpDelete(template: "Delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(c => c.CategoryId == id);
        }
    }
}
