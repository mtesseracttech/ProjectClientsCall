using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Procedural.Auxiliary
{
    class Polygon2D
    {
        private Vector2[] _vertices;

        public Polygon2D(Vector2[] vertices)
        {
            _vertices = vertices;
        }

        public Polygon2D(List<Vector2> vertices)
        {
            _vertices = vertices.ToArray();
        }

        public Polygon2D(string rawString)
        {
            _vertices = TiledParsingHelper.TiledPolyParser(rawString);
        }

        public Vector2[] GetVertices()
        {
            return _vertices;
        }

        public bool IsClockwise()
        {
            return MeshHelper.IsPolyClockWise(_vertices);
        }

        public void RevertVertices()
        {
            _vertices = MeshHelper.InvertPolygon(_vertices);
        }

        public void Debug()
        {
            Utility.DebugArray(_vertices);
        }
    }
}
