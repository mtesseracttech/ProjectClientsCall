using UnityEngine;

public class Converter
{
    public static int[] QuadToTri(int i0, int i1, int i2, int i3)
    {
        int[] triangles = new int[6];

        triangles[0] = i0;
        triangles[1] = i1;
        triangles[2] = i3;
        triangles[3] = i1;
        triangles[4] = i2;
        triangles[5] = i3;

        return triangles;
    }
}