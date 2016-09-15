using System;
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts;
using Assets.Scripts.IO.Tiled;
using Assets.Scripts.Procedural.Darkness;
using Assets.Scripts.Procedural.Trees;
using Debug = UnityEngine.Debug;


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
    public GameObject DarknessPrefab;

    [Header("Other")]
    public GameObject Player;

    //Data containers for the instantiated objects
    private List<TreeData> _treesData;
    private List<PlatformData> _platformsData;
    private List<GameObject> _platforms;
    private List<ProceduralTreeData> _proceduralTreesData;
    private List<DarknessData> _darknessPaths;

    //Collections of all the instantiated gameobjects
    private List<GameObject> _trees;
    private List<GameObject> _proceduralTrees;
    private List<GameObject> _acorns;
    private List<GameObject> _darkness;


    //Other Data Containers
    private TmxMap _map;
    private Vector3 _playerSpawn;

    //Other information
    public static Vector2 MapSize;


    //Initialization
    void Start ()
	{
	    CreateLevel();
	    DebugLevel();
	}


    //Gets a TmxMap object from either a given Tmx file (priority, but has to be passed as a txt file)
    //or a file location and starts interpreting the contained data
    private void CreateLevel()
    {
        if (TmxFile != null)
        {
            _map = TmxReader.ReadLevel(TmxFile);
        }
        else
        {
            _map = TmxReader.ReadLevel(TmxFileName);
        }
        if (_map == null)
        {
            Debug.Log("NO LEVEL WAS LOADED!");
        }
        else
        {
            Debug.Log("Level was successfully loaded");

            MapSize = new Vector2(_map.Width * _map.TileWidth, _map.Height * _map.TileHeight);

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
            
            if (DarknessPrefab != null) CreateDarknessPaths();
            else Debug.Log("No Darkness Prefabs were specified");
            
        }
    }

    private void CreateDarknessPaths()
    {
        _darkness = new List<GameObject>();
        _darknessPaths = new List<DarknessData>();

        const int spacing = 6;
        const int zDist = -5;


        Vector2 darknessIntervals = new Vector2(MapSize.x / spacing, MapSize.y / spacing);

        List<Vector3> darknessSpawnPoints = new List<Vector3>();
        
        for (int i = 0; i <= spacing; i++)
        {
            darknessSpawnPoints.Add(new Vector3(0, darknessIntervals.y * i, zDist));
            darknessSpawnPoints.Add(new Vector3(MapSize.x, darknessIntervals.y * i, zDist));
        }
        

        Utility.DebugArray(darknessSpawnPoints.ToArray(), "DarknessSpawnPoints:");

        foreach (var spawnPoint in darknessSpawnPoints)
        {
            //_darknessPaths.Add(new DarknessData(spawnPoint, new Vector3(MapSize.x/2, MapSize.y/2, zDist)));
            _darknessPaths.Add(new DarknessData(spawnPoint, new Vector3(MapSize.x/2, spawnPoint.y, zDist), 3000, 10));
        }

        foreach (var path in _darknessPaths)
        {
            GameObject instantiatedDarkness = Instantiate(DarknessPrefab, path.Start(), Quaternion.identity) as GameObject;
            _darkness.Add(instantiatedDarkness);
            if (instantiatedDarkness != null)
            {
                instantiatedDarkness.SendMessage("Create", path);
            }
        }
    }

    //Gets a TmxObject from the layer called "playerspawn" and if only 1 has been found,
    //will create a spawning position for the player there
    private void SetPlayerSpawn()
    {
        foreach (var layer in _map.TmxObjectLayers)
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

    //Builds a data tree of data for the trees defined in the format:
    //LayerName: "SliceTrees#" ObjectName: "Tree#_Slice#"
    //After data tree has been compiled successfully, the trees are instantiated and build procedurally
    private void CreateProceduralTrees()
    {
        _proceduralTreesData = new List<ProceduralTreeData>();
        _proceduralTrees = new List<GameObject>();

        List<int> treesLayers = new List<int>();
        List<string> treesNames = new List<string>();

        List<List<TmxObject>> treesRawData = new List<List<TmxObject>>(); //Represents the trees

        //Splices the Objects up into layers that belong together
        foreach (var layer in _map.TmxObjectLayers)
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

        //Combines the tree data into neat packets that are self-contained and easy to handle
        for (int index = 0; index < treesRawData.Count; index++)
        {
            _proceduralTreesData.Add(new ProceduralTreeData(treesRawData[index], treesLayers[index], treesNames[index]));
        }

        //Instantiates and creates the trees procedurally
        foreach (ProceduralTreeData treeData in _proceduralTreesData)
        {
            Vector3 location = treeData.GetStartPosition();
            GameObject tree = Instantiate(ProceduralTreeRenderer, location, Quaternion.identity) as GameObject;
            if (tree != null)
            {
                tree.SendMessage("Create", treeData); //This is the part that triggers the procedural instantiation
                _proceduralTrees.Add(tree);
            }
        }
    }


    //Instantiates acorn prefabs on the positions of every object in the "acorns" layer
    private void CreateAcorns()
    {
        _acorns = new List<GameObject>();

        foreach (var layer in _map.TmxObjectLayers)
        {
            string name = layer.Name;
            if (name.Contains("acorns"))
            {
                if (layer.TmxObjects != null)
                {
                    foreach (var acorn in layer.TmxObjects)
                    {
                        Vector3 acornPos = new Vector3(acorn.X, acorn.Y, 5);
                        GameObject instantiatedAcorn = Instantiate(AcornPrefab, acornPos, Quaternion.identity) as GameObject;
                        if (instantiatedAcorn != null)
                        {
                            instantiatedAcorn.transform.parent = transform;
                            _acorns.Add(instantiatedAcorn);
                        }
                    }
                }
            }
        }
    }

    //Instantiates tree prefabs on every Object in the "treelayer"
    private void CreateTrees()
    {
        _trees = new List<GameObject>();
        _treesData = new List<TreeData>();

        //Compiling data
        foreach (var layer in _map.TmxObjectLayers)
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

        //Instantiation
        foreach (var treeData in _treesData)
        {
            GameObject instantiatedTree = Instantiate(TreePrefab, treeData.GetPosition(), treeData.GetRotation()) as GameObject;
            _trees.Add(instantiatedTree);
        }
    }

    //Creates procedurally generated platforms from TMX data in the platforms layer
    private void CreatePlatforms()
    {
        _platforms = new List<GameObject>();
        _platformsData = new List<PlatformData>();

        //Compiling data
        foreach (var layer in _map.TmxObjectLayers)
        {
            if (layer.Name.ToLower().Contains("platforms"))
            {
                foreach (TmxObject platform in layer.TmxObjects)
                {
                    _platformsData.Add(new PlatformData(platform));
                }
            }
        }

        //Instantiation
        foreach (var platformData in _platformsData)
        {
            Vector3 platformPosition = new Vector3(platformData.GetStartPos().x, - platformData.GetStartPos().y + MapSize.y, platformData.GetScreenOffSet()-1);
            GameObject platform = Instantiate(PlatformRenderer, platformPosition, Quaternion.identity) as GameObject;

            platform.SendMessage("Create", platformData);
            _platforms.Add(platform);

            platform.transform.parent = transform;
        }
    }


    //Debugs prints out all of the level info
    private void DebugLevel()
    {
        if(_map!=null) _map.PrintInfo();
    }



    // Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyDown(KeyCode.L))
	    {
	        RebuildLevel();
	    }

	    foreach (var path in _darknessPaths)
	    {
	        Debug.DrawLine(path.Start(), path.End());
	    }
        
	}

    //Is called when the tester wants to change the level on the fly and presses "L"
    private void RebuildLevel()
    {
        DestroyAllGameData();
        Start();
    }


    //Destroys all game objects in the level
    //TODO: ADDING THE NEW OBJECTS AND CHECKING FOR COMPLETION!!!
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

    //Destroys all gameobjects in the given list
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

    //Empties the entire list
    private void EmptyList<T>(List<T> list)
    {
        if (list != null && list.Count > 0)
        {
            list.RemoveRange(0, list.Count - 1);
        }
    }
}
