using CSharpElevatorSaga.Game.Model;
using CSharpElevatorSaga.Model;

namespace CSharpElevatorSaga.Game;

public class GameController
{
    private int _gameSpeed = 1;

    public GameController()
    {
        const int Fps = 60;

        Level = new Level(3, 1, new ScoringStub(), new UniformPeopleGenerator(60));
        Building = new Building(3, 1, Level.Scoring);
        Timer = new Timer(GameTickHandler, 0, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000.0/Fps));
    }

    public delegate void OnGameTickCompleted();

    public event OnGameTickCompleted? OnTickCompleted;

    private void GameTickHandler(object? state)
    {
        for (int i = 0; i < _gameSpeed; i++)
        {
            Building.Tick();
            Level.Scoring.Tick();
            Level.PeopleGenerator.Tick(Building);
        }

        OnTickCompleted?.Invoke();
    }

    /// <summary>
    /// Number of game ticks per frame
    /// </summary>
    public int GameSpeed
    {
        get => _gameSpeed;
        set => Math.Clamp(value, 0, 16);
    }

    public Level Level { get; }

    public Building Building { get; private set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private Timer Timer { get; }

    public void RunProgram(ProgramEntryPoint.RunProgram? program)
    {
        Building = new Building(Level.Stories, Level.Elevators, Level.Scoring);
        var elevators = Building.Elevators.Select(elevator => (IElevator)elevator.Proxy).ToArray();
        var floors = Building.Floors.Select(floor => (IFloor)floor.Proxy).ToArray();
        program?.Invoke(elevators, floors);
    }
}