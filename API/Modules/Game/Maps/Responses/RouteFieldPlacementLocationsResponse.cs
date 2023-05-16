using API.Modules.Game.Utils;

namespace API.Modules.Game.Maps.Responses;

public class RouteFieldPlacementLocationsResponse
{
    public RouteFieldPlacementLocationsResponse(string fieldType, List<List<Position>> positionsToPlace)
    {
        FieldType = fieldType;
        PositionsToPlace = positionsToPlace;
    }

    public string FieldType { get; set; }
    public List<List<Position>> PositionsToPlace { get; set; }
}