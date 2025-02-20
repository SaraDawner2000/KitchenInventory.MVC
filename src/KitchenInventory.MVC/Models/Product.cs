using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace KitchenInventory.MVC.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public decimal Quantity { get; set; } = 1;
    public string? Unit { get; set; } = "count";
    [Required]
    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }

}