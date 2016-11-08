using System.Collections;
using UnityEngine;

namespace Environment
{
    public class DayNightCycles : MonoBehaviour
    {
        public bool NightHasCome;
        public float Speed;
      //  public FeedingBehavior behavior;
        public bool GameOver;


        [Header("Color changers")]
        public Gradient NightDayColor;
        public Gradient NightDayFogColor;
        public AnimationCurve FogDensityCurve;

        [Header("Current time of day")]
        public Vector3 DayRotateSpeed;  //set how fast it will rotate during day. the lower the slower
        public Vector3 NightRotateSpeed;
        private Light _mainLight;
        private Material _sky;

        private float MaxIntensity = 3f;
        private float MinIntensity = 0f;
        public float MinPoint = -0.2f; 
        private float FogScale = 1f;


        [Header("Sun parameters")]
        private float _timeofDay = 1;
        public int DaysHasPassed = 0;
        public bool dayHasCome;
        private float daySpeed;
        private float nightSpeed;

        void Start()
        {

            _mainLight = GetComponent<Light>();
            _sky = RenderSettings.skybox;
            NightHasCome = false;
            dayHasCome = false;
            daySpeed = DayRotateSpeed.x;
        }
        void Update()
        {
            DirectionalLightCalculations();
        }

        private void DirectionalLightCalculations()
        {
            float tRange = 1 - MinPoint; //total day time
            float dot = Mathf.Clamp01((Vector3.Dot(_mainLight.transform.forward, Vector3.down) - MinPoint) / tRange);// from -1 to 1
            float i = ((MaxIntensity - MinIntensity) * dot) + MinIntensity;

            SpeedPerTime();

            _mainLight.intensity = i;

            _mainLight.color = NightDayColor.Evaluate(dot);//at particular point give excatcly that color :)
            RenderSettings.ambientLight = _mainLight.color;

            RenderSettings.fogColor = NightDayFogColor.Evaluate(dot);
            RenderSettings.fogDensity = FogDensityCurve.Evaluate(dot) * FogScale;

            UpdatingSpeed();
            //dot -1 - 1
            //day
            if (dot > 0)
            {
                transform.Rotate(DayRotateSpeed*Speed);
                NightHasCome = false;
                
            }
            //night
            else
            {
                transform.Rotate(NightRotateSpeed*Speed);
                Debug.Log("Night has come");
                dayHasCome = false;
                UpdatingDays();
            }
        }

        public float SpeedPerTime()
        {
            Speed = Time.deltaTime * _timeofDay;
            return Speed;
        }

        //this doesn't update to second day. just says it's first day all the fucking time 1000x
        private void UpdatingDays()
        {
            if (!NightHasCome)
            {
                DaysHasPassed += 1;
                Debug.Log("One day passed " + DaysHasPassed);
                NightHasCome = true;
            }
            if (DaysHasPassed >= 0)
            {
               // behavior.InTheCave = false;
            }
            
        }

        private void UpdatingSpeed()
        {
          //  if (behavior.InTheCave)
          //  {
         //       DayRotateSpeed.x = NightRotateSpeed.x;
        //    }
         //   else
         //   {
                DayRotateSpeed.x = daySpeed;
           // }
        }
    }
}
