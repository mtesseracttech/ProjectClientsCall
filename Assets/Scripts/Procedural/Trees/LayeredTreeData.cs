using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class LayeredTreeData
{
    private List<TreeSlice> _treeSlices;
    private TmxObjectLayer _tree;
    private string _treeName;

    public LayeredTreeData(TmxObjectLayer tree, string name)
    {
        _treeName = name;
        _tree = tree;
        _treeSlices = new List<TreeSlice>();

        for (int index = 0; index < _tree.TmxObjects.Length; index++)
        {
            TmxObject slice = _tree.TmxObjects[index];
            Debug.Log(_tree.TmxObjects);

            slice.Name = slice.Name.ToLower();
            if (slice.Name.Contains("slice"))
            {
                List<Vector2> vectors = new List<Vector2>();
                string[] vectorStrings = slice.Poly.Points.Split(' ');
                foreach (string vectorString in vectorStrings)
                {
                    string[] coords = vectorString.Split(',');
                    try
                    {
                        vectors.Add(new Vector2(float.Parse(coords[0]), float.Parse(coords[1])));
                    }
                    catch (Exception)
                    {
                        Debug.Log("Failed to load the vector: " + coords[0] + " " + coords[1]);
                    }
                }

                int sliceNum = RetrieveSliceNum(slice.Name);
                _treeSlices.Add(new TreeSlice(vectors, sliceNum, new Vector2(slice.X, slice.Y)));
            }
            else
            {
                Debug.Log("Unknown Layer has been found in tree");
            }
        }
    }

    public List<TreeSlice> GetSlices()
    {
        return _treeSlices;
    }

    public string GetName()
    {
        return _treeName;
    }

    private static int RetrieveSliceNum(string input)
    {
        string outputString = string.Empty;
        int val;

        for (int i = 0; i < input.Length; i++)
        {
            if (Char.IsDigit(input[i]) || i == '-')
                outputString += input[i];
        }

        if (outputString.Length > 0)
        {
            val = int.Parse(outputString);
        }
        else
        {
            throw new Exception("No valid slicenumber was found for: " + input);
        }
        return val;
    }


}

public class TreeSlice
{
    private readonly Vector2[] _vertexes;
    private readonly int _sliceNum;
    private readonly Vector2 _startPos;

    public TreeSlice(List<Vector2> vertices, int layer, Vector2 position)
    {
        _vertexes = vertices.ToArray();
        _sliceNum = layer;
        _startPos = position;
    }

    public Vector2[] GetVertices()
    {
        return _vertexes;
    }

    public int GetSliceNum()
    {
        return _sliceNum;
    }

    public Vector2 GetStartPos()
    {
        return _startPos;
    }
}
