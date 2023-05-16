namespace API.DTOs;
public class MemberDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public LanguageDto Language { get; set; }
    public SettingsDto Settings { get; set; }
    public StatisticsDto Statistics { get; set; }
}