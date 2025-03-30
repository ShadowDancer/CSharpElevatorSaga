using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class Elevator
{
    public Elevator(ElevatorProxy proxy, Building building, int position)
    {
        Proxy = proxy;
        Controls = new ElevatorControls(proxy);
        DirectionIndicators = proxy.DirectionIndicators;
        var properties = building.Properties;
        Position = new Position(properties.ElevatorQueueStartX + properties.QueueSpacing + position * (properties.ElevatorWidth + properties.ElevatorSpacing), proxy.Floor.Number * properties.FloorHeight);
        Cargo = new ElevatorCargo(proxy, this, properties);
    }

    public int Floor => Proxy.Floor.Number;
    
    public Position Position { get; set; }
    
    public float Height { get; } = 100f;

    public ElevatorProxy Proxy { get; }

    public ElevatorDirectionIndicatorsProxy DirectionIndicators { get; }

    public ElevatorControls Controls { get; }

    public ElevatorCargo Cargo { get; }

    public List<int> RequestedFloors => Proxy.RequestedFloors;
}
