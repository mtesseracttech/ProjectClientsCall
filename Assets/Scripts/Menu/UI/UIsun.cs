using System;
using Environment;
using UnityEngine;
using UnityEngine.UI;

public class UIsun : MonoBehaviour
{
    public Image SunProgressBar;
   // public float decreaseSpeed = 0.016f;
    public DayNightCycles dayspeed;
    private float sunAngleRotationSpeed;
    private float speedOfRotation;
    void Start()
    {
        sunAngleRotationSpeed = dayspeed.DayRotateSpeed.x;//-1
       // speedOfRotation = dayspeed.SpeedPerTime();//~0.016
        speedOfRotation = Time.deltaTime;
    }

    void Update()
    {
        if (!dayspeed.NightHasCome)
        {
            
            SunProgressBar.fillAmount -= ((sunAngleRotationSpeed * speedOfRotation) / 220);

        }
        else

        {
            Debug.Log("fill amount ran out");
        }

    }
}
