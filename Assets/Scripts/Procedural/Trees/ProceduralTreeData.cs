﻿using System.Collections.Generic;
using System.Linq;
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
            slices = SortSlices(slices);
            List<Polygon2D> tempPolys = new List<Polygon2D>();
            List<Vector3> relativeStartPositions = new List<Vector3>();

            _startPosition = new Vector3(slices[0].X, TiledParsingHelper.TiledCompensator(slices[0].Y, LevelBuilder.MapSize.y), treesLayer - 0.5f);
            _layerNumber = treesLayer;
            _name = name;

            for (int i = 0; i < slices.Count; i++)
            {
                relativeStartPositions.Add(
                    new Vector3(
                        slices[i].X - _startPosition.x,
                        slices[i].Y - _startPosition.y,
                        ((i * SliceThickness) + SliceThickness/2))
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
        }

        private List<TmxObject> SortSlices(List<TmxObject> slices)
        {
            return slices.OrderBy(o => o.Name).ToList();
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