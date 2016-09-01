using UnityEngine;
using System.Collections;

public class TreeSliceRenderer : MonoBehaviour
{
    public Material TEMPBarkMaterial;

    private TreeSlice _data;



	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

    public void Create(TreeSlice data)
    {
        _data = data;

        // Create Vector2 vertices
        Vector2[] vertices2D = _data.GetVertexes();

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
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
