using System;
using Xunit;
using KitchenInventory.MVC.Models;
using System.ComponentModel.DataAnnotations;

public class ProductTests
{
    [Fact]
    public void Product_Should_Have_Name()
    {
        var product = new Product
        {
            Unit = "count",
            UserId = "user-123"

        };
        var validationContext = new ValidationContext(product);

        bool isValid = Validator.TryValidateObject(product, validationContext, null);

        Assert.False(isValid);

    }
    [Fact]
    public void Product_Should_Have_UserId()
    {
        var product = new Product
        {
            Name = "Apples",
            Unit = "count"
        };
        var validationContext = new ValidationContext(product);

        bool isValid = Validator.TryValidateObject(product, validationContext, null);

        Assert.False(isValid);

    }
}