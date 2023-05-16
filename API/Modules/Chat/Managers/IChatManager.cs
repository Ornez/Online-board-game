using API.Modules.Chat.Data;
using API.Data.Game;

namespace API.Modules.Chat.Managers;
public interface IChatManager
{
    void JoinRoom(UserData playerData, int roomId);
    bool TryLeaveRoom(UserData playerData, out int roomId);
    void AddMessage(int roomId, MessageData message);
    OutputMessage ConvertToOutputMessage(MessageData messageData);
    List<OutputMessage> GetMessages(int roomId);
    bool TryGetRoomIdWithUser(UserData playerData, out int roomId);
}
