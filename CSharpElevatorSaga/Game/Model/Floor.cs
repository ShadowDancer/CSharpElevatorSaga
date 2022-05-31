using CSharpElevatorSaga.Proxy;

namespace CSharpElevatorSaga.Implementation.Model;

public class Floor
{
    public FloorProxy Proxy { get; }

    public Floor(FloorProxy floorProxy)
    {
        Proxy = floorProxy;
        for (int i = 0; i < 5; i++)
        {
            WaitingLine.Enqueue(new Person(0));
        }
    }

    public FloorButtons Buttons { get; } = new();

    public Queue<Person> WaitingLine { get; } = new();

    public List<Person> OutputLine { get; } = new();
}
