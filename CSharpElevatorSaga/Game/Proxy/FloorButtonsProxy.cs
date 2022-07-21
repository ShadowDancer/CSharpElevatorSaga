using CSharpElevatorSaga.Model;

namespace CSharpElevatorSaga.Game.Proxy;

public class FloorButtonsProxy : IFloorButtons
{
    public bool IsUpPressed { get; set; }

    public bool IsDownPressed { get; set; }
}
