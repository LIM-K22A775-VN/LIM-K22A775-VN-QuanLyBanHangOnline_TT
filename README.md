Dự án Quản Lý Bán Hàng Online Hệ thống quản lý bán hàng tích hợp Web API (ASP.NET Core) và Frontend (Angular). Hỗ trợ quản lý sản phẩm, đơn hàng, nhân viên và phân quyền chi tiết.
2. Công nghệ sử dụng (Tech Stack)

Backend: ASP.NET Core 6.0, Entity Framework Core.
Database: SQL Server.
Security: JWT (JSON Web Token), BCrypt để mã hóa mật khẩu.
Validation: FluentValidation cho các DTO.
Frontend: Angular (hoặc tùy framework bạn đang dùng).

3. Tính năng chính (Key Features)
 . Quản lý đơn hàng: Quy trình đặt hàng, trừ kho tự động và cập nhật trạng thái giao hàng.
 .Thông tin giao hàng: Lưu trữ ReceiverName, ReceiverPhone và ShippingAddress cho từng đơn hàng.
 .Phân quyền (RBAC): Quản lý vai trò (Roles) và nhân viên (Staffs) với các quyền hạn khác nhau.
 .Xác thực: Đăng ký/Đăng nhập người dùng và hệ thống Refresh Token.

4. Cấu trúc Database (Database Schema)
User & Staff: Quản lý tài khoản khách hàng và nhân viên nội bộ.
Role: Hệ thống phân quyền (RBAC) cho phép tùy chỉnh quyền hạn.
Product & ProductDetail: Lưu trữ thông tin sản phẩm và chi tiết thuộc tính (Size, Color, Description).
Order & OrderDetail: Quản lý đơn hàng, lưu vết thông tin người nhận tại thời điểm đặt hàng.
Review: Hệ thống đánh giá sản phẩm từ người dùng.


Hướng dẫn cài đặt
1.git clone 
2.Cấu hình Database: Mở file appsettings.json và cập nhật chuỗi kết nối:
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=QuanLyBanHangOnline_n1;Trusted_Connection=True;"
}
3.Migration & Update Database: Chạy lệnh sau trong Package Manager Console
Update-Database
4.Chạy dự án: Nhấn F5 hoặc chạy lệnh dotnet run. Truy cập Swagger tại /swagger để kiểm tra API.
