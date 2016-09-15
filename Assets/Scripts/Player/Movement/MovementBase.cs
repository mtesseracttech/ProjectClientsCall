﻿using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Player.Movement.States;

public class MovementBase : MonoBehaviour
{
    private Dictionary<Type, AbstractMovementState> _movementStates;
    private AbstractMovementState _currentState;

    // Use this for initialization
	void Start ()
	{
	    _movementStates = new Dictionary<Type, AbstractMovementState>();

	    _movementStates[typeof(ClimbingState)] = new ClimbingState(this);
	    _movementStates[typeof(GlidingState)]  = new GlidingState(this);
	    _movementStates[typeof(HangingState)]  = new HangingState(this);
	    _movementStates[typeof(IdlingState)]   = new IdlingState(this);
	    _movementStates[typeof(JumpingState)]  = new JumpingState(this);
	    _movementStates[typeof(RunningState)]  = new RunningState(this);

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