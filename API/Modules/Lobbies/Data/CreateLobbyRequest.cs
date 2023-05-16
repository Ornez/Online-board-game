namespace API.Modules.Lobbies.Data;
public class CreateLobbyRequest
{
    public string LobbyName { get; set; }
    public string Password { get; set; } = "";
    public int MaxPlayersNumber { get; set; }
    public bool IsPrivate { get; set; }
}
