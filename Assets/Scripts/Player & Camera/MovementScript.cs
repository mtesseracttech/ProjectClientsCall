using UnityEngine;
using System.Collections;
using StateMachine;
using System;

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
        moveDelegate += CheckCollision;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // Reset acceleration and apply drag
        acceleration = new Vector2();
        velocity *= 0.9f;
        //

        if (s.HasChanged() == true)
        {
            moveDelegate = null;
            switch(s.CurrentState)
            {
                case SquirrelState.running:
                    moveDelegate += GroundMovement;
                    moveDelegate += CheckCollision;
                    break;

                case SquirrelState.landing:
                    moveDelegate += Land;
                    break;

                case SquirrelState.jumping:
                    acceleration += new Vector2(0, jumpHeight);
                    moveDelegate += AirMovement;
                    break;
            }
        }
        if (moveDelegate != null)
            moveDelegate();

        // Apply physics to squirrel body
        velocity += acceleration;
        transform.position += new Vector3(velocity.x, velocity.y, 0);
        //
    }

    void CheckCollision()
    {
        RaycastHit frontHit = Raycast('f');
        RaycastHit rearHit = Raycast('r');

        if (frontHit.distance != 0 && rearHit.distance != 0)
        {
            transform.eulerAngles = new Vector3(0, 0, Mathf.Asin((frontHit.point.y - rearHit.point.y) / playerCollider.size.x) * Mathf.Rad2Deg);

            float currentHeight;
            if ((currentHeight = (frontHit.distance + rearHit.distance) / 2) != floatHeight)
                transform.position += (floatHeight - currentHeight) * transform.up;
        }
    }


    void AirMovement()
    {
        acceleration -= new Vector2(0, gravity);

        if (Input.GetKey(KeyCode.A))
            acceleration -= new Vector2(speed, 0);

        if (Input.GetKey(KeyCode.D))
            acceleration += new Vector2(speed, 0);

        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);

        RaycastHit hit = Raycast('f');
        if (hit.distance != 0 && hit.distance < floatHeight)
            s.MoveNext(Key.down);

    }

    void Land()
    {
        RaycastHit frontHit = Raycast('f');
        Vector2 normal = new Vector2(-frontHit.normal.y, frontHit.normal.x);

        if (normal != new Vector2())
        {
            float slope = (Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg) % 360;
            float steep = transform.eulerAngles.z % 360;

            float difference = slope - steep;

            transform.Rotate(0, 0, difference * 0.1f);
            print(difference);
            
        }
 
    }

    void GroundMovement()
    {
        if (Input.GetKey(KeyCode.A))
            acceleration -= new Vector2(speed, 0);

        if (Input.GetKey(KeyCode.D))
            acceleration += new Vector2(speed, 0);

        if (Input.GetKeyDown(KeyCode.W))
            s.MoveNext(Key.up);

        if (Input.GetKey(KeyCode.S))
            s.MoveNext(Key.down);
    }

    RaycastHit Raycast(char position)
    {
        RaycastHit hit;
        Vector3 bottomPos = this.transform.up * playerCollider.size.y / 2;
        Vector3 rayPos;
        switch (position)
        {
            case 'r':
                rayPos = this.transform.position - this.transform.right * (playerCollider.size.x / 2) - bottomPos;
                Physics.Raycast(rayPos, Vector3.down, out hit);
                return hit;

            case 'm':
                Physics.Raycast(bottomPos, Vector3.down, out hit);
                return hit;

            case 'f':
                rayPos = this.transform.position + this.transform.right * (playerCollider.size.x / 2) - bottomPos;
                Physics.Raycast(rayPos, Vector3.down, out hit);
                return hit;
        }

        return new RaycastHit();
    }
}
