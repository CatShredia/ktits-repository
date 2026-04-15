using CinemaAPI.Data.Models.DTOs;
using CinemaAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    // GET /api/Chat/me
    [HttpGet("me")]
    public async Task<ActionResult<UserChatDto>> GetCurrentUser()
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _chatService.GetCurrentUserAsync(userId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    // GET /api/Chat/search?login=...
    [HttpGet("search")]
    public async Task<ActionResult<UserChatDto>> SearchUser([FromQuery] string login)
    {
        if (string.IsNullOrEmpty(login))
            return BadRequest("Login parameter is required");

        var result = await _chatService.SearchUserByLoginAsync(login);
        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // GET /api/Chat/conversations
    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationDto>>> GetUserConversations()
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var result = await _chatService.GetUserConversationsAsync(userId.Value);
        return Ok(result);
    }

    // POST /api/Chat/conversations/create
    [HttpPost("conversations/create")]
    public async Task<ActionResult<ConversationDto>> CreateConversation([FromBody] CreateConversationDto dto)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _chatService.CreateConversationAsync(dto, userId.Value);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET /api/Chat/conversations/{id}/messages
    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetConversationMessages(int conversationId)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _chatService.GetConversationMessagesAsync(conversationId, userId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // POST /api/Chat/conversations/{id}/messages
    [HttpPost("conversations/{conversationId}/messages")]
    public async Task<ActionResult<MessageDto>> SendMessage(int conversationId, [FromBody] SendMessageDto dto)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _chatService.SendMessageAsync(conversationId, userId.Value, dto.Content);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    // POST /api/Chat/conversations/{id}/messages/with-image
    [HttpPost("conversations/{conversationId}/messages/with-image")]
    public async Task<ActionResult<MessageDto>> SendMessageWithImage(
        int conversationId,
        [FromForm] SendMessageWithImageDto dto,
        IFormFile? imageFile)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _chatService.SendMessageAsync(conversationId, userId.Value, dto.Content ?? string.Empty, imageFile);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE /api/Chat/conversations/{conversationId}/messages/{messageId}
    [HttpDelete("conversations/{conversationId}/messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(int conversationId, int messageId)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var isAdmin = User.IsInRole("admin");

        try
        {
            await _chatService.DeleteMessageAsync(messageId, conversationId, userId.Value, isAdmin);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // PUT /api/Chat/conversations/{conversationId}/messages/{messageId}
    [HttpPut("conversations/{conversationId}/messages/{messageId}")]
    public async Task<ActionResult<MessageDto>> EditMessage(int conversationId, int messageId, [FromBody] EditMessageDto dto)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var isAdmin = User.IsInRole("admin");

        try
        {
            var result = await _chatService.EditMessageAsync(messageId, conversationId, userId.Value, dto.Content, isAdmin);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE /api/Chat/conversations/{id}
    [HttpDelete("conversations/{conversationId}")]
    public async Task<IActionResult> DeleteConversation(int conversationId)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            await _chatService.DeleteConversationAsync(conversationId, userId.Value);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // POST /api/Chat/conversations/{id}/participants
    [HttpPost("conversations/{conversationId}/participants")]
    public async Task<ActionResult<ConversationDto>> AddParticipants(int conversationId, [FromBody] List<int> userIds)
    {
        var currentUserId = _chatService.GetCurrentUserIdFromClaims(User);
        if (currentUserId == null)
            return Unauthorized();

        try
        {
            var result = await _chatService.AddParticipantsAsync(conversationId, userIds, currentUserId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // DELETE /api/Chat/conversations/{conversationId}/participants/{userId}
    [HttpDelete("conversations/{conversationId}/participants/{userId}")]
    public async Task<IActionResult> RemoveParticipant(int conversationId, int userId)
    {
        var currentUserId = _chatService.GetCurrentUserIdFromClaims(User);
        if (currentUserId == null)
            return Unauthorized();

        try
        {
            await _chatService.RemoveParticipantAsync(conversationId, userId, currentUserId.Value);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // POST /api/Chat/conversations/{id}/transfer-ownership
    [HttpPost("conversations/{conversationId}/transfer-ownership")]
    public async Task<IActionResult> TransferOwnership(int conversationId, [FromBody] int newOwnerId)
    {
        var currentUserId = _chatService.GetCurrentUserIdFromClaims(User);
        if (currentUserId == null)
            return Unauthorized();

        try
        {
            await _chatService.TransferOwnershipAsync(conversationId, newOwnerId, currentUserId.Value);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // PUT /api/Chat/conversations/{id}/participants/{userId}/role
    [HttpPut("conversations/{conversationId}/participants/{userId}/role")]
    public async Task<IActionResult> ChangeRole(int conversationId, int userId, [FromBody] string newRoleName)
    {
        var currentUserId = _chatService.GetCurrentUserIdFromClaims(User);
        if (currentUserId == null)
            return Unauthorized();

        try
        {
            await _chatService.ChangeParticipantRoleAsync(conversationId, userId, newRoleName, currentUserId.Value);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET /api/Chat/messages/{messageId}/comments
    [HttpGet("messages/{messageId}/comments")]
    public async Task<ActionResult<ConversationDto>> GetOrCreateCommentsConversation(int messageId)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        try
        {
            var result = await _chatService.GetOrCreateCommentsConversationAsync(messageId, userId.Value);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // GET /api/Chat/messages/{messageId}/comments/count
    [HttpGet("messages/{messageId}/comments/count")]
    public async Task<ActionResult<int>> GetCommentsCount(int messageId)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var count = await _chatService.GetCommentsCountAsync(messageId);
        return Ok(count);
    }

    // GET /api/Chat/messages/{messageId}/comments/preview
    [HttpGet("messages/{messageId}/comments/preview")]
    public async Task<ActionResult<List<CommentPreviewDto>>> GetMessageCommentsPreview(int messageId)
    {
        var userId = _chatService.GetCurrentUserIdFromClaims(User);
        if (userId == null)
            return Unauthorized();

        var result = await _chatService.GetMessageCommentsPreviewAsync(messageId);
        return Ok(result);
    }
}
