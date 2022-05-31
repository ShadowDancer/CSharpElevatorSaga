namespace CSharpElevatorSaga.Implementation.Model;

public class Person
{
    public Person(int targetFloor)
    {
        TargetFloor = targetFloor;
    }

    public int TargetFloor { get; }
}
