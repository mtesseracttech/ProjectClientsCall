using UnityEngine;
using System.Collections;

public class Smoothing : MonoBehaviour {

    public Transform parent;
    public Animator anim;
    private MovementScript movement;

	void Start () {
        movement = parent.GetComponent<MovementScript>();
        transform.position = parent.position;

        
	}
	

	void Update ()
    {
        anim.SetInteger("State", (int)movement.s.CurrentState);

        float angle = movement._orientation == Orientation.forward ? 0 : 180;

        Vector3 pRV = parent.eulerAngles;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3( 0, 0 + angle, Mathf.Sign((int)movement._orientation) * pRV.z + 90f)) , 0.6f);
        transform.position += (parent.position - transform.position) * 0.3f;
	}
}
