using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(BoxCollider))]
    public class Controller2D : MonoBehaviour
    {
        public LayerMask CollisionMask;
        public CollisionInfo Collisions;
        public int HorizontalRayCount = 5;
        public int VerticalRayCount = 5;
        //   public bool onSlope;
        //   public bool SlopeOnLeft;
        //   public bool SlopeOnRight;

        public float maxClimbAngle = 90;
        public float maxDescebdAbgle = 100;

        [HideInInspector]
        public Vector2 playerInput;

        private const float dstBetweenRays = .15f;
        private const float SkinWidth = .015f;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;

        private BoxCollider _collider;
        private RaycastOrigins _raycastOrigins;

 

        void Start()
        {
            _collider = GetComponent<BoxCollider>();
            CalculateRaySpacing();
            Collisions.faceDir = 1;
            // onSlope = false;
        }

        public void Move(Vector2 moveAmount, bool standingOnPlatform)
        {
            Move(moveAmount, Vector2.zero, standingOnPlatform);
        }

        public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false)
        {
            UpdateRaycastOrigins();
            Collisions.Reset();
            Collisions.moveAmountOld = moveAmount;
            playerInput = input;

            if (moveAmount.y < 0)
            {
                DescendSlope(ref moveAmount);
            }

            if (moveAmount.x != 0)
            {
                Collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
            }

            HorizontalCollisions(ref moveAmount);
            if (moveAmount.y != 0)
            {
                VerticalCollisions(ref moveAmount);
            }

            transform.Translate(moveAmount);

            if (standingOnPlatform)
            {
                Collisions.below = true;
            }
        }

        void HorizontalCollisions(ref Vector2 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

            if (Mathf.Abs(velocity.x) < SkinWidth)
            {
                rayLength = 2 * SkinWidth;
            }

            for (int i = 0; i < HorizontalRayCount; i++)
            {
                /**
                Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);
                /**/
                Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (_horizontalRaySpacing * i);
                RaycastHit hit;

                /**/
                Debug.DrawRay(rayOrigin, Vector2.right * directionX , Color.blue);

                if (Physics.Raycast(rayOrigin, Vector2.right * directionX,out hit, rayLength, CollisionMask))
                {
                    if (hit.distance == 0)
                    {
                        continue;
                    }

                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (i == 0 && slopeAngle <= maxClimbAngle)
                    {
                        if (Collisions.descendingSlope)
                        {
                            Collisions.descendingSlope = false;
                            velocity = Collisions.moveAmountOld;
                        }

                        float distanceToSlopeStart = 0;
                        if (slopeAngle != Collisions.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - SkinWidth;
                            velocity.x -= distanceToSlopeStart * directionX;
                        }

                        ClimbSlope(ref velocity, slopeAngle, hit.normal);
                        velocity.x += distanceToSlopeStart * directionX;
                    }

                    if (!Collisions.climbingSlope || slopeAngle > maxClimbAngle)
                    {
                        velocity.x = (hit.distance - SkinWidth) * directionX;
                        rayLength = hit.distance;

                        if (Collisions.climbingSlope)
                        {
                            velocity.y = Mathf.Tan(Collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                        }

                        Collisions.left = directionX == -1;
                        Collisions.right = directionX == 1;
                    }
                }
            }
        }

        void ClimbSlope(ref Vector2 moveAmount, float pSloapAngle, Vector2 slopeNormal)
        {
            float moveDistance = Mathf.Abs(moveAmount.x);
            float climbVelocityY = Mathf.Sin(pSloapAngle * Mathf.Deg2Rad) * moveDistance;

            if (moveAmount.y <= climbVelocityY)
            {
                moveAmount.y = climbVelocityY;
                moveAmount.x = Mathf.Cos(pSloapAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                Collisions.below = true; //to make jump 
                Collisions.climbingSlope = true;
                Collisions.slopeAngle = pSloapAngle;
                Collisions.slopeNormal = slopeNormal;
            }
        }

        void DescendSlope(ref Vector2 moveAmount)
        {

          //  RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(_raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + SkinWidth, CollisionMask);
           // RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(_raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + SkinWidth, CollisionMask);
           

            float directionX = Mathf.Sign(moveAmount.x);
            /**
                Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, CollisionMask);
            /**/
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
            RaycastHit hit;


            if (Physics.Raycast(rayOrigin, -Vector2.up,out hit, Mathf.Infinity, CollisionMask))
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeAngle != 0 && slopeAngle <= maxDescebdAbgle)
                    {
                        if (Mathf.Sign(hit.normal.x) == directionX)
                        {
                            if (Mathf.Sign(hit.normal.x) == directionX)
                            {
                                if (hit.distance - SkinWidth <=
                                    Mathf.Tan(slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(moveAmount.x))
                                {
                                    float moveDistance = Mathf.Abs(moveAmount.x);
                                    float descendmoveAmountY = Mathf.Sin(slopeAngle*Mathf.Deg2Rad)*moveDistance;
                                    moveAmount.x = Mathf.Cos(slopeAngle*Mathf.Deg2Rad)*moveDistance*
                                                   Mathf.Sign(moveAmount.x);
                                    moveAmount.y -= descendmoveAmountY;

                                    Collisions.slopeAngle = slopeAngle;
                                    Collisions.descendingSlope = true;
                                    Collisions.below = true;
                                    Collisions.slopeNormal = hit.normal;
                                }
                            }
                        }
                     }
              }
        }

        void VerticalCollisions(ref Vector2 velocity)
        {
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

            for (int i = 0; i < VerticalRayCount; i++)
            {
                /**
                Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, CollisionMask);
                /**/

                Vector2 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (_verticalRaySpacing * i + velocity.x);
                RaycastHit hit ;

                Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

                if (Physics.Raycast(rayOrigin, Vector2.up * directionY, out hit,rayLength, CollisionMask))
                {
                    velocity.y = (hit.distance - SkinWidth) * directionY;
                    rayLength = hit.distance;

                    if (Collisions.climbingSlope)
                    {
                        velocity.x = velocity.y / Mathf.Tan(Collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                    }
                    Collisions.below = directionY == -1;
                    Collisions.above = directionY == 1;
                }
            }

            if (Collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(velocity.x);
                rayLength = Mathf.Abs(velocity.x) + SkinWidth;
                /**
                Vector2 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector2.up * velocity.y;
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, CollisionMask);
                /**/
                Vector2 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) + Vector2.up * velocity.y;
                RaycastHit hit ;

                if (Physics.Raycast(rayOrigin, Vector2.right * directionX,out hit, rayLength, CollisionMask))
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (slopeAngle != Collisions.slopeAngle)
                    {
                        velocity.x = (hit.distance - SkinWidth) * directionX;
                        Collisions.slopeAngle = slopeAngle;
                        Collisions.slopeNormal = hit.normal;
                    }
                }
            }
        }

        void UpdateRaycastOrigins()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(SkinWidth * -2);

            _raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            _raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
            _raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
            _raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        void CalculateRaySpacing()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(SkinWidth * -2);

            float boundWidth = bounds.size.x;
            float boundsHeight = bounds.size.y;

            HorizontalRayCount = Mathf.RoundToInt(boundsHeight / dstBetweenRays);
            VerticalRayCount = Mathf.RoundToInt(boundWidth / dstBetweenRays);

            //  HorizontalRayCount = Mathf.Clamp(HorizontalRayCount, 2, int.MaxValue);
            //   VerticalRayCount = Mathf.Clamp(VerticalRayCount, 2, int.MaxValue);

            _horizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
            _verticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
        }

        struct RaycastOrigins
        {
            public Vector2 topLeft, topRight;
            public Vector2 bottomLeft, bottomRight;
        }

        public struct CollisionInfo
        {
            public bool above, below;
            public bool left, right;

            public bool climbingSlope;
            public bool descendingSlope;
            public float slopeAngle, slopeAngleOld;
            public Vector2 slopeNormal;
            public Vector2 moveAmountOld;
            public int faceDir;

            public void Reset()
            {
                above = below = false;
                left = right = false;
                climbingSlope = false;
                descendingSlope = false;

                slopeNormal = Vector2.zero;
                slopeAngleOld = slopeAngle;
                slopeAngle = 0;
            }
        }
    }
}
