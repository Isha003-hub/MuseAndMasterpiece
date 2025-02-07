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
    public class ArtistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArtistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a list of Artists including the Number of Artworks they posted.
        /// </summary>
        /// <returns>
        /// 200 Ok
        /// List of Artists including ID, Name, Bio, Total no. of Artworks they posted, Titles of all the Artworks they posted.
        /// </returns>
        /// <example>
        /// GET: api/Artists/List -> [{ArtistId: 1, Name: "Alice Smith", Bio: "Alice is a contemporary abstract artist.", TotalArtworks: 2, ArtworksTitle:["Elegant Script","Modern Calligraphy"]},{....},{....}]
        /// </example>
        [HttpGet(template: "List")]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> ListArtists()
        {
            List<Artist> Artists = await _context.Artists
                .Include(a => a.Artworks)
                .ToListAsync();

            List<ArtistDto> ArtistDtos = new List<ArtistDto>();

            foreach (Artist Artist in Artists)
            {
                ArtistDtos.Add(new ArtistDto()
                {
                    ArtistId = Artist.ArtistId,
                    Name = Artist.Name,
                    Bio = Artist.Bio,
                    TotalArtworks = Artist.Artworks?.Count() ?? 0,
                    ArtworksTitle = Artist.Artworks != null ? Artist.Artworks.Select(a => a.Title).ToList() : new List<string>()

                });

            }
            // return 200 OK with ArtistDtos
            return Ok(ArtistDtos);
        }


        /// <summary>
        /// Return a Artist specified by it's {id}
        /// </summary>
        /// /// <param name="id">Artist's id</param>
        /// <returns>
        /// 200 Ok
        /// ArtistDto : It includes Artist's ID, Name, Total no. of Artworks they posted, Titles of all the Artworks they posted.
        /// or
        /// 404 Not Found when there is no Artist for that {id}
        /// </returns>
        /// <example>
        /// GET: api/Artists/Find/1 -> {ArtistId: 1, Name: "Alice Smith", Bio: "Alice is a contemporary abstract artist.", TotalArtworks: 2, ArtworksTitle:["Elegant Script","Modern Calligraphy"]}
        [HttpGet(template: "Find/{id}")]
        public async Task<ActionResult<ArtistDto>> FindArtist(int id)
        {
            var artist = await _context.Artists
                .Include(a => a.Artworks)
                .FirstOrDefaultAsync(a => a.ArtistId == id);

            if (artist == null)
            {
                return NotFound();
            }

            ArtistDto artistDto = new ArtistDto()
            {
                ArtistId = artist.ArtistId,
                Name = artist.Name,
                Bio = artist.Bio,
                TotalArtworks = artist.Artworks?.Count() ?? 0,
                ArtworksTitle = artist.Artworks != null ? artist.Artworks.Select(a => a.Title).ToList() : new List<string>()

            };
                

            return Ok(artistDto);
        }

        /// <summary>
        /// It updates an Artist
        /// </summary>
        /// <param name="id">The ID of Artist which we want to update</param>
        /// <param name="ArtistDto">The required information to update the Artist</param>
        /// <returns>
        /// 400 Bad Request
        /// or
        /// 404 Not Found
        /// or
        /// 204 No Content
        /// </returns>       
        [HttpPut(template:"Update/{id}")]
        public async Task<IActionResult> AddUpdArtist(int id, AddUpdArtistDto updateartistDto)
        {
            if (id != updateartistDto.ArtistId)
            {
                return BadRequest();
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            artist.Name = updateartistDto.Name;
            artist.Bio = updateartistDto.Bio;
            artist.Email = updateartistDto.Email;
            

            _context.Entry(artist).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArtistExists(id))
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
        /// Adds an Artist 
        /// </summary>
        /// <remarks>
        /// We add an Artist with the necessary fields in an AddUpdArtistDto
        /// </remarks>
        /// <param name="AddUpdArtistDto">The required information to add the Artist</param
        /// <returns>
        /// 201 Created or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Artists/Add -> Add the Artist
        /// </example>
        [HttpPost(template: "Add")]
        public async Task<ActionResult<Artist>> AddArtist(AddUpdArtistDto addartistDto)
        {
            Artist artist = new Artist()
            {
                Name = addartistDto.Name,
                Bio = addartistDto.Bio,
                Email = addartistDto.Email,

            };

            _context.Artists.Add(artist);
            await _context.SaveChangesAsync();


            AddUpdArtistDto artistDto = new AddUpdArtistDto()
            {
                Name = artist.Name,
                Bio = artist.Bio,
                Email = artist.Email,
            };

            return CreatedAtAction("FindArtist", new { id = artist.ArtistId }, artistDto);
        }

        /// <summary>
        /// Delete a Artist specified by it's {id}
        /// </summary>
        /// <param name="id">The id of the Artist we want to delete</param>
        /// <returns>
        /// 201 No Content or 404 Not Found
        /// </returns>
        /// <example>
        /// api/Artists/Delete/{id} -> Deletes the Artist associated with {id}
        /// </example>
        [HttpDelete(template:"Delete/{id}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.ArtistId == id);
        }
    }
}
