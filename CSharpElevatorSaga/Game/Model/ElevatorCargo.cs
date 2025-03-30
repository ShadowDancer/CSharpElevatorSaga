using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class ElevatorCargo
{
    private readonly ElevatorProxy _proxy;
    private readonly Elevator _elevator;
    private readonly BuildingProperties _buildingProperties;
    private readonly Person?[] _slots;

    public ElevatorCargo(ElevatorProxy proxy, Elevator elevator, BuildingProperties buildingProperties)
    {
        _proxy = proxy;
        _elevator = elevator;
        _buildingProperties = buildingProperties;
        _slots = new Person?[MaxPassengers];
    }
    
    public int MaxPassengers { get; set; } = 3;

    public IEnumerable<Person> Passengers => _slots.Where(p => p != null)!;

    public bool CanTakePassenger => _slots.Any(slot => slot == null);

    public void TakePassenger(Person person)
    {
        var emptySlot = Array.IndexOf(_slots, null);
        if (emptySlot == -1)
        {
            throw new InvalidOperationException("No empty slots in elevator");
        }
        
        _slots[emptySlot] = person;
        person.State = PersonState.InElevator;
        UpdatePassengerPositions(_buildingProperties.MinimumStayTicks);
    }

    public void RemovePassenger(Person person)
    {
        var slotIndex = Array.IndexOf(_slots, person);
        if (slotIndex != -1)
        {
            _slots[slotIndex] = null;
        }
    }

    public void UpdatePassengerPositions(int ticks = 0)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            var person = _slots[i];
            if (person != null)
            {
                var slotX = _elevator.Position.X + (i * _buildingProperties.PersonWidth);
                person.SetPosition(new Position(slotX, _elevator.Position.Y), ticks);
            }
        }
    }
}
