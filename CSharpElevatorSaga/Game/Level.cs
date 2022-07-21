namespace CSharpElevatorSaga.Game;

public class Level
{
    public Level(int stories, int elevators, IScoring scoring, IPeopleGenerator peopleGenerator)
    {
        Stories = stories;
        Elevators = elevators;
        Scoring = scoring;
        PeopleGenerator = peopleGenerator;
    }

    public int Stories { get; }

    public int Elevators { get; }

    public IScoring Scoring { get; }

    public IPeopleGenerator PeopleGenerator { get; }
}