using System.Collections.Immutable;

namespace CSharpElevatorSaga.Implementation.Model;

public class Building
{
    public Building(int stories, int elevators)
    {
        Floors = Enumerable.Range(0, stories).Select(n => new Floor(new Proxy.FloorProxy(n))).ToImmutableArray();

        Elevators = Enumerable.Range(0, elevators).Select(n => new Elevator(new Proxy.ElevatorProxy(Floors[0].Proxy))).ToImmutableArray();
    }

    public ImmutableArray<Elevator> Elevators { get; }

    public ImmutableArray<Floor> Floors { get; }

    public BuildingProperties Properties { get; } = new BuildingProperties();


    public void Tick()
    {
        foreach (var elevator in Elevators)
        {
            UpdateElevator(elevator);
        }
    }

    private void UpdateElevator(Elevator elevator)
    {
        var elevatorFloor = Floors[elevator.Floor];
        new ElevatorController(elevator, elevatorFloor, this).Tick();
    }
}
