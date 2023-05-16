namespace API.Modules.Guests.Data;
public class GuestData
{
    public int Id { get; set; }
    public string Username { get; set; }

    public GuestData(int id, string username) 
    {
        Id = id;
        Username = username;
    }
}
