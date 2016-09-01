using System.Collections.Generic;
using UnityEngine;

public class LayeredTreeRenderer : MonoBehaviour
{
    public GameObject SliceRenderer;
    private LayeredTreeData _data;
    private List<GameObject> _slices;


	void Start ()
	{
	    _slices = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

    public void Create(LayeredTreeData data)
    {
        _data = data;

        foreach (var treeSlice in data.GetSlices())
        {
            GameObject slice = Instantiate(SliceRenderer,
                    new Vector3(treeSlice.GetStartPos().x, treeSlice.GetStartPos().y, -treeSlice.GetSliceNum() * 20),
                    Quaternion.identity) as GameObject;

            slice.SendMessage("Create", treeSlice);
            //_slices.Add(slice);
        }
    }
}
