using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Assets.Scripts.IO.Tiled;
using Assets.Scripts.Procedural.Trees;
using UnityEditor;

public class LevelBuilder : MonoBehaviour
{
    [Header("File Input(TMX File has priority)")]
    public TextAsset TmxFile;
    public string TmxFileName;

    [Header("Renderer GameObjects")]
    public GameObject PlatformRenderer;
    public GameObject ProceduralTreeRenderer;

    [Header("Prefabs")]
    public GameObject TreePrefab;
    public GameObject AcornPrefab;

    [Header("Other")]
    public GameObject Player;

    private TmxMap map;
    private Vector3 _playerSpawn;
    private List<GameObject> _trees;
    private List<TreeData> _treesData;
    private List<GameObject> _platforms;
    private List<PlatformData> _platformsData;
    private List<GameObject> _proceduralTrees;
    private List<ProceduralTreeData> _proceduralTreesData;
    private List<GameObject> _acorns;
    public static Vector2 MapSize;


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
            Debug.Log("Level was successfully loaded");

            MapSize = new Vector2(map.Width * map.TileWidth, map.Height * map.TileHeight);

            if(PlatformRenderer != null) CreatePlatforms();
            else Debug.Log("No PlatformRenderer was specified!");

            if (ProceduralTreeRenderer != null) CreateProceduralTrees();
            else Debug.Log("No ProceduralTreeRenderer was specified!");

            if(TreePrefab != null) CreateTrees();
            else Debug.Log("No Tree prefab was specified!");


            if(AcornPrefab != null) CreateAcorns();
            else Debug.Log("No Acorn Prefab was specified!");

            if (Player != null) SetPlayerSpawn();
            else Debug.Log("No Player linked was specified!");
        }
    }

    private void SetPlayerSpawn()
    {
        foreach (var layer in map.TmxObjectLayers)
        {
            if (layer.Name.ToLower().Contains("playerspawn"))
            {
                if (layer.TmxObjects != null && layer.TmxObjects.Length > 0)
                {
                    if (layer.TmxObjects.Length > 1) Debug.Log("More than one player spawn was defined in the TMX Map");
                    else
                    {
                        _playerSpawn = new Vector3(layer.TmxObjects[0].X, layer.TmxObjects[0].Y, 1);
                    }
                }
            }
        }
        if (_playerSpawn != null)
        {
            //INSERT SETTING THE PLAYER POSITION TO THIS LOCATION
        }
        else Debug.Log("No player spawn was created");
    }

    private void CreateProceduralTrees()
    {
        _proceduralTreesData = new List<ProceduralTreeData>();
        _proceduralTrees = new List<GameObject>();

        List<List<TmxObject>> treesRawData = new List<List<TmxObject>>(); //Represents the trees
        List<int> treesLayers = new List<int>();
        List<string> treesNames = new List<string>();

        foreach (var layer in map.TmxObjectLayers)
        {
            if (layer.Name.ToLower().Contains("slicetrees"))
            {
                int layerNumber = TiledParsingHelper.RetrieveNumFromString(layer.Name.ToLower());

                string currentName = "";

                List<TmxObject> tree = new List<TmxObject>();


                foreach (var slice in layer.TmxObjects)
                {
                    string[] nameParts = slice.Name.Split('_');

                    if (tree.Count > 0 && nameParts[0] != currentName)
                    {
                        treesRawData.Add(tree);
                        treesLayers.Add(layerNumber);
                        treesNames.Add(currentName);
                        tree = new List<TmxObject>();
                    }

                    tree.Add(slice);
                    currentName = nameParts[0];
                }
                if (tree.Count > 0)
                {
                    treesRawData.Add(tree);
                    treesLayers.Add(layerNumber);
                    treesNames.Add(currentName);
                }
            }
        }

        MeshHelper.DebugArray(treesNames.ToArray(), "TREE NAMES:");

        for (int index = 0; index < treesRawData.Count; index++)
        {
            _proceduralTreesData.Add(new ProceduralTreeData(treesRawData[index], treesLayers[index], treesNames[index]));
        }

        foreach (ProceduralTreeData treeData in _proceduralTreesData)
        {
            Vector3 location = treeData.GetStartPosition();
            GameObject tree = Instantiate(ProceduralTreeRenderer, location, Quaternion.identity) as GameObject;
            if (tree != null)
            {
                tree.SendMessage("Create", treeData);
                _proceduralTrees.Add(tree);
            }
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
                        Vector3 acornPos = new Vector3(acorn.X, acorn.Y, 1);
                        GameObject instantiatedacorn = Instantiate(AcornPrefab, acornPos, Quaternion.identity) as GameObject;
                        _acorns.Add(instantiatedacorn);
                        instantiatedacorn.transform.parent = transform;

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
            instantiatedTree.transform.parent = transform;
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
            Vector3 platformPosition = new Vector3(platformData.GetStartPos().x, - platformData.GetStartPos().y + MapSize.y, platformData.GetScreenOffSet());
            GameObject platform = Instantiate(PlatformRenderer, platformPosition, Quaternion.identity) as GameObject;
<<<<<<< HEAD
            platform.SendMessage("Create", platformData);
            _platforms.Add(platform);
            platform.transform.parent = transform;
=======
            if (platform != null)
            {
                platform.SendMessage("Create", platformData);
                _platforms.Add(platform);
            }
>>>>>>> 9402c6d9b63e9852d5e1df20862465798e36b585
        }
    }


    private void DebugLevel()
    {
        if(map != null) map.PrintInfo();
    }



    // Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.L))
	    {
	        RebuildLevel();
	    }
	}

    private void RebuildLevel()
    {
        DestroyAllGameData();
        Start();
    }

    private void DestroyAllGameData()
    {
        //Empty and Nullify Gameobject Lists
        EmptyList(_platformsData);

        EmptyList(_treesData);

        //Empty and Nullify other generated Data
        DestroyGameObjectList(_platforms);

        DestroyGameObjectList(_trees);

        DestroyGameObjectList(_acorns);
    }

    private void DestroyGameObjectList(List<GameObject> gameObjects)
    {
        if (gameObjects != null)
        {
            for (int index = 0; index < gameObjects.Count; index++)
            {
                Destroy(gameObjects[index]);
            }
            EmptyList(gameObjects);
        }
    }

    private void EmptyList<T>(List<T> list)
    {
        if (list != null && list.Count > 0)
        {
            list.RemoveRange(0, list.Count - 1);
        }
    }
}
