using KitchenInventory.MVC.Data;
using KitchenInventory.MVC.Models;
using KitchenInventory.MVC.Services;
using Microsoft.EntityFrameworkCore;

public class ProductServiceTests
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
    public async Task AddProduct_Should_Save_Product()
    {
        var dbContext = GetDbContext();
        var service = new ProductService(dbContext);

        var product = new Product { Name = "Milk", UserId = "user-123" };

        await service.AddProductAsync(product);
        var savedProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.Name == "Milk");

        Assert.NotNull(savedProduct);
        Assert.Equal("Milk", savedProduct.Name);
    }

    [Fact]
    public async Task UpdateProduct_Should_Edit_All_Fields()
    {
        var dbContext = GetDbContext();
        var service = new ProductService(dbContext);

        var product = new Product
        {
            Id = 2,
            Name = "Old Name",
            Unit = "gallon",
            UserId = "user-123"
        };
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        var updatedProduct = new Product
        {
            Id = product.Id,
            Name = "New Name",
            Unit = "liters",
            UserId = "user-123"
        };

        await service.UpdateProductAsync(updatedProduct);
        var result = await dbContext.Products.FindAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("liters", result.Unit);
    }

    [Fact]
    public async Task GetProductById_Should_Return_Correct_Product()
    {
        var dbContext = GetDbContext();
        var service = new ProductService(dbContext);

        var product = new Product
        {
            Id = 2,
            Name = "Milk",
            Unit = "gallon",
            UserId = "user-123"
        };
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        var result = await service.GetProductByIdAsync(product.Id, "user-123");

        Assert.NotNull(result);
        Assert.Equal("Milk", result.Name);
        Assert.Equal("gallon", result.Unit);
    }

    [Fact]
    public async Task GetProductById_Should_Return_Null_If_Not_Owned_By_User()
    {
        var dbContext = GetDbContext();
        var service = new ProductService(dbContext);

        var product = new Product
        {
            Id = 2,
            Name = "Milk",
            Unit = "gallon",
            UserId = "user-123"
        };
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        var result = await service.GetProductByIdAsync(product.Id, "user-456");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductsAsync_Should_Return_All_Products_When_UserId_Is_Null()
    {
        var dbContext = GetDbContext();
        var service = new ProductService(dbContext);

        await dbContext.Products.AddRangeAsync(
            new Product { Name = "Milk", UserId = "user-123" },
            new Product { Name = "Eggs", UserId = "user-456" },
            new Product { Name = "Cream", UserId = "user-456" }
        );
        await dbContext.SaveChangesAsync();

        var allProducts = await service.GetProductsAsync();

        foreach (var product in allProducts)
        {
            Console.WriteLine(product.Name);
        }

        Assert.Equal(3, allProducts.Count());
    }

    [Fact]
    public async Task GetProductsAsync_Should_Return_Only_User_Products()
    {
        var dbContext = GetDbContext();
        var service = new ProductService(dbContext);

        await dbContext.Products.AddRangeAsync(
            new Product { Name = "Milk", UserId = "user-123" },
            new Product { Name = "Eggs", UserId = "user-456" }
        );
        await dbContext.SaveChangesAsync();

        var userProducts = await service.GetProductsAsync("user-123");

        Assert.Single(userProducts);
        Assert.Equal("Milk", userProducts[0].Name);
    }

    [Fact]
    public async Task DeleteProduct_Should_Remove_Product()
    {
        var dbContext = GetDbContext();
        var service = new ProductService(dbContext);


        var product = new Product { Name = "Butter", UserId = "user-123" };
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        await service.DeleteProductAsync(product.Id);
        var deletedProduct = await dbContext.Products.FindAsync(product.Id);

        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task DeleteProduct_Should_Reassign_InventoryItems_To_Deleted_Product()
    {
        var dbContext = GetDbContext();
        var service = new ProductService(dbContext);

        var product = new Product { Name = "Cheese", UserId = "user-123" };
        await dbContext.Products.AddAsync(product);

        var inventoryItem = new InventoryItem { ProductId = product.Id, Quantity = 2, UserId = "user-123" };
        await dbContext.Inventory.AddAsync(inventoryItem);
        await dbContext.SaveChangesAsync();

        await service.DeleteProductAsync(product.Id);
        var reassignedInventory = await dbContext.Inventory.FirstOrDefaultAsync(i => i.Id == inventoryItem.Id);

        Assert.NotNull(reassignedInventory);
        Assert.Equal(1, reassignedInventory.ProductId);
    }
}