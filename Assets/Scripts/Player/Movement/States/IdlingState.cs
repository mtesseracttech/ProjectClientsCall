using Assets.Scripts.Player.Movement.States;
using UnityEngine;

public class IdlingState : AbstractMovementState
{
    public IdlingState(GameObject agent) : base(agent){}

    public override void Update()
    {
        Debug.Log("Idling atm");
        Debug.DrawLine(AgentTransform.position, AgentTransform.position + AgentTransform.forward * 3, Color.cyan);
    }
}
