namespace API.Entities;
public class Settings
{
    public int Id { get; set; }
    public int SoundVolume { get; set; }
    public int MusicVolume { get; set; }
    public Language Language { get; set; }
    public int LanguageId { get; set; }
    public AppUser AppUser { get; set; }
}
