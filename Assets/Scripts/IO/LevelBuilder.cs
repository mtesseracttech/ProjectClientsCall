
using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.IO.Tiled;

public class LevelBuilder : MonoBehaviour
{
    [Header("File Input(TMX File has priority)")]
    public TextAsset TmxFile;
    public String TmxFileName;

    [Header("Renderer GameObjects")]
    public GameObject PlatformRenderer;


    private TmxMap map;
    private List<GameObject> _trees;
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
            Debug.Log(_mapSize);

            CreatePlatforms();
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
            Vector3 platformPosition = new Vector3(platformData.GetStartPos().x, - platformData.GetStartPos().y + _mapSize.y, 0);
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
