using Microsoft.AspNetCore.Mvc;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : Controller
    {
        private readonly PePrn26spP7Context _context;

        public OrdersController(PePrn26spP7Context context)
        {
            _context = context;
        }

        public class RatingRequest
        {
            public float Rating { get; set; }
        }

        [HttpPut("{orderId}/rating")]
        public IActionResult UpdateOrderRating(int orderId, [FromBody] RatingRequest request)
        {
            if (request.Rating < 0 || request.Rating > 5)
            {
                return BadRequest("Rating must be between 0 and 5");
            }

            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            order.Rating = request.Rating;
            _context.SaveChanges();

            return Ok(new
            {
                orderId = order.OrderId,
                customerId = order.CustomerId,
                rating = order.Rating
            });
        }

        [HttpDelete("{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if(order == null)
            {
                return NotFound("No order found with provided OrderId");
            }

            if(order.Rating != null)
            {
                return BadRequest("Cannot cancel an order that has already been rated");
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
