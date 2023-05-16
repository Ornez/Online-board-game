using API.Data;
using API.Data.Game;
using API.Modules.Lobbies.Data;
using API.Modules.Lobby.Data;
using API.Extensions;
using API.Interfaces;
using API.Modules.Game.Keys;

namespace API.Modules.Lobbies.Registries;
public class LobbiesRegistry : ILobbiesRegistry
{
    private readonly List<LobbyData> lobbies = new();
    private int nextLobbyId = 0;
    private readonly IPlayersRegistry playersRegistry;
    private Dictionary<string, string> userToConnectionId = new();


    public LobbiesRegistry(IPlayersRegistry playersRegistry) 
    {
        this.playersRegistry = playersRegistry;
        //MockLobbiesData();
    }

    private void MockLobbiesData() 
    {
        lobbies.Add(new LobbyData 
        {
            Id = nextLobbyId,
            Owner = new(100, "Player100", AllKeys.Roles.USER),
            LobbyName = "Lobby 1",
            Password = "1234",
            Players = new List<API.Data.Game.UserData>() 
            {
                new(100, "Player100", AllKeys.Roles.USER)
            },
            PlayersReady = new List<bool> { true },
            MaxPlayersNumber = 5,
            IsPrivate = true
        });
        nextLobbyId++;

        lobbies.Add(new LobbyData 
        {
            Id = nextLobbyId,
            Owner = new(101, "Player101", AllKeys.Roles.GUEST),
            LobbyName = "Lobby 2",
            Password = "1234",
            Players = new List<API.Data.Game.UserData>() 
            {
                new(101, "Player101", AllKeys.Roles.GUEST),
                new(102, "Player102", AllKeys.Roles.USER)
            },
            PlayersReady = new List<bool> { true, false },
            MaxPlayersNumber = 5,
            IsPrivate = true
        });
        nextLobbyId++;

        lobbies.Add(new LobbyData 
        {
            Id = nextLobbyId,
            Owner = new(103, "Player103", AllKeys.Roles.GUEST),
            LobbyName = "Lobby 3",
            Password = "1234",
            Players = new List<API.Data.Game.UserData>() 
            {
                new(103, "Player103", AllKeys.Roles.GUEST),
                new(104, "Player104", AllKeys.Roles.USER)
            },
            PlayersReady = new List<bool> { true, false },
            MaxPlayersNumber = 5,
            IsPrivate = true
        });

        nextLobbyId++;
    }

    public bool TryCreateLobby(CreateLobbyRequest createLobbyRequest, UserData playerData, out ReturnMessage message) 
    {
        if (createLobbyRequest.LobbyName == "") 
        {
            message = new(ReturnMessageType.CREATE_LOBBY_NAME_CANNOT_BE_EMPTY, false);
            return false;
        }

        if (TryGetLobbyByName(createLobbyRequest.LobbyName, out _, out _)) 
        {
            message = new(ReturnMessageType.CREATE_LOBBY_NAME_IN_USE, false);
            return false;
        }

        CreateLobby(createLobbyRequest, playerData, out message);
        return true;
    }

    private void CreateLobby(CreateLobbyRequest createLobbyRequest, UserData playerData, out ReturnMessage message) 
    {
        lobbies.Add(new LobbyData 
        {
            Id = nextLobbyId,
            Owner = playerData,
            LobbyName = createLobbyRequest.LobbyName,
            Password = createLobbyRequest.Password,
            Players = new() { playerData },
            PlayersReady = new() { false },
            MaxPlayersNumber = createLobbyRequest.MaxPlayersNumber,
            IsPrivate = createLobbyRequest.IsPrivate
        });
        nextLobbyId++;
        message = new(ReturnMessageType.CREATE_LOBBY_SUCCESSFULLY, true);
        Console.WriteLine($"[CREATED_LOBBY] UserName: {playerData.Username} Role: {playerData.Role}");
    }

