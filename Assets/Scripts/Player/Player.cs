using System;
using UnityEngine;

namespace Assets.script
{
    [RequireComponent(typeof(Controller2D))]
    public class Player : MonoBehaviour
    {
        public float MoveSpeed = 6;
        public float Gravity;

        public float accelerationTimeAirborne =0.2f;
        public float accelerationGrounded = 0.1f;

        public float MaxJumpVelocity;
        public float MinJumpVelocity;
        public float MaxJumpHeight = 4;
        public float minnJumpHeight = 1;
        public float TimeToJumpApex = 0.4f;        //time takes to reach the highest jump

        public Animator animator;
        private bool facingRight = true;
        private float velocityXSmoothing;
        private Vector3 _velocity;
        private Controller2D _controller;

        void Start()
        {
            _controller = GetComponent<Controller2D>();

            Gravity = -(MaxJumpHeight)/Mathf.Pow(TimeToJumpApex, 2);
            MaxJumpVelocity = Mathf.Abs(Gravity)*TimeToJumpApex;
            MinJumpVelocity = Mathf.Sqrt(2*Math.Abs(Gravity)*minnJumpHeight);

        }

        void Update()
        {
            if (_controller.Collisions.above || _controller.Collisions.below)
            {
                _velocity.y = 0;
            }

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            //flipping player on the ground
            float moveDirX = Input.GetAxisRaw("Horizontal");
            if (moveDirX != 0 && !_controller.Collisions.climbingSlope)
            {
                transform.eulerAngles = (moveDirX > 0) ? Vector3.zero : Vector3.up*180;
            }
                
            //animation of running if bellow is true
            if (input.x != 0 && _controller.Collisions.below && !Input.GetKeyDown(KeyCode.Space))
            {
                //animations of running
                AnimationStates(false,true,false,false,false);
            }
            else if (!Input.GetKeyDown(KeyCode.Space) && input.x == 0 && _controller.Collisions.below)
            {
                //animations if not moving on ground. idle
                AnimationStates(true,false,false,false,false);
            }
           
            if (Input.GetKeyDown(KeyCode.Space) && _controller.Collisions.below)
            {
                //add animation of jump
                AnimationStates(false,false,true,false,false);
                _velocity.y = MaxJumpVelocity;
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                if (_velocity.y > MinJumpVelocity)
                {
                    _velocity.y = MinJumpVelocity;
                }
            }
            //gliding animation
            if (_velocity.x != 0 && _velocity.y != 0 && !_controller.Collisions.below)
            {
                transform.Translate(Vector3.down * _velocity.x * Time.deltaTime);
                print("glide");
                Glidding();
              
            }

            float targerVelocityX = input.x * MoveSpeed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targerVelocityX, ref velocityXSmoothing, (_controller.Collisions.below)?accelerationGrounded:accelerationTimeAirborne);
            _velocity.y += Gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        void AnimationStates(bool idleState, bool runningState,bool jumpingState, bool glidingState, bool idle_climState)
        {
            animator.SetBool("Idle",idleState);
            animator.SetBool("Jumping",jumpingState);
            animator.SetBool("Running",runningState);
            animator.SetBool("Glidin",glidingState);
            animator.SetBool("Idle_Climb",idle_climState);

            if (idleState)
                print("Idle state");
            else if(runningState)
                print("running state");
            else if(jumpingState)
                print("jumping state");
            else if(glidingState)
                print("gliding State");
            else if(idle_climState)
                print("idle climb State");
        }

        void Glidding()
        {
           AnimationStates(false, false, false, true, false);
        }
    }
}
