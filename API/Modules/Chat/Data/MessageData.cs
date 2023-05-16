using API.Data.Game;

namespace API.Modules.Chat.Data;
public class MessageData
{
    public UserData UserData { get; set; }
    public string Message { get; set; }
    public int RoomId { get; set; }
    public DateTimeOffset SentAt { get; set; }

    public MessageData(UserData userData, string message, int roomId, DateTimeOffset sentAt) 
    {
        UserData = userData;
        Message = message;
        RoomId = roomId;
        SentAt = sentAt;
    }
}
