using cinemaSystem.Data;
using cinemaSystem.Interfaces;
using cinemaSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace cinemaSystem.Areas.Customer.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Cart> _cartRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<PromotionUserUsage> _promotionsUserUsageRepository;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ILogger<CartController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<ProductPromotion> _promotionPromotionRepository;

        public CartController(UserManager<ApplicationUser> userManager,
            IRepository<Cart> cartRepository,
            IRepository<Movie> movieRepository,
            IRepository<PromotionUserUsage> promotionsUserUsageRepository,
            ApplicationDbContext applicationDbContext,
            ILogger<CartController> logger,
            IConfiguration configuration,

          IRepository<Order> orderRepository


            )
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _movieRepository = movieRepository;
            _orderRepository = orderRepository;
            _promotionsUserUsageRepository = promotionsUserUsageRepository;
            _applicationDbContext = applicationDbContext;
            _logger = logger;
            _configuration = configuration;

        }
        public async Task<IActionResult> Index(string? code = null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();

            var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationuserId == user.Id);


            if (code is not null)
            {
                var promotion = await _promotionPromotionRepository.GetOneAsync(e => e.Code == code && DateTime.Now <= e.ValidTo && e.Usage > 0 && e.Status);

                if (promotion is null)
                {
                    TempData["error_notification"] = "Promotion Code is Invalid!";
                    return View(userCart);
                }

                bool productMatch = false;
                foreach (var item in userCart)
                {
                    if (item.MovieId == promotion.ProductId)
                    {
                        var transaction = _applicationDbContext.Database.BeginTransaction();

                        try
                        {
                           
                            var userPromotionUsage = await _promotionsUserUsageRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.ProductPromotionId == promotion.Id);

                            if (userPromotionUsage is not null)
                            {
                                TempData["error_notification"] = "You have already used this promotion code!";
                                break;
                            }

                            item.MoviePrice = item.MoviePrice * (100 - promotion.Discount) / 100;
                            item.TotalPrice = item.MoviePrice;

                            await _promotionsUserUsageRepository.CreateAsync(new()
                            {
                                ApplicationUserId = user.Id,
                                ProductPromotionId = promotion.Id,
                                UsedAt = DateTime.Now,
                                Code = promotion.Code
                            });
                            await _promotionsUserUsageRepository.CommitAsync();

                            promotion.Usage -= 1;

                            TempData["success_notification"] = "Promotion code applied successfully!";
                            productMatch = true;
                            await _promotionPromotionRepository.CommitAsync();
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                            transaction.Rollback();
                        }

                    }
                }
                if (!productMatch)
                    TempData["error_notification"] = "Promotion Code Can not apply on this products!";
            }


            return View(userCart);
        }
        public async Task<IActionResult> AddToCart(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var movie = await _movieRepository.GetOneAsync(e => e.MovieId == movieId);
            if (movie is null) return NotFound();


            
                Cart cart = new()
                {
                    ApplicationuserId = user.Id,
                    MovieId = movieId,
                    MoviePrice = (double)Movie.Price

                };
                await _cartRepository.CreateAsync(cart);
            

            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Increment(int cartId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(e => e.Id == cartId && e.ApplicationuserId == user.Id);
            if (cartInDb is null) return NotFound();

            cartInDb.Quantity += 1;
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Decrement(int cartId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(e => e.Id == cartId && e.ApplicationuserId == user.Id);
            if (cartInDb is null) return NotFound();

            if (cartInDb.Quantity == 1)
                _cartRepository.Delete(cartInDb);
            else
                cartInDb.Quantity -= 1;

            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int cartId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var cartInDb = await _cartRepository.GetOneAsync(e => e.Id == cartId && e.ApplicationuserId == user.Id);
            if (cartInDb is null) return NotFound();

            _cartRepository.Delete(cartInDb);
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Pay()
        {

            var user = await _userManager.GetUserAsync(User);

            if (user is null) return NotFound();

            var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationuserId == user.Id, includes: [e => e.Movie]);

            var orderInDb = await _orderRepository.GetOneAsync(e => e.ApplicationUserId == user.Id && e.OrderStatus == OrderStatus.Pending);
            Order order = new();

            if (orderInDb == null)
            {
                order.ApplicationUserId = user.Id;
                order.TotalPrice = (decimal)userCart.Sum(e => e.ProductPrice * e.Quantity);

                await _orderRepository.CreateAsync(order);
                await _orderRepository.CommitAsync();
            }

          
            var service = new SessionService();
            var session = service.Create(options);
            order.SessionId = session.Id;
            await _orderRepository.CommitAsync();

            return Redirect(session.Url);
        }
    }
}
