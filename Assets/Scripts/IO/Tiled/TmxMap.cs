using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[XmlRootAttribute("map")]
public class TmxMap
{
    [XmlElement("name")]
    public string Name;

    [XmlElement("objectgroup")]
    public TmxObjectLayer[] TmxObjectLayers;

    public void PrintInfo()
    {
        string DebugString = "TMX Map Info:\n";
        DebugString += Name + "\n";
        foreach (var layer in TmxObjectLayers)
        {
            DebugString
                += layer.Name + "\n"
                + "Layer Depth: " + layer.Depth + "\n"
                + "Number of Objects: ";

            if (layer.TmxObjects != null)
            {
                DebugString += layer.TmxObjects.Length + "\n\n";
                foreach (var tmxObject in layer.TmxObjects)
                {
                    DebugString
                        += "Object Name: " + tmxObject.Name + "\n"
                           + "Object ID: " + tmxObject.Id + "\n"
                           + "Object Position: " + tmxObject.X + "," + tmxObject.Y + "\n"
                           + "Object Size: " + tmxObject.Width + "," + tmxObject.Height + "\n\n";
                }
            }
            else
            {
                DebugString += 0 + "\n\n";
            }
        }
        Debug.Log(DebugString);
    }
}

[XmlRootAttribute("objectgroup")]
public class TmxObjectLayer
{
    [XmlAttribute("name")]
    public string Name = "";

    [XmlAttribute("depth")]
    public int Depth = 0;

    [XmlElement("object")]
    public TmxObject[] TmxObjects;
}

[XmlRootAttribute("object")]
public class TmxObject
{
    [XmlAttribute("id")]
    public int Id = 0;

    [XmlAttribute("name")]
    public string Name = "";

    [XmlAttribute("x")]
    public float X = 0.0f;

    [XmlAttribute("y")]
    public float Y = 0.0f;

    [XmlAttribute("width")]
    public float Width = 0.0f;

    [XmlAttribute("height")]
    public float Height = 0.0f;

}


