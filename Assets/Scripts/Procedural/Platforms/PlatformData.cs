using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformData
{
    private readonly string _name;
    private readonly int _id;
    private readonly Vector2 _2DPosition;
    private readonly Vector2[] _vertices;

    private readonly float _depth;
    private readonly float _screenOffSet;

    public PlatformData(TmxObject data)
    {
        _name = data.Name;
        _2DPosition = new Vector2(data.X, data.Y);
        _id = data.Id;
        _vertices = TiledParsingHelper.TiledPolyParser(data.Poly.Points, (int)_2DPosition.y);


        //Inverts the polygons in case they are inverted
        if (MeshHelper.IsPolyClockWise(_vertices))
        {
            _vertices = MeshHelper.InvertPolygon(_vertices);
        }

        if (data.ObjectProperties != null && data.ObjectProperties.Properties != null)
        {
            var properties = data.ObjectProperties.Properties;

            foreach (var property in properties)
            {
                switch (property.Name.ToLower())
                {
                    case "depth":
                        try
                        {
                            _depth = float.Parse(property.Value);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Depth could not be loaded!\n" + ex);
                        }
                        break;
                    case "screenoffset":
                        try
                        {
                            _screenOffSet = float.Parse(property.Value);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Screen offset could not be loaded!\n" + ex);
                        }
                        break;
                    default:
                        Debug.Log("Unknown property was loaded: " + property.Name);
                        break;
                }
            }
        }

        //Default values in case nothing is specified

        if (_depth == 0)
        {
            _depth = 2;
        }
        if (_screenOffSet == 0)
        {
            _screenOffSet = 0;
        }
        
    }

    public void PrintData()
    {
        Debug.Log("Platform Information:\n" +
                  "Name: " + _name + "\n" +
                  "ID: " + _id + "\n"
        );
    }

    public Vector2 GetStartPos()
    {
        return _2DPosition;
    }

    public Vector2[] GetVertices()
    {
        return _vertices;
    }

    public int GetID()
    {
        return _id;
    }

    public string GetName()
    {
        return _name;
    }

    public float GetDepth()
    {
        return _depth;
    }

    public float GetScreenOffSet()
    {
        return _screenOffSet;
    }
}
