using System;
using UnityEngine;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;

public class DayNightCycle : MonoBehaviour
{
    [Header("Object reference")]
    public Transform stars;

    [Header("Color changers")]
    public Gradient nightDayColor;
    public Gradient nightDayFogColor;
    public AnimationCurve fogDensityCurve;

    [Header("Current time of day")]
    public float maxIntensity = 3f;
    public float minIntensity = 0f;
    public float minPoint = -0.2f;
    public Vector3 dayRotateSpeed;
    public Vector3 nightRotateSpeed;
    private Light mainLight;
    private Skybox sky;
    private Material _sky;
    public float fogScale = 1f;

   [Header("Sun parameters")]
    [SerializeField]
    private float TimeofDay = 1;
    [SerializeField]
    private float amplitudeX = 2.0f;
    [SerializeField]
    private float amplitudeZ = 3.0f;
    [SerializeField]
    private float periodInSec = 120;
    private float frequency = 2f;

    private float twoPi = Mathf.PI * 2f;
    
    public Vector3 oldPosition = new Vector3();
    public Vector3 sunposition;

    void Start()
    {
        mainLight = GetComponent<Light>();
        _sky = RenderSettings.skybox;
        frequency = 1 / periodInSec;
        sunposition = transform.localPosition;
    }


    void Update()
    {
        stars.transform.rotation = transform.rotation;//rotate starts with sun 

        float tRange = 1 - minPoint; //total day time
        float dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);// from -1 to 1
        float i = ((maxIntensity - minIntensity) * dot) + minIntensity;
        float speed = Time.deltaTime*TimeofDay;
        mainLight.intensity = i;

        mainLight.color = nightDayColor.Evaluate(dot);//at particular point give excatcly that color :)
        RenderSettings.ambientLight = mainLight.color;

        RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

        //for day
        float x = amplitudeX * Mathf.Cos(twoPi * Time.time * frequency);
        float z = amplitudeZ * Mathf.Sin(twoPi * Time.time * frequency);

        //for night

        float xD = amplitudeX*Mathf.Cos(twoPi*Time.time*0.02f);
        float zD = amplitudeZ * Mathf.Sin(twoPi * Time.time * 0.02f);

        //day
        if (dot > 0)
        {
            transform.Rotate(dayRotateSpeed*speed);
            transform.localPosition = new Vector3(-x, z, 0);


        }
        //night
        else
        {
            transform.Rotate(nightRotateSpeed * speed);
            transform.localPosition = new Vector3(-xD, zD, 0);

        }
       // transform.localPosition = new Vector3(Mathf.Cos(speed), 0, Mathf.Sin(speed));
    }

}
