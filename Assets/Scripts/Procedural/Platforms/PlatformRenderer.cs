using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlatformRenderer : MonoBehaviour
{
    private PlatformData _data;
    public Material PlatformMaterial;

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

        //Debug.Log(meshFront);

        //Back

        Vector2[] verticesBack = new Vector2[verticesFront.Length];

        for (int i = 0; i < verticesBack.Length; i++)
        {
            verticesBack[i] = new Vector2(verticesFront[verticesBack.Length - i -1].x, verticesFront[verticesBack.Length - i -1].y);
        }


        Mesh meshBack = CreateMesh(verticesBack, _data.GetDepth());
        meshBack.triangles = meshBack.triangles.Reverse().ToArray();

        //Between

        Mesh meshBetween = CreateBetweenMesh(meshFront.vertices, _data.GetDepth());

        //Stitching

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
        GetComponent<MeshRenderer>().material = PlatformMaterial;
    }



    //Creates a mesh from the front vertices to an offset, intended to connect the front vertices to the back vertices
    private Mesh CreateBetweenMesh(Vector3[] frontVertices, int offsetDepth)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        vertices.AddRange(frontVertices);

        //Creating the back vertices
        foreach (Vector3 fV in frontVertices)
        {
            vertices.Add(new Vector3(fV.x, fV.y, fV.z + offsetDepth)); // is the offset
        }

        //Creating the indices from quads
        //TODO : Creating new vertices for every quad

        int offs = frontVertices.Length;
        for (int i = 0; i < frontVertices.Length -1; i++)
        {
            indices.AddRange(MeshConverter.QuadToTri(
                    i, i+offs, i + 1 + offs, i+1
            ));
        }

        indices.AddRange(MeshConverter.QuadToTri(frontVertices.Length - 1, frontVertices.Length - 1 + frontVertices.Length,
            frontVertices.Length, 0 ));
        


        //Implementation for proper defined meshes
        List < Vector3 > finalVertices = new List<Vector3>();
        List<int> finalIndices = new List<int>();

        int finalIndiceIterator = 0;

        for (int i = 0; i < frontVertices.Length - 1; i++)
        {
            finalVertices.Add(new Vector3(vertices[i].x, vertices[i].y, vertices[i].z));
            finalVertices.Add(new Vector3(vertices[i + offs].x, vertices[i + offs].y, vertices[i + offs].z));
            finalVertices.Add(new Vector3(vertices[i + 1 + offs].x, vertices[i + 1 + offs].y, vertices[i + 1 + offs].z));
            finalVertices.Add(new Vector3(vertices[i + 1].x, vertices[i + 1].y, vertices[i + 1].z));

            finalIndices.AddRange(MeshConverter.QuadToTri(finalIndiceIterator++, finalIndiceIterator++, finalIndiceIterator++, finalIndiceIterator++));
        }

        finalVertices.Add(new Vector3(vertices[frontVertices.Length - 1].x, vertices[frontVertices.Length - 1].y, vertices[frontVertices.Length - 1].z));
        finalVertices.Add(new Vector3(vertices[frontVertices.Length - 1 + frontVertices.Length].x, vertices[frontVertices.Length - 1 + frontVertices.Length].y, vertices[frontVertices.Length - 1 + frontVertices.Length].z));
        finalVertices.Add(new Vector3(vertices[frontVertices.Length].x, vertices[frontVertices.Length].y, vertices[frontVertices.Length].z));
        finalVertices.Add(new Vector3(vertices[0].x, vertices[0].y, vertices[0].z));

        finalIndices.AddRange(MeshConverter.QuadToTri(finalIndiceIterator++, finalIndiceIterator++, finalIndiceIterator++, finalIndiceIterator++));

        List<Vector2> finalUVs = new List<Vector2>();

        //MeshHelper.DebugArray(finalVertices.ToArray());
        //MeshHelper.DebugArray(finalIndices.ToArray());

        Mesh mesh = new Mesh();
        mesh.vertices = finalVertices.ToArray();
        mesh.triangles = finalIndices.ToArray();
        //mesh.vertices = vertices.ToArray();
        //mesh.triangles = indices.ToArray();

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

    public PlatformData GetData()
    {
        return _data;
    }



    // Update is called once per frame
    void Update()
    {

    }
}
