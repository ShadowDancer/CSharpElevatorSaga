namespace CSharpElevatorSaga.Model;

public interface IElevator
{
    /// <summary>
    /// Returns current floor elevator is at
    /// </summary>
    IFloor Floor { get; }

    /// <summary>
    /// List of floors, which elevator will visit
    /// </summary>
    public IList<int> DestinationQueue { get; }

    /// <summary>
    /// List of buttons pressed by people in elevator
    /// 
    /// Button is removed from list when elevator visits floor and person leaves
    /// </summary>
    public IReadOnlyCollection<int> RequestedFloors { get; }

    /// <summary>
    /// People will get into elevator only if elevator has correct direction indicator lit
    /// </summary>
    public IDirectionIndicators DirectionIndicators { get; }

    /// <summary>
    /// Elevator is stopped at floor and has no destinations
    /// </summary>
    public event Idle? OnIdle;

    /// <summary>
    /// Button was pressed inside elevator
    /// </summary>
    public event ButtonPressed? OnButtonPressed;

    /// <summary>
    /// Elevator reached destination floor
    /// </summary>
    public event Stopped? OnStopped;

    public delegate void Idle();

    public delegate void ButtonPressed(int floorNumber);

    public delegate void Stopped();
}
