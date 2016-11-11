﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDBehavior : MonoBehaviour
{
    public float DurationOfDay;
    public GameObject GameOver;
    public GameObject TimeManager;
    private Animator _uIdayPass;
    private InventoryBehavior _inventory;
    private Image _arrowImage;
    private Text _textDayPassUpdate;
    private TimeManager _timeManager;

    void Awake()
    {
        _arrowImage = GameObject.FindGameObjectWithTag("UIarrow").GetComponent<Image>();
        _inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryBehavior>();
        _uIdayPass = GameObject.FindGameObjectWithTag("DayChange").GetComponent<Animator>();
        _textDayPassUpdate = GameObject.FindGameObjectWithTag("DayChangeUI").GetComponent<Text>();
        _timeManager = TimeManager.GetComponent<TimeManager>();
    }

    void Start()
    {
        _timeManager.NewDay(1);
        StartCoroutine(DayHasPassedScreen());
    }


    void Update()
    {
         StartNewDay();

        if (!_timeManager.DayOver())
        {
            StartCoroutine(StartClock());
        }
        else
        {
            StartCoroutine(GameOverScreen());
        }
    }

    IEnumerator StartClock()
    {
        _timeManager.enabled = false;
        yield return new WaitForSeconds(3f);
        _timeManager.enabled = true;
        RotateClock();
    }

    void RotateClock()
    {
        float ratio = _timeManager.GetTime() / _timeManager.TotalTimePerDay;
        float dayBase = (360f / _timeManager.TotalDays); //Gets the angle per day
        float dayBaseSpecific = dayBase * (_timeManager.GetDay() - 1); //Gets the angle of the start of a specific day
        float angleForSpecificDay = dayBase*ratio;
        float clockAngle = angleForSpecificDay + dayBaseSpecific;
        _arrowImage.transform.rotation = Quaternion.AngleAxis(clockAngle, Vector3.back);
    }

    void StartNewDay()
    {
        if (!_timeManager.DayOver() && _inventory.InventoryFull && _inventory.InTheNest && _inventory.GetDay(1))
        {
            _textDayPassUpdate.text = "Day 2";
           // StartCoroutine(DayHasPassedScreen());
            print("first day passed");
            _inventory.SetDay(2,true);
            _inventory.acornCount = 0;
            _inventory.SetDay(1, false);
        }
        else if (!_timeManager.DayOver() && _inventory.InventoryFull && _inventory.InTheNest && _inventory.GetDay(2))
        {
            _textDayPassUpdate.text = "Day 3";
          //  StartCoroutine(DayHasPassedScreen());
            print("second day passed");
            _inventory.SetDay(3, true);
            _inventory.acornCount = 0;
            _inventory.SetDay(2, false);
        }
        else if (!_timeManager.DayOver() && _inventory.InventoryFull && _inventory.InTheNest && _inventory.GetDay(3))
        {
            _textDayPassUpdate.text = "Day 4";
          //  StartCoroutine(DayHasPassedScreen());
            print("third day passed");
            _inventory.SetDay(4, true);
            _inventory.acornCount = 0;
            _inventory.SetDay(3, false);
        }
        else if (!_timeManager.DayOver() && _inventory.InventoryFull && _inventory.InTheNest && _inventory.GetDay(4))
        {
            _textDayPassUpdate.text = "Day 5";
          //  StartCoroutine(DayHasPassedScreen());
            _inventory.SetDay(5, true);
            print("forth day passed");
            _inventory.acornCount = 0;
            _inventory.SetDay(4, false);
        }
        else if (!_timeManager.DayOver() && _inventory.InventoryFull && _inventory.InTheNest && _inventory.GetDay(5))
        {
            print("win");
            StartCoroutine(WinScreen());
        }
    }

    IEnumerator GameOverScreen()
    {
        GameOver.SetActive(true); 
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }

    IEnumerator WinScreen()
    {

        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }

    IEnumerator DayHasPassedScreen()
    {
        _uIdayPass.enabled = true;
        yield return new WaitForSeconds(4f);
        _uIdayPass.enabled = false;
    }
}
