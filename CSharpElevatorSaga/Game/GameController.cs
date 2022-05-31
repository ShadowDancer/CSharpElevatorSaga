using CSharpElevatorSaga.Implementation.Model;
using CSharpElevatorSaga.Model;

namespace CSharpElevatorSaga.Implementation;

public class GameController
{
    private int _gameSpeed = 1;

    public GameController()
    {
        Building = new Building(3, 1);
        Timer = new Timer(GameTickHandler, 0, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(16));
    }

    public delegate void OnGameTickCompleted();

    public event OnGameTickCompleted? OnTickCompleted;

    private void GameTickHandler(object? state)
    {
        for (int i = 0; i < _gameSpeed; i++)
        {
            Building.Tick();
        }

        OnTickCompleted?.Invoke();
    }

    public int Floors { get; } = 3;

    public int Elevators { get; } = 1;

    /// <summary>
    /// Number of game ticks per frame
    /// </summary>
    public int GameSpeed
    {
        get => _gameSpeed;
        set => Math.Clamp(value, 0, 16);
    }

    public Building Building { get; set; }

    private Timer Timer { get; }

    public void RunProgram(ProgramEntryPoint.RunProgram program)
    {
        Building = new Building(Floors, Elevators);
        var elevators = Building.Elevators.Select(elevator => (IElevator)elevator.Proxy).ToArray();
        var floors = Building.Floors.Select(floor => (IFloor)floor.Proxy).ToArray();
        program?.Invoke(elevators, floors);
    }
}
