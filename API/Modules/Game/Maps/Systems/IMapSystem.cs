using API.Data.Game;
using API.Modules.Game.Maps.Responses;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Maps.Systems;

public interface IMapSystem
{
    Task PrepareMap(UserData userData);
    RouteField GetRandomRouteField(Map map);
    RouteFieldPlacementLocationsResponse GetRouteFieldPlacementLocations(Map map, RouteField routeField, PlayerData playerData);
    RouteFieldType GetRouteFieldType(RouteField routeField);
    Task PlaceRouteField(UserData userData, Position position, int rotation);
    RouteField GetClosestFountain(RouteField routeField, Map map);
}