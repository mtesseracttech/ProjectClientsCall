using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlatformRenderer : MonoBehaviour
{
    private PlatformData _data;

    public void Create(PlatformData data)
    {
        _data = data;
        SetInfo();
        CreateModel();
    }

    private void SetInfo()
    {
        name = _data.GetName();
    }

    private void CreateModel()
    {
        //Front
        // Create Vector2 vertices
        Vector2[] verticesFront = _data.GetVertices();

        Mesh meshFront = CreateMesh(verticesFront);

        Debug.Log(meshFront);

        //Back

        Vector2[] verticesBack = new Vector2[verticesFront.Length];

        for (int i = 0; i < verticesBack.Length; i++)
        {
            verticesBack[i] = new Vector2(verticesFront[verticesBack.Length - i -1].x, verticesFront[verticesBack.Length - i -1].y);
        }

        Mesh meshBack = CreateMesh(verticesBack, 1000);
        meshBack.triangles = meshBack.triangles.Reverse().ToArray();


        Mesh meshBetween = CreateBetweenMesh(meshFront.vertices, meshBack.vertices);

        Mesh meshFinal = MeshHelper.CombineMeshes(new Mesh[] {meshFront, meshBack, meshBetween});

        List<Vector3> verticesFinal = new List<Vector3>();
        verticesFinal.AddRange(meshFront.vertices);
        verticesFinal.AddRange(meshBack.vertices);
        verticesFinal.AddRange(meshBetween.vertices);


        meshFinal.vertices = verticesFinal.ToArray();

        meshFinal.RecalculateNormals();
        meshFinal.RecalculateBounds();

        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = meshFinal;
    }

    private Mesh CreateBetweenMesh(Vector3[] frontVertices, Vector3[] backVertices)
    {
        /*
        List<Vector3> vertices = new List<Vector3>();

        for (int i = 0; i < frontVertices.Length; i++)
        {
            vertices.Add(new Vector3(frontVertices[i].x, frontVertices[i].y, frontVertices[i].z));
            vertices.Add(new Vector3(backVertices[i].x , backVertices[i].y , backVertices[i].z ));
        }

        List<int> indices = new List<int>();
        int limit = frontVertices.Length - 1;
        for (int i = 0; i < vertices.Count; i+=2)
        {
            try
            {
                indices.AddRange(Converter.QuadToTri(
                    (i)%limit,
                    (i + 1)%limit,
                    (i + frontVertices.Length + 1)%limit,
                    (i + frontVertices.Length)%limit
                ));
            }
            catch (Exception)
            {
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;

        */

        List<Vector3> vertices = new List<Vector3>();
        vertices.AddRange(frontVertices);
        vertices.AddRange(backVertices);

        List<int> indices = new List<int>();
        int lim = vertices.Count;
        for (int i = 0; i < frontVertices.Length; i++)
        {
            indices.AddRange(Converter.QuadToTri(
            (i+0) % lim,
            (i+1) % lim,
            (i + frontVertices.Length + 1) % lim,
            (i + frontVertices.Length + 0) % lim)
            );
        }


        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private Mesh CreateMesh(Vector2[] vertices2D, int depth = 0)
    {
        // Use the triangulator to get triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] triangles = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, depth);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = triangles;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        return msh;
    }











    // Update is called once per frame
    void Update()
    {

    }















    private void DebugArray(Vector2[] array)
    {
        if (array == null && array.Length < 1)
        {
            Debug.Log("array is empty/null");
        }
        else
        {
            string debugString = "Array Debug\n";
            foreach (var o in array)
            {
                debugString += o;
            }

            Debug.Log(debugString);
        }
    }

    private void DebugArray(Vector3[] array)
    {
        if (array == null && array.Length < 1)
        {
            Debug.Log("array is empty/null");
        }
        else
        {
            string debugString = "Array Debug\n";
            foreach (var o in array)
            {
                debugString += o;
            }

            Debug.Log(debugString);
        }
    }

    private void DebugArray(int[] array)
    {
        if (array == null && array.Length < 1)
        {
            Debug.Log("array is empty/null");
        }
        else
        {
            string debugString = "Array Debug\n";
            foreach (var o in array)
            {
                debugString += o;
            }

            Debug.Log(debugString);
        }
    }
}
