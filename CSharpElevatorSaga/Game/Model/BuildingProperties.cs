namespace CSharpElevatorSaga.Implementation.Model;

public class BuildingProperties
{
    public int TicksPerStory { get; } = 40;

    public int TicksToMoveDoors { get; } = 2;

    public int MinimumStayTicks { get; } = 20;
}
