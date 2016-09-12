using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Text NutsText;
    public int NutCount;
    public bool GotAcorn;
    public FeedingBehavior Feeding;
   public int Nuts
    {
        get { return NutCount; }
    }

    private void Update()
    {
        NutsText.text =  NutCount.ToString() + " / " + Feeding.maxAmount;
        AccornInInventory();
    }

    private void AccornInInventory()
    {
        if (NutCount > 0)
        {
            GotAcorn = true;
        }
        else
        {
            GotAcorn = false;
        }
    }

}
