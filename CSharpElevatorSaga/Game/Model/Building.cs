using System.Collections.Immutable;
using System.Collections.Generic;

namespace CSharpElevatorSaga.Game.Model;

public class Building
{
    private readonly IScoring _scoring;

    private int NextPersonId { get; set; } = 0;

    public Building(int stories, int elevators, IScoring scoring)
    {
        _scoring = scoring;
        Floors = Enumerable.Range(0, stories).Select(floorNumber => new Floor(new Proxy.FloorProxy(floorNumber), People.Where(p => p.CurrentFloor == floorNumber))).ToImmutableArray();
        Elevators = Enumerable.Range(0, elevators).Select(_ => new Elevator(new Proxy.ElevatorProxy(Floors[0].Proxy), this)).ToImmutableArray();
    }

    public ImmutableArray<Elevator> Elevators { get; }

    public ImmutableArray<Floor> Floors { get; }

    public BuildingProperties Properties { get; } = new();

    public List<Person> People { get; } = new();

    public Person CreatePerson(int floor){
        var person = new Person(NextPersonId++, floor); 
        People.Add(person);
        Floors[floor].AddPersonToWaitingLine(person);
        return person;
    }

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
        new ElevatorController(elevator, elevatorFloor, this, _scoring).Tick();
    }
}
