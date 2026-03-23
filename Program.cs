using Rainbow.Services;

namespace Rainbow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // Đăng ký ProductService để các Controller có thể sử dụng
            builder.Services.AddHttpClient<ProductService>(client => {
                client.BaseAddress = new Uri("http://localhost:5000/");
            });
            builder.Services.AddHttpClient<CategoryService>(client => {
                client.BaseAddress = new Uri("http://localhost:5000/"); // Địa chỉ chạy Node.js của bạn
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            // 1. Route cho các Area (Admin, v.v.)
            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            
           

            app.Run();
        }
    }
}
