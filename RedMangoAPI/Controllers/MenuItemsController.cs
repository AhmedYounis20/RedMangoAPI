using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RedMangoAPI.Models;
using RedMangoAPI.Utility;

namespace RedMangoAPI;

[Route("api/[controller]")]
[ApiController]
public class MenuItemsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private ApiResponse _response;
    private IBlobService _blobService;
    public MenuItemsController(ApplicationDbContext db, IBlobService blobService)
    {
        _db = db;
        _response = new ApiResponse();
        _blobService = blobService;
    }

    [Authorize(Roles = SD.Role_Admin)]
    [HttpGet]
    public async Task<IActionResult> GetMenuItems()
    {
        _response.Result = await _db.MenuItems.ToListAsync();
        _response.IsSuccess = true;
        _response.StatusCode = System.Net.HttpStatusCode.OK;
        return Ok(_response);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMenuItem(int id)
    {
        if (id == 0)
        {
            _response.IsSuccess = false;
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            return BadRequest(_response);
        }
        else
        {
            MenuItem menuItem = _db.MenuItems.FirstOrDefault(u => u.Id == id);
            if (menuItem is null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return NotFound(_response);
            }
            else
            {
                _response.Result = _db.MenuItems;
                _response.IsSuccess = true;
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateMenuItem(MenuItemCreateDTO menuItemCreateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }


                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemCreateDTO.File.FileName)}";
                MenuItem menuItem = new()
                {
                    Name = menuItemCreateDTO.Name,
                    Price = menuItemCreateDTO.Price,
                    Category = menuItemCreateDTO.Category,
                    SpecialTag = menuItemCreateDTO.SpecialTag,
                    Description = menuItemCreateDTO.Description,
                    Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, menuItemCreateDTO.File)
                };
                _db.MenuItems.Add(menuItem);
                _db.SaveChanges();
                _response.Result = menuItem;
                _response.StatusCode = System.Net.HttpStatusCode.Created;
                return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, _response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string>() { ex.ToString()};
            return BadRequest(_response);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id, [FromBody] MenuItemUpdateDTO menuItemUpdateDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (menuItemUpdateDTO == null || id != menuItemUpdateDTO.Id)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                MenuItem? menuItemFromDb = await _db.MenuItems.FindAsync(id);

                if (menuItemFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                menuItemFromDb.Name = menuItemUpdateDTO.Name;
                menuItemFromDb.Price = menuItemUpdateDTO.Price;
                menuItemFromDb.Category = menuItemUpdateDTO.Category;
                menuItemFromDb.SpecialTag = menuItemUpdateDTO.SpecialTag;
                menuItemFromDb.Description = menuItemUpdateDTO.Description;


                if (menuItemUpdateDTO.File != null && menuItemUpdateDTO.File.Length > 0)
                {
                    await _blobService.DeleteBlob(menuItemFromDb.Image, SD.SD_Storage_Container);
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemUpdateDTO.File.FileName)}";
                    menuItemFromDb.Image = await _blobService.UploadBlob(fileName, SD.SD_Storage_Container, menuItemUpdateDTO.File);
                }



                _db.MenuItems.Update(menuItemFromDb);
                _db.SaveChanges();
                _response.StatusCode = System.Net.HttpStatusCode.NoContent;

                return Ok(_response);
            }
            else
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { ex.ToString()
};
            return Ok(_response);
        }
    }
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
    {
        try
        {

            if (id == 0)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            MenuItem? menuItemFromDb = await _db.MenuItems.FindAsync(id);

            if (menuItemFromDb == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if (!string.IsNullOrEmpty(menuItemFromDb.Image))
                await _blobService.DeleteBlob(menuItemFromDb.Image, SD.SD_Storage_Container);

            Thread.Sleep(2000);
            _db.MenuItems.Remove(menuItemFromDb);
            await _db.SaveChangesAsync();
            _response.StatusCode = System.Net.HttpStatusCode.NoContent;

            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.ErrorMessages = new List<string>() { ex.ToString()};
            return BadRequest(_response);
        }
    }
}
