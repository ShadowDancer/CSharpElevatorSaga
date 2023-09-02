using CSharpElevatorSaga.Model;

namespace CSharpElevatorSaga.Game.Proxy;

public class ElevatorProxy : IElevator
{
    public ElevatorProxy(IFloor startingFloor)
    {
        Floor = startingFloor;
    }

    public IFloor Floor { get; set; }

    public IList<int> DestinationQueue { get; } = new List<int>();

    public List<int> RequestedFloors { get; } = new();

    public ElevatorDirectionIndicatorsProxy DirectionIndicators { get; } = new();

    IDirectionIndicators IElevator.DirectionIndicators => DirectionIndicators;

    IReadOnlyCollection<int> IElevator.RequestedFloors => throw new NotImplementedException();

    public event IElevator.Idle? OnIdle;
    public event IElevator.ButtonPressed? OnButtonPressed;
    public event IElevator.Stopped? OnStopped;

    public void RaiseOnIdle()
    {
        OnIdle?.Invoke();
    }

    public void RaiseOnButtonPressed(int floorNumber)
    {
        OnButtonPressed?.Invoke(floorNumber);
    }

    public void RaiseOnStopped()
    {
        OnStopped?.Invoke();
    }
}

public class ElevatorDirectionIndicatorsProxy : IDirectionIndicators
{
    public bool Up { get; set; }
    public bool Down { get; set; }
}
