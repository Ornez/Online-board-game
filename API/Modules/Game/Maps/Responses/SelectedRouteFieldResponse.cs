namespace API.Modules.Game.Maps.Responses;

public class SelectedRouteFieldResponse
{
    public SelectedRouteFieldResponse(RouteFieldType routeFieldType)
    {
        FieldType = routeFieldType.ToString();
    }
    
    public string FieldType { get; set; }
}