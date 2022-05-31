namespace CSharpElevatorSaga.Model;

public interface IFloor
{
    public int Number { get; }

    public IFloorButtons Buttons { get; }

    public event ButtonPressed? OnUpButtonPressed;

    public event ButtonPressed? OnDownButtonPressed;

    public delegate void ButtonPressed();
}
