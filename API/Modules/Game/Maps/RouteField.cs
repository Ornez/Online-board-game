using API.Modules.Game.Enemies;
using API.Modules.Game.Inventories;
using API.Modules.Game.Utils;

namespace API.Modules.Game.Maps;

public class RouteField
{
    public RouteField(bool[] connections, int rotation, bool isRoom, bool isFountain)
    {
        Connections = connections;
        Rotation = rotation;
        IsRoom = isRoom;
        IsFountain = isFountain;
    }

    public RouteField(bool[] connections, Position position, int rotation, bool isRoom, bool isFountain)
    {
        Connections = connections;
        Rotation = rotation;
        IsRoom = isRoom;
        IsFountain = isFountain;
        Position = position;
    }

    public bool[] Connections { get; set; }
    public int Rotation { get; set; }
    public bool IsRoom { get; set; }
    public bool IsFountain { get; set; }
    public Position Position { get; set; }
    public RouteField Parent { get; set; }
    public Item? Item { get; set; }
    public Enemy? Enemy { get; set; }
    public bool IsChest { get; set; }

    public float DistanceToTarget { get; set; } = -1;
    public float Cost { get; set; } = 1;
    public float Weight { get; set; } = 1;

    public float F
    {
        get
        {
            if (DistanceToTarget != -1 && Cost != -1)
                return DistanceToTarget + Cost;
            return -1;
        }
    }

    public bool TopConnection => Connections[((0 - Rotation) % 4 + 4) % 4];
    public bool RightConnection => Connections[((1 - Rotation) % 4 + 4) % 4];
    public bool BottomConnection => Connections[((2 - Rotation) % 4 + 4) % 4];
    public bool LeftConnection => Connections[((3 - Rotation) % 4 + 4) % 4];

    public bool IsConnectedTo(RouteField other)
    {
        bool isConnected = (TopConnection && other.BottomConnection && Position.Y + 1 == other.Position.Y)
               || (RightConnection && other.LeftConnection && Position.X + 1 == other.Position.X)
               || (BottomConnection && other.TopConnection && Position.Y - 1 == other.Position.Y)
               || (LeftConnection && other.RightConnection && Position.X - 1 == other.Position.X);
        //Console.WriteLine($"IS CONNECTED [{Position.X},{Position.Y}] [{other.Position.X},{other.Position.Y}]  {Rotation} {other.Rotation}: {isConnected}");
        return isConnected;
    }
}