using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class Floor
{
    private readonly IEnumerable<Person> _peopleOnFloor;
    
    public FloorProxy Proxy { get; }
    
    public float Y { get; }
    
    
    private BuildingProperties _buildingProperties;

    public Floor(FloorProxy floorProxy, IEnumerable<Person> peopleOnFloor, BuildingProperties buildingProperties)
    {
        Proxy = floorProxy;
        Y = floorProxy.Number * buildingProperties.FloorHeight;
        _peopleOnFloor = peopleOnFloor;
        _buildingProperties = buildingProperties;
    }

    public FloorButtons Buttons { get; } = new();

    public IEnumerable<Person> ElevatorQueue => _peopleOnFloor.Where(p => p.State == PersonState.Waiting);

    public IEnumerable<Person> Exiting => _peopleOnFloor.Where(p => p.State == PersonState.Exiting);
    
    public void AddPersonToWaitingLine(Person person)
    {
        person.State = PersonState.Waiting;
        person.CurrentFloor = Proxy.Number;
        person.SetPosition(QueuePositionToWorldPosition(ElevatorQueue.Count()-1), _buildingProperties.MinimumStayTicks);
    }
    
    public void UpdateWaitingLinePositions()
    {
        var waitingLine = ElevatorQueue.ToList();
        for (int i = 0; i < waitingLine.Count; i++)
        {
            waitingLine[i].SetPosition(QueuePositionToWorldPosition(i), _buildingProperties.QueueRepositionTicks);
        }
    }

    private Position QueuePositionToWorldPosition(int i)
    {
        return new Position(_buildingProperties.ElevatorQueueStartX - (i * _buildingProperties.PersonWidth), Y);
    }

    public void AddPersonToOutputLine(Person person)
    {
        person.State = PersonState.Exiting;
        person.CurrentFloor = Proxy.Number;
        person.SetPosition(new Position(_buildingProperties.OutputLineStart + (Exiting.Count() * _buildingProperties.PersonWidth), Y), _buildingProperties.MinimumStayTicks);
    }
}
