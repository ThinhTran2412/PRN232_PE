using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Q2.Pages.Instructor
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;

        public DetailsModel(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
            _baseUrl = _config["GivenAPIBaseUrl"] ?? "http://localhost:5100";
        }

        public InstructorDetailDTO InstructorInfo { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // TODO: Call API GET /api/instructors/{id}
            // Populate InstructorInfo. If not found, return NotFound().
            return Page();
        }
    }

    public class InstructorDetailDTO
    {
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = null!;
        public string? Department { get; set; }
        public List<CourseDTO> Courses { get; set; } = new List<CourseDTO>();
    }

    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public int Credits { get; set; }
    }
}
