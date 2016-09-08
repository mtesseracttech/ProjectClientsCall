using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TreeSliceRenderer : MonoBehaviour
{
    public Material TEMPBarkMaterial;

    private TreeSlice _data;



	// Use this for initialization
	void Start ()
	{
	    //Keep empty except for object container object initialization
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

    public void Create(TreeSlice data)
    {
        _data = data;

        // Create Vector2 vertices
        Vector2[] vertices2D = _data.GetVertices();

        // Use the triangulator to get triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] triangles = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        List<Vector3> verticesWalls = new List<Vector3>();

        for (int i = 0; i < vertices.Length; i++)
        {
            verticesWalls.Add(new Vector3(vertices2D[i].x, vertices2D[i].y, 0));
            verticesWalls.Add(new Vector3(vertices2D[i].x, vertices2D[i].y, 100));
        }

        List<int> trianglesWalls = new List<int>();
        for (int i = 0; i < verticesWalls.Count; i+=2)
        {
            int limit = verticesWalls.Count;
            int[] trianglesFromQuad = MeshConverter.QuadToTri(i, i+1 % limit, i+3 % limit, i+2 % limit);
            trianglesWalls.AddRange(trianglesFromQuad);
        }

        verticesWalls.InsertRange(0, vertices);
        trianglesWalls.InsertRange(0, triangles);

        vertices = verticesWalls.ToArray();
        triangles = trianglesWalls.ToArray();


        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = triangles;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
        Renderer render = gameObject.GetComponent(typeof(MeshRenderer)) as Renderer;
        render.material = TEMPBarkMaterial;
    }


}
