using UnityEngine;
using System.Collections;

public class MovementScript : MonoBehaviour {

    public float speed;
    public float rayTollerance = 1;

    private BoxCollider playerCollider;

	void Start () {

        playerCollider = gameObject.GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        Movement();
        CheckCollision();

    }

    void CheckCollision()
    {
        Vector3 bottomRayPos = this.transform.up * playerCollider.size.y / 2;

        Vector3 frontRayPos = this.transform.position + this.transform.right * (playerCollider.size.x / 2) - bottomRayPos;
        Vector3 rearRayPos = this.transform.position - this.transform.right * (playerCollider.size.x / 2) - bottomRayPos;

        Ray frontRay = new Ray(frontRayPos, Vector3.down);
        Ray rearRay = new Ray(rearRayPos, Vector3.down);

        Debug.DrawRay(rearRayPos, Vector3.down, Color.red);
        Debug.DrawRay(frontRayPos, Vector3.down, Color.green);

        RaycastHit frontHit;
        RaycastHit rearHit;
        
        if (Physics.Raycast(frontRay, out frontHit, rayTollerance) && Physics.Raycast(rearRay, out rearHit, rayTollerance))
        {
            if (Vector3.Angle(frontHit.normal, rearHit.normal) > 0.1f)
            {
                transform.eulerAngles = new Vector3(0, 0, Mathf.Asin((frontHit.point.y - rearHit.point.y) / playerCollider.size.x) * Mathf.Rad2Deg);
                print(transform.eulerAngles.z);
            }
            else
            {
                if()
            }
            
        }
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += new Vector3(0, speed, 0);
        else if (Input.GetKey(KeyCode.S))
            transform.position -= new Vector3(0, speed, 0);

        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * speed;
        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * speed;
    }
}
