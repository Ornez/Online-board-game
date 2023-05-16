using System.Diagnostics;
using API.Modules.Game.Keys;
using API.Modules.Game.Maps;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Pahtfindings;

public class Pathfinding : IPathfinding
{
    public Stack<RouteField> CalculatePath(RouteField start, RouteField end, Map map)
    {
        Stack<RouteField> path = new();
        PriorityQueue<RouteField, float> openList = new();
        List<RouteField> closedList = new();
        List<RouteField> neighbours;
        RouteField currentField = start;

        openList.Enqueue(start, start.F);

        while (openList.Count != 0 && !closedList.Exists(x => x.Position.IsSameAs(end.Position)))
        {
            currentField = openList.Dequeue();
            closedList.Add(currentField);
            neighbours = GetAdjacentNodes(currentField, map);

            foreach (RouteField n in neighbours)
                if (!closedList.Contains(n))
                {
                    var isFound = false;
                    foreach (var openListField in openList.UnorderedItems)
                        if (openListField.Element == n)
                            isFound = true;
                    
                    if (!isFound)
                    {
                        n.Parent = currentField;
                        n.DistanceToTarget = Math.Abs(n.Position.X - end.Position.X) +
                                             Math.Abs(n.Position.Y - end.Position.Y);
                        n.Cost = n.Weight + n.Parent.Cost;
                        openList.Enqueue(n, n.F);
                    }
                }
        }

        if (!closedList.Exists(x => x.Position.IsSameAs(end.Position))) return null;

        RouteField temp = closedList[closedList.IndexOf(currentField)];
        if (temp == null) return null;
        do
        {
            path.Push(temp);
            temp = temp.Parent;
        } while (temp != start && temp != null);

        return path;
    }

    public List<Position> GetPath(RouteField start, RouteField end, Map map)
    {
        var path = CalculatePath(start, end, map) ?? throw new Exception("Path not found.");
        List<Position> positions = new();
        foreach (var routeField in path)
        {
            positions.Add(routeField.Position);
        }
        return positions;
    }

    public int CalculateMovementPointsCost(RouteField start, Position endPos, Map map)
    {
        if (IsConnectedToEmptySpace(start, endPos))
        {
            return 1;
        }
        Stack<RouteField> Path = new();
        PriorityQueue<RouteField, float> OpenList = new();
        List<RouteField> ClosedList = new();
        List<RouteField> adjacencies;
        RouteField current = start;

        OpenList.Enqueue(start, start.F);

        for (int y = 0; y < AllKeys.MapKeys.MAP_SIZE; y++)
        {
            for (int x = 0; x < AllKeys.MapKeys.MAP_SIZE; x++)
            {
                if (map.RouteFields[x, y] != null)
                    map.RouteFields[x, y].Parent = null;
            }
        }
        

        while (OpenList.Count != 0 && !ClosedList.Exists(x => IsConnectedToEmptySpace(x, endPos)))
        {
            current = OpenList.Dequeue();
            ClosedList.Add(current);
            //Console.WriteLine($"ClosedList ADDED: {current.Position}");
            adjacencies = GetAdjacentNodes(current, map);

            foreach (RouteField neighbour in adjacencies)
                if (!ClosedList.Contains(neighbour))
                {
                    var isFound = false;
                    foreach (var oLNode in OpenList.UnorderedItems)
                        if (oLNode.Element == neighbour)
                            isFound = true;
                    if (!isFound)
                    {
                        neighbour.Parent = current;
                        neighbour.DistanceToTarget = Math.Abs(neighbour.Position.X - endPos.X) + Math.Abs(neighbour.Position.Y - endPos.Y);
                        neighbour.Cost = neighbour.Weight + neighbour.Parent.Cost;
                        OpenList.Enqueue(neighbour, neighbour.F);
                        
                    }
                }
        }
        

        // if all good, return path
        //RouteField temp = ClosedList[ClosedList.IndexOf(current)];
        RouteField temp = ClosedList.Find(x => IsConnectedToEmptySpace(x, endPos));
        
        //Console.WriteLine($"CURRENT: {temp.Position}");

        //Console.WriteLine($"NEW PATH==================================");
        if (temp == null)
        {
            //Console.WriteLine($"NO PATH==================================");
            return 0;
        }
        
        do
        {
            Path.Push(temp);
            temp = temp.Parent;
        } while (temp != start && temp != null);

        if (Path.Count + 1 <= 4)
        {
            //Console.WriteLine($"END POS: {endPos}");
            foreach (var node in Path)
            {
                // Console.WriteLine($"POS: {node.Position} ROT: {node.Rotation.ToString()} " +
                //                   $"| {node.TopConnection}" +
                //                   $" {node.RightConnection}" +
                //                   $" {node.BottomConnection}" +
                //                   $" {node.LeftConnection}");
            }
        }
        
        return Path.Count + 1;
    }

    private bool IsConnectedToEmptySpace(RouteField routeField, Position otherPosition)
    {
        return ((routeField.Position.X == otherPosition.X - 1 && routeField.RightConnection)
               || (routeField.Position.X == otherPosition.X + 1 && routeField.LeftConnection)
               || (routeField.Position.Y == otherPosition.Y - 1 && routeField.TopConnection)
               || (routeField.Position.Y == otherPosition.Y + 1 && routeField.BottomConnection)) 
               && (Math.Abs(routeField.Position.X - otherPosition.X) + Math.Abs(routeField.Position.Y - otherPosition.Y) <= 1.1f);
    }

    private List<RouteField> GetAdjacentNodes(RouteField n, Map map)
    {
        List<RouteField> temp = new();

        var row = (int)n.Position.Y;
        var col = (int)n.Position.X;

        if (row + 1 < AllKeys.MapKeys.MAP_SIZE && map.RouteFields[col, row + 1] != null
                                               && n.IsConnectedTo(map.RouteFields[col, row + 1]))
            temp.Add(map.RouteFields[col, row + 1]);
        if (row - 1 >= 0 && map.RouteFields[col, row - 1] != null
                         && n.IsConnectedTo(map.RouteFields[col, row - 1]))
            temp.Add(map.RouteFields[col, row - 1]);
        if (col - 1 >= 0 && map.RouteFields[col - 1, row] != null
                         && n.IsConnectedTo(map.RouteFields[col - 1, row]))
            temp.Add(map.RouteFields[col - 1, row]);
        if (col + 1 < AllKeys.MapKeys.MAP_SIZE && map.RouteFields[col + 1, row] != null
                                               && n.IsConnectedTo(map.RouteFields[col + 1, row]))
            temp.Add(map.RouteFields[col + 1, row]);

        return temp;
    }
}