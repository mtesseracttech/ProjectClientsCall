using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.IO.Tiled;

public class LevelBuilder : MonoBehaviour
{
    public TextAsset TmxFile;
    private TmxMap map;
    private List<LayeredTreeData> _treesData;
    public GameObject LayeredTree;

	void Start ()
	{
	    CreateLevel();

	    DebugLevel();
	}

    private void CreateLevel()
    {
        map = TmxReader.ReadLevel(TmxFile);

        CreateTrees();
    }

    private void CreateTrees()
    {
        _treesData = new List<LayeredTreeData>();

        foreach (var layer in map.TmxObjectLayers)
        {
            if (layer.Name.ToLower().Contains("tree"))
            {
                _treesData.Add(new LayeredTreeData(layer, layer.Name));
            }
        }

        foreach (var treeData in _treesData)
        {
            GameObject tree = Instantiate(LayeredTree, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            if(tree != null) tree.SendMessage("Create", treeData);
        }
    }















    private void DebugLevel()
    {
        map.PrintInfo();

        string treeDebug = "Layered Tree Debug Info: \n\n";
        foreach (var layeredTree in _treesData)
        {
            treeDebug += ("* Tree: " + layeredTree.GetName()+ "\n");

            foreach (var slice in layeredTree.GetSlices())
            {
                treeDebug += ("> TreeSlice: " + slice.GetSliceNum() +
                          ", Slice Vertexes: " + slice.GetVertexes().Length +
                          ", Starting Pos: " + slice.GetStartPos()+
                "\n");
            }
            treeDebug += "\n";
        }
        Debug.Log(treeDebug);
    }



    // Update is called once per frame
	void Update ()
	{

	}
}
