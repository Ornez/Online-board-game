using API.Modules.Chat.Data;
using API.Data.Game;
using API.Extensions;

namespace API.Modules.Chat.Managers;
public class ChatManager : IChatManager
{
    private Dictionary<int, ChatData> _rooms = new();

    public void JoinRoom(UserData userData, int roomId) 
    {
        if (!_rooms.ContainsKey(roomId)) 
        {
            CreateRoom(roomId);
        }

        if (!_rooms[roomId].Users.Contains(userData)) 
        {
            _rooms[roomId].Users.Add(userData);
        }
    }

    private void CreateRoom(int roomId) 
    {
        _rooms[roomId] = new(roomId);
    }

    public bool TryLeaveRoom(UserData userData, out int roomId) 
    {
        roomId = -1;
        if (TryGetRoomIdWithUser(userData, out roomId))
        {
            UserData userToRemove = _rooms[roomId].Users.Find(x => x.IsSameUserAs(userData)) ?? throw new InvalidOperationException();
            _rooms[roomId].Users.Remove(userToRemove);

            if (_rooms[roomId].Users.Count == 0) 
            {
                DeleteRoom(_rooms[roomId].Id);
            }
            return true;
        }
        return false;
    }

    private void DeleteRoom(int roomId) 
    {
        _rooms.Remove(roomId);
    }

    public void AddMessage(int roomId, MessageData message) 
    {
        if (_rooms.ContainsKey(roomId)) 
        {
            _rooms[roomId].Messages.Add(message);
        }
    }

    public OutputMessage ConvertToOutputMessage(MessageData messageData) 
    {
        OutputMessage outputMessage = new(messageData.UserData.Username,
            ConvertDateTimeOffsetToString(messageData.SentAt),
            messageData.Message);
            
        return outputMessage;
    }

    private string ConvertDateTimeOffsetToString(DateTimeOffset time) 
    {
        string hour = (time.Hour <= 9) ? "0" + time.Hour : time.Hour.ToString();
        string minute = (time.Minute <= 9) ? "0" + time.Minute : time.Minute.ToString();
        string second = (time.Second <= 9) ? "0" + time.Second : time.Second.ToString();
        return $"{hour}:{minute}:{second}";
    }

    public List<OutputMessage> GetMessages(int roomId) 
    {
        if (_rooms.ContainsKey(roomId)) 
        {
            List<OutputMessage> outputMessages = new();
            foreach (MessageData messageData in _rooms[roomId].Messages) 
            {
                outputMessages.Add(ConvertToOutputMessage(messageData));
            }
            return outputMessages;
        }
        throw new ArgumentException($"Rooms doesn't contains room with id: {roomId}");
    }

    public bool TryGetRoomIdWithUser(UserData playerData, out int roomId) 
    {
        foreach (int key in _rooms.Keys) 
        {
            foreach (UserData player in _rooms[key].Users) 
            {
                if (player.IsSameUserAs(playerData)) 
                {
                    roomId = _rooms[key].Id;
                    return true;
                }
            }
        }
        roomId = -1;
        return false;
    }
}
