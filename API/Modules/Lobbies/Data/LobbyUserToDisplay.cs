namespace API.Modules.Lobbies.Data;
public class LobbyUserToDisplay
{
    public string Username { get; set; }
    public bool IsReady { get; set; }
    public bool IsOwner { get; set; }

    public LobbyUserToDisplay(string username, bool isReady, bool isOwner)
    {
        Username = username;
        IsReady = isReady;
        IsOwner = isOwner;
    }
}
