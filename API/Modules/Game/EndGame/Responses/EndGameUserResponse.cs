using API.Data.Game;

namespace API.Modules.Game.EndGame.Responses;

public class EndGameUserResponse
{
    public EndGameUserResponse(UserData userData, string characterName)
    {
        UserData = userData;
        CharacterName = characterName;
    }

    public UserData UserData { get; set; }
    public string CharacterName { get; set; }
}