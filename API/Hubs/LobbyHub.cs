using API.Data;
using API.Modules.Chat.Data;
using API.Data.Game;
using API.Modules.Lobbies.Data;
using API.Modules.Lobby.Data;
using API.Extensions;
using API.Modules.Game.Keys;
using API.Modules.Chat.Managers;
using API.Modules.Game.Managers;
using API.Modules.Lobbies.Registries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;
[Authorize]
public class LobbyHub : Hub
{
    private readonly ILobbiesRegistry lobbiesRegistry;
    private readonly IHubContext<LobbiesHub> lobbiesHubContext;
    private readonly IHubContext<ChatHub> chatHubContext;
    private readonly IGameManager gameManager;
    private readonly IChatManager chatManager;

    public LobbyHub(ILobbiesRegistry lobbiesRegistry,
        IHubContext<LobbiesHub> lobbiesHubContext,
        IHubContext<ChatHub> chatHubContext,
        IGameManager gameManager,
        IChatManager chatManager) 
    {
        this.lobbiesRegistry = lobbiesRegistry;
        this.lobbiesHubContext = lobbiesHubContext;
        this.gameManager = gameManager;
        this.chatHubContext = chatHubContext;
        this.chatManager = chatManager;
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        
        lobbiesRegistry.AddUserToConnection($"{Context.User.Id()} {Context.User.Role()}", Context.ConnectionId);
        Console.WriteLine("[LobbyHub OnConnectedAsync]");
        if (lobbiesRegistry.TryGetLobbyWithUser(Context.User.UserData(), out LobbyData lobby, out ReturnMessage errorMessage)) 
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobby.LobbyName);
            LobbyPlayerData lobbyPlayerData = lobbiesRegistry.GetLobbyPlayerData(Context.User.UserData(), lobby);
            await Clients.GroupExcept(lobby.LobbyName, Context.ConnectionId).SendAsync(AllKeys.LobbyMessages.PLAYER_JOINED, lobbyPlayerData);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (lobbiesRegistry.TryGetLobbyWithUser(Context.User.UserData(), out LobbyData lobby, out ReturnMessage errorMessage)) 
        {
            await LeaveLobby(new LeaveLobbyRequest() 
            {
                LobbyId = lobby.Id
            });
        }
        lobbiesRegistry.RemoveUserToConnection($"{Context.User.Id()} {Context.User.Role()}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<ReturnMessage> LeaveLobby(LeaveLobbyRequest request) 
    {
        lobbiesRegistry.TryGetLobbyById(request.LobbyId, out LobbyData lobbyData, out _);
        if (lobbyData.Players.Count == 1) 
        {
            if (lobbiesRegistry.TryLeaveLobby(Context.User.UserData(), request, out ReturnMessage message)) 
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyData.LobbyName);
                LobbyDataToDisplay dataToDisplay = lobbiesRegistry.ConvertToLobbyDataToDisplay(lobbyData);
                await lobbiesHubContext.Clients.All.SendAsync(AllKeys.LobbyMessages.DELETE, dataToDisplay);
            }
            return message;
        }
        else 
        {
            if (lobbiesRegistry.TryLeaveLobby(Context.User.UserData(), request, out ReturnMessage message)) 
            {
                lobbiesRegistry.TryGetLobbyById(request.LobbyId, out lobbyData, out _);
                LobbyPlayerData lobbyPlayerData = lobbiesRegistry.GetLobbyPlayerData(Context.User.UserData(), lobbyData);
                await Clients.Group(lobbyData.LobbyName).SendAsync(AllKeys.LobbyMessages.PLAYER_LEFT, lobbyPlayerData);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyData.LobbyName);
                LobbyDataToDisplay dataToDisplay = lobbiesRegistry.ConvertToLobbyDataToDisplay(lobbyData);
                await lobbiesHubContext.Clients.All.SendAsync(AllKeys.LobbyMessages.UPDATE, dataToDisplay);
            }
            return message;
        }
    }

    public async Task<ReturnMessage> SetReady(SetReadyRequest request) 
    {
        if (lobbiesRegistry.TrySetUserReady(Context.User.UserData(), request, out ReturnMessage message)) 
        {
            if (lobbiesRegistry.TryGetLobbyWithUser(Context.User.UserData(), out LobbyData lobbyData, out ReturnMessage errorMessage)) 
            {
                LobbyDataToDisplay dataToDisplay = lobbiesRegistry.ConvertToLobbyDataToDisplay(lobbyData);
                await lobbiesHubContext.Clients.All.SendAsync(AllKeys.LobbyMessages.UPDATE, dataToDisplay);
                
                LobbyPlayerData lobbyPlayerData = lobbiesRegistry.GetLobbyPlayerData(Context.User.UserData(), lobbyData);
                await Clients.GroupExcept(lobbyData.LobbyName, Context.ConnectionId).SendAsync(AllKeys.LobbyMessages.PLAYER_CHANGED_READINESS, lobbyPlayerData);
            }
            else 
            {
                return errorMessage;
            }
        }
        return message;
    }

    public List<LobbyPlayerData> GetPlayers()
    {
        Console.WriteLine("[LobbyHub GetPlayers]");
        if (lobbiesRegistry.TryGetLobbyWithUser(Context.User.UserData(), out LobbyData lobby, out ReturnMessage errorMessage))
        {
            LobbyPlayersData lobbyPlayersData = new();

            for (int i = 0; i < lobby.Players.Count; i++)
            {
                lobbyPlayersData.Players.Add(new LobbyPlayerData()
                {
                    Id = lobby.Players[i].Id,
                    Username = lobby.Players[i].Username,
                    Role = lobby.Players[i].Role,
                    IsReady = lobby.PlayersReady[i],
                    IsOwner = lobby.Players[i].IsSameUserAs(lobby.Owner)
                });
            }
            return lobbyPlayersData.Players;
        }
        
        throw new Exception($"No lobby with {Context.User.UserData().Username} found.");
    }

    public async Task<ReturnMessage> StartTheGame() 
    {
        if (lobbiesRegistry.TryGetLobbyWithUser(Context.User.UserData(), out LobbyData lobbyData, out ReturnMessage errorMessage)) 
        {
            if (lobbiesRegistry.ArePlayersReady(lobbyData.Id)) 
            {
                foreach (UserData playerData in lobbyData.Players) 
                {
                    gameManager.AddPlayer(playerData, lobbyData.Id);
                }
                
                await Clients.Group(lobbyData.LobbyName).SendAsync(AllKeys.LobbyMessages.GAME_STARTED); 
                return new ReturnMessage(ReturnMessageType.Lobby.GAME_STARTED, true);
            }
            else 
            {
                return new ReturnMessage(ReturnMessageType.Lobby.PLAYERS_NOT_READY, false);
            }
        }
        return errorMessage;
    }

    public async Task<ReturnMessage> KickPlayer(KickPlayerRequest request) 
    {
        UserData userData = new(request.Id, request.Username, request.Role);

        if (lobbiesRegistry.CanKickUser(Context.User.UserData(), userData, out ReturnMessage message)) 
        {
            lobbiesRegistry.TryGetLobbyWithUser(userData, out LobbyData lobbyData, out _);
            MessageData messageData = new(new UserData(-1, "SYSTEM", ""), $"{request.Username} kicked.", lobbyData.Id, DateTimeOffset.Now);
            chatManager.AddMessage(lobbyData.Id, messageData);
            string currentDate = messageData.SentAt.Hour + ":" + messageData.SentAt.Minute + ":" + messageData.SentAt.Second;
            OutputMessage outputMessage = new("SYSTEM", currentDate, messageData.Message);
            LobbyPlayerData lobbyPlayerData = lobbiesRegistry.GetLobbyPlayerData(userData, lobbyData);
            await chatHubContext.Clients.Group(lobbyData.Id.ToString()).SendAsync(AllKeys.ChatMessages.SEND_MESSAGE, outputMessage);

            await Clients.Client(lobbiesRegistry.GetConnection($"{request.Id} {request.Role}")).SendAsync(AllKeys.LobbyMessages.PLAYER_KICKED, lobbyPlayerData);
            return message;
        }
        return message;
    }
}
