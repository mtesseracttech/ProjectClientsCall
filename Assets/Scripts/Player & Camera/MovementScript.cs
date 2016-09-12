using UnityEngine;
using System.Collections;
using StateMachine;
using System;

public class MovementScript : MonoBehaviour
{
    // Enums -----------------------------------
    private enum Orientation
    {
        forward = 1,
        backward = -1
    }


    // Public Variables ------------------------
    public float speed;
    public float gravity;
    public float jumpHeight;

    [Range(0.0f, 1.0f)]
    public float drag;

    private Orientation _orientation;


    // Private Variables -----------------------
    private SquirrelMachine s;

    private BoxCollider playerCollider;

    private Vector2 velocity;
    private Vector2 acceleration;


    private Vector3 center;
    private Vector3 bottom; //Bottom middle
    private Vector3 rear; //Center left
    private Vector3 front; //Center right

    private delegate void Movement();
    Movement moveDelegate;

    // ----------------------------------------
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = true;

        s = new SquirrelMachine(SquirrelState.running);

        playerCollider = gameObject.GetComponent<BoxCollider>();
        moveDelegate += GroundMovement;
        moveDelegate += ApplyPhysics;
        moveDelegate += CheckCollision;

        UpdateLocalVectors();
    }

    void Update()
    {
        // Reset acceleration and apply drag
        acceleration = new Vector2();
        velocity *= 1 - drag;
        //

        if (s.HasChanged() == true)
        {
            moveDelegate = null;
            switch (s.CurrentState)
            {
                case SquirrelState.running:
                    moveDelegate += GroundMovement;
                    moveDelegate += ApplyPhysics;
                    moveDelegate += CheckCollision;
                    break;

                case SquirrelState.jumping:
                    acceleration += new Vector2(transform.up.x, transform.up.y) * jumpHeight;
                    moveDelegate += AirMovement;
                    moveDelegate += ApplyPhysics;
                    break;
            }
        }
        if (moveDelegate != null)
            moveDelegate();

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void ApplyPhysics()
    {
        velocity = new Vector2();
        velocity += acceleration;
        transform.position += new Vector3(velocity.x, velocity.y, 0);
        UpdateLocalVectors();
    }

    void AirMovement()
    {
        acceleration -= new Vector2(0, gravity);

        if (Input.GetKey(KeyCode.A))
            acceleration -= new Vector2(speed, 0);

        if (Input.GetKey(KeyCode.D))
            acceleration += new Vector2(speed, 0);

        float angle = (_orientation == Orientation.forward ? 180 : 0) + Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, 180 + angle);

        if (Vector3.Dot(Vector3.up, transform.up) < 0)
        {
            transform.eulerAngles += new Vector3(0, 0, 180);
            _orientation = _orientation == Orientation.backward ? Orientation.forward : Orientation.backward;
        }

        UpdateLocalVectors();

        RaycastHit hit1 = Raycast(center + front + bottom, front, Color.magenta);
        RaycastHit hit2 = Raycast(center + front - bottom, front, Color.magenta);

        RaycastHit hit0;

        if (hit1.distance == 0 & hit2.distance == 0)
            return;
        else if (hit1.distance == 0)
            hit0 = hit2;
        else if (hit2.distance == 0)
            hit0 = hit1;
        else
            hit0 = hit1.distance < hit2.distance ? hit1 : hit2;

        if (hit0.distance < velocity.magnitude + 0.1f)
        {
            transform.position = hit0.point + rear + (hit0.distance == hit2.distance ? bottom : -bottom);
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(-hit0.normal.x, hit0.normal.y) * Mathf.Rad2Deg);
            print(transform.eulerAngles.z);
            UpdateLocalVectors();
            CheckCollision();
            s.MoveNext(Key.down);
            //Debug.Break();
        }

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

        if (velocity == new Vector2())
            return;
        else if (Vector2.Dot(transform.right.normalized, new Vector2(velocity.x, velocity.y).normalized) > 0)
            _orientation = Orientation.forward;
        else
            _orientation = Orientation.backward;
    }


    void CheckCollision()
    {
        RaycastHit frontHit;
        RaycastHit rearHit;
        RaycastHit forwardHit;

        // Calculate points -----------------------------------
        rearHit = Raycast(center + rear, bottom, Color.green);

        if (rearHit.distance == 0 || rearHit.distance > 1.5f)
            rearHit = Raycast(center + bottom + rear, bottom + front, Color.blue);

        frontHit = Raycast(center + front, bottom, Color.white);
        forwardHit = Raycast(center + bottom, front, Color.cyan);

        // If player is on flat surface -----------------------
        if (frontHit.normal == rearHit.normal && frontHit.distance <= 1.5f)
        {
            // If player hits wall --------------------------------
            if (forwardHit.distance != 0 && forwardHit.distance < (transform.lossyScale.x / 2) + 0.2f)
            {
                Vector3 newPos = ClosestToRaycastHit(forwardHit, center + bottom) + new Vector3(forwardHit.normal.y, -forwardHit.normal.x, 0) * (int)_orientation;
                Vector3 oldPos = transform.position;

                transform.position = newPos - front - bottom;
                UpdateLocalVectors();

                float heightDif = Vector3.Dot(center - oldPos, rearHit.normal) + (rearHit.distance - transform.lossyScale.y / 2);
                float rot = Mathf.Asin(heightDif / transform.lossyScale.x) * Mathf.Rad2Deg;

                transform.RotateAround(center + front + bottom, Vector3.forward, rot * (int)_orientation);
            }
            else if (rearHit.distance <= 1.5f)
            {
                if (_orientation == Orientation.forward)
                    transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(frontHit.point.y - rearHit.point.y, frontHit.point.x - rearHit.point.x) * Mathf.Rad2Deg);
                if (_orientation == Orientation.backward)
                    transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(rearHit.point.y - frontHit.point.y, rearHit.point.x - frontHit.point.x) * Mathf.Rad2Deg);

                transform.position = frontHit.point + frontHit.normal * (transform.localScale.y / 2) + (transform.localScale.x / 2) * (-transform.right * (int)_orientation);
            }
        }
        else
        {
            if (frontHit.distance == 0 || frontHit.distance > 1.5f)
                frontHit = Raycast(center + front + bottom, bottom - front, Color.blue);

            if (frontHit.distance > 0 && frontHit.distance <= 1.5f && rearHit.distance != 0)
            {
                //Set new position
                Vector3 oldPos = center;

                Vector3 newPosition = ClosestToRaycastHit(frontHit, frontHit.point);
                transform.position = newPosition - bottom + (transform.localScale.x / 2) * ((-transform.right + new Vector3(0, 0, transform.position.z)) * (int)_orientation);
                UpdateLocalVectors();


                //Calculations for atan2 formula
                float totalDistance = Vector3.Dot(center - oldPos, bottom.normalized);

                float rot = Mathf.Asin(totalDistance / transform.localScale.x) * Mathf.Rad2Deg * (1.6f);

                transform.RotateAround(center + -transform.up * (transform.localScale.y / 2) + (transform.right * (int)_orientation) * (transform.localScale.x / 2), new Vector3(0, 0, -1), rot * (int)_orientation);
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

    private void UpdateLocalVectors()
    {
        center = transform.position;

        bottom = -transform.up * (transform.lossyScale.y / 2);

        rear = transform.right * (transform.lossyScale.x / 2) * -(int)_orientation;
        front = transform.right * (transform.lossyScale.x / 2) * (int)_orientation;
    }
}