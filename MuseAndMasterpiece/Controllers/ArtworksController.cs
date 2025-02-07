using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuseAndMasterpiece.Data;
using MuseAndMasterpiece.Models;
using NuGet.DependencyResolver;

namespace MuseAndMasterpiece.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtworksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArtworksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Artworks including the Name of Artist who created the Artwork and Category it belongs.
        /// </summary>
        /// <returns>
        /// 200 Ok
        /// List of Artworks including ID, Title, ArtistName and CategoryName
        /// </returns>
        /// <example>
        /// GET: api/Artworks/List -> [{ArtworkId: 1, Title: "Elegant Script", ArtistName : "Alice Smith", CategoryName : "Portrait"},{....},{....}]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<ArtworkDto>>> ListArtworks()
        {
            List<Artwork> Artworks = await _context.Artworks
                .Include(a => a.Category)
                .Include(a => a.Artist)
                .ToListAsync();

            List<ArtworkDto> ArtworkDtos = new List<ArtworkDto>();

            foreach (Artwork Artwork in Artworks)
            {
                ArtworkDtos.Add(new ArtworkDto()
                {
                    ArtWorkId = Artwork.ArtWorkId,
                    Title = Artwork.Title,
                    DatePosted = Artwork.DatePosted,
                    ArtistName = Artwork.Artist.Name,
                    CategoryName = Artwork.Category.CName                    

                });

            }
            // return 200 OK with ArtworkDtos
            return Ok(ArtworkDtos);
        }


        /// <summary>
        /// Return a Artwork specified by it's {id}
        /// </summary>
        /// /// <param name="id">Artwork's id</param>
        /// <returns>
        /// 200 Ok
        /// ArtworkDto : It includes Artwork's ID, Title, ArtistName and CategoryName.
        /// or
        /// 404 Not Found when there is no Artwork for that {id}
        /// </returns>
        /// <example>
        /// GET: api/Artworks/Find/1 -> {ArtworkId: 1, Title: "Elegant Script", ArtistName : "Alice Smith", CategoryName : "Portrait"}
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<ArtworkDto>> FindArtwork(int id)
        {
            var artwork = await _context.Artworks
                .Include(a => a.Category)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.ArtWorkId == id);

            if (artwork == null)
            {
                return NotFound();
            }

            ArtworkDto artistDto = new ArtworkDto()
            {
                ArtWorkId = artwork.ArtWorkId,
                Title = artwork.Title,
                DatePosted = artwork.DatePosted,
                ArtistName = artwork.Artist.Name,
                CategoryName = artwork.Category.CName

            };


            return Ok(artistDto);
        }

        /// <summary>
        /// It updates an Artwork
        /// </summary>
        /// <param name="id">The ID of Artwork which we want to update</param>
        /// <param name="ArtworkDto">The required information to update the Artwork</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>       
        [HttpPut(template:"Update/{id}")]
        public async Task<IActionResult> AddUpdArtwork(int id, AddUpdArtworkDto updateartworkDto)
        {
            if (id != updateartworkDto.ArtWorkId)
            {
                return BadRequest();
            }

            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                return NotFound();
            }

            artwork.ArtWorkId = updateartworkDto.ArtWorkId;
            artwork.Title = updateartworkDto.Title;
            artwork.DatePosted = updateartworkDto.DatePosted;
            artwork.Description = updateartworkDto.Description;
            artwork.CategoryId = updateartworkDto.CategoryId;
            artwork.ArtistId = updateartworkDto.ArtistId;


            _context.Entry(artwork).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtworkExists(id))
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
        /// Adds an Artwork 
        /// </summary>
        /// <remarks>
        /// We add an Artwork with the necessary fields in an AddUpdArtworkDto
        /// </remarks>
        /// <param name="AddUpdArtworkDto">The required information to add the Artwork</param
        /// <returns>
        /// 201 Created or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Artworks/Add -> Add the Artwork
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<Artwork>> AddArtwork(AddUpdArtworkDto addartworkDto)
        {
            Artwork artwork = new Artwork()
            {
                Title = addartworkDto.Title,
                DatePosted = addartworkDto.DatePosted,
                Description = addartworkDto.Description,
                CategoryId = addartworkDto.CategoryId,
                ArtistId = addartworkDto.ArtistId,

            };

            _context.Artworks.Add(artwork);
            await _context.SaveChangesAsync();


            AddUpdArtworkDto artworkDto = new AddUpdArtworkDto()
            {
                Title = artwork.Title,
                DatePosted = artwork.DatePosted,
                Description = artwork.Description,
                CategoryId = artwork.CategoryId,
                ArtistId = artwork.ArtistId
            };

            return CreatedAtAction("FindArtwork", new { id = artwork.ArtWorkId }, artworkDto);
        }

        /// <summary>
        /// Delete a Artwork specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Artwork we want to delete</param>
        /// <returns>
        /// 201 No Content or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Artworks/Delete/{id} -> Deletes the Artwork associated with {id}
        /// </example>
        [HttpDelete(template:"Delete/{id}")]
        public async Task<IActionResult> DeleteArtwork(int id)
        {
            var artwork = await _context.Artworks.FindAsync(id);
            if (artwork == null)
            {
                return NotFound();
            }

            _context.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Retrieves a list of artwork titles based on the specified category ID.
        /// </summary>
        /// <param name="categoryId">The ID of the category to filter artworks.</param>
        /// <returns>
        /// 200 OK - A list of artwork titles.
        /// 404 Not Found - If no artworks exist for the specified category.
        /// </returns>
        /// <example>
        /// GET: api/Artworks/category/1 -> ["Sunset Glow", "Ocean Breeze", "Mountain Serenity"]
        /// </example>


        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetArtworkTitlesByCategory(int categoryId)
        {
            var artworkTitles = await _context.Artworks
                                              .Where(a => a.CategoryId == categoryId)
                                              .Select(a => a.Title)
                                              .ToListAsync();

            if (!artworkTitles.Any())
            {
                return NotFound(new { message = "No artworks found for this category." });
            }

            return Ok(artworkTitles);
        }


        /// <summary>
        /// Links an artwork to a specified category.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to be linked.</param>
        /// <param name="categoryId">The ID of the category to link the artwork to.</param>
        /// <returns>
        /// 200 OK if successfully linked.
        /// 404 Not Found if the artwork or category does not exist.
        /// </returns>
        /// <example>
        /// POST: api/Artworks/LinkArtwork?artworkId=5&categoryId=2 -> Artwork successfully linked to category.
        /// </example>
        [HttpPost("LinkArtwork")]
        public async Task<IActionResult> LinkArtworkToCategory(int artworkId, int categoryId)
        {
            var artwork = await _context.Artworks.FindAsync(artworkId);
            if (artwork == null)
            {
                return NotFound(new { message = "Artwork not found." });
            }

            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            artwork.CategoryId = categoryId;
            _context.Entry(artwork).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Artwork successfully linked to category." });
        }



        /// <summary>
        /// Unlinks an artwork from its current category and assigns it to a default category.
        /// </summary>
        /// <param name="artworkId">The ID of the artwork to be unlinked.</param>
        /// <returns>
        /// 200 OK if successfully unlinked and moved.
        /// 404 Not Found if the artwork does not exist.
        /// 400 Bad Request if the default category is missing.
        /// </returns>
        /// <example>
        /// POST: api/Artworks/UnlinkArtwork?artworkId=5&categoryId=1 -> Artwork 5 unlinked from Category 1.
        /// </example>
        [HttpDelete("UnlinkArtwork")]
        public async Task<IActionResult> UnlinkArtworkFromCategory(int artworkId, int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Artworks)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                return NotFound("Category not found.");
            }

            var artwork = category.Artworks.FirstOrDefault(a => a.ArtWorkId == artworkId);
            if (artwork == null)
            {
                return NotFound("Artwork is not linked to this Category.");
            }

            category.Artworks.Remove(artwork);
            await _context.SaveChangesAsync();

            return Ok($"Artwork {artworkId} unlinked from Category {categoryId}.");
        }



        private bool ArtworkExists(int id)
        {
            return _context.Artworks.Any(e => e.ArtWorkId == id);
        }
    }
}
