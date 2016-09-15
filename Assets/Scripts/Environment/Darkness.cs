using UnityEngine;
using System.Collections;
using Environment;

public class Darkness : MonoBehaviour
{

    public GameObject darknessLeft;
    public GameObject darknessRight;
    private GameObject player;
    private DayNightCycles cycle;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cycle = player.GetComponent(typeof(DayNightCycles)) as DayNightCycles;
    }

    void MoveTowardsPlayer()
    {
        
    }

}
