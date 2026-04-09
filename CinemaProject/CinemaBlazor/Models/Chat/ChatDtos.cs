namespace CinemaBlazor.Models.Chat;

public class UserChatDto
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}

public class MessageDto
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ConversationDto
{
    public int Id { get; set; }
    public string ConversationTypeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
    public MessageDto? LastMessage { get; set; }
}

public class CreateConversationDto
{
    public List<int> ParticipantIds { get; set; } = new();
    public string ConversationTypeName { get; set; } = "Direct";
}

public class SendMessageDto
{
    public string Content { get; set; } = string.Empty;
}

public class MessageResponse
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ConversationCreatedDto
{
    public int Id { get; set; }
    public string ConversationTypeName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
}

public class ParticipantDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}
