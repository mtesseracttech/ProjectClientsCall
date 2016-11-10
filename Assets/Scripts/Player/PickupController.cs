using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickupController : MonoBehaviour
{
    public GameObject DummyAcornPrefab;
    public Vector2 HUDAcornPosition;

    private List<GameObject> DummyAcorns;

	// Use this for initialization
	void Start ()
    {
        DummyAcorns = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(DummyAcorns.Count > 0)
        {
            Vector3 ScreenSpacePosition = Camera.main.ScreenToWorldPoint(new Vector3(HUDAcornPosition.x, HUDAcornPosition.y, 10));
            for (int i = DummyAcorns.Count -1; i >= 0; i--)
            {
                DummyAcorns[i].transform.position = Vector3.Lerp(DummyAcorns[i].transform.position, ScreenSpacePosition, 1);
                if(Vector3.Distance(DummyAcorns[i].transform.position, ScreenSpacePosition) <= 0.01f)
                {
                    Destroy(DummyAcorns[i]);
                    DummyAcorns.RemoveAt(i);
                }
            }
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.transform.CompareTag("Acorn"))
        {
            Transform originalTransform = col.transform;
            Destroy(col.gameObject);
            GameObject acorn = Instantiate(DummyAcornPrefab, originalTransform) as GameObject;
            DummyAcorns.Add(acorn);
        }
    }
}
