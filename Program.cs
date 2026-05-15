using Rainbow.Services;

namespace Rainbow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddHttpContextAccessor();
            // login
            builder.Services.AddDistributedMemoryCache(); 

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session hết hạn sau 30 phút
                options.Cookie.HttpOnly = true; // Bảo mật Cookie
                options.Cookie.IsEssential = true;
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddHttpClient<ProductService>(client => {
                client.BaseAddress = new Uri("http://localhost:5000/");
            });
            builder.Services.AddHttpClient<CategoryService>(client => {
                client.BaseAddress = new Uri("http://localhost:5000/"); // Địa chỉ chạy Node.js của bạn
            });
            builder.Services.AddHttpClient<UserService>(client => {
                client.BaseAddress = new Uri("http://localhost:5000/"); 
            });
            builder.Services.AddHttpClient<ICartService, CartService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddHttpClient<ICheckoutService, CheckoutService>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5000/"); // Địa chỉ API Node.js của bạn
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
            app.UseSession();

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
