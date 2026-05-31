using cinemaSystem.Interfaces;
using cinemaSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace cinemaSystem.Areas.Customer.Controllers
{
    
        [Area("Customer")]
        [Authorize]
        public class CheckoutController : Controller
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly IRepository<Order> _orderRepository;
            private readonly IRepository<OrderItem> _orderItemRepository;
            private readonly IRepository<Cart> _cartRepository;
            private readonly IRepository<Movie> _MovieRepository;

            public CheckoutController(
                UserManager<ApplicationUser> userManager,
                IRepository<Order> orderRepository,
                IRepository<OrderItem> orderItemRepository,
                IRepository<Cart> cartRepository,
                IRepository<Movie> MovieRepository)
            {
                _userManager = userManager;
                _orderRepository = orderRepository;
                _orderItemRepository = orderItemRepository;
                _cartRepository = cartRepository;
                _MovieRepository = MovieRepository;
            }

            public async Task<IActionResult> Success(int orderId)
            {
                

                var order = await _orderRepository.GetOneAsync(e => e.Id == orderId);
                if (order is null) return NotFound();

                var service = new SessionService();
                var session = service.Get(order.SessionId);

                order.OrderStatus = OrderStatus.InProcessing;
                order.PaymentStatus = PaymentStatus.Completed;
                order.TransactionId = session.PaymentIntentId;
                await _orderRepository.CommitAsync();

                var user = await _userManager.GetUserAsync(User);
                if (user is null) return NotFound();

                var userCart = await _cartRepository
                    .GetAsync(e => e.ApplicationuserId == user.Id, includes: [e => e.Movie]);

                foreach (var item in userCart)
                {
                    await _orderItemRepository.CreateAsync(new()
                    {
                        OrderId = orderId,
                        MovieId = item.MovieId,
                        MoviePrice = (decimal)item.MoviePrice,
                    });
                }
                await _orderItemRepository.CommitAsync();

                
                await _MovieRepository.CommitAsync();

                foreach (var item in userCart)
                    _cartRepository.Delete(item);
                await _cartRepository.CommitAsync();

                return View();
            }

            public IActionResult Cancel()
            {
                return View();
            }
        }
    }