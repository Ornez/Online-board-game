using API.Modules.Game.Maps;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Pahtfindings;

public interface IPathfinding
{
    Stack<RouteField> CalculatePath(RouteField start, RouteField end, Map map);
    List<Position> GetPath(RouteField start, RouteField end, Map map);
    int CalculateMovementPointsCost(RouteField start, Position endPos, Map map);
}