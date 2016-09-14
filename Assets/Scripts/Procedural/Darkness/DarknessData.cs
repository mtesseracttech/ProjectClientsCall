using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace Assets.Scripts.Procedural.Darkness
{
    public class DarknessData
    {
        private readonly Vector3 _start;
        private readonly Vector3 _end;
        private int _stepsTillCenter;
        private float _stopDistanceFromNest;

        public DarknessData(Vector3 start, Vector3 end, int stepsTillCenter, float stopDistanceFromNest)
        {
            _start = start;
            _end = end;
            _stepsTillCenter = stepsTillCenter;
            _stopDistanceFromNest = stopDistanceFromNest;
        }

        public Vector3 End()
        {
            return _end;
        }

        public Vector3 Start()
        {
            return _start;
        }

        public float GetStepsTillCenter()
        {
            return _stepsTillCenter;
        }

        public float GetStopDistance()
        {
            return _stopDistanceFromNest;
        }

        public void SetStepsTillCenter(int steps)
        {
            _stepsTillCenter = steps;
        }

        public void SetStopDistance(float distance)
        {
            _stopDistanceFromNest = distance;
        }

        public override string ToString()
        {
            return "Start: " + _start + " End: " + _end;
        }

        public string GetName()
        {
            return "Darkness - " + ToString();
        }
    }
}
