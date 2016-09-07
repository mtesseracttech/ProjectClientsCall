﻿using System.Collections.Generic;
using UnityEngine;

public class MeshHelper
{
    public static Mesh CombineMeshes(Mesh[] meshes, string newName = "Combined Mesh")
    {
        CombineInstance[] combine = new CombineInstance[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            combine[i].mesh = meshes[i];
        }

        Mesh returnMesh = new Mesh();
        returnMesh.CombineMeshes(combine, true);
        returnMesh.name = newName;

        returnMesh.RecalculateNormals();
        returnMesh.RecalculateBounds();

        return returnMesh;
    }


    private static Mesh CreateTestCube()
    {
        /*  1 2
        L1: 0 3

            5 6
        L2: 4 7
        */

        List<Vector3> vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(0, 1, 0));
        vertices.Add(new Vector3(0, 1, 1));
        vertices.Add(new Vector3(0, 0, 1));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(1, 1, 0));
        vertices.Add(new Vector3(1, 1, 1));
        vertices.Add(new Vector3(1, 0, 1));

        List<int> indices = new List<int>();
        indices.AddRange(MeshConverter.QuadToTri(0, 1, 2, 3)); //front
        indices.AddRange(MeshConverter.QuadToTri(7, 6, 5, 4)); //back
        indices.AddRange(MeshConverter.QuadToTri(0, 3, 7, 4)); //bottom
        indices.AddRange(MeshConverter.QuadToTri(1, 5, 6, 2)); //top
        indices.AddRange(MeshConverter.QuadToTri(0, 4, 5, 1)); //left
        indices.AddRange(MeshConverter.QuadToTri(3, 2, 6, 7)); //right

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }


    public static void DebugArray<T>(T[] array)
    {
        if (array == null)
        {
            Debug.Log("Array is null");
        }
        else if (array.Length < 1)
        {
            Debug.Log("Array is null");
        }
        else
        {
            string debugString = "Array Debug\n";
            foreach (var o in array)
            {
                debugString += o + "\n";
            }

            Debug.Log(debugString);
        }
    }
}