namespace Assets.Scripts.Player.Movement.States
{
    public abstract class AbstractMovementState
    {
        protected MovementBase Agent;

        protected AbstractMovementState(MovementBase agent)
        {
            Agent = agent;
        }

        public abstract void Update();
    }
}
