using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedMangoAPI;

public class ShoppingCart
{
    public int Id { get; set; }
    [Required]
    public string? UserId { get; set; }

    public ICollection<CartItem>? CartItems { get; set; }

    [NotMapped]
    public double CartTotal { get; set; }
    [NotMapped]
    public string? StripePaymentIntentId { get; set; }
    [NotMapped]
    public string? ClientSecret { get; set; }
}