    public bool TryJoinLobby(JoinLobbyRequest request, UserData playerData, out ReturnMessage message) 
    {
        if (lobbies.Find(oneLobby => oneLobby.Players.Find(onePlayer => onePlayer.IsSameUserAs(playerData)) != null) != null) 
        {
            message = new("Player cannot be in two lobbies in same time.", false);
            return false;
        }

        if (!TryGetLobbyByName(request.LobbyName, out LobbyData lobby, out message)) 
        {
            return false;
        }
        
        if (lobby.IsPrivate && !IsPasswordValid(lobby, request.Password, out message)) 
        {
            return false;
        }

        if (lobby.Players.Find(x => x.IsSameUserAs(playerData)) != null) 
        {
            message = new(ReturnMessageType.JOIN_LOBBY_PLAYER_ALREADY_IN_LOBBY, false);
            return false;
        }

        if (IsLobbyFull(lobby, out message)) 
        {
            return false;
        }

        JoinLobby(playerData, lobby, out message);
        return true;
    }

    public bool TryGetLobbyByName(string lobbyName, out LobbyData lobby, out ReturnMessage message) 
    {
        lobby = lobbies.Find(lobby => lobby.LobbyName == lobbyName);
        if (lobby != null) 
        {
            message = new();
            return true;
        }
        message = new(ReturnMessageType.LOBBY_WITH_THIS_NAME_DOESNT_EXISTS, false);
        return false;
    }

    public bool TryGetLobbyById(int lobbyId, out LobbyData lobby, out ReturnMessage message) 
    {
        lobby = lobbies.Find(lobby => lobby.Id == lobbyId);
        if (lobby != null) 
        {
            message = new();
            return true;
        }
        message = new(ReturnMessageType.LOBBY_WITH_THIS_ID_DOESNT_EXISTS, false);
        return false;
    }

    private bool IsPasswordValid(LobbyData lobby, string password, out ReturnMessage message) 
    {
        if (lobby.Password == password) 
        {
            message = new();
            return true;
        }
        message = new(ReturnMessageType.JOIN_LOBBY_WRONG_PASSWORD, false);
        return false;
    }

    private bool IsLobbyFull(LobbyData lobbyData, out ReturnMessage message) 
    {
        if (lobbyData.MaxPlayersNumber > lobbyData.Players.Count() + 1) 
        {
            message = new();
            return false;
        }
        message = new(ReturnMessageType.JOIN_LOBBY_NO_SLOTS, false);
        return false;
    }

    private void JoinLobby(UserData playerData, LobbyData lobbyData, out ReturnMessage message) 
    {
        lobbyData.Players.Add(playerData);
        lobbyData.PlayersReady.Add(false);
        message = new(ReturnMessageType.JOIN_LOBBY_SUCCESSFULLY, true);
        Console.WriteLine("[JOIN_LOBBY]");
    }

    public bool TryLeaveLobby(UserData playerData, LeaveLobbyRequest request, out ReturnMessage message) 
    {
        if (!TryGetLobbyById(request.LobbyId, out LobbyData lobbyData, out message)) 
        {
            return false;
        }
        var itemToRemove = lobbyData.Players.Find(x => x.Id == playerData.Id && x.Role == playerData.Role);
        int indexToRemove = lobbyData.Players.IndexOf(itemToRemove);
        
        //TODO: Think about better aproach
        if (indexToRemove != -1)
        {
            lobbyData.Players.RemoveAt(indexToRemove);
            lobbyData.PlayersReady.RemoveAt(indexToRemove);
        }
        
        if (lobbyData.Players.Count == 0) {
            var lobbyToRemove = lobbies.Find(x => x.Id == lobbyData.Id);
            lobbies.Remove(lobbyToRemove);
        }

        message = new(ReturnMessageType.LEAVE_LOBBY_SUCCESSFULLY, true);
        return true;
    }

    public LobbyDataToDisplay ConvertToLobbyDataToDisplay(LobbyData lobbyData)
    {
        List<LobbyUserToDisplay> lobbyUsersToDisplay = new();
        for (int i = 0; i < lobbyData.Players.Count; i++) 
        {
            lobbyUsersToDisplay.Add(new LobbyUserToDisplay(lobbyData.Players[i].Username, lobbyData.PlayersReady[i], lobbyData.Players[i].IsSameUserAs(lobbyData.Owner)));
        }

        return new LobbyDataToDisplay 
        {
            Id = lobbyData.Id,
            LobbyName = lobbyData.LobbyName,
            Players = lobbyUsersToDisplay,
            MaxPlayersNumber = lobbyData.MaxPlayersNumber,
            IsPrivate = lobbyData.IsPrivate
        };
    }

