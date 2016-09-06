using UnityEngine;
using System.Collections;
using StateMachine;
using System;

public class MovementScript : MonoBehaviour {

    public enum Orientation
    {
        down,
        left,
        right
    }

    public float speed;
    public float floatHeight;
    public float gravity;
    public float jumpHeight;
    public float maxHeightDifference;

    public Orientation orientation;

    private SquirrelMachine s;

    private Vector2 velocity;
    private Vector2 acceleration;

    private BoxCollider playerCollider;

    private delegate void Movement();
    Movement moveDelegate;

	void Start () {

        s = new SquirrelMachine(SquirrelState.idling);

        orientation = Orientation.down;

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


    void AirMovement()
    {
        //acceleration -= new Vector2(0, gravity);

        //if (Input.GetKey(KeyCode.A))
        //    acceleration -= new Vector2(speed, 0);

        //if (Input.GetKey(KeyCode.D))
        //    acceleration += new Vector2(speed, 0);

        //transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);

        //RaycastHit hit = Raycast('f');

        //if (velocity.y < 0 && hit.distance != 0 && hit.distance < 2 * floatHeight)
        //    s.MoveNext(Key.down);

    }

    void Land()
    {
        //RaycastHit frontHit = Raycast('f');
        //RaycastHit rearHit = Raycast('r');

        //Vector2 normal = new Vector2(frontHit.normal.y, -frontHit.normal.x);

        //if (normal != new Vector2())
        //{
        //    float slope = (Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg) % 360;
        //    float steep = transform.eulerAngles.z % 360;

        //    float angleDif = (slope - steep) % 360;

        //    if (angleDif < -180)
        //        angleDif += 360;

        //    transform.Rotate(0, 0, angleDif * 0.1f);

        //    if (Input.GetKey(KeyCode.A))
        //        acceleration -= new Vector2(speed, 0);

        //    if (Input.GetKey(KeyCode.D))
        //        acceleration += new Vector2(speed, 0);

        //    float currentHeight = (frontHit.distance + rearHit.distance) / 2;
        //    if (currentHeight != floatHeight)
        //        transform.position += ((floatHeight - currentHeight) * frontHit.normal) * 0.2f;

        //    if (Mathf.Abs(angleDif) < 0.5f && Mathf.Abs(currentHeight - floatHeight) < 0.05f)
        //        s.MoveNext(Key.down);
        //}
 
    }

    void GroundMovement()
    {
        if (Input.GetKey(KeyCode.A))
            acceleration -= new Vector2(transform.right.x, transform.right.y) * speed;

        if (Input.GetKey(KeyCode.D))
            acceleration += new Vector2(transform.right.x, transform.right.y) * speed;

        if (Input.GetKeyDown(KeyCode.W))
            s.MoveNext(Key.up);

        if (Input.GetKey(KeyCode.S))
            s.MoveNext(Key.down);
    }


    void CheckCollision()
    {
        Vector3 bottomPos = this.transform.up * transform.localScale.y / 2;
        Vector3 rayOrientation = new Vector3();

        switch (orientation)
        {
            case Orientation.down:
                rayOrientation = Vector3.down;
                break;

            case Orientation.right:
                rayOrientation = Vector3.left;
                break;

            case Orientation.left:
                rayOrientation = Vector3.right;
                break;
        }

        /// Calculate points -----------------------------------
        // Front
        RaycastHit frontHit;
        Vector3 frontRayPos = this.transform.position + this.transform.right * (transform.localScale.x / 2) - bottomPos;
        Physics.Raycast(frontRayPos, rayOrientation, out frontHit);
        Debug.DrawRay(frontRayPos, rayOrientation);
        //

        //Rear
        RaycastHit rearHit;
        Vector3 rearRayPos = this.transform.position - this.transform.right * (transform.localScale.x / 2) - bottomPos;
        Physics.Raycast(rearRayPos, -transform.up, out rearHit);
        Debug.DrawRay(rearRayPos, -transform.up);
        //
        /// ----------------------------------------------------


        //Check current side
        Orientation frontOrientation = CheckSide(frontHit);
        Orientation rearOrientation = CheckSide(rearHit);


        /// check if the player is on 1 platform ---------------
        RaycastHit extraHit;
        Physics.Raycast(frontRayPos, new Vector3(-rayOrientation.y, rayOrientation.x, 0), out extraHit, floatHeight);

        if (frontOrientation == rearOrientation || extraHit.distance != 0 && CheckSide(extraHit) == rearOrientation)
        {
            orientation = rearOrientation;
        }
        /// ----------------------------------------------------


        /// Check if near inside corner ------------------------
        Debug.DrawRay(frontRayPos, new Vector3(-rayOrientation.y, rayOrientation.x, 0), Color.green);
        if (Physics.Raycast(frontRayPos, new Vector3(-rayOrientation.y, rayOrientation.x, 0), floatHeight -0.1f)) //0.1f has to be replaced
        {
            rayOrientation = (rayOrientation + new Vector3(-rayOrientation.y, rayOrientation.x)).normalized;
            Physics.Raycast(frontRayPos, rayOrientation, out frontHit);
            Debug.DrawRay(frontRayPos, rayOrientation, Color.red);
        }

        if (frontHit.distance > 1.5f * floatHeight)
        {
            Physics.Raycast(frontRayPos, new Vector3(rayOrientation.y, -rayOrientation.x), out frontHit);
            rayOrientation = new Vector3(rayOrientation.y, -rayOrientation.x);
            Debug.DrawRay(frontRayPos, new Vector3(rayOrientation.y, -rayOrientation.x), Color.yellow);
            print(true);

        }

        if (frontHit.distance != 0 && rearHit.distance != 0)
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(frontHit.point.y - rearHit.point.y, frontHit.point.x - rearHit.point.x) * Mathf.Rad2Deg);


        Physics.Raycast(frontRayPos, rayOrientation, out frontHit);
        Physics.Raycast(rearRayPos, -transform.up, out rearHit);

        float currentHeight = (frontHit.distance + rearHit.distance) / 2;
        if (currentHeight != floatHeight)
            transform.position += (floatHeight - currentHeight) * -rayOrientation;
    }



    Orientation CheckSide(RaycastHit hit)
    {
        float slopeNormal = Mathf.Atan2(hit.normal.y, hit.normal.x) * Mathf.Rad2Deg;

        if (slopeNormal >= 45 && slopeNormal < 135)
        {
            return Orientation.down;
        }
        if (slopeNormal >= 135 && slopeNormal < 225)
        {
            return Orientation.left;
        }
        else if (slopeNormal < 45)
        {
            return Orientation.right;
        }

        print(slopeNormal);
        return Orientation.down;
    }
}
