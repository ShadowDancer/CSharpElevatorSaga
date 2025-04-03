namespace CSharpElevatorSaga.Game.Model;

public enum PersonState
{
    Waiting,
    InElevator,
    Exiting
}

public class Person
{
    private Position _position;
    private Position _targetPosition;
    private int _transitionTimeRemaining;
    private int _totalTransitionTime;

    public Person(int id, int targetFloor, int width)
    {
        Id = id;
        TargetFloor = targetFloor;
        _position = Position.Zero;
        _targetPosition = Position.Zero;
        Width = width;
    }

    public int Id { get; }
    public int TargetFloor { get; }
    public int CurrentFloor { get; set; }
    public PersonState State { get; set; } = PersonState.Waiting;
    public Position Position => _position;

    public int Width { get; }
    public Position TargetPosition => _targetPosition;

    public void SetPosition(Position newPosition, int transitionTime)
    {
        _targetPosition = newPosition;
        if (transitionTime == 0)
        {
            _position = newPosition;
            _transitionTimeRemaining = 0;
            _totalTransitionTime = 0;
        }
        else
        {
            _transitionTimeRemaining = transitionTime;
            _totalTransitionTime = transitionTime;
        }
    }

    public void Tick()
    {
        if (_transitionTimeRemaining > 0)
        {
            float progress = 1f - (_transitionTimeRemaining / (float)_totalTransitionTime);
            _position = new Position(
                _position.X + (_targetPosition.X - _position.X) * progress,
                _position.Y + (_targetPosition.Y - _position.Y) * progress
            );
            _transitionTimeRemaining--;
            
            if (_transitionTimeRemaining == 0)
            {
                _position = _targetPosition;
            }
        }
    }
}
