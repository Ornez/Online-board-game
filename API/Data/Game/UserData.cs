namespace API.Data.Game
{
    public class UserData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        public UserData(int id, string username, string role) 
        {
            Id = id;
            Username = username;
            Role = role;
        }

        public override string ToString()
        {
            return $"Id: {Id} Username: {Username} Role: {Role}";
        }
    }
}