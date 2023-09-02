using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class Floor
{
    public FloorProxy Proxy { get; }

    public Floor(FloorProxy floorProxy)
    {
        Proxy = floorProxy;
    }

    public FloorButtons Buttons { get; } = new();

    public Queue<Person> WaitingLine { get; } = new();

    public List<Person> OutputLine { get; } = new();
}
