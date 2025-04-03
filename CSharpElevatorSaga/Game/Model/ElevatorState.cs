namespace CSharpElevatorSaga.Game.Model;

public enum ElevatorState
{
    Idle,
    Moving,
    /// <summary>
    /// Elevator is stopped at a floor and waiting for passengers to get on or off
    /// </summary>
    Stopped,
}
