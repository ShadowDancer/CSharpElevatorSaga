using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class Elevator
{
    public Elevator(ElevatorProxy proxy)
    {
        Proxy = proxy;
        Controls = new ElevatorControls(proxy);
        DirectionIndicators = proxy.DirectionIndicators;
        Cargo = new ElevatorCargo(proxy);
    }

    public int Floor => Proxy.Floor.Number;

    public ElevatorProxy Proxy { get; }

    public ElevatorDirectionIndicatorsProxy DirectionIndicators { get; }

    public ElevatorControls Controls { get; }

    public ElevatorCargo Cargo { get; }

    public List<int> RequestedFloors => Proxy.RequestedFloors;
}
