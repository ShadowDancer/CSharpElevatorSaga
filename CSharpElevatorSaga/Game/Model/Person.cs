namespace CSharpElevatorSaga.Game.Model;

public class Person
{
    public Person(int targetFloor)
    {
        TargetFloor = targetFloor;
    }

    public int TargetFloor { get; }
}
