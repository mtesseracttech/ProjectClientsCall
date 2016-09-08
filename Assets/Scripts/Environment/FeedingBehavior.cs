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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && haveEnoguhAcorns)
        {
            InTheCave = true;
            Debug.Log("We are in the cave");
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

    private void CheckingTheDay()
    {
        if (HaveAcorn.GotAcorn)
        {
            if (HaveAcorn.NutCount == FirstDayAcorn)
            {
               DayChecking(true,false,false,false,false,true);
                Debug.Log("we have for first day");
            }
            else if(HaveAcorn.NutCount == SecondDayAcorn)
            {
                DayChecking(false, true, false, false, false,true);
                Debug.Log("we have for second day");
            }
            else if (HaveAcorn.NutCount == ThirdDayAcorn)
            {
                DayChecking(false, false, true, false, false,true);
                Debug.Log("we have for third day");
            }
            else if (HaveAcorn.NutCount == FourthDayAcorn)
            {
                DayChecking(false, false, false, true, false,true);
                Debug.Log("we have for fourth day");
            }
            else if (HaveAcorn.NutCount == FifthDayAcorn)
            {
                DayChecking(false, false, false, false, true, true);
                Debug.Log("we have for fifth day");
            }
            else
            {
                DayChecking(false, false, false, false, false, false);
            }
        }
    }

    private void ResetingDay()
    {
        if (!Cycle.NightHasCome && Cycle.DaysHasPassed == 0 && _1stDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            DayChecking(false, false, false, false, false, true);
            Debug.Log("1st day");
            maxAmount = SecondDayAcorn;

        }
        else if (!Cycle.NightHasCome && Cycle.DaysHasPassed == 1 && _2ndDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            DayChecking(false, false, false, false, false, true);
            Debug.Log("2nd day");
            maxAmount = ThirdDayAcorn;
        }
        else if(!Cycle.NightHasCome && Cycle.DaysHasPassed == 2 && _3rdDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            DayChecking(false, false, false, false, false, true);
            Debug.Log("3rd day");
            maxAmount = FourthDayAcorn;
        }
        else if (!Cycle.NightHasCome && Cycle.DaysHasPassed == 3 && _4thDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            DayChecking(false, false, false, false, false, true);
            Debug.Log("4th day");
            maxAmount = FifthDayAcorn;
        }
        else if (!Cycle.NightHasCome && Cycle.DaysHasPassed == 4 && _5thDayAmountOfAcorn && InTheCave)
        {
            HaveAcorn.NutCount = 0;
            DayChecking(false, false, false, false, false, true);
            Debug.Log("5th day");
        }
    }
}
