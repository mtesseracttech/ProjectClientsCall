using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    class Utility
    {
        public static Random Random = new Random();

        public static void DebugArray<T>(T[] array, string addedMessage = "")
        {
            string debugString = addedMessage + "\n";
            if (array == null)
            {
                debugString += "Array is Null";
            }
            else if (array.Length < 1)
            {
                debugString += "Array is Empty";
            }
            else
            {
                for (int i = 0; i < array.Length; i++)
                {
                    debugString += i + ". " + array[i] + "\n";
                }
            }
            Debug.Log(debugString);
        }
    }
}
