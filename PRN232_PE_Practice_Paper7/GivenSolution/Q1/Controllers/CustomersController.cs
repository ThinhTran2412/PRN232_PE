using Microsoft.AspNetCore.Mvc;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly PePrn26spP7Context _context;

        public CustomersController(PePrn26spP7Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetCustomers()
        {
            var customers = _context.Customers.Select(c => new
            {
                customerId = c.CustomerId,
                customerName = c.CustomerName,
                email = c.Email,
                avgRating = c.Orders.Where(o => o.Rating != null).Any() ?
                c.Orders.Where(o => o.Rating != null).Average(a => a.Rating) : 0
            }).ToList();

            return Ok(customers);
        }

        [HttpGet("/api/customer-loyalty")]
        public IActionResult GetCustomerLoyalty([FromQuery] double? minRating, [FromQuery] string? customerName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if(page <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }

            var query = _context.Customers.Select(c => new
            {
                customerId = c.CustomerId,
                customerName = c.CustomerName,
                email = c.Email,
                avgRating = c.Orders.Where(o => o.Rating != null).Any() ?
                c.Orders.Where(o => o.Rating != null).Average(a => a.Rating) : 0
            }).AsQueryable();

            if (minRating.HasValue)
            {
                query = query.Where(c => c.avgRating >= minRating);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(c => c.customerName.ToLower().Contains(customerName.ToLower()));
            }

            int totalCustomers = query.Count();
            int totalPages = (int)Math.Ceiling(totalCustomers / (double)pageSize);
            var data = query.Skip((page -1) * pageSize).Take(pageSize).ToList();

            return Ok(new
            {
                data = data,
                totalCustomers = totalCustomers,
                totalPages = totalPages,
                currentPage = page,
                pageSize = pageSize
            });
        }
    }
}
