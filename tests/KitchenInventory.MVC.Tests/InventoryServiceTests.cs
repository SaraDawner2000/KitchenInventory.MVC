using KitchenInventory.MVC.Data;
using KitchenInventory.MVC.Models;
using KitchenInventory.MVC.Services;
using Microsoft.EntityFrameworkCore;
public class InventoryServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }

    [Fact]
    public async Task AddInventoryItem_Should_Save_Item()
    {
        var dbContext = GetDbContext();
        var service = new InventoryService(dbContext);

        var inventoryItem = new InventoryItem
        {
            ProductId = 2,
            Quantity = 5,
            UserId = "user-123"
        };
        await service.AddInventoryItemAsync(inventoryItem);
        var savedItem = await dbContext.Inventory.FirstOrDefaultAsync(i => i.Id == inventoryItem.Id);

        Assert.NotNull(savedItem);
        Assert.Equal(5, savedItem.Quantity);
    }


    [Fact]
    public async Task GetUserInventory_Should_Return_Only_User_Items()
    {
        var dbContext = GetDbContext();
        var service = new InventoryService(dbContext);

        await dbContext.Inventory.AddRangeAsync(
            new InventoryItem { ProductId = 2, Quantity = 5, UserId = "user-123" },
            new InventoryItem { ProductId = 3, Quantity = 10, UserId = "user-456" }
        );
        await dbContext.SaveChangesAsync();

        var userInventory = await service.GetUserInventoryAsync("user-123");

        Assert.Single(userInventory);
        Assert.Equal(5, userInventory[0].Quantity);

    }

    [Fact]
    public async Task GetInventoryItemById_Should_Return_Correct_Item()
    {
        var dbContext = GetDbContext();
        var service = new InventoryService(dbContext);

        var inventoryItem = new InventoryItem
        {
            ProductId = 2,
            Quantity = 5,
            ExpirationDate = DateTime.UtcNow.AddDays(10),
            AmountLeft = AmountStatus.OneHundred,
            UserId = "user-123"
        };
        await dbContext.Inventory.AddAsync(inventoryItem);
        await dbContext.SaveChangesAsync();

        var result = await service.GetInventoryItemByIdAsync(inventoryItem.Id, "user-123");

        Assert.NotNull(result);
        Assert.Equal(5, result.Quantity);
        Assert.Equal(AmountStatus.OneHundred, result.AmountLeft);
        Assert.Equal(inventoryItem.ExpirationDate, result.ExpirationDate);
    }

    [Fact]
    public async Task GetInventoryItemById_Should_Return_Null_If_Not_Owned_By_User()
    {
        var dbContext = GetDbContext();
        var service = new InventoryService(dbContext);

        var inventoryItem = new InventoryItem
        {
            ProductId = 2,
            Quantity = 5,
            ExpirationDate = DateTime.UtcNow.AddDays(10),
            AmountLeft = AmountStatus.OneHundred,
            UserId = "user-123"
        };
        await dbContext.Inventory.AddAsync(inventoryItem);
        await dbContext.SaveChangesAsync();

        var result = await service.GetInventoryItemByIdAsync(inventoryItem.Id, "user-456");

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateInventoryItem_Should_Change_Fields()
    {
        var dbContext = GetDbContext();
        var service = new InventoryService(dbContext);

        var inventoryItem = new InventoryItem
        {
            ProductId = 2,
            Quantity = 5,
            ExpirationDate = DateTime.UtcNow.AddDays(10),
            AmountLeft = AmountStatus.OneHundred,
            UserId = "user-123"
        };
        await dbContext.Inventory.AddAsync(inventoryItem);
        await dbContext.SaveChangesAsync();

        var updatedItem = new InventoryItem
        {
            Id = inventoryItem.Id,
            ProductId = 2,
            Quantity = 10,
            ExpirationDate = DateTime.UtcNow.AddDays(20),
            AmountLeft = AmountStatus.Fifty,
            UserId = "user-123"
        };

        await service.UpdateInventoryItemAsync(updatedItem);
        var result = await dbContext.Inventory.FindAsync(inventoryItem.Id);

        Assert.NotNull(result);
        Assert.Equal(10, result.Quantity);
        Assert.Equal(AmountStatus.Fifty, result.AmountLeft);
        Assert.Equal(updatedItem.ExpirationDate, result.ExpirationDate);
    }

    [Fact]
    public async Task DeleteInventoryItem_Should_Remove_Item()
    {
        var dbContext = GetDbContext();
        var service = new InventoryService(dbContext);

        var inventoryItem = new InventoryItem { ProductId = 2, Quantity = 5, UserId = "user-123" };
        await dbContext.Inventory.AddAsync(inventoryItem);
        await dbContext.SaveChangesAsync();

        await service.DeleteInventoryItemAsync(inventoryItem.Id, "user-123");
        var deletedItem = await dbContext.Inventory.FindAsync(inventoryItem.Id);

        Assert.Null(deletedItem);
    }
}