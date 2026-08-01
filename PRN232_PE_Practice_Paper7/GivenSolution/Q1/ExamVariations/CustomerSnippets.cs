using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models;
using System;
using System.Linq;

namespace Q1.ExamVariations
{
    /* 
     * HƯỚNG DẪN SỬ DỤNG:
     * Đây là các biến thể (variations) thường gặp trong đề thi PE PRN232.
     * Tùy vào đề bài yêu cầu gì, bạn hãy COPY HÀM TƯƠNG ỨNG và paste vào CustomersController gốc của bạn.
     * Mã code đã được viết hoàn chỉnh để bạn đọc hiểu và "ốp" luôn vào bài thi.
     */
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerSnippets : Controller
    {
        private readonly PePrn26spP7Context _context;

        public CustomerSnippets(PePrn26spP7Context context)
        {
            _context = context;
        }

        // ====================================================================================
        // DẠNG 1: SẮP XẾP (SORTING) KẾT HỢP PHÂN TRANG (PAGINATION)
        // Yêu cầu: Trả về khách hàng, sắp xếp Rating giảm dần, Tên tăng dần, có phân trang
        // ====================================================================================
        [HttpGet("sorted")]
        public IActionResult GetCustomersSorted([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = _context.Customers.Select(c => new
            {
                customerId = c.CustomerId,
                customerName = c.CustomerName,
                avgRating = c.Orders.Any() ? c.Orders.Average(o => o.Rating) : 0
            }).AsQueryable();

            // BẮT BUỘC SẮP XẾP TRƯỚC KHI PHÂN TRANG
            query = query.OrderByDescending(c => c.avgRating) // Giảm dần
                         .ThenBy(c => c.customerName);        // Tăng dần

            int total = query.Count();
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Ok(new { data, total, page, pageSize });
        }

        // ====================================================================================
        // DẠNG 2: THỐNG KÊ (SUM, COUNT, MAX) THAY VÌ AVERAGE
        // Yêu cầu: Tính tổng tiền đã mua (Sum), số lượng đơn (Count), ngày mua gần nhất (Max)
        // ====================================================================================
        [HttpGet("stats")]
        public IActionResult GetCustomersStats()
        {
            var data = _context.Customers.Select(c => new
            {
                customerId = c.CustomerId,
                customerName = c.CustomerName,
                totalOrders = c.Orders.Count(), // Đếm số đơn hàng
                totalSpent = c.Orders.Any() ? c.Orders.Sum(o => o.Rating) : 0, // Đề có thể bắt tính Sum(o => o.TotalAmount)
                lastOrderDate = c.Orders.Any() ? c.Orders.Max(o => o.OrderDate) : null // Lấy ngày gần nhất
            }).ToList();

            return Ok(data);
        }

        // ====================================================================================
        // DẠNG 3: LỌC THEO KHOẢNG THỜI GIAN (DATE RANGE)
        // Yêu cầu: Truyền vào startDate, endDate. Tìm các Customer có Order trong khoảng này
        // ====================================================================================
        [HttpGet("daterange")]
        public IActionResult GetCustomersByDateRange([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var query = _context.Customers.AsQueryable();

            // Lọc những khách hàng CÓ ÍT NHẤT 1 ĐƠN HÀNG nằm trong khoảng thời gian
            if (startDate.HasValue)
            {
                query = query.Where(c => c.Orders.Any(o => o.OrderDate >= startDate.Value));
            }
            if (endDate.HasValue)
            {
                query = query.Where(c => c.Orders.Any(o => o.OrderDate <= endDate.Value));
            }

            var data = query.Select(c => new
            {
                c.CustomerId,
                c.CustomerName
            }).ToList();

            return Ok(data);
        }

        // ====================================================================================
        // DẠNG 4: NHẬN THAM SỐ TỪ ROUTE (URL PATH) THAY VÌ QUERY STRING
        // Yêu cầu: URL có dạng /api/customers/loyalty/{minRating} (vd: /api/customers/loyalty/4)
        // ====================================================================================
        [HttpGet("loyalty/{minRating}")]
        public IActionResult GetCustomerLoyaltyByRoute([FromRoute] double minRating)
        {
            var data = _context.Customers.Select(c => new
            {
                c.CustomerId,
                c.CustomerName,
                avgRating = c.Orders.Any() ? c.Orders.Average(o => o.Rating) : 0
            })
            .Where(c => c.avgRating >= minRating)
            .ToList();

            return Ok(data);
        }

        // ====================================================================================
        // DẠNG 5: TRẢ VỀ DỮ LIỆU LỒNG NHAU (NESTED LIST)
        // Yêu cầu: Trả về thông tin Khách hàng, KÈM THEO danh sách các Đơn hàng của họ
        // ====================================================================================
        [HttpGet("with-orders")]
        public IActionResult GetCustomersWithOrders()
        {
            var data = _context.Customers.Select(c => new
            {
                c.CustomerId,
                c.CustomerName,
                // Trả về một List lồng bên trong
                recentOrders = c.Orders.Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.Rating
                })
                .OrderByDescending(o => o.OrderDate)
                .Take(3) // Ví dụ: Lấy 3 đơn gần nhất
                .ToList()
            }).ToList();

            return Ok(data);
        }

        // ====================================================================================
        // DẠNG 6: THÊM XÓA CÓ ĐIỀU KIỆN (SAFE DELETE)
        // Yêu cầu: Xóa Customer. Nếu đã có Order thì không cho xóa (trả về BadRequest).
        // ====================================================================================
        [HttpDelete("{id}")]
        public IActionResult DeleteCustomerSafe(int id)
        {
            // Bắt buộc dùng Include() để Entity Framework nạp dữ liệu Orders của người này lên
            var customer = _context.Customers.Include(c => c.Orders).FirstOrDefault(c => c.CustomerId == id);
            
            if (customer == null)
            {
                return NotFound("Không tìm thấy khách hàng.");
            }

            // Kiểm tra ràng buộc khóa ngoại (Foreign Key)
            if (customer.Orders.Any())
            {
                return BadRequest("Không thể xóa khách hàng đã có đơn hàng.");
            }

            _context.Customers.Remove(customer);
            _context.SaveChanges();

            return Ok("Xóa thành công.");
        }
    }
}
