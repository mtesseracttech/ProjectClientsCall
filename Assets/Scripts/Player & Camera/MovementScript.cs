using UnityEngine;
using System.Collections;
using StateMachine;
using System;

public class MovementScript : MonoBehaviour
{

    public enum Orientation
    {
        forward = 1,
        backward = -1
    }

    public float speed;
    public float floatHeight;
    public float gravity;
    public float jumpHeight;
    public float maxHeightDifference;

    public Orientation _orientation;

    private SquirrelMachine s;

    private Vector2 velocity;
    private Vector2 acceleration;

    private BoxCollider playerCollider;

    private delegate void Movement();
    Movement moveDelegate;

    void Start()
    {

        s = new SquirrelMachine(SquirrelState.idling);

        playerCollider = gameObject.GetComponent<BoxCollider>();
        moveDelegate += GroundMovement;
        moveDelegate += ApplyPhysics;
        moveDelegate += CheckCollision;
    }

    void Update()
    {
        // Reset acceleration and apply drag
        acceleration = new Vector2();
        velocity *= 0.9f;
        //

        if (s.HasChanged() == true)
        {
            moveDelegate = null;
            switch (s.CurrentState)
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

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void ApplyPhysics()
    {
        velocity += acceleration;
        transform.position += new Vector3(velocity.x, velocity.y, 0);
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
        {
            acceleration -= new Vector2(transform.right.x, transform.right.y) * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            acceleration += new Vector2(transform.right.x, transform.right.y) * speed;
        }
        if (Input.GetKeyDown(KeyCode.W))
            s.MoveNext(Key.up);

        if (Input.GetKey(KeyCode.S))
            s.MoveNext(Key.down);

        if (velocity == new Vector2())
            return;
        else if (Vector2.Dot(transform.right.normalized, new Vector2(velocity.x, velocity.y).normalized) > 0)
            _orientation = Orientation.forward;
        else
            _orientation = Orientation.backward;
    }


    void CheckCollision()
    {
        Vector3 bottomPos = this.transform.up * transform.localScale.y / 2;

        RaycastHit frontHit;
        Vector3 frontRayPos;

        RaycastHit rearHit;
        Vector3 rearRayPos;

        RaycastHit forwardHit;
        Vector3 forwardPos;

        // Calculate points -----------------------------------
        rearRayPos = this.transform.position - this.transform.right * (transform.localScale.x / 2) * (int)_orientation;
        rearHit = Raycast(rearRayPos, -transform.up, Color.green);

        if (rearHit.distance == 0 || rearHit.distance > 1.5f)
            frontHit = Raycast(rearRayPos - bottomPos, -transform.up + (transform.right * (int)_orientation), Color.blue);

        frontRayPos = this.transform.position + this.transform.right * (transform.localScale.x / 2) * (int)_orientation;
        frontHit = Raycast(frontRayPos, -transform.up, Color.white);

        forwardPos = transform.position - bottomPos + transform.up * 0.05f;
        forwardHit = Raycast(forwardPos, transform.right * (int)_orientation, Color.cyan);

        // If player is on flat surface -----------------------
        if (frontHit.normal == rearHit.normal && frontHit.distance <= 1.5f)
        {
            // If player hits wall --------------------------------
            if (forwardHit.distance != 0 && forwardHit.distance < (transform.lossyScale.x / 2) + 0.2f)
            {
                Vector3 newPos = ClosestToRaycastHit(forwardHit, forwardPos) + new Vector3(forwardHit.normal.y, -forwardHit.normal.x, 0) * (int)_orientation;
                Vector3 oldPos = transform.position;

                transform.position = newPos + transform.up * (transform.localScale.y / 2) + (transform.localScale.x / 2) * -transform.right * (int)_orientation;

                float heightDif = Vector3.Dot(transform.position - oldPos, rearHit.normal) + (rearHit.distance - transform.lossyScale.y / 2);
                float rot = Mathf.Asin(heightDif / transform.lossyScale.x) * Mathf.Rad2Deg;

                transform.RotateAround(transform.position + (transform.right * (int)_orientation) * (transform.lossyScale.x / 2) - bottomPos, new Vector3(0, 0, 1), rot * (int)_orientation);
            }
            else if (rearHit.distance <= 1.5f)
            {
                if (_orientation == Orientation.forward)
                    transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(frontHit.point.y - rearHit.point.y, frontHit.point.x - rearHit.point.x) * Mathf.Rad2Deg);
                if (_orientation == Orientation.backward)
                    transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(rearHit.point.y - frontHit.point.y, rearHit.point.x - frontHit.point.x) * Mathf.Rad2Deg);

                transform.position = frontHit.point + frontHit.normal * (floatHeight + transform.localScale.y / 2) + (transform.localScale.x / 2) * (-transform.right * (int)_orientation);
            }
            else 
            {
                print("something went wrong!");
            }
        }
        else
        {
            if (frontHit.distance == 0 || frontHit.distance > 1.5f)
                frontHit = Raycast(frontRayPos - bottomPos, -transform.up - (transform.right * (int)_orientation), Color.blue);

            if (frontHit.distance > 0 && frontHit.distance <= 1.5f && rearHit.distance != 0)
            {
                //Get rear distance to platform
                rearRayPos -= bottomPos;
                Vector3 firstPoint = ClosestToRaycastHit(rearHit, rearRayPos);

                //Set new position
                Vector3 oldPos = transform.position;

                Vector3 newPosition = ClosestToRaycastHit(frontHit, frontHit.point);
                transform.position = newPosition + transform.up * (transform.localScale.y / 2) + (transform.localScale.x / 2) * ((-transform.right + new Vector3(0, -0.02f, transform.position.z)) * (int)_orientation);

                //get rear distance to platform after setting new position
                rearRayPos = this.transform.position - (this.transform.right * (int)_orientation) * (transform.localScale.x / 2) - bottomPos;

                //Calculations for atan2 formula
                float totalDistance = Vector3.Dot(transform.position - oldPos, -transform.up);

                float rot = Mathf.Asin(totalDistance / transform.localScale.x) * Mathf.Rad2Deg * (1.6f + velocity.magnitude * 2);

                transform.RotateAround(transform.position + -transform.up * (transform.localScale.y / 2) + (transform.right * (int)_orientation) * (transform.localScale.x / 2), new Vector3(0, 0, -1), rot * (int)_orientation);
            }
            else
            {
                //Debug.LogWarning("No fronthit!");
            }
        }
    }

