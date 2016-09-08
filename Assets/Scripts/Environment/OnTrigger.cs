using UnityEngine;

public class OnTrigger : MonoBehaviour
{
    public Inventory Inventory;
    public FeedingBehavior PickingUp;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !PickingUp.haveEnoguhAcorns)
        {
            Inventory.NutCount++;
            Destroy(gameObject);
            Debug.Log("Got acorn");
        }
        
    }
}
