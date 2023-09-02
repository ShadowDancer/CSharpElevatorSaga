using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class Elevator
{
    public Elevator(ElevatorProxy proxy, Building building)
    {
        Proxy = proxy;
        Controls = new ElevatorControls(proxy);
        DirectionIndicators = proxy.DirectionIndicators;
        Position = new Position(200f, proxy.Floor.Number * 100f);
        Cargo = new ElevatorCargo(proxy, this, building);
    }

    public int Floor => Proxy.Floor.Number;
    
    public Position Position { get; set; }
    
    public float Height { get; } = 100f;

    public ElevatorProxy Proxy { get; }

    public ElevatorDirectionIndicatorsProxy DirectionIndicators { get; }

    public ElevatorControls Controls { get; }

    public ElevatorCargo Cargo { get; }

    public List<int> RequestedFloors => Proxy.RequestedFloors;
    
    public void UpdatePosition(int targetFloor, float progress)
    {
        float sourceY = Floor * 100f;
        int directionVector = Math.Sign(targetFloor - Floor);
        int nextFloor = Floor + directionVector;
        float nextFloorY = nextFloor * 100f;
        Position = new Position(Position.X, sourceY + (nextFloorY - sourceY) * progress);
        
        Cargo.UpdatePassengerPositions();
    }
}
