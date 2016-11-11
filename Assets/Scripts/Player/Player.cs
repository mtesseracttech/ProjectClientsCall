using System;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(Controller2D))]
    public class Player : MonoBehaviour
    {
        private float turnSmoothVelocity;
        public float MoveSpeed = 6;
        public float Gravity;

        public float accelerationTimeAirborne = 0.2f;
        public float accelerationGrounded = 0.1f;

        public float MaxJumpVelocity;
        public float MaxJumpHeight = 4;
        public float TimeToJumpApex = 0.4f;        //time takes to reach the highest jump

        public Animator animator;
        private bool facingRight = true;
        private float velocityXSmoothing;
        private Vector3 _velocity;
        private Controller2D _controller;
        Vector2 directionalInput;
        int wallDirX;
        public GameObject child;

        void Start()
        {
            _controller = GetComponent<Controller2D>();

           

        }

        void Update()
        {
            Gravity = -(2 * MaxJumpHeight) / Mathf.Pow(TimeToJumpApex, 2);
            MaxJumpVelocity = Mathf.Abs(Gravity) * TimeToJumpApex;

            if (_controller.Collisions.above || _controller.Collisions.below)
            {
                _velocity.y = 0;
            }

            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            SetDirectionalInput(input);
            //flipping player on the ground
            float moveDirX = Input.GetAxisRaw("Horizontal");
            // Vector2 inputDir = input.normalized;

            //movement direction
            if (moveDirX != 0 && _controller.Collisions.slopeAngle == 0)
            {
               child.transform.eulerAngles = (moveDirX > 0) ? Vector3.up * 90 : Vector3.up * -90 ;
            }

            //if going up the slope
            if (moveDirX != 0 && _controller.Collisions.climbingSlope)
            {
              //  float angle = Mathf.LerpAngle(0, -_controller.Collisions.slopeAngle, Time.deltaTime*2);
                if (moveDirX < 0)
                 child.transform.eulerAngles = new Vector3(-_controller.Collisions.slopeAngle, -90,0);
                else
                    child.transform.eulerAngles = new Vector3(-_controller.Collisions.slopeAngle, 90, 0);
            }

            //if not moving on the slope
            if (moveDirX == 0.0f && (_controller.Collisions.climbingSlope || _controller.Collisions.descendingSlope ) && _controller.Collisions.below && input.x == 0.0f)
            {
                //animation of climbing
                AnimationStates(false,false,false,false,true);
            }

            //if going down the slope
            if (moveDirX != 0.0f && !_controller.Collisions.climbingSlope && _controller.Collisions.descendingSlope)
            {
              
                if (moveDirX < 0)
                child.transform.eulerAngles = new Vector3(_controller.Collisions.slopeAngle, -90, 0);
                else
                    child.transform.eulerAngles = new Vector3(_controller.Collisions.slopeAngle, 90, 0);
            }

            //animation of running if bellow is true
            if (input.x != 0.0f && _controller.Collisions.below && !Input.GetKeyDown(KeyCode.Space))
            {
                //animations of running
                AnimationStates(false, true, false, false, false);
            }
            else if (!Input.GetKeyDown(KeyCode.Space) && input.x == 0 && _controller.Collisions.below && _controller.Collisions.slopeAngle == 0)
            {
                //animations if not moving on ground. idle
                AnimationStates(true, false, false, false, false);
            }
            
            if (Input.GetKeyDown(KeyCode.Space) && _controller.Collisions.below && !_controller.Collisions.above )//&& input.x == 0)
            {
                //add animation of jump
                _velocity.y = MaxJumpVelocity; 
                AnimationStates(false, false, true, false, false);
                
            }

            //gliding animation
            if (_velocity.x != 0 && _velocity.y != 0 && !_controller.Collisions.below && !Input.GetKeyDown(KeyCode.Space))
            {
                Gravity /= 2;
              
               //   _velocity.y = Mathf.Clamp(_velocity.y, maxVerticleSpeed, -maxVerticleSpeed);
                
                AnimationStates(false, false, false, true, false);

            }

            float targerVelocityX = input.x * MoveSpeed;
            _velocity.x = Mathf.SmoothDamp(_velocity.x, targerVelocityX, ref velocityXSmoothing, (_controller.Collisions.below) ? accelerationGrounded : accelerationTimeAirborne);
            _velocity.y += Gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime, directionalInput);
        }

        void AnimationStates(bool idleState, bool runningState, bool jumpingState, bool glidingState, bool idleClimState)
        {
            animator.SetBool("Idle", idleState);
            animator.SetBool("Jumping", jumpingState);
            animator.SetBool("Running", runningState);
            animator.SetBool("Glidin", glidingState);
            animator.SetBool("Idle_Climb", idleClimState);

            /**
            if (idleState)
                print("Idle state");
            else if (runningState)
                print("running state");
            else if (jumpingState)
                print("jumping state");
            else if (glidingState)
                print("gliding State");
            else if (idle_climState)
                print("idle climb State");
            /**/
        }

        public void SetDirectionalInput(Vector2 input)
        {
            directionalInput = input;
        }
    }
}
