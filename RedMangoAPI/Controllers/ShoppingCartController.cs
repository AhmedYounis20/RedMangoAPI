using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RedMangoAPI;

[Route("api/[controller]")]
[ApiController]
public class ShoppingCartController : ControllerBase
{
    protected ApiResponse _response;
    private readonly ApplicationDbContext _context;
    public ShoppingCartController(ApplicationDbContext context)
    {
        _response = new ApiResponse();
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> AddOrUpdateItemInCart(string userId, int menuItemId, int updateQuantityBy)
    {
        ShoppingCart? shoppingCart = _context.ShoppingCarts.Include(e => e.CartItems).FirstOrDefault(e => e.UserId == userId);
        MenuItem? menuItem = _context.MenuItems.FirstOrDefault(e => e.Id == menuItemId);

        if (menuItem == null)
        {
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            return BadRequest(_response);
        }
        if (shoppingCart == null && updateQuantityBy > 0)
        {

            ShoppingCart newCart = new()
            {
                UserId = userId,
            };
            _context.ShoppingCarts.Add(newCart);
            await _context.SaveChangesAsync();


            CartItem newCartItem = new()
            {
                MenuItemId = menuItemId,
                Quantity = updateQuantityBy,
                ShoppingCartId = newCart.Id,
                MenuItem = null
            };
            await _context.CartItems.AddAsync(newCartItem);
            await _context.SaveChangesAsync();
        }
        else
        {
            CartItem cartItemInCart = shoppingCart.CartItems.FirstOrDefault(e => e.MenuItemId == menuItemId);
            if (cartItemInCart == null)
            {
                CartItem cartItem = new()
                {
                    ShoppingCartId = shoppingCart.Id,
                    MenuItemId = menuItemId,
                    Quantity = updateQuantityBy,
                };
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                int newQuantity = cartItemInCart.Quantity + updateQuantityBy;
                if (updateQuantityBy == 0 || newQuantity <= 0)
                {
                    _context.CartItems.Remove(cartItemInCart);
                    if (shoppingCart.CartItems.Count() == 1)
                    {
                        _context.ShoppingCarts.Remove(shoppingCart);
                    }
                    _context.SaveChanges();
                }
                else
                {
                    cartItemInCart.Quantity = newQuantity;
                    _context.SaveChanges();
                }
            }
        }
        return Ok(_response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetShoppingCart(string userId)
    {
        try
        {
            ShoppingCart? shoppingCart = null;
            if (string.IsNullOrEmpty(userId))
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                shoppingCart = new();
                _response.Result = shoppingCart;

                return Ok(_response);
            }

            shoppingCart = await _context.ShoppingCarts.Include(e=>e.CartItems).ThenInclude(e=>e.MenuItem).FirstOrDefaultAsync(e=>e.UserId == userId);

            if(shoppingCart?.CartItems != null && shoppingCart.CartItems.Any())
            {
                shoppingCart.CartTotal = shoppingCart.CartItems.Sum(e=>e.Quantity * e.MenuItem.Price);
            }

            _response.Result = shoppingCart;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            _response.IsSuccess = true;

            return Ok(_response);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;

            return BadRequest(_response);
        }
    }

}
