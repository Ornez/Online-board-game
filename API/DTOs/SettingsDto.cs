namespace API.DTOs;
public class SettingsDto
{
    public int SoundVolume { get; set; }
    public int MusicVolume { get; set; }

    public SettingsDto(int soundVolume, int musicVolume) 
    {
        SoundVolume = soundVolume;
        MusicVolume = musicVolume;
    }
}
