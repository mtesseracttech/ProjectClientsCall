using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PickUpBehavior : MonoBehaviour {

    public bool PickedAcord;
    private InventoryBehavior _inventory;
    private Image _acornImage;
    private bool AcornFull;
    private bool acornPickup;

    void Awake()
    {
        _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryBehavior>();
        _acornImage = GameObject.FindGameObjectWithTag("UIacorn").GetComponent<Image>();
    }

    void Update()
    {
        ResetImage();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !_inventory.InventoryFull) //check if it's player and inventory full
        {
            _inventory.acornCount += 1;
            _acornImage.fillAmount += 1f/_inventory._MaxAcornAmount;
            acornPickup = true;
            Destroy(gameObject);
        }
        else acornPickup = false;

    }

    void ResetImage()
    {
        if (_acornImage.fillAmount == 1f )
        {
            AcornFull = true;
        }
        else AcornFull = false;

        if(AcornFull && _inventory.GetDay(1) && acornPickup)
        {
           _acornImage.fillAmount = 0f;
        }
        else if (AcornFull && _inventory.GetDay(2) && acornPickup)
        {
            _acornImage.fillAmount = 0f;
        }
        else if (AcornFull && _inventory.GetDay(3) && acornPickup)
        {
            _acornImage.fillAmount = 0f;
        }
        else if (AcornFull && _inventory.GetDay(4) && acornPickup)
        {
            _acornImage.fillAmount = 0f;
        }
        else if (AcornFull && _inventory.GetDay(5) && acornPickup)
        {
            _acornImage.fillAmount = 0f;
        }
    }
}
