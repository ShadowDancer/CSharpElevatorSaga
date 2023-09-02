using CSharpElevatorSaga.Game.Model;

namespace CSharpElevatorSaga.Game;

public class UniformPeopleGenerator : IPeopleGenerator
{
    private const int MaxPeoplePerFloor = 5;
    private int _floorNumber;
    private int _ticks;
    private readonly int _ticksPerPerson;

    public UniformPeopleGenerator(int ticksPerPerson)
    {
        _ticksPerPerson = ticksPerPerson;
    }

    public void Tick(Building building)
    {
        _ticks++;
        if (_ticks > _ticksPerPerson)
        {
            _ticks = 0;
            GeneratePerson(building);
        }
    }

    private void GeneratePerson(Building building)
    {
        FindEmptyFloor(building);

        if (building.Floors[_floorNumber].WaitingLine.Count >= MaxPeoplePerFloor)
        {
            return;
        }
        
        var targetFloor = new Random().Next(0, building.Floors.Length - 1);
        if (targetFloor >= _floorNumber)
        {
            targetFloor++;
        }

        var newPassenger = new Person(targetFloor);
        building.Floors[_floorNumber].WaitingLine.Enqueue(newPassenger);

        NextFloor(building);
    }

    private void FindEmptyFloor(Building building)
    {
        int attempts = 0;
        while (building.Floors[_floorNumber].WaitingLine.Count > MaxPeoplePerFloor)
        {
            NextFloor(building);
            attempts++;
            if (attempts > building.Floors.Length)
            {
                break;
            }
        }
    }

    private void NextFloor(Building building)
    {
        _floorNumber++;
        if (_floorNumber >= building.Floors.Length)
        {
            _floorNumber = 0;
        }
    }
}