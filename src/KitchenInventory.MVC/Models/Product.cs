using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace KitchenInventory.MVC.Models;

public class Product
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public string? Unit { get; set; } = "count";

    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }

}