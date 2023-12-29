using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace RedMangoAPI;

public class OrderHeaderUpdateDTO
{
    public int OrderHeaderId { get; set; }
    [Required]
    public string? PickupName { get; set; }
    [Required]
    public string? PickupPhoneNumber { get; set; }
    [Required]
    public string? PickupEmail { get; set; }

    public DateTime OrderDate { get; set; }
    public string? StripePaymentIntentID { get; set; }
    public string? Status { get; set; }
}
