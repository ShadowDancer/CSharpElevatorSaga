using CSharpElevatorSaga.Proxy;

namespace CSharpElevatorSaga.Implementation.Model;

public class Elevator
{

    public Elevator(ElevatorProxy proxy)
    {
        Proxy = proxy;
        Controls = new(proxy);
        DirectionIndicators = proxy.DirectionIndicators;
        Cargo = new(proxy);
    }

    public int Floor => Proxy.Floor.Number;

    public ElevatorProxy Proxy { get; }

    public ElevatorDirectionIndicatorsProxy DirectionIndicators { get; }

    public ElevatorControls Controls { get; }

    public ElevatorCargo Cargo { get; }

    public List<int> RequestedFloors => Proxy.RequestedFloors;
}
