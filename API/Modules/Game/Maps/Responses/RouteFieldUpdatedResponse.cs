using API.Modules.Game.Enemies.Responses;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Maps.Responses;

public class RouteFieldUpdatedResponse
{
    public RouteFieldUpdatedResponse(Position position, string itemName, EnemyResponse? enemy, bool chest)
    {
        Position = position;
        ItemName = itemName;
        Enemy = enemy;
        Chest = chest;
    }

    public Position Position { get; set; }
    public string ItemName { get; set; }
    public EnemyResponse? Enemy { get; set; }
    public bool Chest { get; set; }
}