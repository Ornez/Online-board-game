using API.Modules.Game.Enemies;
using API.Modules.Game.Keys;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Maps;

public class Map
{
    public Map()
    {
        var centerIndex = AllKeys.MapKeys.MAP_CENTER;
        RouteFields[centerIndex, centerIndex] = new RouteField(new[] { true, true, true, true },
            new Position(centerIndex, centerIndex), 0, true, true);
    }

    public List<RouteField> AvailableRouteFields { get; set; } = new();
    public List<Enemy> AvailableEnemies { get; set; } = new();
    public RouteField[,] RouteFields { get; set; } = new RouteField[AllKeys.MapKeys.MAP_SIZE, AllKeys.MapKeys.MAP_SIZE];
    public RouteField? SelectedRouteField { get; set; }
}