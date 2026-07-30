using Microsoft.AspNetCore.Mvc;
using givenAPI.Models;
using System.Linq;

namespace givenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        // 1. GET /api/suppliers/search?name={name}&specialty={specialty}
        // Tra ve danh sach nha cung cap kem so luong san pham cung ung
        [HttpGet("search")]
        public IActionResult Search(string name = "", string specialty = "")
        {
            var result = DataInitializer.Suppliers
                .Where(s => (string.IsNullOrEmpty(name) || s.SupplierName.Contains(name, StringComparison.OrdinalIgnoreCase))
                         && (string.IsNullOrEmpty(specialty) || s.Specialty.Contains(specialty, StringComparison.OrdinalIgnoreCase)))
                .Select(s => new {
                    SupplierId = s.SupplierId,
                    SupplierName = s.SupplierName,
                    Specialty = s.Specialty,
                    ContractDate = s.ContractDate,
                    // Dem tong so san pham tu bang trung gian ProductSuppliers
                    TotalProducts = DataInitializer.ProductSuppliers.Count(ps => ps.SupplierId == s.SupplierId)
                }).ToList();

            return Ok(result);
        }

        // 2. GET /api/suppliers/{supplierId}
        // Tra ve thong tin chi tiet nha cung cap va danh sach san pham cung ung
        [HttpGet("{supplierId}")]
        public IActionResult GetDetails(int supplierId)
        {
            var supplier = DataInitializer.Suppliers.FirstOrDefault(s => s.SupplierId == supplierId);
            if (supplier == null) return NotFound(new { Message = "Supplier not found." });

            // Lay danh sach san pham thong qua bang ProductSuppliers
            var suppliedProducts = from ps in DataInitializer.ProductSuppliers
                                   join p in DataInitializer.Products on ps.ProductId equals p.ProductId
                                   where ps.SupplierId == supplierId
                                   select new
                                   {
                                       ProductId = p.ProductId,
                                       ProductName = p.ProductName,
                                       Price = p.Price
                                   };

            return Ok(new
            {
                SupplierId = supplier.SupplierId,
                SupplierName = supplier.SupplierName,
                Specialty = supplier.Specialty,
                ContractDate = supplier.ContractDate,
                Products = suppliedProducts.ToList()
            });
        }
    }
}
