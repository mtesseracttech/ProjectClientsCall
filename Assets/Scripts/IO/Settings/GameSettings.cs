using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Scripts.IO.Settings
{
    [Serializable]
    [XmlRoot("settings")]
    public class GameSettings
    {
        [XmlElement("player")]
        public PlayerSettings PlayerSettings;

        [XmlElement("cycle")]
        public DayNightCycleSettings DayNightCycleSettings;

        public void PrintInfo(string prefix = "")
        {
            prefix +=
                "Settings: \n\n" +
                "Player Settings: \n" +
                "Jump Height: " + PlayerSettings.JumpingHeight + "\n" +
                "Running Speed: " + PlayerSettings.RunningSpeed + "\n" +
                "Flying Speed: " + PlayerSettings.FlyingSpeed + "\n" +
                "Gravity: " + PlayerSettings.Gravity + "\n" +
                "Air Drag: " + PlayerSettings.AirDrag + "\n\n" +
                "D/N Cycle Settings: \n" +
                "Day Cycle Speed: " + DayNightCycleSettings.DaySpeed + "\n" +
                "Night Cycle Speed: " + DayNightCycleSettings.NightSpeed;

            Debug.Log(prefix);
        }
    }


    [XmlRoot("player")]
    public class PlayerSettings
    {
        [XmlAttribute("jumpingheight")]
        public float JumpingHeight;

        [XmlAttribute("runningspeed")]
        public float RunningSpeed;

        [XmlAttribute("flyingspeed")]
        public float FlyingSpeed;

        [XmlAttribute("gravity")]
        public float Gravity;

        [XmlAttribute("airdrag")]
        public float AirDrag;
    }

    [XmlRoot("cycle")]
    public class DayNightCycleSettings
    {
        [XmlAttribute("dayspeed")] public float DaySpeed;

        [XmlAttribute("nightspeed")] public float NightSpeed;
    }
}