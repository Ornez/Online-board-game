using API.Modules.Game.Utils;

namespace API.Modules.Game.Maps.Requests;

public class SetRouteFieldRequest
{
    public Position Position { get; set; }
    public int Rotation { get; set; }
}