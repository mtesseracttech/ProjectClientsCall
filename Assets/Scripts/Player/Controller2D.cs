using UnityEngine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(BoxCollider))]
    public class Controller2D : MonoBehaviour
    {

        public LayerMask CollisionMask;
        public CollisionInfo Collisions;
        public int HorizontalRayCount = 5;
        public int VerticalRayCount = 5;
        public bool onSlope;
        public bool SlopeOnLeft;
        public bool SlopeOnRight;

        public float maxClimbAngle = 90;
        public float maxDescebdAbgle = 90;
       
        private const float dstBetweenRays = .25f;
        private const float SkinWidth = .015f;
        private float _horizontalRaySpacing;
        private float _verticalRaySpacing;

        private BoxCollider _collider;
        private RaycastOrigins _raycastOrigins;
        
        void Start()
        {
            _collider = GetComponent<BoxCollider>();
            CalculateRaySpacing();
            onSlope = false;
        }

        public void Move(Vector3 velocity)
        {
            UpdateRaycastOrigins();
            Collisions.Reset();
            Collisions.velocityOld = velocity;
            
            if (velocity.y < 0)
            {
                DescendSlope(ref velocity);
            }
            if (velocity.x != 0)
            {
                onSlope = false;
                HorizontalCollisions(ref velocity);
            }
            if (velocity.y != 0)
            {
                VerticalCollisions(ref velocity);
            }
           
            transform.Translate(velocity,Space.World);
            //You would then set it's rotation on the z axis to collisions.slopeAngle at the end of the Move() method.﻿
           // transform.Rotate(0, 0, Collisions.slopeAngle);
        }

        void HorizontalCollisions(ref Vector3 velocity)
        {
            float directionX = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

            for (int i = 0; i < HorizontalRayCount; i++)
            {
                Vector3 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight;
                rayOrigin += Vector3.up * (_horizontalRaySpacing * i);
                RaycastHit hit;

                
                Debug.DrawRay(rayOrigin, Vector3.right * directionX ,Color.blue);

                if (Physics.Raycast(rayOrigin, Vector3.right * directionX,out hit, rayLength, CollisionMask))
                {
                    float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                    if (i == 0 && slopeAngle <= maxClimbAngle)
                    {
                        if (Collisions.descendingSlope)
                        {
                            Collisions.descendingSlope = false;
                            velocity = Collisions.velocityOld;
                        }

                        float distanceToSlopeStart = 0;
                        if (slopeAngle != Collisions.slopeAngleOld)
                        {
                            distanceToSlopeStart = hit.distance - SkinWidth;
                            velocity.x -= distanceToSlopeStart*directionX;
                        }

                        ClimbSlope(ref velocity, slopeAngle);
                        velocity.x += distanceToSlopeStart*directionX;
                    }

                    if (!Collisions.climbingSlope || slopeAngle > maxClimbAngle)
                    {
                        velocity.x = (hit.distance - SkinWidth) * directionX;
                        rayLength = hit.distance;

                        if (Collisions.climbingSlope)
                        {
                            velocity.y = Mathf.Tan(Collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x);
                        }

                        Collisions.left = directionX == -1;
                        Collisions.right = directionX == 1;
                    }
                }
            }
        }

        void ClimbSlope(ref Vector3 pVelocity, float pSloapAngle)
        {
            float moveDistance = Mathf.Abs(pVelocity.x);
            float climbVelocityY = Mathf.Sin(pSloapAngle * Mathf.Deg2Rad) * moveDistance;

            if (pVelocity.y <= climbVelocityY)
            {
                pVelocity.y = climbVelocityY;
                pVelocity.x = Mathf.Cos(pSloapAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(pVelocity.x);
                Collisions.below = true; //to make jump 
                Collisions.climbingSlope = true;
                Collisions.slopeAngle = pSloapAngle;
            }
        }

        void DescendSlope(ref Vector3 pVelocity)
        {
            
            float directionX = Mathf.Sign(pVelocity.x);
            Vector2 rayOrigin = (directionX == -1) ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, -Vector2.up,out hit, Mathf.Infinity, CollisionMask))
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxDescebdAbgle)
                {
                   if( Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - SkinWidth <= Mathf.Tan(slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(pVelocity.x))
                        {
                            float moveDistance = Mathf.Abs(pVelocity.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle*Mathf.Deg2Rad)*moveDistance;
                            pVelocity.x = Mathf.Cos(slopeAngle*Mathf.Deg2Rad)*moveDistance*Mathf.Sign(pVelocity.x);
                            pVelocity.y -= descendVelocityY;

                            Collisions.slopeAngle = slopeAngle;
                            Collisions.descendingSlope = true;
                            Collisions.below = true;
                        }
                    }
                }
            }
        }

        void VerticalCollisions(ref Vector3 velocity)
        {
            float directionY = Mathf.Sign(velocity.y);
            float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

            for (int i = 0; i < VerticalRayCount; i++)
            {
                Vector3 rayOrigin = (directionY == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.topLeft;
                rayOrigin += Vector3.right * (_verticalRaySpacing * i + velocity.x);
                // RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, CollisionMask);
                RaycastHit hit;
                Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

                if ( Physics.Raycast(rayOrigin, Vector3.up * directionY * rayLength, out hit, rayLength, CollisionMask))
                {
                    velocity.y = (hit.distance - SkinWidth) * directionY;
                    rayLength = hit.distance;

                    if (Collisions.climbingSlope)
                    {
                        velocity.x = velocity.y/Mathf.Tan(Collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Sign(velocity.x);
                    }
                    Collisions.below = directionY == -1;
                    Collisions.above = directionY == 1;
                }
            }

            if (Collisions.climbingSlope)
            {
                float directionX = Mathf.Sign(velocity.x);
                rayLength = Mathf.Abs(velocity.x) + SkinWidth;
                Vector2 rayOrigin = ((directionX == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins.bottomRight) +
                                    Vector2.up*velocity.y;
                RaycastHit hit;
                if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit,rayLength, CollisionMask))
                {
                    float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                    if (slopeAngle != Collisions.slopeAngle)
                    {
                        velocity.x = (hit.distance - SkinWidth)*directionX;
                        Collisions.slopeAngle = slopeAngle;
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

            HorizontalRayCount = Mathf.RoundToInt(boundsHeight/dstBetweenRays);
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
            public Vector3 velocityOld;


            public void Reset()
            {
                above = below = false;
                left = right = false;
                climbingSlope = false;
                descendingSlope = false;
                slopeAngleOld = slopeAngle;
                slopeAngle = 0;
            }
        }

    }   
}
