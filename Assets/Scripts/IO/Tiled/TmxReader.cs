using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets.Scripts.IO.Tiled
{
    public class TmxReader
    {

        public static TmxMap ReadLevel(string filename)
        {
            TmxMap level = null;

            try
            {
                XmlSerializer tmxSerializer = new XmlSerializer(typeof(TmxMap));
                TextReader reader = new StreamReader(filename);
                level = tmxSerializer.Deserialize(reader) as TmxMap;
            }
            catch (Exception ex)
            {
                Debug.Log("TMXReader threw the following exception:" + ex);
            }

            return level;
        }

        public static TmxMap ReadLevel(TextAsset file)
        {
            TmxMap level = null;
            try
            {
                XmlSerializer tmxSerializer = new XmlSerializer(typeof(TmxMap));
                StringReader reader = new System.IO.StringReader(file.text);
                level = tmxSerializer.Deserialize(reader) as TmxMap;
            }
            catch (Exception ex)
            {
                Debug.Log("TMXReader threw the following exception:" + ex);
            }

            return level;
        }
    }



}
