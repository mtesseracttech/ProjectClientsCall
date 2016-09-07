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

    public PlatformData(TmxObject data)
    {
        _name = data.Name;
        _2DPosition = new Vector2(data.X, data.Y);
        _id = data.Id;
        _vertices = TiledParsingHelper.TiledPolyParser(data.Poly.Points, (int)_2DPosition.y);
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
}
