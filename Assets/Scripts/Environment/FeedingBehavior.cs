using UnityEngine;
using System.Collections;
using Environment;
using UnityEngine.Assertions.Must;

public class FeedingBehavior : MonoBehaviour
{
    public bool InTheCave;
    public bool haveEnoguhAcorns;
    private bool _1stDayAmountOfAcorn = false;
    private bool _2ndDayAmountOfAcorn = false;
    private bool _3rdDayAmountOfAcorn = false;
    private bool _4thDayAmountOfAcorn = false;
    private bool _5thDayAmountOfAcorn = false;
    private bool _1stDayPassed = false;
    private bool _2ndDayPassed = false;
    private bool _3rdDayPassed = false;
    private bool _4thDayPassed = false;
    private bool _5thDayPassed = false;

    public int maxAmount;
    public Inventory HaveAcorn;
    public DayNightCycles Cycle;

    public int FirstDayAcorn = 2;
    public int SecondDayAcorn = 8;
    public int ThirdDayAcorn = 18;
    public int FourthDayAcorn = 24;
    public int FifthDayAcorn = 30;

    private void Start()
    {
        InTheCave = false;
        maxAmount = FirstDayAcorn;
    }

    private void Update()
    {
        ResetingDay();
        CheckingTheDay();
        GameOver();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && haveEnoguhAcorns)
        {
            InTheCave = true;
            Debug.Log("We are in the cave");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" )
        {
            InTheCave = false;
        }
    }

    private void DayChecking(bool first, bool second, bool third, bool fourth, bool fifth, bool all)
    {
        _1stDayAmountOfAcorn = first;
        _2ndDayAmountOfAcorn = second;
        _3rdDayAmountOfAcorn = third;
        _4thDayAmountOfAcorn = fourth;
        _5thDayAmountOfAcorn = fifth;
        haveEnoguhAcorns = all;
    }

    private void CheckingDaysPassed(bool first, bool second, bool third, bool fourth, bool fifth)
    {
        _1stDayPassed = first;
        _2ndDayPassed = second;
        _3rdDayPassed = third;
        _4thDayPassed = fourth;
        _5thDayPassed = fifth;
    }

    private void CheckingTheDay()
    {
        if (HaveAcorn.HasAcorn())
        {
            if (HaveAcorn.NutCount == FirstDayAcorn && !_1stDayPassed)
            {
               DayChecking(true,false,false,false,false,true);
                CheckingDaysPassed(true,false,false,false,false);
                Debug.Log("we have for first day");
            }
            else if(HaveAcorn.NutCount == SecondDayAcorn && _1stDayPassed)
            {
                DayChecking(false, true, false, false, false,true);
                CheckingDaysPassed(true, true, false, false, false);
                Debug.Log("we have for second day");
            }
            else if (HaveAcorn.NutCount == ThirdDayAcorn && _2ndDayPassed)
            {
                DayChecking(false, false, true, false, false,true);
                CheckingDaysPassed(true, true, true, false, false);
                Debug.Log("we have for third day");
            }
            else if (HaveAcorn.NutCount == FourthDayAcorn && _3rdDayPassed)
            {
                DayChecking(false, false, false, true, false,true);
                CheckingDaysPassed(true, true, true, true, false);
                Debug.Log("we have for fourth day");
            }
            else if (HaveAcorn.NutCount == FifthDayAcorn && _4thDayPassed)
            {
                DayChecking(false, false, false, false, true, true);
                CheckingDaysPassed(true, true, true, true, true);
                Debug.Log("we have for fifth day");
            }
        }
    }

    private void ResetingDay()
    {
        if (!Cycle.NightHasCome && Cycle.DaysHasPassed == 0 && _1stDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            Debug.Log("1st day");
            DayChecking(false, false, false, false, false, false);
            maxAmount = SecondDayAcorn;
        }
        else if (!Cycle.NightHasCome && Cycle.DaysHasPassed == 1 && _2ndDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            Debug.Log("2nd day");
            DayChecking(false, false, false, false, false, false);
            maxAmount = ThirdDayAcorn;

        }
        else if(!Cycle.NightHasCome && Cycle.DaysHasPassed == 2 && _3rdDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            Debug.Log("3rd day");
            DayChecking(false, false, false, false, false, false);
            maxAmount = FourthDayAcorn;

        }
        else if (!Cycle.NightHasCome && Cycle.DaysHasPassed == 3 && _4thDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            Debug.Log("4th day");
            DayChecking(false, false, false, false, false, false);
            maxAmount = FifthDayAcorn;

        }
        else if (!Cycle.NightHasCome && Cycle.DaysHasPassed == 4 && _5thDayAmountOfAcorn && InTheCave)
        {
            DayChecking(false, false, false, false, false, false);
            HaveAcorn.NutCount = 0;
            Debug.Log("5th day");
            Debug.Log("you win");
        }
    }

    void GameOver()
    {
        if (!InTheCave && !haveEnoguhAcorns && Cycle.NightHasCome)
        {
            Debug.Log("game overs");
            Time.timeScale = 0;
        }
    }
}
