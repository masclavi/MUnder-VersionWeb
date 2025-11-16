using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MUnder.Data;
using MUnder.Models;

namespace MUnder.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Reviews/Song/5
        [HttpGet("Song/{songId}")]
        public async Task<IActionResult> GetSongReview(int songId)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Ok(new { hasReview = false });
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.SongId == songId && r.UserId == userId);

            if (review == null)
            {
                return Ok(new { hasReview = false });
            }

            return Ok(new
            {
                hasReview = true,
                id = review.Id,
                rating = review.Rating,
                comment = review.Comment,
                createdAt = review.CreatedAt
            });
        }

        // POST: api/Reviews/CheckRatingDifference
        [HttpPost("CheckRatingDifference")]
        public async Task<IActionResult> CheckRatingDifference([FromBody] CheckRatingRequest request)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.SongId == request.SongId && r.UserId == userId);

            if (existingReview == null)
            {
                return Ok(new { needsWarning = false });
            }

            int difference = Math.Abs(existingReview.Rating - request.NewRating);

            return Ok(new
            {
                needsWarning = difference >= 3,
                oldRating = existingReview.Rating,
                newRating = request.NewRating,
                difference = difference
            });
        }

        // POST: api/Reviews
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateReview([FromBody] ReviewRequest request)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Usuario no autenticado" });
                }

                // Validaciones
                if (request.Rating < 1 || request.Rating > 5)
                {
                    return BadRequest(new { message = "La calificación debe estar entre 1 y 5" });
                }

                if (string.IsNullOrWhiteSpace(request.Comment))
                {
                    return BadRequest(new { message = "El comentario es obligatorio" });
                }

                if (request.Comment.Length > 500)
                {
                    return BadRequest(new { message = "El comentario no puede exceder 500 caracteres" });
                }

                // Verificar si la canción existe
                var songExists = await _context.Songs.AnyAsync(s => s.Id == request.SongId);
                if (!songExists)
                {
                    return NotFound(new { message = "Canción no encontrada" });
                }

                // Buscar review existente
                var existingReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.SongId == request.SongId && r.UserId == userId);

                if (existingReview != null)
                {
                    // Actualizar review existente
                    existingReview.Rating = request.Rating;
                    existingReview.Comment = request.Comment;
                    existingReview.CreatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Valoración actualizada correctamente",
                        isNew = false,
                        review = new
                        {
                            id = existingReview.Id,
                            rating = existingReview.Rating,
                            comment = existingReview.Comment
                        }
                    });
                }
                else
                {
                    // Crear nueva review
                    var newReview = new Review
                    {
                        UserId = userId,
                        SongId = request.SongId,
                        Rating = request.Rating,
                        Comment = request.Comment,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Reviews.Add(newReview);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Valoración guardada correctamente",
                        isNew = true,
                        review = new
                        {
                            id = newReview.Id,
                            rating = newReview.Rating,
                            comment = newReview.Comment
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CreateOrUpdateReview: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
            }
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.Id == reviewId && r.UserId == userId);

                if (review == null)
                {
                    return NotFound(new { message = "Valoración no encontrada" });
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Valoración eliminada" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteReview: {ex.Message}");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }
    }

    // Modelos de request
    public class ReviewRequest
    {
        public int SongId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }

    public class CheckRatingRequest
    {
        public int SongId { get; set; }
        public int NewRating { get; set; }
    }
}