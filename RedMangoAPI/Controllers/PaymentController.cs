using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace RedMangoAPI;

[Route("api/[controller]")]
[ApiController]
public class PaymentController : ControllerBase
{
    ApplicationDbContext _context;
    private ApiResponse _response;
    private IConfiguration _configuration;
    public PaymentController(ApplicationDbContext context, IConfiguration configuration)
    {

        _configuration = configuration;
        _context = context;
        _response = new ApiResponse();
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> MakePayment(string userId)
    {
        ShoppingCart? cart = await _context.ShoppingCarts.Include(e => e.CartItems).ThenInclude(e => e.MenuItem).FirstOrDefaultAsync(e => e.UserId == userId);
        if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
        {
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }



        StripeConfiguration.ApiKey = _configuration.GetValue<string>("StripeSettings");
        cart.CartTotal = cart.CartItems.Sum(e => e.Quantity * e.MenuItem.Price);
        var options = new PaymentIntentCreateOptions
        {
            Amount = (int) (cart.CartTotal*100),
            Currency = "EGP",
            PaymentMethodTypes = ["card"]
        };
        var service = new PaymentIntentService();
        PaymentIntent intent = service.Create(options);
        cart.StripePaymentIntentId = intent.Id;
        cart.ClientSecret = intent.ClientSecret;

        _response.Result = cart;
        _response.StatusCode = System.Net.HttpStatusCode.OK;
        return Ok(_response);
    }
}
