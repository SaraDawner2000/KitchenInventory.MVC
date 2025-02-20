using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace KitchenInventory.MVC.Models;

public enum AmountStatus
{
    [Display(Name = "0%")]
    Zero = 0,

    [Display(Name = "25%")]
    TwentyFive = 25,

    [Display(Name = "50%")]
    Fifty = 50,

    [Display(Name = "75%")]
    SeventyFive = 75,

    [Display(Name = "100%")]
    OneHundred = 100
}
public class InventoryItem
{
    public int Id { get; set; }

    [Required]
    public decimal Quantity { get; set; } = 1;

    [Display(Name = "Expires")]
    [DataType(DataType.Date)]
    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(7);

    [Display(Name = "Amount left")]
    public AmountStatus AmountLeft { get; set; } = AmountStatus.OneHundred;

    [Required]
    [ForeignKey("Product")]
    public int? ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    [ForeignKey("User")]
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }
}