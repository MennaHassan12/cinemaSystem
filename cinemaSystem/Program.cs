using cinemaSystem.Data;
using cinemaSystem.Interfaces;
using cinemaSystem.Interfaces.cinemaSystem.Interfaces;
using cinemaSystem.Models;
using cinemaSystem.Repositories;
using cinemaSystem.Services;
using cinemaSystem.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Stripe;

namespace cinemaSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 6;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IActorService, ActorService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IRepository < Cart>, Repository<Cart>>();
            builder.Services.AddScoped<IRepository < Cart>, Repository<Cart>>();
            builder.Services.AddScoped<IRepository < FavoriteItem>, Repository<FavoriteItem>>();
            builder.Services.AddScoped<IRepository < ProductPromotion>, Repository<ProductPromotion>>();
            builder.Services.AddScoped<IRepository < PromotionUserUsage>, Repository<PromotionUserUsage>>();
            builder.Services.AddScoped<IRepository < Order>, Repository<Order>>();
            builder.Services.AddScoped<IRepository < OrderItem>, Repository<OrderItem>>();
            builder.Services.AddScoped<IRepository < Models.Review>, Repository<Models.Review>>();
            builder.Services.AddScoped<ApplicationUserOTP, ApplicationUserOTP>();
            builder.Services.AddScoped<IDbInitializer,DbInitializer>();
            builder.Services.AddScoped<IAccountService, Services.AccountService>();
            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDbInitializer>();
            service.Initialize();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Strip"));
            StripeConfiguration.ApiKey = builder.Configuration("Stripe:SecretKey");

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }
    }
}