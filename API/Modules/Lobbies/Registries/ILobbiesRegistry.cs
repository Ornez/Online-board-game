using API.Data;
using API.Data.Game;
using API.Modules.Lobbies.Data;
using API.Modules.Lobby.Data;

namespace API.Modules.Lobbies.Registries;
public interface ILobbiesRegistry
{
    bool TryCreateLobby(CreateLobbyRequest createLobbyRequest, UserData playerData, out ReturnMessage message);
    bool TryJoinLobby(JoinLobbyRequest request, UserData playerData, out ReturnMessage message);
    bool TryLeaveLobby(UserData playerData, LeaveLobbyRequest request, out ReturnMessage message);
    List<LobbyDataToDisplay> GetLobbiesToDisplay();
    bool TryGetLobbyByName(string lobbyName, out LobbyData lobby, out ReturnMessage message);
    bool TryGetLobbyById(int lobbyId, out LobbyData lobby, out ReturnMessage message);
    LobbyDataToDisplay ConvertToLobbyDataToDisplay(LobbyData data);
    bool TrySetUserReady(UserData playerData, SetReadyRequest request, out ReturnMessage message);
    bool TryGetLobbyWithUser(UserData playerData, out LobbyData lobby, out ReturnMessage message);
    LobbyPlayerData GetLobbyPlayerData(UserData playerData, LobbyData lobbyData);
    bool ArePlayersReady(int lobbyId);
    bool CanKickUser(UserData userData, UserData userToKick, out ReturnMessage message);
    void AddUserToConnection(string userIdAndRole, string connectionId);
    void RemoveUserToConnection(string userIdAndRole);
    string GetConnection(string userIdAndRole);
}
