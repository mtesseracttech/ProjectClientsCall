using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Procedural.Trees
{
    public class ProceduralTreeData
    {
        private const float SliceThickness = 1f;
        private readonly Polygon2D[] _slices;
        private readonly Vector3 _startPosition;
        private readonly Vector3[] _relativeStartPositions;
        private readonly int[] _sliceNumber;
        private readonly int _layerNumber;
        private readonly string _name;

        public ProceduralTreeData(List<TmxObject> slices, int treesLayer, string name)
        {
            //Debug.Log("NEW Procedural Tree Data (" + slices[0].Id + ")");
            List<Polygon2D> tempPolys = new List<Polygon2D>();
            List<Vector3> relativeStartPositions = new List<Vector3>();

            _startPosition = new Vector3(slices[0].X, TiledParsingHelper.TiledCompensator(slices[0].Y, LevelBuilder.MapSize.y), (treesLayer + 1) * 5);
            _layerNumber = treesLayer;
            _name = name;

            for (int i = 0; i < slices.Count; i++)
            {
                relativeStartPositions.Add(
                    new Vector3(
                        slices[i].X - _startPosition.x,
                        slices[i].Y - _startPosition.y,
                        ((i * SliceThickness) + SliceThickness/2/* - _startPosition.z*/))
                );
            }

            foreach (var slice in slices)
            {
                tempPolys.Add(new Polygon2D(slice.Poly.Points));
            }

            foreach (var poly in tempPolys)
            {
                if(!poly.IsClockwise()) poly.RevertVertices();
            }

            _slices = tempPolys.ToArray();
            _relativeStartPositions = relativeStartPositions.ToArray();
            //Debug.Log("Layer Number: " + _layerNumber);
            //Debug.Log("Start Position" + _startPosition);
            //MeshHelper.DebugArray(_slices, "Slices:");
            //MeshHelper.DebugArray(_relativeStartPositions, "Relative Start Positions:");
        }

        public Polygon2D[] GetSlices()
        {
            return _slices;
        }

        public int GetLayerNumber()
        {
            return _layerNumber;
        }

        public Vector3[] GetRelativeStartPositions()
        {
            return _relativeStartPositions;
        }

        public Vector3 GetStartPosition()
        {
            return _startPosition;
        }

        public string GetName()
        {
            return _name;
        }

        public float GetThickness()
        {
            return SliceThickness;
        }
    }
}