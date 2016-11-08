using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider))]
public class ControllerSQ : MonoBehaviour {

	public LayerMask CollisionMask;

	const float SkinWidth = .015f;
	public int HorizontalRayCount = 4;
	public int VerticalRayCount = 4;
    public int zRayCount = 4;

	private float _horizontalRaySpacing;
	private float _verticalRaySpacing;
    private float _zRaySpacing;

	private BoxCollider _collider;
	private RaycastOrigins _raycastOrigins;

	void Start()
    {
		_collider = GetComponent<BoxCollider> ();
		CalculateRaySpacing ();
	}

	public void Move(Vector3 velocity)
    {
		UpdateRaycastOrigins ();

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}
       // if (velocity.z != 0)
     //   {
           ZCollisions(ref velocity);
       // }


	//	transform.Translate (velocity);
	}

	void HorizontalCollisions(ref Vector3 velocity)
    { 
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + SkinWidth;
		
		for (int i = 0; i < HorizontalRayCount; i ++) {
			Vector3 rayOrigin = (directionX == -1)?_raycastOrigins.bottomLeft:_raycastOrigins.bottomRight;
			rayOrigin += Vector3.up * (_horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.right * directionX, rayLength, CollisionMask);

			Debug.DrawRay(rayOrigin, Vector3.right * directionX * rayLength,Color.red);

			if (hit) {
				velocity.x = (hit.distance - SkinWidth) * directionX;
				rayLength = hit.distance;
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity)
    {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + SkinWidth;

		for (int i = 0; i < VerticalRayCount; i ++) {
			Vector3 rayOrigin = (directionY == -1)?_raycastOrigins.bottomLeft:_raycastOrigins.topLeft;
			rayOrigin += Vector3.right * (_verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.up * directionY, rayLength, CollisionMask);

			Debug.DrawRay(rayOrigin, Vector3.up * directionY * rayLength,Color.blue);

			if (hit) {
				velocity.y = (hit.distance - SkinWidth) * directionY;
				rayLength = hit.distance;
			}
		}
	}

    void ZCollisions(ref Vector3 velocity)
    {
        float directionZ = Mathf.Sign(velocity.z);
        float rayLength = Mathf.Abs(velocity.z) + SkinWidth;

        for (int i = 0; i < zRayCount; i++)
        {
            Vector3 rayOrigin = (directionZ == -1) ? _raycastOrigins.bottomLeft : _raycastOrigins._bottomLeft;
            rayOrigin += Vector3.right * (_verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector3.up * directionZ, rayLength, CollisionMask);

            Debug.DrawRay(rayOrigin, Vector3.up * directionZ * rayLength, Color.green);
            if (hit)
            {
                velocity.z = (hit.distance - SkinWidth) * directionZ;
                rayLength = hit.distance;
            }
        }
    }

	void UpdateRaycastOrigins()
    {
		Bounds bounds = _collider.bounds;
		bounds.Expand (SkinWidth * -2);

		_raycastOrigins.bottomLeft = new Vector3 (bounds.min.x, bounds.min.y, bounds.min.z);
		_raycastOrigins.bottomRight = new Vector3 (bounds.max.x, bounds.min.y, bounds.min.z);
		_raycastOrigins.topLeft = new Vector3 (bounds.min.x, bounds.max.y, bounds.min.z);
		_raycastOrigins.topRight = new Vector3 (bounds.max.x, bounds.max.y, bounds.min.z);


        _raycastOrigins._bottomLeft = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        _raycastOrigins._bottomRight = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
        _raycastOrigins._topLeft = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        _raycastOrigins._topRight = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
    }


	void CalculateRaySpacing()
    {
		Bounds bounds = _collider.bounds;
		bounds.Expand (SkinWidth * -2);

		HorizontalRayCount = Mathf.Clamp (HorizontalRayCount, 2, int.MaxValue);
		VerticalRayCount = Mathf.Clamp (VerticalRayCount, 2, int.MaxValue);
        zRayCount = Mathf.Clamp(zRayCount, 2, int.MaxValue);

        _horizontalRaySpacing = bounds.size.y / (HorizontalRayCount - 1);
		_verticalRaySpacing = bounds.size.x / (VerticalRayCount - 1);
        _zRaySpacing = bounds.size.z/(zRayCount - 1);


    }

	struct RaycastOrigins
    {
		public Vector3 topLeft, topRight;
		public Vector3 bottomLeft, bottomRight;


        public Vector3 _topLeft, _topRight;
        public Vector3 _bottomLeft, _bottomRight;

    }

}
