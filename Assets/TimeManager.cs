using UnityEngine;
using System.Collections;

public class TimeManager : MonoBehaviour
{
    public int TotalDays;
    public int TotalTimePerDay;
    private int _currentDay;
    private float _currentTime;
    private float _timeAccumulator;

	// Use this for initialization
	void Awake ()
	{
	    _currentDay = -1;
	    _currentTime = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    _timeAccumulator += Time.deltaTime;
	    if (_timeAccumulator >= 1.0f)
	    {
	        Ticker();
	        _timeAccumulator -= 1.0f;
	    }
	}

    private void Ticker()
    {       
        _currentTime += 1.0f;
        if (_currentTime >= TotalTimePerDay)
        {
            //Meh
        }
    }

    public int GetDay()
    {
        return _currentDay;
    }

    public float GetTime()
    {
        return _currentTime;
    }

    public void NewDay(int day)
    {
        _currentDay = day;
        _currentTime = 0;
    }

    public void NextDay()
    {
        if (_currentDay < TotalDays)
        {
            NewDay(++_currentDay);
        }
        else Debug.Log("There is no next day");
    }

    public float TimeLeft()
    {
        return TotalTimePerDay - _currentTime;
    }

    public bool DayOver()
    {
        if (TimeLeft() > 0)
        {
            return false;
        }
        return true;
    }
}
