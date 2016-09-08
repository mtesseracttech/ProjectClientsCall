using System;
using System.Collections.Generic;
using UnityEngine;

public class TiledParsingHelper
{

    public static Vector2[] TiledPolyParser(string inputPolyString, int startPos)
    {
        List<Vector2> vertices = new List<Vector2>();
        string[] vectorStrings = inputPolyString.Split(' ');
        foreach (string vectorString in vectorStrings)
        {
            string[] coords = vectorString.Split(',');
            try
            {
                vertices.Add(new Vector2(float.Parse(coords[0]), float.Parse(coords[1])));
            }
            catch (Exception ex)
            {
                Debug.Log("Failed to load the vector: " + coords[0] + " " + coords[1] + "\n" + ex);
            }
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = new Vector2(vertices[i].x, -vertices[i].y);
        }

        return vertices.ToArray();
    }

    public static int RetrieveNumFromString(string input)
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
            throw new Exception("No valid number was found in " + input);
        }
        return val;
    }
}
