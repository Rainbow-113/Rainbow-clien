using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rainbow.Models;
using Rainbow.Services;

namespace Rainbow.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICheckoutService _checkoutService;
        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }
        public IActionResult Index()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Index", "Shop");

            // Tạo object Order ban đầu để View có dữ liệu hiển thị danh sách sản phẩm
            var cartItems = JsonConvert.DeserializeObject<List<CartItemDetailDto>>(cartJson);
            var order = new OrderDto
            {
                Items = cartItems.Select(x => new OrderItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    Size = x.Size,
                    Color = x.Color
                }).ToList(),
                TotalAmount = cartItems.Sum(x => x.Price * x.Quantity)
            };

            return View(order);
        }
        [HttpGet]
     

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(OrderDto order)
        {

            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Index", "Shop");

            var cartItems = JsonConvert.DeserializeObject<List<CartItemDetailDto>>(cartJson);

            order.Items = cartItems.Select(x => new OrderItemDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Price = x.Price,
                Quantity = x.Quantity,
                Size = x.Size,
                Color = x.Color
            }).ToList();

            order.TotalAmount = order.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDate = DateTime.Now;

            ModelState.Remove("Id");
            ModelState.Remove("Items");

            if (!ModelState.IsValid)
            {
                return View("Index", order);
            }

            var token = HttpContext.Session.GetString("JWToken");
            var isSuccess = await _checkoutService.PlaceOrderAsync(order, token);
            var response = await _checkoutService.PlaceOrderResponseAsync(order, token);
            if (response.IsSuccessStatusCode)
            {
                
                HttpContext.Session.Remove("Cart");
                TempData["OrderSuccess"] = JsonConvert.SerializeObject(order);

                return RedirectToAction("Success");
            }
            else
            {
               
                var errorDetail = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Lỗi từ Server: {errorDetail}");
                return View("Index", order);
            }
        }
        public IActionResult Success()
        {
            var orderJson = TempData["OrderSuccess"] as string;
            if (string.IsNullOrEmpty(orderJson))
            {
                return RedirectToAction("Index", "Home"); // Nếu không có dữ liệu thì về trang chủ
            }

            var order = JsonConvert.DeserializeObject<OrderDto>(orderJson);
            return View(order); // Truyền order vào View ở đây!
        }







    }
}
