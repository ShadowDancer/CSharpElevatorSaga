namespace CSharpElevatorSaga.Model;

public static class ProgramEntryPoint
{
    public delegate void RunProgram(IElevator[] elevators, IFloor[] floors);
}