    public List<LobbyDataToDisplay> GetLobbiesToDisplay() 
    {
        List<LobbyDataToDisplay> lobbiesToDisplay = new();
        foreach (LobbyData lobbyData in lobbies) 
        {
            List<LobbyUserToDisplay> lobbyUsersToDisplay = new();

            for (int i = 0; i < lobbyData.Players.Count; i++) 
            {
                lobbyUsersToDisplay.Add(new LobbyUserToDisplay(lobbyData.Players[i].Username, lobbyData.PlayersReady[i], lobbyData.Players[i].IsSameUserAs(lobbyData.Owner)));
            }

            lobbiesToDisplay.Add(new LobbyDataToDisplay 
            {
                Id = lobbyData.Id,
                LobbyName = lobbyData.LobbyName,
                Players = lobbyUsersToDisplay,
                MaxPlayersNumber = lobbyData.MaxPlayersNumber,
                IsPrivate = lobbyData.IsPrivate
            });
        }
        return lobbiesToDisplay;
    }

    public bool TrySetUserReady(UserData playerData, SetReadyRequest request, out ReturnMessage message) 
    {
        if (!TryGetLobbyWithUser(playerData, out LobbyData lobbyData, out message)) 
        {
            return false;
        }
        var item = lobbyData.Players.Find(x => x.Id == playerData.Id && x.Role == playerData.Role);
        int index = lobbyData.Players.IndexOf(item);
        lobbyData.PlayersReady[index] = request.IsReady;

        message = new(ReturnMessageType.USER_CHANGED_READINESS, true);
        return true;
    }

    public bool TryGetLobbyWithUser(UserData playerData, out LobbyData lobby, out ReturnMessage message) 
    {
        lobby = null;
        for (int i = 0; i < lobbies.Count; i++) 
        {
            for (int j = 0; j < lobbies[i].Players.Count; j++) 
            {
                if (lobbies[i].Players[j].IsSameUserAs(playerData)) 
                {
                    lobby = lobbies[i];
                }
            }
        }
        if (lobby != null) 
        {
            message = new();
            return true;
        }
        message = new(ReturnMessageType.NO_LOBBY_WITH_THIS_PLAYER_ID, false);
        return false;
    }

    public LobbyPlayerData GetLobbyPlayerData(UserData playerData, LobbyData lobbyData) 
    {
        int playerIndex = 0;
        for (int i = 0; i < lobbyData.Players.Count; i++) 
        {
            if (lobbyData.Players[i].IsSameUserAs(playerData))
            {
                playerIndex = i;
                break;
            }
        }

        LobbyPlayerData lobbyPlayerData = new() 
        {
            Id = playerData.Id,
            Username = playerData.Username,
            Role = playerData.Role,
            IsReady = lobbyData.PlayersReady[playerIndex],
            IsOwner = lobbyData.Owner.IsSameUserAs(playerData)
        };
        return lobbyPlayerData;
    }

    public bool ArePlayersReady(int lobbyId) 
    {
        LobbyData lobbyData = lobbies.Find(x => x.Id == lobbyId);
        if (lobbyData != null) 
        {
            foreach (bool playerReady in lobbyData.PlayersReady) 
            {
                if (playerReady == false) 
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public bool CanKickUser(UserData userData, UserData userToKick, out ReturnMessage message) 
    {
        if (TryGetLobbyWithUser(userData, out LobbyData lobbyData, out message)) 
        {
            if (lobbyData.Owner.IsSameUserAs(userData)) 
            {
                message = new(ReturnMessageType.Lobby.PLAYER_KICKED_SUCCESSFULLY, true);
                return true;
            }
            message = new(ReturnMessageType.Lobby.ONLY_OWNER_CAN_KICK_PLAYERS, false);
        }
        return false;
    }

    public void AddUserToConnection(string userIdAndRole, string connectionId) 
    {
        userToConnectionId.Add(userIdAndRole, connectionId);
    }

    public void RemoveUserToConnection(string userIdAndRole) 
    {
        userToConnectionId.Remove(userIdAndRole);
    }

    public string GetConnection(string userIdAndRole) 
    {
        return userToConnectionId[userIdAndRole];
    }
}