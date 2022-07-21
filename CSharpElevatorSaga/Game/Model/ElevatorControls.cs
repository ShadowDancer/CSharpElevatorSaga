using CSharpElevatorSaga.Game.Proxy;

namespace CSharpElevatorSaga.Game.Model;

public class ElevatorControls
{
    private ElevatorProxy _proxy;

    public ElevatorControls(ElevatorProxy proxy)
    {
        _proxy = proxy;
    }

    public IList<int> DestinationQueue => _proxy.DestinationQueue;

    public int ActivityTicks { get; set; }

    public int TargetFloor { get; set; }

    public ElevatorState State { get; set; }
}