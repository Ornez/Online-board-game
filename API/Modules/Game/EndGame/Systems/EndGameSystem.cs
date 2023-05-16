using API.Data.Game;
using API.Hubs;
using API.Modules.Game.Managers;
using API.Modules.Game.EndGame.Responses;
using Microsoft.AspNetCore.SignalR;

namespace API.Modules.Game.EndGame.Systems;

public class EndGameSystem : BaseSystem, IEndGameSystem
{
    public EndGameSystem(IGameManager gameManager, IHubContext<GameHub> gameHubContext) : base(gameManager, gameHubContext)
    {
    }

    public EndGameResponse GetEndGameResponse(UserData userData)
    {
        GameData game = CurrentGame(userData);
        List<PlayerData> players = game.Players;
        players.Sort((player1, player2) => player2.Treasures.CompareTo(player1.Treasures));

        foreach (var p in players)
        {
            Console.WriteLine($"PLAYER {p.UserData.Id} TREASURES: {p.Treasures}");
        }
        
        int place = 1;

        List<EndGameUserResponse> firstPlace = new();
        List<EndGameUserResponse> secondPlace = new();
        List<EndGameUserResponse> thirdPlace = new();
        firstPlace.Add(new (players[0].UserData, players[0].ChampionType.ToString()));
        for (int i = 1; i < players.Count; i++)
        {
            if (players[i].Treasures < players[i - 1].Treasures)
                place++;

            if (place > 3)
                break;
            
            switch (place)
            {
                case 1:
                    firstPlace.Add(new (players[i].UserData, players[i].ChampionType.ToString()));
                    break;
                case 2:
                    secondPlace.Add(new (players[i].UserData, players[i].ChampionType.ToString()));
                    break;
                case 3:
                    thirdPlace.Add(new (players[i].UserData, players[i].ChampionType.ToString()));
                    break;
            }
        }

        return new EndGameResponse(firstPlace, secondPlace, thirdPlace);
    }
}