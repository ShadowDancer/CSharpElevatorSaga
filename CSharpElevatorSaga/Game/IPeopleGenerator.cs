using CSharpElevatorSaga.Game.Model;

namespace CSharpElevatorSaga.Game;

public interface IPeopleGenerator
{
    public void Tick(Building building);
}
