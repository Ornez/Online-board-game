namespace API.Modules.Game.Fights.Responses;

public class RollDiceResponse
{
    public RollDiceResponse(List<int> rollDiceResults)
    {
        RollDiceResults = rollDiceResults;
    }

    public List<int> RollDiceResults { get; set; }
}