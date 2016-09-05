using UnityEngine;
using System.Collections;

public class OnTrigger : MonoBehaviour
{

    public Inventory inventory;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            inventory.addItem = true;
            Destroy(gameObject);
            Debug.Log("Fuck you");
        }
        
    }
}
