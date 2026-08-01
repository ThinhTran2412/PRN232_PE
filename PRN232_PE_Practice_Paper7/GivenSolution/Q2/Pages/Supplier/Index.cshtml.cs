using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace Q2.Pages.Supplier
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

        public List<SupplierDTO> Suppliers { get; set; } = new List<SupplierDTO>();

        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Specialty { get; set; }


        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


            string url = _baseUrl + $"/api/suppliers/search?name={Name}&specialty={Specialty}";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Suppliers = JsonSerializer.Deserialize<List<SupplierDTO>>(content,options) ?? new List<SupplierDTO>();
            }
        }    
    }
    public class SupplierDTO
    {
        public int supplierId { get; set; }
        public string supplierName { get; set; }
        public string specialty { get; set; }
        public DateTime contractDate { get; set; }
        public int totalProducts { get; set; }
    }
}
