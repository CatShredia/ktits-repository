namespace CinemaAPI.Hubs;


// Интерфейс для вызовов клиента из ChatHub

public interface IChatClient
{

    // Получение нового сообщения
    Task ReceiveMessage(MessageResponse message);


    // Пользователь начал печатать сообщение
    Task UserTyping(int conversationId, int userId, string userName);


    // Пользователь подключился к чату
    Task UserConnected(int conversationId, int userId, string userName);


    // Пользователь отключился от чата
    Task UserDisconnected(int conversationId, int userId, string userName);


    // Сообщение было удалено
    Task MessageDeleted(int messageId, int conversationId);
}


// DTO для передачи сообщения через SignalR

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
