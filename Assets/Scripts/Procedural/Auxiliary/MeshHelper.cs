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
}