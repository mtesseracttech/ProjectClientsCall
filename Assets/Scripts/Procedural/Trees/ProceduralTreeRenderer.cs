using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Procedural;
using Assets.Scripts.Procedural.Trees;

public class ProceduralTreeRenderer : MonoBehaviour
{
    public Material BarkMaterial;
    private ProceduralTreeData _data;

    public void Create(ProceduralTreeData data)
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
        //The final mesh that all othes will be added to
        Mesh meshFinal = new Mesh();

        //Front

        List<Vector3> frontVertices = new List<Vector3>();
        List<List<Vector3>> frontVerticeLists = new List<List<Vector3>>();

        for (int i = 0; i < _data.GetSlices().Length; i++)
        {
            frontVerticeLists.Add(new List<Vector3>());
            foreach (var vertice in _data.GetSlices()[i].GetVertices())
            {
                frontVertices.Add(new Vector3(
                    _data.GetRelativeStartPositions()[i].x + vertice.x,
                    vertice.y,
                    -_data.GetRelativeStartPositions()[i].z));

                frontVerticeLists[i].Add(new Vector3(
                    _data.GetRelativeStartPositions()[i].x + vertice.x,
                    vertice.y,
                    -_data.GetRelativeStartPositions()[i].z));
            }

            Mesh frontMesh = CreateMeshFromPoly(_data.GetSlices()[i].GetVertices(),
                _data.GetRelativeStartPositions()[i].z);

            frontMesh.uv = _data.GetSlices()[i].GetVertices();

            meshFinal = MeshHelper.CombineMeshesSingleMaterial(new[]
            {
                meshFinal,
                frontMesh
            });
        }

        //Back

        List<Vector3> backVertices = new List<Vector3>();
        List<List<Vector3>> backVerticeLists = new List<List<Vector3>>();

        Polygon2D[] slicesBack = new Polygon2D[_data.GetSlices().Length];

        for (int i = 0; i < _data.GetSlices().Length; i++)
        {

            List<Vector2> slice = new List<Vector2>();
            for (int j = _data.GetSlices()[i].GetVertices().Length - 1; j >= 0 ; j--)
            {
                slice.Add(new Vector2(_data.GetSlices()[i].GetVertices()[j].x, _data.GetSlices()[i].GetVertices()[j].y));
            }
            slicesBack[i] = new Polygon2D(slice);
        }

        for (int i = 0; i < slicesBack.Length; i++)
        {
            Mesh meshBack = CreateMeshFromPoly(
                slicesBack[i].GetVertices(),
                _data.GetRelativeStartPositions()[i].z
            );

            backVerticeLists.Add(new List<Vector3>());
            foreach (var vertex in meshBack.vertices)
            {
                backVertices.Add(new Vector3(
                    _data.GetRelativeStartPositions()[i].x + vertex.x,
                    vertex.y,
                    _data.GetRelativeStartPositions()[i].z));
                backVerticeLists[i].Add(new Vector3(
                    _data.GetRelativeStartPositions()[i].x + vertex.x,
                    vertex.y,
                    _data.GetRelativeStartPositions()[i].z));
            }

            //finalVertices.AddRange(meshBack.vertices);

            meshBack.triangles = meshBack.triangles.Reverse().ToArray();
            meshBack.uv = slicesBack[i].GetVertices();
            meshFinal = MeshHelper.CombineMeshesSingleMaterial(new Mesh[]
            {
                meshFinal,
                meshBack
            });
        }

        //Between

        List<Mesh> betweenMeshes = new List<Mesh>();

        for (int j = 0; j < frontVerticeLists.Count; j++)
        {
            var frontVertexList = frontVerticeLists[j];
            betweenMeshes.Add(CreateBetweenMesh(frontVertexList.ToArray(), _data.GetThickness()));
        }

        for (int j = 1; j < backVerticeLists.Count; j++) //The 1 for j is to avoid Z fighting in the middle layer
        {
            var backVertexList = backVerticeLists[j];
            betweenMeshes.Add(CreateBetweenMesh(backVertexList.ToArray(), -_data.GetThickness()));
        }


        foreach (var betweenMesh in betweenMeshes)
        {
            meshFinal = MeshHelper.CombineMeshesSingleMaterial(new Mesh[]
            {
                meshFinal,
                betweenMesh
            });
        }

        //Back of Front Slices

        List<Vector3> backSliceFrontVertices = new List<Vector3>();
        List<List<Vector3>> backSliceFrontVerticeLists = new List<List<Vector3>>();

        for (int i = 0; i < _data.GetSlices().Length; i++)
        {
            backSliceFrontVerticeLists.Add(new List<Vector3>());
            foreach (var vertice in _data.GetSlices()[i].GetVertices())
            {
                backSliceFrontVertices.Add(new Vector3(
                    _data.GetRelativeStartPositions()[i].x + vertice.x,
                    vertice.y,
                    -_data.GetRelativeStartPositions()[i].z + _data.GetThickness()));

                backSliceFrontVerticeLists[i].Add(new Vector3(
                    _data.GetRelativeStartPositions()[i].x + vertice.x,
                    vertice.y,
                    -_data.GetRelativeStartPositions()[i].z + _data.GetThickness()));
            }

            Mesh backSliceFrontMesh = CreateMeshFromPoly(_data.GetSlices()[i].GetVertices(),
                _data.GetRelativeStartPositions()[i].z);

            backSliceFrontMesh.triangles = backSliceFrontMesh.triangles.Reverse().ToArray();

            backSliceFrontMesh.uv = _data.GetSlices()[i].GetVertices();

            meshFinal = MeshHelper.CombineMeshesSingleMaterial(new[]
            {
                meshFinal,
                backSliceFrontMesh
            });
        }

