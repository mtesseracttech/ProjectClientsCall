using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using Assets.Scripts.IO.Settings;
using JetBrains.Annotations;

public class SettingManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject DayNightCycleManager;


    private static SettingManager _instance;

    private GameSettings _settings;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public static SettingManager Instance()
    {
        return _instance;
    }


	// Use this for initialization
	void Start ()
	{
	    string message = "Settings Manager Status:\n";
	    string problems = "";
	    if (Player == null) problems += "No player was assigned!\n";
	    if (DayNightCycleManager == null) problems += "No DayNightCycleManager was assigned!\n";
	    if (problems.Length > 0)
	    {
	        message += problems;
	        message += "Saving settings is not supported because of undefined variables";
	    }
	    else
	    {
	        message += "Working!";
	    }
	    Debug.Log(message);

        GameSettings temp = SettingsIO.ReadSettingsFromFile("GameSettings.xml");
        if (temp != null)
        {
            _settings = temp;
        }
        SetSettings();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    //Load the settings
	    if (Input.GetKeyDown(KeyCode.O))
	    {
	        GameSettings temp = SettingsIO.ReadSettingsFromFile("GameSettings.xml");
	        if (temp != null)
	        {
	            _settings = temp;
	        }
	        SetSettings();
	    }

	    if (Input.GetKeyDown(KeyCode.P))
	    {
	        if (!(Player != null || DayNightCycleManager != null)) //REMOVE INVERSION!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	        {
                ComposeSettings();
	            SettingsIO.SaveSettingsToFile(_settings);
	        }
	    }
	}

    private void SetSettings() //Link to the belonging refs
    {
        if (Player != null)
        {
            //Set settings in player script
        }
        if (DayNightCycleManager != null)
        {
            //Set settings in DNCM script
        }
    }

    private void ComposeSettings() //Link to belonging refs
    {
        if (_settings == null)
        {
            _settings = new GameSettings();
            _settings.PlayerSettings = new PlayerSettings();
            _settings.DayNightCycleSettings = new DayNightCycleSettings();
        }

        //Get Settings from player script
        _settings.PlayerSettings.JumpingHeight = 1;
        _settings.PlayerSettings.FlyingSpeed = 1;
        _settings.PlayerSettings.RunningSpeed = 1;
        _settings.PlayerSettings.Gravity = 1;
        _settings.PlayerSettings.AirDrag = 1;


        //Get Settings from DNCM script
        _settings.DayNightCycleSettings.DaySpeed = 1;
        _settings.DayNightCycleSettings.NightSpeed = 1;
    }
}
