using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Q1.Models; // Giả định namespace Models chứa PePrn26spP7Context

namespace Q1.ExamVariations
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvancedSupplierSnippets : ControllerBase
    {
        private readonly PePrn26spP7Context _context;

        public AdvancedSupplierSnippets(PePrn26spP7Context context)
        {
            _context = context;
        }

        // ====================================================================================
        // TRUY VẤN NÂNG CAO (PHÒNG HỜ): Thống kê theo Quarter với Grouping/SelectMany
        // Mức độ: Khó hơn PE thông thường một chút, kết hợp nhiều bảng (Nhiều-Nhiều)
        // ====================================================================================
        [HttpGet("performance")]
        public IActionResult GetPerformance([FromQuery] string quarter, [FromQuery] int minTotalQuantity = 0)
        {
            if (string.IsNullOrEmpty(quarter))
            {
                return BadRequest("Quarter is required.");
            }

            var query = _context.Suppliers.Select(s => new
            {
                supplierId = s.SupplierId,
                supplierName = s.SupplierName,
                specialty = s.Specialty,
                
                totalQuantity = s.ProductSuppliers
                                 .Select(ps => ps.Product)
                                 .SelectMany(p => p.Batches)
                                 .Where(b => b.Quarter == quarter)
                                 .Sum(b => (int?)b.Quantity) ?? 0,
                                 
                productCount = s.ProductSuppliers
                                .Where(ps => ps.Product.Batches.Any(b => b.Quarter == quarter))
                                .Select(ps => ps.ProductId)
                                .Distinct()
                                .Count()
            }).AsQueryable();

            if (minTotalQuantity > 0)
            {
                query = query.Where(q => q.totalQuantity >= minTotalQuantity);
            }

            query = query.OrderByDescending(q => q.totalQuantity)
                         .ThenBy(q => q.supplierName);

            return Ok(query.ToList());
        }
    }
}
