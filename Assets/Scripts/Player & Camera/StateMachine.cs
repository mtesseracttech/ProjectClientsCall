using System;
using System.Collections.Generic;

public enum SquirrelState
{
    idling,
    running,
    jumping,
    climbing,
    flying,
    landing,
}

public enum Orientation //for later state
{
    left,
    right
}

public enum Key
{
    up,
    down,
}

namespace StateMachine
{
    public class SquirrelMachine
    {
        class StateTransition
        {
            readonly SquirrelState CurrentState;
            readonly Key Command;

            public StateTransition(SquirrelState currentState, Key command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
            }
        }

        Dictionary<StateTransition, SquirrelState> transitions;
        public SquirrelState CurrentState { get; private set; }
        private SquirrelState OldState;

        public SquirrelMachine(SquirrelState startState)
        {
            CurrentState = startState;
            OldState = CurrentState;

            transitions = new Dictionary<StateTransition, SquirrelState>
            {
                { new StateTransition(SquirrelState.idling, Key.up), SquirrelState.jumping },
                { new StateTransition(SquirrelState.running, Key.up), SquirrelState.jumping }
            };
        }

        public SquirrelState GetNext(Key command)
        {
            StateTransition transition = new StateTransition(CurrentState, command);
            SquirrelState nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
            return nextState;
        }

        public SquirrelState MoveNext(Key command)
        {
            CurrentState = GetNext(command);
            return CurrentState;
        }

        public bool HasChanged()
        {
            if (CurrentState != OldState)
            {
                OldState = CurrentState;
                return true;
            }
            return false;
        }
    }
}