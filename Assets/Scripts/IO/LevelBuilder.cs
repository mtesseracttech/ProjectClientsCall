using UnityEngine;
using System.Collections;
using System.IO;
using Assets.Scripts.IO.Tiled;

public class LevelBuilder : MonoBehaviour
{
    public TextAsset TmxFile;
    private TmxMap map;

	void Start ()
	{
	    map = TmxReader.ReadLevel(TmxFile);
        map.PrintInfo();

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
