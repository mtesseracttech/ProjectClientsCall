using System;
using UnityEngine;
using System.Collections;

public class DayNightCycle : MonoBehaviour
{

    public Gradient nightDayColor;

    public float maxIntensity = 3f;
    public float minIntensity = 0f;
    public float minPoint = -0.2f;

    public Gradient nightDayFogColor;
    public AnimationCurve fogDensityCurve;
    public float fogScale = 1f;

    public float dayAtmosphereThickness = 0.4f;
    public float nightAtmosphereThickness = 0.87f;

    public Vector3 dayRotateSpeed;
    public Vector3 nightRotateSpeed;

    float skySpeed = 1;


    Light mainLight;
    Skybox sky;
    Material _sky;

    public Transform stars;

    void Start()
    {

        mainLight = GetComponent<Light>();


        _sky = RenderSettings.skybox;
    }

    void Update()
    {
        stars.transform.rotation = transform.rotation;//rotate starts with sun 

        float tRange = 1 - minPoint; //total day time
        float dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);// from -1 to 1
        float i = ((maxIntensity - minIntensity) * dot) + minIntensity; 

        mainLight.intensity = i;

        mainLight.color = nightDayColor.Evaluate(dot);//at particular point give excatcly that color :)
        RenderSettings.ambientLight = mainLight.color;

        RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

        i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
      

        if (dot > 0)
            transform.Rotate(dayRotateSpeed * Time.deltaTime * skySpeed);
        else
            transform.Rotate(nightRotateSpeed * Time.deltaTime * skySpeed);

        if (Input.GetKeyDown(KeyCode.Q)) skySpeed *= 0.5f;
        if (Input.GetKeyDown(KeyCode.E)) skySpeed *= 2f;


    }
}
