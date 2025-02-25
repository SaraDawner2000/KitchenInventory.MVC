using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using KitchenInventory.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using KitchenInventory.MVC.Services;
using Microsoft.AspNetCore.Identity;

namespace KitchenInventory.MVC.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly InventoryService _inventoryService;
        private readonly ProductService _productService;
        private readonly UserManager<IdentityUser> _userManager;

        public InventoryController(InventoryService inventoryService, ProductService productService, UserManager<IdentityUser> userManager)
        {
            _inventoryService = inventoryService;
            _productService = productService;
            _userManager = userManager;
        }

        // GET: Inventory
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var inventoryItems = await _inventoryService.GetUserInventoryAsync(userId);
            return View(inventoryItems);
        }


        // GET: Inventory/Create
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User);
            var products = await _productService.GetProductsAsync(userId);

            ViewBag.Products = new SelectList(
                products.Select(p => new { Id = p.Id, DisplayText = $"{p.Name}, {p.Unit}" }),
                "Id",
                "DisplayText"
            );
            ViewBag.AmountLeftOptions = new SelectList(Enum.GetValues(typeof(AmountStatus)));
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Quantity,ExpirationDate,AmountLeft")] InventoryItem inventoryItem)
        {
            var userId = _userManager.GetUserId(User);
            inventoryItem.UserId = userId;

            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                await _inventoryService.AddInventoryItemAsync(inventoryItem);
                return RedirectToAction(nameof(Index));
            }

            var products = await _productService.GetProductsAsync(userId);
            ViewBag.Products = new SelectList(
                products.Select(p => new { Id = p.Id, DisplayText = $"{p.Name}, {p.Unit}" }),
                "Id",
                "DisplayText",
                inventoryItem.ProductId
            );
            ViewBag.AmountLeftOptions = new SelectList(Enum.GetValues(typeof(AmountStatus)));
            return View(inventoryItem);
        }

        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var inventoryItem = await _inventoryService.GetInventoryItemByIdAsync(id, userId);
            if (inventoryItem == null) return NotFound();

            var products = await _productService.GetProductsAsync(userId);
            ViewBag.Products = new SelectList(
                products.Select(p => new { Id = p.Id, DisplayText = $"{p.Name}, {p.Unit}" }),
                "Id",
                "DisplayText",
                inventoryItem.ProductId
            );
            ViewBag.AmountLeftOptions = new SelectList(Enum.GetValues(typeof(AmountStatus)));

            return View(inventoryItem);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,Quantity,ExpirationDate,AmountLeft")] InventoryItem inventoryItem)
        {
            var userId = _userManager.GetUserId(User);
            inventoryItem.UserId = userId;

            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                await _inventoryService.UpdateInventoryItemAsync(inventoryItem);
                return RedirectToAction(nameof(Index));
            }

            var products = await _productService.GetProductsAsync(userId);
            ViewBag.Products = new SelectList(
                products.Select(p => new { Id = p.Id, DisplayText = $"{p.Name}, {p.Unit}" }),
                "Id",
                "DisplayText",
                inventoryItem.ProductId
            );
            ViewBag.AmountLeftOptions = new SelectList(Enum.GetValues(typeof(AmountStatus)));

            return View(inventoryItem);
        }

        // GET: Inventory/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var inventoryItem = await _inventoryService.GetInventoryItemByIdAsync(id, userId);

            return View(inventoryItem);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _inventoryService.DeleteInventoryItemAsync(id, userId);
            return RedirectToAction(nameof(Index));
        }
    }
}
