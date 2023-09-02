namespace CSharpElevatorSaga.Game.Model;

public enum PersonState
{
    Waiting,
    InElevator,
    Exited
}

public class Person
{
    public Person(int id, int targetFloor)
    {
        Id = id;
        TargetFloor = targetFloor;
        Position = Position.Zero;
        State = PersonState.Waiting;
        CurrentFloor = 0;
    }

    public int Id { get; }

    public int TargetFloor { get; }
    
    public Position Position { get; set; }

    public PersonState State { get; set; }

    public int CurrentFloor { get; set; }
}
