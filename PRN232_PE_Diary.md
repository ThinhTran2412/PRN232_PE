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
### Phiên 1: Phân tích & Làm Paper 7 (Q1 & Q2)

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
