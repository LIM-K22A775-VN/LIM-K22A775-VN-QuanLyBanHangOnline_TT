using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quanlybanhangonline.Models;
using QuanLyBanHangOnline.Constants;
using QuanLyBanHangOnline.DTO.Review;

namespace QuanLyBanHangOnline.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetReviews()
        {
            return await _context.Review
                .Include(r => r.Product)
                .Include(r => r.User)
                .Select(r => new {
                    r.IdReview,
                    r.IdSP,
                    r.IdUser,
                    r.User.Email, 
                    r.Product.Name,
                    r.Rating,
                    r.Comment
                })
                .ToListAsync();
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            if (_context.Review == null)
            {
                return NotFound();
            }
            var review = await _context.Review.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // PUT: api/Reviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.IdReview)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
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

        // POST: api/Reviews
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PostReview(CreateReviewDto dto)
        {
            // 1. Lấy IdUser từ Token
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int currentUserId = int.Parse(userIdClaim);

            // 2. Kiểm tra đã mua & nhận hàng chưa
            var hasPurchased = await _context.Order
                .AnyAsync(o => o.IdUser == currentUserId
                               && o.Status == Enums.OrderStatus.DaNhanHang
                               && o.OrderDetails.Any(od => od.IdSP == dto.IdSP));

            if (!hasPurchased) return BadRequest("Bạn chỉ có thể đánh giá sản phẩm đã mua và nhận hàng thành công.");

            // 3. XỬ LÝ ĐÁNH GIÁ LẠI: Kiểm tra xem đã có đánh giá cũ chưa
            var existingReview = await _context.Review
                .FirstOrDefaultAsync(r => r.IdUser == currentUserId && r.IdSP == dto.IdSP);

            if (existingReview != null)
            {
                // Cập nhật lại đánh giá cũ
                existingReview.Rating = dto.Rating;
                existingReview.Comment = dto.Comment;
                _context.Review.Update(existingReview);
            }
            else
            {
                // Tạo đánh giá mới nếu chưa có
                var review = new Review
                {
                    IdUser = currentUserId,
                    IdSP = dto.IdSP,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                };
                _context.Review.Add(review);
            }

            // 4. Lưu thay đổi để dữ liệu trong DB được cập nhật trước khi tính trung bình
            await _context.SaveChangesAsync();

            // 5. Tính toán lại AverageRating cho Product
            var ratings = await _context.Review
                .Where(r => r.IdSP == dto.IdSP)
                .Select(r => (int)r.Rating)
                .ToListAsync();

            var product = await _context.Product.FindAsync(dto.IdSP);
            if (product != null && ratings.Any())
            {
                product.AverageRating = Math.Round(ratings.Average(), 1);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Cập nhật đánh giá thành công!", averageRating = product?.AverageRating });
        }
        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            if (_context.Review == null)
            {
                return NotFound();
            }
            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Review.Remove(review);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return (_context.Review?.Any(e => e.IdReview == id)).GetValueOrDefault();
        }
    }
}
