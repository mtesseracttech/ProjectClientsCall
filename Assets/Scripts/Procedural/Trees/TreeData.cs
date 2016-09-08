using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Procedural.Trees
{
    class TreeData
    {
        private Vector3 _position;
        private Quaternion _rotation;

        public TreeData(TmxObject data ,int layerNumber)
        {
            _position = new Vector3(data.X, data.Y, layerNumber + 1 * 10);

            if (data.ObjectProperties != null && 
                data.ObjectProperties.Properties != null &&
                data.ObjectProperties.Properties.Length > 0)
            {
                foreach (var property in data.ObjectProperties.Properties)
                {
                    if (property.Name.ToLower().Contains("rotation"))
                    {
                        try
                        {
                            Quaternion.AngleAxis(int.Parse(property.Value), Vector3.up);
                        }
                        catch (Exception ex)
                        {
                            Debug.Log("Could not create an appropriate rotation!");
                        }
                    }
                }
            }
        }

        public Quaternion GetRotation()
        {
            return _rotation;
        }

        public Vector3 GetPosition()
        {
            return _position;
        }
    }
}
