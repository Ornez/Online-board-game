using API.Modules.Chat.Data;
using API.Modules.Lobbies.Data;
using API.Extensions;
using API.Interfaces;
using API.Modules.Game.Keys;
using API.Modules.Chat.Managers;
using API.Modules.Lobbies.Registries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;
[Authorize]
public class ChatHub : Hub
{
    private readonly ILobbiesRegistry _lobbiesRegistry;
    private readonly IChatManager _chatManager;

    public ChatHub(ILobbiesRegistry lobbiesRegistry, IChatManager chatManager)
    {
        _lobbiesRegistry = lobbiesRegistry;
        _chatManager = chatManager;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        if (_lobbiesRegistry.TryGetLobbyWithUser(Context.User!.UserData(), out LobbyData lobbyData, out _))
            await CreateOrJoinRoom(lobbyData.Id);
    }

    private async Task CreateOrJoinRoom(int roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
        _chatManager.JoinRoom(Context.User!.UserData(), roomId);

        MessageData messageData = new(new(-1, "SYSTEM", ""), $"{Context.User!.UserName()} joined.", roomId,
            DateTimeOffset.Now);
        _chatManager.AddMessage(roomId, messageData);

        OutputMessage outputMessage = _chatManager.ConvertToOutputMessage(messageData);
        await Clients.Group(roomId.ToString()).SendAsync(AllKeys.ChatMessages.SEND_MESSAGE, outputMessage);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (_chatManager.TryLeaveRoom(Context.User!.UserData(), out int roomId))
        {
            MessageData messageData = new(new(-1, "SYSTEM", ""), $"{Context.User!.UserName()} left.", roomId,
                DateTimeOffset.Now);
            _chatManager.AddMessage(roomId, messageData);

            OutputMessage outputMessage = _chatManager.ConvertToOutputMessage(messageData);
            await Clients.Group(roomId.ToString()).SendAsync(AllKeys.ChatMessages.SEND_MESSAGE, outputMessage);

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId.ToString());
        }

        await base.OnDisconnectedAsync(exception);
    }

    public List<OutputMessage> GetMessages()
    {
        if (_chatManager.TryGetRoomIdWithUser(Context.User!.UserData(), out int roomId))
        {
            return _chatManager.GetMessages(roomId);
        }

        return new();
    }

    public async Task SendMessage(InputMessage request)
    {
        if (_chatManager.TryGetRoomIdWithUser(Context.User!.UserData(), out int roomId))
        {
            MessageData messageData = new(Context.User!.UserData(), request.Message, roomId, DateTimeOffset.Now);
            _chatManager.AddMessage(roomId, messageData);

            OutputMessage outputMessage = _chatManager.ConvertToOutputMessage(messageData);
            await Clients.Group(roomId.ToString()).SendAsync(AllKeys.ChatMessages.SEND_MESSAGE, outputMessage);
        }
    }
}
