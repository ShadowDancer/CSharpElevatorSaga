using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class Floor
{
    private readonly IEnumerable<Person> _peopleOnFloor;
    
    public FloorProxy Proxy { get; }
    
    public float Y { get; }
    
    public float WaitingLineX { get; } = 50f;
    
    public float OutputLineX { get; } = 300f;
    
    public float PersonSpacing { get; } = 25f;

    public Floor(FloorProxy floorProxy, IEnumerable<Person> peopleOnFloor)
    {
        Proxy = floorProxy;
        Y = floorProxy.Number * 100f;
        _peopleOnFloor = peopleOnFloor;
    }

    public FloorButtons Buttons { get; } = new();

    public IEnumerable<Person> WaitingLine => _peopleOnFloor.Where(p => p.State == PersonState.Waiting);

    public IEnumerable<Person> OutputLine => _peopleOnFloor.Where(p => p.State == PersonState.Exited);
    
    public void AddPersonToWaitingLine(Person person)
    {
        person.State = PersonState.Waiting;
        person.CurrentFloor = Proxy.Number;
        UpdateWaitingLinePositions();
    }
    
    public void UpdateWaitingLinePositions()
    {
        var waitingLine = WaitingLine.ToList();
        for (int i = 0; i < waitingLine.Count; i++){
            waitingLine[i].Position = new Position(WaitingLineX + (i * PersonSpacing), Y);
        }        
    }

    public void AddPersonToOutputLine(Person person)
    {
        person.State = PersonState.Exited;
        person.CurrentFloor = Proxy.Number;
        person.Position = new Position(OutputLineX + (OutputLine.Count() * PersonSpacing), Y);
    }
}
