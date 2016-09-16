using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Text NutsText;
    public int NutCount;
    public FeedingBehavior Feeding;
    public int Nuts
    {
        get { return NutCount; }
    }

    private void Update()
    {
        NutsText.text =  NutCount.ToString() + " / " + Feeding.maxAmount;
    }

    private bool AccornInInventory()
    {
        if (NutCount > 0) return true;
        return false;
    }

}
