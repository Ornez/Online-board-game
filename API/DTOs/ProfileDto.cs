namespace API.DTOs;
public class ProfileDto
{
    public string Email { get; set; }
    public string Username { get; set; }

    public ProfileDto(string email, string username) 
    {
        Email = email;
        Username = username;
    }
}