    private Vector2[] ParseLineSection(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        Vector2 l0 = new Vector2(p0.x, p0.y);
        Vector2 l1 = new Vector2(p1.x, p1.y);
        Vector2 l2 = new Vector2(p2.x, p2.y);

        if (l0 == l1)
            return new Vector2[] { l0, l2 };
        else if (l0 == l2)
            return new Vector2[] { l0, l1 };
        else
            return new Vector2[] { l0, l1 };
    }

    public Vector3 ClosestToRaycastHit(RaycastHit hit, Vector3 point)
    {
        MeshCollider meshCollider = hit.collider as MeshCollider;

        try
        {
            if (meshCollider != null || meshCollider.sharedMesh != null)
            {
                Mesh mesh = meshCollider.sharedMesh;

                int[] triangles = mesh.triangles;
                Vector3[] vertices = mesh.vertices;

                Vector3 p0 = meshCollider.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 0]]);
                Vector3 p1 = meshCollider.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 1]]);
                Vector3 p2 = meshCollider.transform.TransformPoint(vertices[triangles[hit.triangleIndex * 3 + 2]]);

                Vector2[] linePoints = ParseLineSection(p0, p1, p2);

                Vector2 closestPoint = GetClosestPoint(linePoints[0], linePoints[1], new Vector2(point.x, point.y));

                return new Vector3(closestPoint.x, closestPoint.y, 0);
            }
        }
        catch { }
        //Debug.LogWarning("Object has no collider!");
        return new Vector3();
    }

    public Vector2 GetClosestPoint(Vector2 A, Vector2 B, Vector2 P)
    {
        Vector2 AP = P - A;
        Vector2 AB = B - A;

        float magnitudeAB = Mathf.Pow(AB.magnitude, 2);
        float ABAPproduct = Vector2.Dot(AP, AB);
        float distance = ABAPproduct / magnitudeAB;

        if (distance < 0)
            return A;

        else if (distance > 1)
            return B;

        else
            return A + AB * distance;
    }


    private RaycastHit Raycast(Vector3 origin, Vector3 direction, Color color = new Color(), float distance = 0)
    {
        RaycastHit hit;
        Physics.Raycast(origin, direction, out hit);

        if (color != new Color() && (distance == 0 || hit.distance <= distance))
            Debug.DrawRay(origin, direction, color);

        if (distance == 0)
            return hit;
        else if (hit.distance <= distance)
            return hit;
        else
            return new RaycastHit();
    }
}
