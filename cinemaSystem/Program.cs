using cinemaSystem.Data;
using cinemaSystem.Interfaces;
using cinemaSystem.Interfaces.cinemaSystem.Interfaces;
using cinemaSystem.Repositories;
using cinemaSystem.Services;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IActorService, ActorService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IMovieService, MovieService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            var app = builder.Build();

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