using UnityEngine;

namespace Assets.Scripts.Player.Movement.States
{
    public abstract class AbstractMovementState
    {
        protected GameObject Agent;
        protected Transform AgentTransform;

        protected AbstractMovementState(GameObject agent)
        {
            Agent = agent;
            AgentTransform = agent.transform;
        }

        public abstract void Update();
    }
}
