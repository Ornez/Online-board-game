namespace API.Modules.Game.Utils;

public class Position
{
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }

    public override string ToString()
    {
        return $"X: {X} Y: {Y}";
    }

    public bool IsSameAs(Position other)
    {
        return X == other.X && Y == other.Y;
    }

    public bool IsInDistance(Position other, float maxDistance)
    {
        return (Math.Abs(X - other.X) + Math.Abs(Y - other.Y)) <= maxDistance;
    }
}