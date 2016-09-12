using UnityEngine;
using System.Collections;

public class Smoothing : MonoBehaviour {

    public Transform parentAngle;
    Quaternion childAngle;

	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 pRV = parentAngle.eulerAngles;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3( pRV.x, pRV.y, pRV.z + 90f)) , 0.1f);
        transform.position += (parentAngle.position - transform.position) * 0.3f;
	}
}
