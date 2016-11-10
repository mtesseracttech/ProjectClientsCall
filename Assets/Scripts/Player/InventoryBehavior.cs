using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class InventoryBehavior : MonoBehaviour
{
    public bool InventoryFull;
    public bool InTheNest;

    public int acornCount;  //how much we collected acorns
    public int[] _acornsPerDay = new int[5];
    public int _MaxAcornAmount;
    public bool[] _days = new bool[5];
    //check acorn amount for days
    //check what day it is
    //update UI of acorn amount

    void Start()
    {
        SetDay(1,true);
    }

    void FixedUpdate()
    {
        print("acorn nedded " + _MaxAcornAmount);
    }

    void Update()
    {
        UpdateAcornPerDay();
        CheckInventory();
    }

    void OnTriggerEnter(Collider other)
    {
        //if tag is player and have enough acorn can come in and start next day
        if (other.gameObject.CompareTag("nest") && InventoryFull)
        {
            print("we are in the nest");
            //start next day
            //transition to the next day
            //fade out fade in
            InTheNest = true;
        }
    }

    void UpdateAcornPerDay()
    {
        if (GetDay(1))
        {
            _MaxAcornAmount = _acornsPerDay[0];
        }
        else if (GetDay(2))
        {
            _MaxAcornAmount = _acornsPerDay[1];
        }
        else if (GetDay(3))
        {
            _MaxAcornAmount = _acornsPerDay[2];
        }
        else if (GetDay(4))
        {
            _MaxAcornAmount = _acornsPerDay[3];
        }
        else if (GetDay(5))
        {
            _MaxAcornAmount = _acornsPerDay[4];
        }

    }

    void CheckInventory()
    {
        if (acornCount == _MaxAcornAmount)
        {
            InventoryFull = true;
        }
        else InventoryFull = false;
    
    }

    public void SetDay(int day, bool state)
    {
        if (day >= _days.Length || day < 0) return;
        _days[day - 1] = state;
    }

    public bool GetDay(int day)
    {
        return _days[day - 1];
    }

    public int GetDay()
    {
        for (int i = 0; i < _days.Length; i++)
        {
            if (_days[i]) return i;
        }
        return -1;
    }


}
