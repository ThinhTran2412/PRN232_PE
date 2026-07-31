# Hướng dẫn từng bước làm bài thi PRN232 - Paper 7

Tài liệu này sẽ hướng dẫn bạn từ lúc mở project `GivenSolution` cho đến khi hoàn thiện 100% hai câu Q1 và Q2 mà không viết code dư thừa, tránh tối đa các lỗi thường gặp (đặc biệt là lỗi đặt sai Id thẻ HTML ở Q2). 

Hãy làm theo từng bước cùng mình nhé!

---

## 🎯 CÂU 1 (Q1) - XÂY DỰNG WEB API

### Bước 1: Khôi phục Models & Cấu hình Connection String
### Bước 1: Chạy lệnh Scaffold Models từ Database
Trong phòng thi, bạn sẽ không có sẵn folder `Models`, do đó bạn phải tự generate từ file `.sql` đề bài cho.
- Bước 1.1: Chạy file `database.sql` (nằm trong file zip đề bài) trong SQL Server Management Studio (SSMS) để tạo database.
- Bước 1.2: Mở Terminal ở thư mục gốc của project `Q1`. (Nếu dùng Visual Studio, mở **View > Terminal**).
- Bước 1.3: Chạy lệnh sau để tạo Models (lưu ý đổi lại tên Server cho đúng máy bạn):
```powershell
dotnet ef dbcontext scaffold "Data Source=LAPTOP-0S0P0DLI\SQL;Initial Catalog=PE_PRN_26SP_P7;User ID=sa;Password=12345;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False" Microsoft.EntityFrameworkCore.SqlServer -o Models
```
*(Nếu máy báo lỗi `dotnet ef` chưa được cài, hãy gõ lệnh: `dotnet tool install --global dotnet-ef` trước, sau đó chạy lại lệnh scaffold).*

Tiếp theo, mở `appsettings.json` trong Q1 và thêm `ConnectionStrings`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "MyCnn": "Server=localhost;Database=PE_PRN_26SP_P7;Trusted_Connection=True;Encrypt=False"
  }
}
```
*(Lưu ý: Thay đổi phần `Server=localhost` cho phù hợp với tên Server trên máy bạn).*

### Bước 2: Cấu hình `Program.cs`
Đề bài yêu cầu API `/api/customers` phải hỗ trợ trả về cả **JSON và XML**. Do đó, bạn cần sửa `builder.Services.AddControllers()` thành:

```csharp
using Microsoft.EntityFrameworkCore;
using Q1.Models;

var builder = WebApplication.CreateBuilder(args);

// Bắt buộc cấu hình AddXmlDataContractSerializerFormatters để hỗ trợ trả về XML
builder.Services.AddControllers(options =>
{
    options.RespectBrowserAcceptHeader = true;
}).AddXmlDataContractSerializerFormatters();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đọc ConnectionString bắt buộc dùng "MyCnn"
builder.Services.AddDbContext<PePrn26spP7Context>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### Bước 3: Tạo `CustomersController.cs`
Tạo folder `Controllers` trong Q1, sau đó add class `CustomersController.cs`. 
Chúng ta cần giải quyết 2 API: `GET /api/customers` và `GET /api/customer-loyalty`.

