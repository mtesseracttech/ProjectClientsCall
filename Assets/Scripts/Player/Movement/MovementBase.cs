using System;
using System.Collections.Generic;
using Assets.Scripts.Player.Movement.States;
using UnityEngine;

namespace Assets.Scripts.Player.Movement
{
    public class MovementBase : MonoBehaviour
    {
        private Dictionary<Type, AbstractMovementState> _movementStates;
        private AbstractMovementState _currentState;

        // Use this for initialization
        void Start ()
        {
            //Setting up the movement States
            _movementStates = new Dictionary<Type, AbstractMovementState>();

<<<<<<< HEAD
	    _movementStates[typeof(ClimbingState)] = new ClimbingState(gameObject);
	    _movementStates[typeof(GlidingState)]  = new GlidingState(gameObject);
	    _movementStates[typeof(HangingState)]  = new HangingState(gameObject);
	    _movementStates[typeof(IdlingState)]   = new IdlingState(gameObject);
	    _movementStates[typeof(JumpingState)]  = new JumpingState(gameObject);
	    _movementStates[typeof(RunningState)]  = new RunningState(gameObject);
=======
            _movementStates[typeof(ClimbingState)] = new ClimbingState(gameObject);
            _movementStates[typeof(GlidingState)] = new GlidingState(gameObject);
            _movementStates[typeof(HangingState)] = new HangingState(gameObject);
            _movementStates[typeof(IdlingState)] = new IdlingState(gameObject);
            _movementStates[typeof(JumpingState)] = new JumpingState(gameObject);
            _movementStates[typeof(RunningState)] = new RunningState(gameObject);
>>>>>>> 86f38e8c3c9512f7ecfd59a03130e0b48e5f8d81

            SetState(typeof(IdlingState));
        }

        // Update is called once per frame
        void Update ()
        {
            _currentState.Update();
        }

        public void SetState(Type newState)
        {
            Debug.Log("Switching movementstate to: " + newState.FullName);
            _currentState = _movementStates[newState];
        }
    }
}
