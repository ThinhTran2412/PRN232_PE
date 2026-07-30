using Microsoft.AspNetCore.Mvc;
using givenAPI.Models;
using System.Linq;

namespace givenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecialtiesController : ControllerBase
    {
        // GET /api/specialties
        [HttpGet]
        public IActionResult GetSpecialties()
        {
            var specialties = DataInitializer.Suppliers
                .Select(s => s.Specialty)
                .Distinct()
                .ToList();
            return Ok(specialties);
        }
    }
}
