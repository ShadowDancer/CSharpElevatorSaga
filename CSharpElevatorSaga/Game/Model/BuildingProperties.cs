namespace CSharpElevatorSaga.Game.Model;

public class BuildingProperties
{
    public int TicksPerStory { get; } = 40;

    public int TicksToMoveDoors { get; } = 2;

    public int MinimumStayTicks { get; } = 20;
    public int QueueRepositionTicks { get; } = 10;
    
    public int FloorHeight { get; } = 100;

    public int PersonWidth { get; } = 10;
    public int ElevatorQueueStartX { get; internal set; } = 300;
    public int OutputLineStart { get; internal set; } = 600;
    public int ElevatorWidth { get; internal set; } = 50;
    /// <summary>
    /// Distance between elevators
    /// </summary>
    public int ElevatorSpacing { get; internal set; } = 25;
    /// <summary>
    /// Distance between waiting line and elevator
    /// </summary>
    public int QueueSpacing { get; internal set; } = 100;
    
}
