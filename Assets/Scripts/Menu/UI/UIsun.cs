using System;
using Environment;
using UnityEngine;
using UnityEngine.UI;

public class UIsun : MonoBehaviour
{
    public Image SunProgressBar;
    public DayNightCycles dayspeed;

    void Start()
    {
        SunProgressBar.fillAmount = 0;
    }

    void Update()
    {
            
      SunProgressBar.fillAmount += (dayspeed.Speed * dayspeed.DayRotateSpeed.x/ 198);
    }
}
