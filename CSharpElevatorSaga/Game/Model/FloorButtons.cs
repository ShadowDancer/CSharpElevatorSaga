using CSharpElevatorSaga.Model;

namespace CSharpElevatorSaga.Game.Model;

public class FloorButtons : IFloorButtons
{
    public bool IsUpPressed { get; set; }

    public bool IsDownPressed { get; set; }
}
