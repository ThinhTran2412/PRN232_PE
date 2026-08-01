using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Q2.Pages.Supplier
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


        public SupplierDetailDTO SupplierInfo { get; set; }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient();
            string url = _baseUrl + $"/api/suppliers/{id}";
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                SupplierInfo = JsonSerializer.Deserialize<SupplierDetailDTO>(content, options);
                return Page();
            }
            return NotFound();
        }
    }

    public class SupplierDetailDTO
    {
        public int supplierId { get; set; }
        public string supplierName { get; set; }
        public string specialty { get; set; }
        public List<ProductDTO> products { get; set; }
    }

    public class ProductDTO
    {
        public int productID { get; set; }
        public string productName { get; set; }
        public int price { get; set; }
    }
}
