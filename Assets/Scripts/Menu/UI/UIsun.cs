using System;
using Environment;
using UnityEngine;
using UnityEngine.UI;

public class UIsun : MonoBehaviour
{
    public Image SunProgressBar;
    public DayNightCycles dayspeed;

    void Update()
    {
            
      SunProgressBar.fillAmount -= (dayspeed.Speed / 198);
    }
}
