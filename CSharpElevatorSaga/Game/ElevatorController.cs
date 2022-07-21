using CSharpElevatorSaga.Game.Model;

namespace CSharpElevatorSaga.Game;

public class ElevatorController
{
    private readonly Elevator _elevator;
    private readonly Floor _floor;
    private readonly Building _building;
    private readonly IScoring _scoring;

    private ElevatorControls Controls => _elevator.Controls;

    public ElevatorController(Elevator elevator, Floor floor, Building building, IScoring scoring)
    {
        _elevator = elevator;
        _floor = floor;
        _building = building;
        _scoring = scoring;
    }

    internal void Tick()
    {
        Controls.ActivityTicks++;
        switch (_elevator.Controls.State)
        {
            case ElevatorState.Moving:
                HandleMoving();
                break;
            case ElevatorState.Idle:
                HandleIdle();
                break;
        }
    }

    private void HandleMoving()
    {
        bool noDestination = Controls.DestinationQueue.Count == 0;
        if (noDestination)
        {
            Controls.DestinationQueue.Add(_elevator.Floor);
        }

        int directionVector = Math.Sign(Controls.DestinationQueue[0] - _elevator.Floor);
        int oldDirection = Math.Sign(Controls.TargetFloor - _elevator.Floor);
        Controls.TargetFloor = Controls.DestinationQueue[0];

        if (oldDirection != 0 && directionVector != 0 && directionVector != oldDirection)
        {
            // Because activity ticks always go up, we need to recompute it when changing direction
            Controls.ActivityTicks = _building.Properties.TicksPerStory - Controls.ActivityTicks;
        }

        bool reachedFloor = Controls.ActivityTicks >= _building.Properties.TicksPerStory;
        if (!reachedFloor)
        {
            return;
        }

        Controls.ActivityTicks = 0;

        var nextFloorNumber = Math.Clamp(_elevator.Floor + directionVector, 0, _building.Floors.Length);
        var newFloor = _building.Floors[nextFloorNumber];
        _elevator.Proxy.Floor = newFloor.Proxy;

        if (_elevator.Floor == Controls.TargetFloor)
        {
            var leavingPassengers = _elevator.Cargo.RemovePassengersGoingTo(_elevator.Floor);

            newFloor.OutputLine.AddRange(leavingPassengers);
            _scoring.PersonTransported(leavingPassengers.Count);

            while (_elevator.Controls.DestinationQueue.Remove(_elevator.Floor))
            {
            }
            _scoring.ElevatorMoved();
            Transition(ElevatorState.Idle);

            _elevator.Proxy.RaiseOnStopped();
        }
    }

    private void HandleIdle()
    {
        while (_floor.WaitingLine.Any() && _elevator.Cargo.CanTakePassenger)
        {
            var newPassenger = _floor.WaitingLine.Dequeue();
            _elevator.Cargo.TakePassenger(newPassenger);

            PressButtonInElevator(newPassenger.TargetFloor);
        }

        bool minimumStayTimeElapsing = Controls.ActivityTicks < _building.Properties.MinimumStayTicks;
        if (minimumStayTimeElapsing)
        {
            return;
        }

        // Remove current floor so we do not start to move on current floor
        while (Controls.DestinationQueue.Count > 0 && Controls.DestinationQueue.First() == _elevator.Floor)
        {
            Controls.DestinationQueue.RemoveAt(0);
        }

        bool hasDestination = Controls.DestinationQueue.Count > 0;
        if (hasDestination)
        {
            Transition(ElevatorState.Moving);
            return;
        }

        _elevator.Proxy.RaiseOnIdle();
    }

    private void PressButtonInElevator(int targetFloor)
    {
        if (_elevator.RequestedFloors.Contains(targetFloor))
        {
            return;
        }

        _elevator.RequestedFloors.Add(targetFloor);
        _elevator.Proxy.RaiseOnButtonPressed(targetFloor);
    }

    private void Transition(ElevatorState newState)
    {
        Controls.State = newState;
        Controls.ActivityTicks = 0;
    }
}
