namespace API.DTOs;
public class SettingsWithLanguageDto
{
    public int SoundVolume { get; set; }
    public int MusicVolume { get; set; }
    public string LanguageCode { get; set; }

    public SettingsWithLanguageDto()
    {
    }

    public SettingsWithLanguageDto (int soundVolume, int musicVolume, string langaugeCode) 
    {
        SoundVolume = soundVolume;
        MusicVolume = musicVolume;
        LanguageCode = langaugeCode;
    }
}
