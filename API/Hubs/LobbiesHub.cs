using API.Data;
using API.Modules.Lobbies.Data;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using API.Modules.Game.Keys;
using API.Modules.Lobbies.Registries;

namespace API.Hubs;
[Authorize]
public class LobbiesHub : Hub
{
    private readonly ILobbiesRegistry lobbiesRegistry;
    
    public LobbiesHub(ILobbiesRegistry lobbiesRegistry) 
    {
        this.lobbiesRegistry = lobbiesRegistry;
    }

    public async Task<CreateLobbyResponse> CreateLobby(CreateLobbyRequest request)
    {
        if (lobbiesRegistry.TryCreateLobby(request, Context.User.UserData(), out ReturnMessage message)) 
        {
            if (lobbiesRegistry.TryGetLobbyByName(request.LobbyName, out LobbyData lobbyData, out _)) 
            {
                LobbyDataToDisplay dataToDisplay = lobbiesRegistry.ConvertToLobbyDataToDisplay(lobbyData);
                await Clients.Others.SendAsync(AllKeys.LobbyMessages.CREATE, dataToDisplay);
                return new CreateLobbyResponse() 
                {
                    MessageType = message.MessageType,
                    Success = message.Success,
                    LobbyId = lobbyData.Id
                };
            }
        }
        return new CreateLobbyResponse() 
        {
            MessageType = message.MessageType,
            Success = message.Success,
            LobbyId = 0
        };
    }

    public async Task<ReturnMessage> JoinLobby(JoinLobbyRequest request)
    {
        if (lobbiesRegistry.TryJoinLobby(request, Context.User.UserData(), out ReturnMessage message)) 
        {
            if (lobbiesRegistry.TryGetLobbyByName(request.LobbyName, out LobbyData lobbyData, out _)) 
            {
                LobbyDataToDisplay dataToDisplay = lobbiesRegistry.ConvertToLobbyDataToDisplay(lobbyData);
                await Clients.Others.SendAsync(AllKeys.LobbyMessages.UPDATE, dataToDisplay);
            }
        }
        return message;
    }

    public List<LobbyDataToDisplay> GetLobbies()
    {
        return lobbiesRegistry.GetLobbiesToDisplay();
    }
}
