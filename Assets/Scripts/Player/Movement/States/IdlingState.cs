using Assets.Scripts.Player.Movement.States;
using UnityEngine;

public class IdlingState : AbstractMovementState
{
    public IdlingState(GameObject agent) : base(agent){}

<<<<<<< HEAD
    public override void Update(){}
=======
    public override void Update()
    {
        Debug.Log("Idling atm");
        Debug.DrawLine(AgentTransform.position, AgentTransform.position + AgentTransform.forward * 3, Color.cyan);
    }
>>>>>>> 86f38e8c3c9512f7ecfd59a03130e0b48e5f8d81
}
