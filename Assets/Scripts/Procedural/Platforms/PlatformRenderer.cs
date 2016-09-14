using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlatformRenderer : MonoBehaviour
{
    private PlatformData _data;
    public Material SideMaterial;
    public Material TopMaterial;

    public void Create(PlatformData data)
    {
        _data = data;
        SetInfo();
        CreateModel();
    }

    private void SetInfo()
    {
        name = _data.GetName();
        try
        {
            tag = "platform";
        }
        catch (Exception ex)
        {
            Debug.Log("The tag 'platform' needs to be created in the inspector!\n" + ex);
        }

    }

    private void CreateModel()
    {
        //Front/////////////////////////

        Vector2[] verticesFront = _data.GetVertices();

        Mesh meshFront = CreateMeshFromPoly(verticesFront);

        meshFront.uv = verticesFront;


        //Back//////////////////////////

        Vector2[] verticesBack = new Vector2[verticesFront.Length];

        for (int i = 0; i < verticesBack.Length; i++)
        {
            verticesBack[i] = new Vector2(verticesFront[verticesBack.Length - i -1].x, verticesFront[verticesBack.Length - i -1].y);
        }


        Mesh meshBack = CreateMeshFromPoly(verticesBack, _data.GetDepth());
        meshBack.triangles = meshBack.triangles.Reverse().ToArray();

        meshBack.uv = verticesBack;


        //Between///////////////////////

        Mesh meshBetween = CreateBetweenMesh(meshFront.vertices, _data.GetDepth());


        //Stitching/////////////////////

        Mesh meshFinal = MeshHelper.CombineMeshesMultiMaterial(new Mesh[] {meshFront, meshBack, meshBetween});

        List<Vector3> verticesFinal = new List<Vector3>();
        verticesFinal.AddRange(meshFront.vertices);
        verticesFinal.AddRange(meshBack.vertices);
        verticesFinal.AddRange(meshBetween.vertices);

        meshFinal.vertices = verticesFinal.ToArray();

        meshFinal.RecalculateNormals();
        meshFinal.RecalculateBounds();

        
        //Add the Renderer and Filter///
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>();
        gameObject.GetComponent<MeshFilter>().sharedMesh = meshFinal;
        gameObject.AddComponent<MeshCollider>();

        //Texturing/////////////////////
        //Submeshes get textured in same oreder they were added to the main mesh
        GetComponent<MeshRenderer>().materials = new[]
        {
            SideMaterial,
            SideMaterial,
            TopMaterial
        };
    }



    //Creates a mesh from the front vertices to an offset, intended to connect the front vertices to the back vertices
    private Mesh CreateBetweenMesh(Vector3[] frontVertices, float offsetDepth)
    {
        //Container declarations///////////
        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        //PREPARATION://///////////////////

        vertices.AddRange(frontVertices);

        //Creating the back vertices
        foreach (Vector3 fV in frontVertices)
        {
            vertices.Add(new Vector3(fV.x, fV.y, fV.z + offsetDepth)); // is the offset
        }

        //Creating the Indices from Quads//

        int offs = frontVertices.Length;
        for (int i = 0; i < frontVertices.Length -1; i++)
        {
            indices.AddRange(MeshConverter.QuadToTri(
                    i, i+offs, i + 1 + offs, i+1
            ));
        }

        indices.AddRange(MeshConverter.QuadToTri(frontVertices.Length - 1, frontVertices.Length - 1 + frontVertices.Length,
            frontVertices.Length, 0 ));
        

        //FINALIZATION:////////////////////

        //Implementation for proper quads with own vertices and easier to work with order/
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

        //Doing the last vertices seperately because wrapping is not working since
        //there are 2 different points where the wrapping is needed at the same time.

        finalVertices.Add(new Vector3(vertices[frontVertices.Length - 1].x, vertices[frontVertices.Length - 1].y, vertices[frontVertices.Length - 1].z));
        finalVertices.Add(new Vector3(vertices[frontVertices.Length - 1 + frontVertices.Length].x, vertices[frontVertices.Length - 1 + frontVertices.Length].y, vertices[frontVertices.Length - 1 + frontVertices.Length].z));
        finalVertices.Add(new Vector3(vertices[frontVertices.Length].x, vertices[frontVertices.Length].y, vertices[frontVertices.Length].z));
        finalVertices.Add(new Vector3(vertices[0].x, vertices[0].y, vertices[0].z));

        finalIndices.AddRange(MeshConverter.QuadToTri(finalIndiceIterator++, finalIndiceIterator++, finalIndiceIterator++, finalIndiceIterator++));


        //Setting the UVs per 4 vertices/////

        for (int i = 0; i < finalVertices.Count; i += 4)
        {
            uvs.Add(new Vector2(finalVertices[i].x, 0));
            uvs.Add(new Vector2(finalVertices[i].x, 1));
            uvs.Add(new Vector2(finalVertices[i].x + Vector3.Distance(finalVertices[i], finalVertices[i + 2]), 1));
            uvs.Add(new Vector2(finalVertices[i].x + Vector3.Distance(finalVertices[i], finalVertices[i + 2]), 0));
        }

        //Creating and Populating the Mesh////

        Mesh mesh = new Mesh();
        mesh.vertices = finalVertices.ToArray();
        mesh.triangles = finalIndices.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }



    private Mesh CreateMeshFromPoly(Vector2[] vertices2D, float depth = 0)
    {
        //Using the Triangulator to get Triangle Indices from the non-complex polygon

        Triangulator tr = new Triangulator(vertices2D);
        int[] triangles = tr.Triangulate();


        //Creating the vertices

        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, depth);
        }


        //Creating and Populating the Mesh////

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
}
