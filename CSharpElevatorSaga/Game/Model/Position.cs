namespace CSharpElevatorSaga.Game.Model;

public struct Position
{
    public Position(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float X { get; set; }
    public float Y { get; set; }

    public static Position Zero => new Position(0, 0);
} 