using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class ElevatorCargo
{
    private ElevatorProxy _proxy;

    public ElevatorCargo(ElevatorProxy proxy)
    {
        _proxy = proxy;
    }

    public int MaxPassengers { get; set; } = 3;

    public List<Person> Passengers { get; } = new();

    public bool CanTakePassenger => Passengers.Count < MaxPassengers;

    public void TakePassenger(Person person)
    {
        Passengers.Add(person);
    }

    internal List<Person> RemovePassengersGoingTo(int floor)
    {
        var leavingPassengers = Passengers.Where(n => n.TargetFloor == floor).ToList();

        foreach (var passenger in leavingPassengers)
        {
            Passengers.Remove(passenger);
        }

        return leavingPassengers;
    }
}
