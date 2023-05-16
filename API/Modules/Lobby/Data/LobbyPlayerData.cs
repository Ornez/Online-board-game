namespace API.Modules.Lobby.Data;
public class LobbyPlayerData
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public bool IsReady { get; set; }
    public bool IsOwner { get; set; }
}
