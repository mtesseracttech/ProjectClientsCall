
using System;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.Scripts.IO.Tiled;
using Assets.Scripts.Procedural.Trees;

public class LevelBuilder : MonoBehaviour
{
    [Header("File Input(TMX File has priority)")]
    public TextAsset TmxFile;
    public String TmxFileName;

    [Header("Renderer GameObjects")]
    public GameObject PlatformRenderer;

    [Header("Prefabs")]
    public GameObject TreePrefab;
    public GameObject AcornPrefab;

    private TmxMap map;
    private List<GameObject> _trees;
    private List<TreeData> _treesData;
    private List<GameObject> _acorns;
    private List<GameObject> _platforms;
    private List<PlatformData> _platformsData;
    private Vector2 _mapSize;



    void Start ()
	{
	    CreateLevel();

	    DebugLevel();
	}

    private void CreateLevel()
    {
        if (TmxFile != null)
        {
            map = TmxReader.ReadLevel(TmxFile);
        }
        else
        {
            map = TmxReader.ReadLevel(TmxFileName);
        }
        if (map == null)
        {
            Debug.Log("NO LEVEL WAS LOADED!");
        }
        else
        {
            _mapSize = new Vector2(map.Width * map.TileWidth, map.Height * map.TileHeight);
            //Debug.Log(_mapSize);

            if(PlatformRenderer != null) CreatePlatforms();
            else Debug.Log("No PlatformRenderer was specified!");

            if(TreePrefab != null) CreateTrees();
            else Debug.Log("No Tree prefab was specified!");

            if(AcornPrefab != null) CreateAcorns();
            else Debug.Log("No Acorn Prefab was specified!");
        }
    }

    private void CreateAcorns()
    {
        _acorns = new List<GameObject>();

        foreach (var layer in map.TmxObjectLayers)
        {
            string name = layer.Name;
            if (name.Contains("acorns"))
            {
                if (layer.TmxObjects != null)
                {
                    foreach (var acorn in layer.TmxObjects)
                    {
                        Vector3 acornPos = new Vector3(acorn.X, acorn.Y, 5);
                        GameObject instantiatedTree = Instantiate(AcornPrefab, acornPos, Quaternion.identity) as GameObject;
                        _acorns.Add(instantiatedTree);
                    }
                }
            }
        }
    }

    private void CreateTrees()
    {
        _trees = new List<GameObject>();
        _treesData = new List<TreeData>();

        foreach (var layer in map.TmxObjectLayers)
        {
            string name = layer.Name;
            if (name.Contains("treelayer"))
            {
                int layerNumber = TiledParsingHelper.RetrieveNumFromString(name);
                if (layer.TmxObjects != null)
                {
                    foreach (var tree in layer.TmxObjects)
                    {
                        _treesData.Add(new TreeData(tree, layerNumber));
                    }
                }
            }
        }

        foreach (var treeData in _treesData)
        {
            GameObject instantiatedTree = Instantiate(TreePrefab, treeData.GetPosition(), treeData.GetRotation()) as GameObject;
            _trees.Add(instantiatedTree);
        }
    }

    private void CreatePlatforms()
    {
        _platforms = new List<GameObject>();
        _platformsData = new List<PlatformData>();

        foreach (var layer in map.TmxObjectLayers)
        {
            if (layer.Name.ToLower().Contains("platforms"))
            {
                foreach (TmxObject platform in layer.TmxObjects)
                {
                    _platformsData.Add(new PlatformData(platform));
                }
            }
        }

        foreach (var platformData in _platformsData)
        {
            Vector3 platformPosition = new Vector3(platformData.GetStartPos().x, - platformData.GetStartPos().y + _mapSize.y, platformData.GetScreenOffSet());
            GameObject platform = Instantiate(PlatformRenderer, platformPosition, Quaternion.identity) as GameObject;
            platform.SendMessage("Create", platformData);
            _platforms.Add(platform);
        }
    }


    private void DebugLevel()
    {
        if(map != null) map.PrintInfo();
    }



    // Update is called once per frame
	void Update ()
	{

	}
}
