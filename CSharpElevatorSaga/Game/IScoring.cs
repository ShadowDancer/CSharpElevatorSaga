namespace CSharpElevatorSaga.Game;

public interface IScoring
{
    public void PersonTransported(int count);

    public void ElevatorMoved();

    public void Tick();
}