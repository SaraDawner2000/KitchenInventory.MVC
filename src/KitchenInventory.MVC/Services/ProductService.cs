using KitchenInventory.MVC.Models;
using KitchenInventory.MVC.Data;
using Microsoft.EntityFrameworkCore;


namespace KitchenInventory.MVC.Services;

public class ProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProductsAsync(string? userId = null)
    {
        return await _context.Products
            .Where(p => userId == null || p.UserId == userId) // Returns all if userId is null, otherwise filters by userId
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id, string userId)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
    }

    public async Task AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product updatedProduct)
    {
        var existingProduct = await _context.Products.FindAsync(updatedProduct.Id);
        if (existingProduct == null || existingProduct.UserId != updatedProduct.UserId) return;

        existingProduct.Name = updatedProduct.Name;
        existingProduct.Unit = updatedProduct.Unit;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null || product.Id == 1) // Prevent deletion of "DELETED" product
        {
            return;
        }

        // Reassign all inventory items to the "DELETED" product (Id = 1)
        var affectedInventory = await _context.Inventory.Where(i => i.ProductId == id).ToListAsync();
        foreach (var inventory in affectedInventory)
        {
            inventory.ProductId = 1;
        }

        await _context.SaveChangesAsync();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}