using System;
using Xunit;
using KitchenInventory.MVC.Models;
using System.ComponentModel.DataAnnotations;

public class InventoryItemTests
{
    [Fact]
    public void Inventory_Item_Should_ProductId()
    {
        var inventoryItem = new InventoryItem
        {
            Quantity = 5,
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            AmountLeft = AmountStatus.SeventyFive,
            UserId = "user-123"
        };
        var validationContext = new ValidationContext(inventoryItem);


        bool isValid = Validator.TryValidateObject(inventoryItem, validationContext, null);

        Assert.False(isValid);

    }
    [Fact]
    public void Inventory_Item_Should_Have_UserId()
    {
        var inventoryItem = new InventoryItem
        {
            Quantity = 5,
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            AmountLeft = AmountStatus.SeventyFive,
            ProductId = 1
        };
        var validationContext = new ValidationContext(inventoryItem);

        bool isValid = Validator.TryValidateObject(inventoryItem, validationContext, null);

        Assert.False(isValid);

    }
}