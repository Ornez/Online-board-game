using API.Modules.Game.Enemies;
using API.Modules.Game.Enemies.Responses;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Maps.Responses;

public class RouteFieldPlacedResponse
{
    public RouteFieldPlacedResponse(string fieldType, Position position, int rotation, EnemyResponse? enemy, bool chest)
    {
        FieldType = fieldType;
        Position = position;
        Rotation = rotation;
        Enemy = enemy;
        Chest = chest;
    }

    public string FieldType { get; set; }
    public Position Position { get; set; }
    public int Rotation { get; set; }
    public EnemyResponse? Enemy { get; set; }
    public bool Chest { get; set; }
}