        //Back of Back slices

        List<Vector3> backSliceBackVertices = new List<Vector3>();
        List<List<Vector3>> backSliceBackVerticeLists = new List<List<Vector3>>();

        for (int i = 0; i < _data.GetSlices().Length; i++)
        {
            backSliceBackVerticeLists.Add(new List<Vector3>());
            foreach (var vertice in _data.GetSlices()[i].GetVertices())
            {
                backSliceBackVertices.Add(new Vector3(
                    _data.GetRelativeStartPositions()[i].x + vertice.x,
                    vertice.y,
                    _data.GetRelativeStartPositions()[i].z - _data.GetThickness()));

                backSliceBackVerticeLists[i].Add(new Vector3(
                    _data.GetRelativeStartPositions()[i].x + vertice.x,
                    vertice.y,
                    _data.GetRelativeStartPositions()[i].z - _data.GetThickness()));
            }

            Mesh backSliceBackMesh = CreateMeshFromPoly(_data.GetSlices()[i].GetVertices(),
                _data.GetRelativeStartPositions()[i].z);

            backSliceBackMesh.uv = _data.GetSlices()[i].GetVertices();

            meshFinal = MeshHelper.CombineMeshesSingleMaterial(new[]
            {
                meshFinal,
                backSliceBackMesh
            });
        }

        //Combining of all the vertices

        List<Vector3> finalVertices = new List<Vector3>();

        finalVertices.AddRange(frontVertices);
        finalVertices.AddRange(backVertices);
        foreach (var betweenMesh in betweenMeshes)
        {
            finalVertices.AddRange(betweenMesh.vertices);
        }
        finalVertices.AddRange(backSliceFrontVertices);
        finalVertices.AddRange(backSliceBackVertices);



        meshFinal.vertices = finalVertices.ToArray(); //Needs to happen since combineMeshes doesn't do this...

        //MeshHelper.DebugArray(meshFinal.vertices, "Final Mesh Vertices:");

        meshFinal.RecalculateNormals();
        meshFinal.RecalculateBounds();

        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>().sharedMesh = meshFinal;
        gameObject.GetComponent<MeshRenderer>().material = BarkMaterial;
        gameObject.AddComponent<MeshCollider>();
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
        for (int i = 0; i < frontVertices.Length - 1; i++)
        {
            indices.AddRange(MeshConverter.QuadToTri(
                i, i + offs, i + 1 + offs, i + 1
            ));
        }

        indices.AddRange(MeshConverter.QuadToTri(frontVertices.Length - 1,
            frontVertices.Length - 1 + frontVertices.Length,
            frontVertices.Length, 0));


        //FINALIZATION:////////////////////

        //Implementation for proper quads with own vertices and easier to work with order
        List<Vector3> finalVertices = new List<Vector3>();
        List<int> finalIndices = new List<int>();

        int finalIndiceIterator = 0;

        for (int i = 0; i < frontVertices.Length - 1; i++)
        {
            finalVertices.Add(new Vector3(vertices[i].x, vertices[i].y, vertices[i].z));
            finalVertices.Add(new Vector3(vertices[i + offs].x, vertices[i + offs].y, vertices[i + offs].z));
            finalVertices.Add(new Vector3(vertices[i + 1 + offs].x, vertices[i + 1 + offs].y, vertices[i + 1 + offs].z));
            finalVertices.Add(new Vector3(vertices[i + 1].x, vertices[i + 1].y, vertices[i + 1].z));

            finalIndices.AddRange(MeshConverter.QuadToTri(finalIndiceIterator++, finalIndiceIterator++,
                finalIndiceIterator++, finalIndiceIterator++));
        }

        //Doing the last vertices seperately because wrapping is not working since
        //there are 2 different points where the wrapping is needed at the same time.

        finalVertices.Add(
            new Vector3(
                vertices[frontVertices.Length - 1].x,
                vertices[frontVertices.Length - 1].y,
                vertices[frontVertices.Length - 1].z)
        );
        finalVertices.Add(
            new Vector3(
                vertices[frontVertices.Length - 1 + frontVertices.Length].x,
                vertices[frontVertices.Length - 1 + frontVertices.Length].y,
                vertices[frontVertices.Length - 1 + frontVertices.Length].z)
        );
        finalVertices.Add(
            new Vector3(
                vertices[frontVertices.Length].x,
                vertices[frontVertices.Length].y,
                vertices[frontVertices.Length].z)
        );
        finalVertices.Add(
            new Vector3(
                vertices[0].x,
                vertices[0].y,
                vertices[0].z)
        );

        finalIndices.AddRange(
            MeshConverter.QuadToTri(
                finalIndiceIterator++, finalIndiceIterator++,
                finalIndiceIterator++, finalIndiceIterator++)
        );


        //Setting the UVs per 4 vertices/////

        for (int i = 0; i < finalVertices.Count; i += 4)
        {
            uvs.Add(new Vector2(1, finalVertices[i].y));
            uvs.Add(new Vector2(0, finalVertices[i].y));
            uvs.Add(new Vector2(0, finalVertices[i].y + Vector3.Distance(finalVertices[i], finalVertices[i + 2])));
            uvs.Add(new Vector2(1, finalVertices[i].y + Vector3.Distance(finalVertices[i], finalVertices[i + 2])));

        }


        //Creating and Populating the Mesh////

        Mesh mesh = new Mesh();
        mesh.vertices = finalVertices.ToArray();
        mesh.triangles = finalIndices.ToArray();
        mesh.uv = uvs.ToArray();

        return mesh;
    }
}
