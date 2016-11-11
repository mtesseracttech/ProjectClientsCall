using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelObjectManager : MonoBehaviour
{

    public GameObject LevelBuilder;
    public GameObject TimeManager;
    private TimeManager _timeManager;
    private LevelBuilder _levelBuilder;
    private List<DarknessMovement> _darknessMovements;

	// Use this for initialization
	void Start ()
	{
	    _timeManager = TimeManager.GetComponent<TimeManager>();
	    _levelBuilder = LevelBuilder.GetComponent<LevelBuilder>();
	    _darknessMovements = _levelBuilder.GetDarknessEntities();
        ManageDarkness();
	}

    private void ManageDarkness()
    {
        foreach (var movement in _darknessMovements)
        {
            movement.SetTotalTravelTime(_timeManager.TotalTimePerDay);
        }
        foreach (var movement in _darknessMovements)
        {
            movement.StartMoving();
        }
        Debug.Log("Darkness Movement Speed: " + _darknessMovements[0].GetMovementSpeed());
    }

    // Update is called once per frame
    void Update()
    {
    }

}
