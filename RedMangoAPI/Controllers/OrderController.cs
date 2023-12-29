using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMangoAPI.Utility;
using System.Net;

namespace RedMangoAPI;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    ApplicationDbContext _context;
    private ApiResponse _response;
    public OrderController(ApplicationDbContext context)
    {
        _context = context;
        _response = new ApiResponse();
    }


    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetOrders(string? userId)
    {
        try
        {
            var orderHeaders = _context.OrderHeaders.Include(e => e.OrderDetails).ThenInclude(e => e.MenuItem).OrderByDescending(e => e.OrderHeaderId);
            if (!string.IsNullOrEmpty(userId))
            {
                _response.Result = orderHeaders.Where(e => e.ApplicationUserId == userId);
            }
            else
            {
                _response.Result = orderHeaders;
            }
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetOrders(int? id)
    {
        try
        {
            if (id == 0)
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return Ok(_response);
            }
            else
            {
                var orderHeaders = _context.OrderHeaders.Include(e => e.OrderDetails).ThenInclude(e => e.MenuItem).Where(e => e.OrderHeaderId == id);
                if (orderHeaders == null)
                {
                    _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                _response.Result = orderHeaders;
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
        }

        return _response;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
    {
        try
        {
            OrderHeader orderHeader = new OrderHeader
            {
                ApplicationUserId = orderHeaderDTO.ApplicationUserId,
                PickupEmail = orderHeaderDTO.PickupEmail,
                PickupName = orderHeaderDTO.PickupName,
                PickupPhoneNumber = orderHeaderDTO.PickupPhoneNumber,
                OrderTotal = orderHeaderDTO.OrderTotal,
                OrderDate = DateTime.Now,
                StripePaymentIntentID = orderHeaderDTO.StripePaymentIntentID,
                TotalItems = orderHeaderDTO.TotalItems,
                Status = string.IsNullOrEmpty(orderHeaderDTO.Status) ? SD.status_pending : orderHeaderDTO.Status,
            };
            if (ModelState.IsValid)
            {
                await _context.OrderHeaders.AddAsync(orderHeader);
                await _context.SaveChangesAsync();
                foreach (var orderDetailsDTO in orderHeaderDTO.OrderDetailsDTO)
                {
                    OrderDetails orderDetails = new OrderDetails()
                    {
                        OrderHeaderId = orderHeader.OrderHeaderId,
                        ItemName = orderDetailsDTO.ItemName,
                        MenuItemId = orderDetailsDTO.MenuItemId,
                        Price = orderDetailsDTO.Price,
                        Quantity = orderDetailsDTO.Quantity
                    };
                    await _context.OrderDetails.AddAsync(orderDetails);
                }
                await _context.SaveChangesAsync();
                _response.Result = orderHeader;
                orderHeader.OrderDetails = null;
                _response.StatusCode = System.Net.HttpStatusCode.OK;

                return Ok(HttpStatusCode.OK);
            }
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string> { ex.Message };
        }
        return BadRequest(_response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse>> UpdateOrderHeader(int id, [FromBody] OrderHeaderUpdateDTO orderHeaderDTO)
    {
        try
        {
            if (orderHeaderDTO == null || id != orderHeaderDTO.OrderHeaderId)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            OrderHeader orderFromDb = _context.OrderHeaders.FirstOrDefault(e => e.OrderHeaderId == id);
            if (orderFromDb == null)
            {

                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            if (!string.IsNullOrEmpty(orderHeaderDTO.PickupName))
                orderFromDb.PickupName = orderHeaderDTO.PickupName;
            if (!string.IsNullOrEmpty(orderHeaderDTO.PickupEmail))
                orderFromDb.PickupName = orderHeaderDTO.PickupEmail;
            if (!string.IsNullOrEmpty(orderHeaderDTO.PickupPhoneNumber))
                orderFromDb.PickupName = orderHeaderDTO.PickupPhoneNumber;
            if (!string.IsNullOrEmpty(orderHeaderDTO.Status))
                orderFromDb.PickupName = orderHeaderDTO.Status;
            if (!string.IsNullOrEmpty(orderHeaderDTO.StripePaymentIntentID))
                orderFromDb.PickupName = orderHeaderDTO.StripePaymentIntentID;

            await _context.SaveChangesAsync();
            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;
            return Ok(_response);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString() };
            _response.StatusCode = HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
        return _response;
    }
}