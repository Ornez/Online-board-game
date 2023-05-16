namespace API.Modules.Lobbies.Data;
public class LobbyDataToDisplay
{
    public int Id { get; set;}
    public string LobbyName { get; set; }
    public List<LobbyUserToDisplay> Players { get; set; }
    public int MaxPlayersNumber { get; set; }
    public bool IsPrivate { get; set; }
}
