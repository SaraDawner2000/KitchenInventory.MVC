using KitchenInventory.MVC.Models;
using KitchenInventory.MVC.Data;
using Microsoft.EntityFrameworkCore;


namespace KitchenInventory.MVC.Services;

public class InventoryService
{
    private readonly ApplicationDbContext _context;

    public InventoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task UpdateInventoryItemAsync(InventoryItem updatedItem)
    {
        var existingItem = await _context.Inventory.FindAsync(updatedItem.Id);
        if (existingItem == null || existingItem.UserId != updatedItem.UserId) return;

        existingItem.ProductId = updatedItem.ProductId;
        existingItem.Quantity = updatedItem.Quantity;
        existingItem.ExpirationDate = updatedItem.ExpirationDate;
        existingItem.AmountLeft = updatedItem.AmountLeft;

        await _context.SaveChangesAsync();
    }

    public async Task AddInventoryItemAsync(InventoryItem inventoryItem)
    {
        _context.Inventory.Add(inventoryItem);
        await _context.SaveChangesAsync();
    }

    public async Task<InventoryItem?> GetInventoryItemByIdAsync(int id, string userId)
    {
        return await _context.Inventory
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
    }

    public async Task<List<InventoryItem>> GetUserInventoryAsync(string userId)
    {
        return await _context.Inventory
            .Where(i => i.UserId == userId)
            .Include(i => i.Product)
            .ToListAsync();
    }

    public async Task DeleteInventoryItemAsync(int id, string userId)
    {
        var inventoryItem = await _context.Inventory.Include(i => i.Product).FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);
        if (inventoryItem == null) return;

        _context.Inventory.Remove(inventoryItem);
        await _context.SaveChangesAsync();
    }
}