using Microsoft.EntityFrameworkCore;
using cinemaSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using cinemaSystem.ViewModel;

namespace cinemaSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext <ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<MovieImage> MovieImages { get; set; }
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
        //public DbSet<ApplicationUser> ApplicationUsers{get; set;}
        public DbSet<Cart> Carts { get; set; }
        public DbSet<FavoriteItem> FavoriteItems { get; set; }
        public DbSet<ProductPromotion> ProductPromotions { get; set; }
        public DbSet<PromotionUserUsage> PromotionUserUsages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(ma => ma.MovieId);

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActors)
                .HasForeignKey(ma => ma.ActorId);

            //modelBuilder.Entity<IdentityUser>()
            //    .ToTable("",);
        }
        public DbSet<cinemaSystem.ViewModel.NewPasswordVM> NewPasswordVM { get; set; } = default!;
        
    }
}