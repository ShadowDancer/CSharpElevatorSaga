using CSharpElevatorSaga.Model;

namespace CSharpElevatorSaga.Game.Proxy;

public class FloorProxy : IFloor
{
    public FloorProxy(int level)
    {
        Number = level;
    }

    public int Number { get; }

    public FloorButtonsProxy Buttons { get; } = new();

    IFloorButtons IFloor.Buttons => Buttons;

    public event IFloor.ButtonPressed? OnUpButtonPressed;

    public event IFloor.ButtonPressed? OnDownButtonPressed;

    private void RaiseOnUpButtonPressed()
    {
        OnUpButtonPressed?.Invoke();
    }

    public void RaiseOnDownButtonPressed()
    {
        OnDownButtonPressed?.Invoke();
    }
}
