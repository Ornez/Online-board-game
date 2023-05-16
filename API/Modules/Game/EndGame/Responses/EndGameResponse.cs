namespace API.Modules.Game.EndGame.Responses;

public class EndGameResponse
{
    public EndGameResponse(List<EndGameUserResponse> firstPlace, List<EndGameUserResponse> secondPlace, List<EndGameUserResponse> thirdPlace)
    {
        FirstPlace = firstPlace;
        SecondPlace = secondPlace;
        ThirdPlace = thirdPlace;
    }

    public List<EndGameUserResponse> FirstPlace { get; set; }
    public List<EndGameUserResponse> SecondPlace { get; set; }
    public List<EndGameUserResponse> ThirdPlace { get; set; }
}