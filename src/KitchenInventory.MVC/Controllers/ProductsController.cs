using Microsoft.AspNetCore.Mvc;
using KitchenInventory.MVC.Models;
using KitchenInventory.MVC.Services;
using Microsoft.AspNetCore.Identity;

namespace KitchenInventory.MVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductsController(ProductService productService, UserManager<IdentityUser> userManager)
        {
            _productService = productService;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var products = await _productService.GetProductsAsync(userId);
            return View(products);
        }

        // GET: Products/Create
        public IActionResult Create() => View();

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            product.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                await _productService.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var product = await _productService.GetProductByIdAsync(id, userId);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            var userId = _userManager.GetUserId(User);
            if (product.UserId != userId) return Unauthorized();

            if (ModelState.IsValid)
            {
                await _productService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var product = await _productService.GetProductByIdAsync(id, userId);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}