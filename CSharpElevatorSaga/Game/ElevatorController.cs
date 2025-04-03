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
            case ElevatorState.Stopped:
                HandleStopped();
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
            Controls.ActivityTicks = _building.Properties.TicksPerStory - Controls.ActivityTicks;
        }

        float progress = (float)Controls.ActivityTicks / _building.Properties.TicksPerStory;
        UpdateElevatorPosition(Controls.TargetFloor, progress);

        bool reachedFloor = Controls.ActivityTicks >= _building.Properties.TicksPerStory;
        if (reachedFloor)
        {
            Transition(ElevatorState.Stopped);
            OnFloorReached(directionVector);
        }
    }

    private void UpdateElevatorPosition(int targetFloor, float progress)
    {
        float sourceY = _elevator.Floor * 100f;
        int directionVector = Math.Sign(targetFloor - _elevator.Floor);
        int nextFloor = _elevator.Floor + directionVector;
        float nextFloorY = nextFloor * 100f;
        _elevator.Position = new Position(_elevator.Position.X, sourceY + (nextFloorY - sourceY) * progress);
        
        _elevator.Cargo.UpdatePassengerPositions();
    }

    private void OnFloorReached(int directionVector)
    {
        Controls.ActivityTicks = 0;

        var nextFloorNumber = Math.Clamp(_elevator.Floor + directionVector, 0, _building.Floors.Length);
        var newFloor = _building.Floors[nextFloorNumber];
        _elevator.Proxy.Floor = newFloor.Proxy;
        _scoring.ElevatorMoved();

        if (_elevator.Floor == Controls.TargetFloor)
        {
            var leavingPassengers = _elevator.Cargo.Passengers.Where(p => p.TargetFloor == _elevator.Floor).ToList();
            foreach (var passenger in leavingPassengers)
            {
                _elevator.Cargo.RemovePassenger(passenger);
                newFloor.AddPersonToOutputLine(passenger);
            }

            _scoring.PersonTransported(leavingPassengers.Count);

            while (_elevator.Controls.DestinationQueue.Remove(_elevator.Floor))
            {
            }

            Transition(ElevatorState.Stopped);
            _elevator.Proxy.RaiseOnStopped();
        }
    }
    
    private void HandleStopped()
    {
        if(TryLoadPassengers())
        {
            return;
        }


        bool minimumStayTimeElapsing = Controls.ActivityTicks < _building.Properties.MinimumStayTicks;
        if (minimumStayTimeElapsing)
        {
            return;
        }
            
        Transition(ElevatorState.Idle);
    }

    private void HandleIdle()
    {
        if(TryLoadPassengers())
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

    private bool TryLoadPassengers()
    {
        bool loadedPassenger = false;
        var waitingPeople = _floor.ElevatorQueue.ToList();
        for (int i = 0; i < waitingPeople.Count && _elevator.Cargo.CanTakePassenger; i++)
        {
            var newPassenger = waitingPeople[i];
            _elevator.Cargo.TakePassenger(newPassenger);

            PressButtonInElevator(newPassenger.TargetFloor);
            loadedPassenger = true;
        }

        if(loadedPassenger)
        {
            _floor.UpdateWaitingLinePositions();
            Transition(ElevatorState.Stopped);
        }


        return loadedPassenger;
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
