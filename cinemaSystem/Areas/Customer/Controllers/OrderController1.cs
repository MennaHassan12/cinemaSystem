using cinemaSystem.Interfaces;
using cinemaSystem.Models;
using cinemaSystem.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace cinemaSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;

        public OrderController(
            UserManager<ApplicationUser> userManager,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<IActionResult> Index(int page = 1, string? query = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return NotFound();

            var orders = await _orderRepository.GetAsync(e => e.ApplicationUserId == user.Id);

            if (query is not null)
                orders = orders.Where(e => e.CarrierName != null && e.CarrierName.ToLower().Contains(query.ToLower().Trim()));

            int totalPages = (int)Math.Ceiling(orders.Count() / 5.0);
            orders = orders.Skip((page - 1) * 5).Take(5);

            return View(new OrderWithRelatedVM()
            {
                Orders = orders.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page,
                Query = query
            });
        }

        

        public async Task<IActionResult> Refund(int id)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == id);
            if (order is null) return NotFound();

            var options = new RefundCreateOptions()
            {
                PaymentIntent = order.TransactionId,
                Amount = ((long)order.TotalPrice * 100) - (5 * 100),
                Reason = RefundReasons.Unknown,
            };

            var service = new RefundService();
            var session = service.Create(options);

            return View();
        }
    }
}