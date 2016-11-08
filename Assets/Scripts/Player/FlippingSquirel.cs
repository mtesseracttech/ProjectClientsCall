using UnityEngine;
using Assets.script;

public class FlippingSquirel : MonoBehaviour
{

    public Player parentOfModel;
    public Controller2D control;

    void Update()
    {
        //on the ground
        float moveDirX = Input.GetAxisRaw("Horizontal");
        if(moveDirX!=0)
        transform.eulerAngles = (moveDirX > 0) ? Vector3.up*90 : -Vector3.up * 90;

        //on the slope
        //if slope is on right
        if (moveDirX != 0 && control.onSlope)
        {
            if (moveDirX > 0)
            {
                transform.eulerAngles = new Vector3(-90,90,0);
            }
            else
            {
                transform.eulerAngles = new Vector3(-270,0,90);
            }
        }

        //if slope is on left

    }
}
