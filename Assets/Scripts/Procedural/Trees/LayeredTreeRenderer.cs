using System.Collections.Generic;
using UnityEngine;

public class LayeredTreeRenderer : MonoBehaviour
{
    public GameObject SliceRenderer;
    private LayeredTreeData _data;
    private List<GameObject> _slices;


	void Start ()
	{
        //Keep empty!
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

    public void Create(LayeredTreeData data)
    {
        _data = data;
        _slices = new List<GameObject>();


        for (int index = 0; index < _data.GetSlices().Count; index++)
        {

            var treeSlice = _data.GetSlices()[index];

            GameObject slice = Instantiate(SliceRenderer,
                new Vector3(0, 0, -treeSlice.GetSliceNum() * 20),
                Quaternion.AngleAxis(180, Vector3.forward)) as GameObject;

            slice.SendMessage("Create", treeSlice);
            _slices.Add(slice);
        }
    }
}
