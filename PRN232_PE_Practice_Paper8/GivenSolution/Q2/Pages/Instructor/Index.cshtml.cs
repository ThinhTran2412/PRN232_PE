using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Q2.Pages.Instructor
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly string _baseUrl;

        public IndexModel(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _config = config;
            _baseUrl = _config["GivenAPIBaseUrl"] ?? "http://localhost:5100";
        }

        public List<InstructorDTO> Instructors { get; set; } = new List<InstructorDTO>();

        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Department { get; set; }

        public async Task OnGetAsync()
        {
            // TODO: Call API GET /api/instructors/search?name={name}&department={department}
            // Populate the Instructors list.
        }
    }

    public class InstructorDTO
    {
        public int InstructorId { get; set; }
        public string InstructorName { get; set; } = null!;
        public string? Department { get; set; }
        public DateTime? HireDate { get; set; }
        public int TotalCourses { get; set; }
    }
}
