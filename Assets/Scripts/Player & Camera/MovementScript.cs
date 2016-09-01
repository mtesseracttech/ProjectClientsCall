using UnityEngine;
using System.Collections;
using StateMachine;

public class MovementScript : MonoBehaviour {

    public float speed;
    public float floatHeight;
    public float gravity;
    public float jumpHeight;
    public float maxHeightDifference;

    private SquirrelMachine s;

    private Vector2 velocity;
    private Vector2 acceleration;

    private BoxCollider playerCollider;

    private delegate void Movement();
    Movement moveDelegate;

	void Start () {

        s = new SquirrelMachine(SquirrelState.idling);

        playerCollider = gameObject.GetComponent<BoxCollider>();
        moveDelegate += GroundMovement;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (s.HasChanged() == true)
        {
            switch(s.CurrentState)
            {
                case SquirrelState.running:
                    print(true);
                    break;
            }
        }

        moveDelegate();
        CheckCollision();

    }

    void CheckCollision()
    {
        Vector3 bottomRayPos = this.transform.up * playerCollider.size.y / 2;

        Vector3 frontRayPos = this.transform.position + this.transform.right * (playerCollider.size.x / 2) - bottomRayPos;
        Vector3 rearRayPos = this.transform.position - this.transform.right * (playerCollider.size.x / 2) - bottomRayPos;

        Ray frontRay = new Ray(frontRayPos, -transform.up);
        Ray rearRay = new Ray(rearRayPos, -transform.up);

        Debug.DrawRay(rearRayPos, Vector3.down, Color.red);
        Debug.DrawRay(frontRayPos, Vector3.down, Color.green);

        RaycastHit frontHit;
        RaycastHit rearHit;

        if (Physics.Raycast(frontRay, out frontHit, floatHeight + maxHeightDifference) && Physics.Raycast(rearRay, out rearHit, floatHeight + maxHeightDifference))
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Asin((frontHit.point.y - rearHit.point.y) / playerCollider.size.x) * Mathf.Rad2Deg);

            float currentHeight;
            if ((currentHeight = (frontHit.distance + rearHit.distance) / 2) != floatHeight)
                transform.position += (floatHeight - currentHeight) * transform.up;
        }


        acceleration = new Vector2();
    }


    void GroundMovement()
    {
        acceleration = new Vector2();
        velocity *= 0.9f;

        if (Input.GetKey(KeyCode.A))
            acceleration -= new Vector2(speed, 0);

        if (Input.GetKey(KeyCode.D))
            acceleration += new Vector2(speed, 0);

        if (Input.GetKey(KeyCode.W))
            s.MoveNext(Key.up);
        if (Input.GetKey(KeyCode.S))
            s.MoveNext(Key.down);

        velocity += acceleration;

        transform.position += new Vector3(velocity.x, velocity.y, 0);
    }
}
