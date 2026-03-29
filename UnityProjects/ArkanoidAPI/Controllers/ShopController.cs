using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ArkanoidAPI.Database.DTOs;
using ArkanoidAPI.Services;
using ArkanoidAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ArkanoidAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShopController : ControllerBase
{
    private readonly IShopService _shopService;

    public ShopController(IShopService shopService)
    {
        _shopService = shopService;
    }

    /// <summary>
    /// Получить все доступные скины в магазине (публично)
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<SkinDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SkinDto>>> GetAllSkins()
    {
        var skins = await _shopService.GetAllSkinsAsync();
        return Ok(skins);
    }

    /// <summary>
    /// Получить скин по ID (публично)
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SkinDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkinDto>> GetSkinById(int id)
    {
        var skin = await _shopService.GetSkinByIdAsync(id);
        if (skin == null)
        {
            return NotFound(new { message = "Скин не найден" });
        }

        return Ok(skin);
    }

    /// <summary>
    /// Получить инвентарь скинов текущего пользователя
    /// </summary>
    [HttpGet("inventory")]
    [Authorize]
    [ProducesResponseType(typeof(UserInventoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserInventoryDto>> GetInventory()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var inventory = await _shopService.GetInventoryAsync(userId);
        return Ok(inventory);
    }

    /// <summary>
    /// Получить все скины текущего пользователя
    /// </summary>
    [HttpGet("inventory/skins")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<UserSkinDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserSkinDto>>> GetUserSkins()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var skins = await _shopService.GetUserSkinsAsync(userId);
        return Ok(skins);
    }

    /// <summary>
    /// Купить скин
    /// </summary>
    [HttpPost("purchase")]
    [Authorize]
    [ProducesResponseType(typeof(PurchaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PurchaseResponse>> PurchaseSkin([FromBody] PurchaseSkinRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var result = await _shopService.PurchaseSkinAsync(userId, request.SkinId);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Надеть скин
    /// </summary>
    [HttpPost("equip")]
    [Authorize]
    [ProducesResponseType(typeof(EquipSkinResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EquipSkinResponse>> EquipSkin([FromBody] EquipSkinRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var result = await _shopService.EquipSkinAsync(userId, request.UserSkinId);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Снять скин (экипировать скин по умолчанию)
    /// </summary>
    [HttpPost("unequip")]
    [Authorize]
    [ProducesResponseType(typeof(EquipSkinResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<EquipSkinResponse>> UnequipSkin([FromBody] EquipSkinRequest request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var result = await _shopService.UnequipSkinAsync(userId, request.UserSkinId);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Получить экипированные скины текущего пользователя
    /// </summary>
    [HttpGet("equipped")]
    [Authorize]
    [ProducesResponseType(typeof(Dictionary<string, UserSkinDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Dictionary<string, UserSkinDto>>> GetEquippedSkins()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Пользователь не авторизован" });
        }

        var equipped = await _shopService.GetEquippedSkinsAsync(userId);
        return Ok(equipped);
    }
}
