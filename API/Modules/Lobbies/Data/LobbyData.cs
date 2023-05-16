using API.Data.Game;

namespace API.Modules.Lobbies.Data;
public class LobbyData
{
    public int Id { get; set; }
    public UserData Owner { get; set; }
    public string LobbyName { get; set; }
    public string Password { get; set; }
    public List<UserData> Players { get; set; }
    public List<bool> PlayersReady { get; set; }
    public int MaxPlayersNumber { get; set; }
    public bool IsPrivate { get; set; }
}