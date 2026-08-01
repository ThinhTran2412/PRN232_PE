# Nhật ký ôn thi PRN232 - Practical Exam (PE)

## 📌 Các quy định chung & Lưu ý quan trọng (Đúc kết từ file hướng dẫn của trường)
1. **Nộp bài theo từng Question** và bắt buộc **publish project** trước khi nộp.
   - Dùng lệnh: `dotnet publish -c release -o ./[QuestionNumber_StudentCode]`
   - Nộp toàn bộ thư mục gốc của project (có thể xóa `/bin`, `/obj`, `/runtimes` nếu dung lượng quá lớn).
2. **Framework & Công cụ:**
   - Visual Studio 2022++
   - .NET 8.0
3. **Tuyệt đối không dùng code có sẵn, không chia sẻ data.** Chỉ dùng tài liệu trên máy.
4. **Đối với Q1 (Web API):**
   - Kết nối DB từ `appsettings.json` bằng key `"MyCnn"`. KHÔNG được hardcode.
   - Thêm formatters cho JSON và XML nếu được yêu cầu (nhớ thêm config trong Program.cs).
   - API endpoints phải chuẩn format (camelCase mặc định).
5. **Đối với Q2 (Razor Pages / Web App):**
   - Lấy URL của API gốc từ biến `GivenAPIBaseUrl` trong `appsettings.json`. KHÔNG tự ý hardcode hoặc lấy bằng cách khác.
   - Nối URL bằng toán tử `+` tường minh (vd: `baseUrl + "/endpoint"`).
   - **Đặc biệt lưu ý:** Gắn thẻ HTML `Id` chính xác 100% theo yêu cầu đề bài. Không được thêm bớt các thẻ con không cần thiết.

---

## 🗓️ Ngày ôn tập: 31/07/2026
### Phiên 1: Phân tích & Làm Paper 7 (Q1)

#### Bài học rút ra từ Q1:
1. Lệnh Scaffold Models (nhớ mở đúng project trong PMC):
`Scaffold-DbContext "Server=...;Database=...;Trusted_Connection=True;Encrypt=False" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models`
2. Toán tử 3 ngôi kết hợp `Any()` để chống lỗi `Sequence contains no elements` khi tính trung bình (Average) của list rỗng:
`c.Orders.Where(o => o.Rating != null).Any() ? c.Orders.Where(o => o.Rating != null).Average(o => o.Rating) : 0`
3. Công thức phân trang kinh điển: `.Skip((page - 1) * pageSize).Take(pageSize)`
4. Nếu API yêu cầu nhận Object JSON (có dấu ngoặc nhọn `{}`) -> Bắt buộc phải tạo `Class/DTO` và hứng bằng đúng 1 chữ `[FromBody]`.

#### Những chỗ dễ sai & Kinh nghiệm:
- Kiểm tra cực kỹ tên biến lúc xuất JSON (vd: `customerId` chứ không phải `customerid` hay `CustomerId`).
- Để ý các dấu chấm câu trong chuỗi thông báo lỗi (vd: `"Invalid pagination parameters."` có dấu chấm).
- KHÔNG dùng 2 từ khóa `[FromBody]` trong cùng 1 hàm.

---

## 🗓️ Ngày ôn tập: 01/08/2026
### Phiên 2: Hoàn thiện Paper 7 (Q2 - Razor Pages)

#### Bài học rút ra từ Q2 (Razor Pages & HttpClient):
1. **Tuyệt chiêu Model Binding cho form GET:** Dùng `[BindProperty(SupportsGet = true)]` cho các tham số search để tự động hứng dữ liệu từ URL xuống thẳng biến C# (rất nhàn, khỏi cần móc từ QueryString).
2. **Copy Y Nguyên Giao Diện Của Đề:** Nếu đề cho sẵn các file `list.html` và `detail.html`, hãy lấy toàn bộ HTML dán ngay bên dưới `@model` (tuyệt đối KHÔNG nhét HTML vào lồng `@{ }`). Sau đó mới dùng `@foreach` để biến tĩnh thành động.
3. **Format ngày tháng:** Bắt buộc ép kiểu `ToString("yyyy-MM-dd")` cho ngày tháng hiển thị trên View để qua ải máy chấm (nếu nullable thì dùng `?.ToString("yyyy-MM-dd")`).
4. **Quy tắc đúc khuôn JSON (Deserialize):**
   - Đọc kỹ cấu trúc JSON trả về từ Postman/Swagger hoặc đọc đề bài phân tích.
   - Nếu JSON trả mảng `[ ]` ngoài cùng -> Dùng `List<ClassName>`.
   - Nếu JSON trả mảng `[ ]` lồng bên trong một mảng khác -> C# phải khai báo một biến kiểu `public List<...> TênBiến { get; set; }` nằm bên trong Class bọc ngoài.
   - Nhớ bật bùa `PropertyNameCaseInsensitive = true` để chống lỗi viết hoa/thường, nhưng khi làm bài vẫn nên Copy đúng y xì đúc 100% tên biến của JSON sang.
5. **Cạm bẫy thẻ Input:** Thường hay bị ảo tưởng các Dropdown là thẻ `<select>`, nhưng nhớ phải "Soi" thật kỹ yêu cầu đề thi (phần `Summary of HTML Elements ID`). Nó bảo `<input>` thì bắt buộc phải để là `<input type="text">`. Đừng tự "chế" bậy bạ.