```csharp
using Microsoft.AspNetCore.Mvc;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly PePrn26spP7Context _context;

        public CustomersController(PePrn26spP7Context context)
        {
            _context = context;
        }

        // 1. GET /api/customers
        [HttpGet]
        public IActionResult GetCustomers()
        {
            // LINQ xử lý gọn gàng: Dùng toán tử 3 ngôi (Điều_Kiện ? Nếu_Đúng : Nếu_Sai)
            var customers = _context.Customers.Select(c => new
            {
                customerId = c.CustomerId,
                customerName = c.CustomerName,
                email = c.Email,
                
                // Lọc bỏ Rating null. 
                // Nếu Any() trả về True (có order hợp lệ) -> Gọi Average()
                // Nếu Any() trả về False (không có order nào) -> Trả về 0
                avgRating = c.Orders.Where(o => o.Rating != null).Any()
                            ? c.Orders.Where(o => o.Rating != null).Average(o => o.Rating)
                            : 0 
            }).ToList();

            return Ok(customers);
        }

        // 2. GET /api/customer-loyalty
        [HttpGet("/api/customer-loyalty")]
        public IActionResult GetCustomerLoyalty([FromQuery] double? minRating, [FromQuery] string? customerName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Invalid pagination parameters.");
            }

            var query = _context.Customers.Select(c => new
            {
                customerId = c.CustomerId,
                customerName = c.CustomerName,
                email = c.Email,
                avgRating = c.Orders.Where(o => o.Rating != null).Any()
                            ? c.Orders.Where(o => o.Rating != null).Average(o => o.Rating)
                            : 0
            }).AsQueryable();

            if (minRating.HasValue)
            {
                query = query.Where(c => c.avgRating >= minRating.Value);
            }

            if (!string.IsNullOrEmpty(customerName))
            {
                query = query.Where(c => c.customerName.ToLower().Contains(customerName.ToLower()));
            }

            int totalCustomers = query.Count();
            int totalPages = (int)Math.Ceiling(totalCustomers / (double)pageSize);
            var data = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

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
```

### Bước 4: Tạo `OrdersController.cs`
Xử lý các logic cập nhật rating và xóa (cancel) order. Đặc biệt cẩn thận các mã lỗi 400, 404, 204.

```csharp
using Microsoft.AspNetCore.Mvc;
using Q1.Models;

namespace Q1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
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

        // 3. PUT /api/orders/{orderId}/rating
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
                return NotFound(); // 404
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

        // 4. DELETE /api/orders/{orderId}
        [HttpDelete("{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            var order = _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                return NotFound("No order found with provided OrderId"); // 404
            }

            if (order.Rating != null)
            {
                return BadRequest("Cannot cancel an order that has already been rated"); // 400
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent(); // 204
        }
    }
}
```

---

## 🎯 CÂU 2 (Q2) - RAZOR PAGES & HTTPCLIENT

### Bước 1: Cấu hình `appsettings.json` và `Program.cs`
Thêm URL theo đúng yêu cầu đề vào `appsettings.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "GivenAPIBaseUrl": "http://localhost:5100"
}
```
Mở `Program.cs` của Q2 (Thường project có sẵn đã add `builder.Services.AddRazorPages();`, nếu thiếu bạn hãy chắc chắn rằng nó có đủ).
Đồng thời, thêm `builder.Services.AddHttpClient();` để dùng `IHttpClientFactory`.

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpClient(); // Thêm dòng này 

var app = builder.Build();
...
app.MapRazorPages();
app.Run();
```

### Bước 2: Bố trí thư mục `Pages/Supplier`
Tạo thư mục `Supplier` bên trong `Pages`. Trong thư mục này, ta tạo 2 trang:
- `Index.cshtml` & `Index.cshtml.cs`
- `Details.cshtml` & `Details.cshtml.cs`

#### A. Trang Danh Sách (Index)
Tạo file `Index.cshtml.cs`:
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            // Đề bài bắt buộc lấy URL và nối kiểu cộng xâu
            _baseUrl = _config["GivenAPIBaseUrl"] ?? "http://localhost:5100";
        }

        public List<SupplierDTO> Suppliers { get; set; } = new List<SupplierDTO>();
        public List<string> Specialties { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public string? Name { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Specialty { get; set; }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // Lấy danh sách specialties cho Dropdown (nếu muốn, hoặc để text input theo đề bài yêu cầu là <input>)
            // Đề bài yêu cầu: Input field cho Specialty. Ta cứ làm ô input text.

            // Lấy danh sách suppliers
            string url = _baseUrl + $"/api/suppliers/search?name={Name}&specialty={Specialty}";
            
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Suppliers = JsonSerializer.Deserialize<List<SupplierDTO>>(content, options) ?? new List<SupplierDTO>();
            }
        }
    }

    // Các class DTO để hứng dữ liệu
    public class SupplierDTO
    {
        public int supplierId { get; set; }
        public string supplierName { get; set; }
        public string specialty { get; set; }
        public DateTime contractDate { get; set; }
        public int totalProducts { get; set; }
    }
}
```

