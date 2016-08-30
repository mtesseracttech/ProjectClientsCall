using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public GameObject player;
    public float cameraDistance = 10;

    [Range(0, 1)]
    public float smoothness = 0.95f;

	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 desiredPos = player.transform.position - new Vector3(0,0,cameraDistance);
        Vector3 currentPos = this.transform.position;

        Vector3 newPos = ((desiredPos - currentPos) * (1 - smoothness)) + currentPos;
        this.transform.position = newPos;
	}
}
