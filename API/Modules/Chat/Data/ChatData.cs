using API.Data.Game;

namespace API.Modules.Chat.Data;
public class ChatData
{
    public int Id { get; set; }
    public List<UserData> Users { get; set; } = new();
    public List<MessageData> Messages { get; set; } = new();

    public ChatData(int id)
    {
        Id = id;
    }
}