Tạo file `Index.cshtml`:
**LƯU Ý:** Quan trọng nhất của bài này là **TẤT CẢ ID PHẢI ĐÚNG 100% YÊU CẦU**.
```html
@page
@model Q2.Pages.Supplier.IndexModel
@{
}

<h2>Search Suppliers</h2>

<form method="get">
    <div>
        <label>Supplier Name:</label>
        <input type="text" name="Name" id="ip_supplierName" value="@Model.Name" />
    </div>
    <div>
        <label>Specialty:</label>
        <input type="text" name="Specialty" id="ip_specialty" value="@Model.Specialty" />
    </div>
    <button type="submit" id="bt_search">Search</button>
</form>

<hr />

<table border="1">
    <thead>
        <tr>
            <th>Supplier Name</th>
            <th>Specialty</th>
            <th>Contract Date</th>
            <th>Total Products</th>
            <th>Action</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var s in Model.Suppliers)
        {
            <tr>
                <td id="td_supplierName_@(s.supplierId)">@s.supplierName</td>
                <td id="td_specialty_@(s.supplierId)">@s.specialty</td>
                <td id="td_contractDate_@(s.supplierId)">@s.contractDate.ToString("yyyy-MM-dd")</td>
                <td id="td_totalProducts_@(s.supplierId)">@s.totalProducts</td>
                <td>
                    <!-- Đề bài yêu cầu ID chuẩn là a_{supplierId} -->
                    <a id="a_@(s.supplierId)" href="/Supplier/Details?id=@s.supplierId">View Products</a>
                </td>
            </tr>
        }
    </tbody>
</table>
```
*(Lưu ý: Bạn có thể thay đổi routing của trang detail là `/Supplier/{SupplierId}` thay vì query string nếu cấu hình Route `[page "{id}"]` trong `Details.cshtml`)*

#### B. Trang Chi Tiết (Details)
Tạo file `Details.cshtml.cs`:
```csharp
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
```

Tạo file `Details.cshtml`:
```html
@page "{id:int?}"
@model Q2.Pages.Supplier.DetailsModel
@{
}

<!-- ID cho thẻ span đặc biệt quan trọng -->
<h3>Supplier Details</h3>
<div>
    <strong>Supplier ID:</strong> <span id="span_@(Model.SupplierInfo.supplierId)">@Model.SupplierInfo.supplierId</span><br/>
    <strong>Name:</strong> <span id="span_@(Model.SupplierInfo.supplierName)">@Model.SupplierInfo.supplierName</span><br/>
    <strong>Specialty:</strong> <span id="span_@(Model.SupplierInfo.specialty)">@Model.SupplierInfo.specialty</span>
</div>

<hr />
<h4>Products</h4>
<table border="1">
    <thead>
        <tr>
            <th>Product ID</th>
            <th>Product Name</th>
            <th>Price</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var p in Model.SupplierInfo.products)
        {
            <tr>
                <td id="td_productID_@(p.productID)">@p.productID</td>
                <td id="td_productName_@(p.productID)">@p.productName</td>
                <td id="td_price_@(p.productID)">@p.price</td>
            </tr>
        }
    </tbody>
</table>

<a href="/Supplier">Back to List</a>
```
*(Nếu bạn cấu hình `@page "{id:int?}"` thì link từ trang Index sửa thành `href="/Supplier/Details/@s.supplierId"` sẽ rất đẹp và chuẩn)*

---

Đó là toàn bộ luồng code. Bạn thấy đoạn nào chưa rõ hoặc muốn tự tay gõ vào chỗ nào trước, thì cứ bảo tui. Khi bạn làm xong Q1, hãy test thử trên Postman/Swagger trước khi qua Q2 nhé!
