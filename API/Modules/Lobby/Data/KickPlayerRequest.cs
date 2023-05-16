namespace API.Modules.Lobby.Data;
public class KickPlayerRequest
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }

    public KickPlayerRequest(int id, string username, string role)
    {
        Id = id;
        Username = username;
        Role = role;
    }
}
