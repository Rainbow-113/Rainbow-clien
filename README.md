# 🌈 Rainbow — Website Thương Mại Điện Tử (Frontend)

Giao diện người dùng của hệ thống bán hàng thời trang **Rainbow**, xây dựng bằng ASP.NET Core MVC. Ứng dụng giao tiếp với backend Node.js thông qua HTTP Client để hiển thị sản phẩm, quản lý giỏ hàng và xử lý đặt hàng.

> **Công nghệ:** ASP.NET Core MVC (.NET 8) · HTML · CSS · SCSS · JavaScript · Session Authentication

---

## ✨ Tính năng

- 🛍️ **Danh sách sản phẩm** — Hiển thị sản phẩm theo danh mục
- 🛒 **Giỏ hàng** — Thêm, xóa, cập nhật số lượng sản phẩm
- 💳 **Thanh toán (Checkout)** — Xử lý đơn hàng
- 👤 **Đăng nhập / Đăng ký** — Xác thực người dùng qua Session
- 🔐 **Khu vực Admin** — Quản lý sản phẩm, danh mục (Area riêng biệt)

---

## 🛠️ Công nghệ sử dụng

| Tầng | Công nghệ |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| Giao diện | Razor Views, HTML, CSS, SCSS, Less |
| Script | JavaScript |
| Xác thực | Session (`AddSession`) |
| Giao tiếp backend | `HttpClient` (gọi REST API Node.js) |
| Backend API | Node.js (chạy tại `http://localhost:5000`) |

---

## 📁 Cấu trúc thư mục

```
Rainbow-clien/
├── Areas/
│   └── Admin/                  # Khu vực quản trị (Admin Area)
├── Controllers/                # Các controller xử lý request
├── Models/                     # Các model dữ liệu (Product, Category, User, ...)
├── Services/                   # Tầng gọi API: ProductService, CategoryService,
│                               # UserService, CartService, CheckoutService
├── Views/                      # Razor Views (giao diện người dùng)
├── wwwroot/                    # File tĩnh (CSS, JS, hình ảnh)
├── Program.cs                  # Cấu hình ứng dụng, đăng ký service, routing
├── appsettings.json            # Cấu hình ứng dụng
└── Rainbow.sln                 # Solution file
```

---

## 🚀 Hướng dẫn cài đặt

### Yêu cầu

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Backend Node.js (repo [Rainbow-api](https://github.com/Rainbow-113/Rainbow-api)) đang chạy tại `http://localhost:5000`

---

### Các bước chạy

1. Clone repository:
   ```bash
   git clone https://github.com/Rainbow-113/Rainbow-clien.git
   cd Rainbow-clien
   ```

2. Đảm bảo backend Node.js đang chạy trước:
   ```bash
   # Trong thư mục Rainbow-api
   node server.js
   # hoặc
   npm start
   ```

3. Chạy ứng dụng ASP.NET Core:
   ```bash
   dotnet run
   ```

4. Truy cập trình duyệt tại:
   ```
   https://localhost:7xxx   (HTTPS)
   http://localhost:5xxx    (HTTP)
   ```

> **Lưu ý:** URL backend mặc định là `http://localhost:5000`. Nếu Node.js chạy ở cổng khác, hãy cập nhật lại trong `Program.cs`.

---

## 🏗️ Kiến trúc hệ thống

```
Trình duyệt
    ↓
ASP.NET Core MVC (Rainbow-clien)   ←→   Node.js API (Rainbow-api)
    ↓                                          ↓
Razor Views                              MongoDB Atlas
```

### Tầng Service
Mỗi nghiệp vụ có một Service riêng sử dụng `HttpClient` để gọi API:

| Service | Chức năng |
|---|---|
| `ProductService` | Lấy danh sách, chi tiết sản phẩm |
| `CategoryService` | Lấy danh mục sản phẩm |
| `UserService` | Đăng ký, đăng nhập người dùng |
| `CartService` | Quản lý giỏ hàng |
| `CheckoutService` | Xử lý thanh toán, đặt hàng |

### Xác thực
Ứng dụng sử dụng **Session** để lưu trạng thái đăng nhập. Session hết hạn sau **30 phút** không hoạt động.

---

## 🔗 Repository liên quan

| Repo | Mô tả | Link |
|---|---|---|
| **Rainbow-clien** (repo này) | Giao diện ASP.NET Core MVC | [xem](https://github.com/Rainbow-113/Rainbow-clien) |
| **Rainbow-api** | Backend Node.js + MongoDB | [xem](https://github.com/Rainbow-113/Rainbow-api) |

---

---

## 👤 Tác giả

**Nguyễn Văn Đan**  
Sinh viên CNTT @ Bachkhoa-Aptech, Hà Nội  
GitHub: [@Rainbow-113](https://github.com/Rainbow-113)

---
Dự án này được xây dựng phục vụ mục đích học tập và portfolio cá nhân.
