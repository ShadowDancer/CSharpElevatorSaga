using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class ElevatorCargo
{
    private readonly ElevatorProxy _proxy;
    private readonly Elevator _elevator;
    private readonly Building _building;
    private const float PersonSpacing = 15f;

    public ElevatorCargo(ElevatorProxy proxy, Elevator elevator, Building building)
    {
        _proxy = proxy;
        _elevator = elevator;
        _building = building;
    }
    
    public int MaxPassengers { get; set; } = 3;

    public IEnumerable<Person> Passengers => _building.People.Where(p => p.State == PersonState.InElevator);

    public bool CanTakePassenger => Passengers.Count() < MaxPassengers;

    public void TakePassenger(Person person)
    {
        person.State = PersonState.InElevator;
        UpdatePassengerPositions();
    }

    public void UpdatePassengerPositions()
    {
        var passengers = Passengers.ToList();
        for (int i = 0; i < passengers.Count; i++)
        {
            var xPos = _elevator.Position.X + (i * PersonSpacing);
            passengers[i].Position = new Position(xPos, _elevator.Position.Y);
        }
    }
}
