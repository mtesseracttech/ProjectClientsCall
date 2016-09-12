using System;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[XmlRootAttribute("map")]
public class TmxMap
{
    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("width")]
    public int Width;

    [XmlAttribute("height")]
    public int Height;

    [XmlAttribute("tilewidth")]
    public int TileWidth;

    [XmlAttribute("tileheight")]
    public int TileHeight;

    [XmlElement("objectgroup")]
    public TmxObjectLayer[] TmxObjectLayers;




    public void PrintInfo()
    {
        string debugString = "TMX Map Info:\n";
        debugString += Name + "\n";
        foreach (var layer in TmxObjectLayers)
        {
            debugString
                +="Layer Name:" + layer.Name + "\n"
                + "Layer Depth: " + layer.Depth + "\n"
                + "Number of Objects: ";

            if (layer.TmxObjects != null)
            {
                debugString += layer.TmxObjects.Length + "\n\n";
                foreach (var tmxObject in layer.TmxObjects)
                {
                    debugString
                        += "Object Name: " + tmxObject.Name + "\n"
                           + "Object ID: " + tmxObject.Id + "\n"
                           + "Object Position: " + tmxObject.X + "," + tmxObject.Y + "\n"
                           + "Object Size: " + tmxObject.Width + "," + tmxObject.Height + "\n\n";


                    if (tmxObject.ObjectProperties != null && tmxObject.ObjectProperties.Properties != null && tmxObject.ObjectProperties.Properties.Length > 0)
                    {
                        var properties = tmxObject.ObjectProperties.Properties;
                        debugString += "Properties: \n";

                        foreach (var property in properties)
                        {
                            debugString += property.Name + " " + property.Value + "\n";
                        }
                    }
                    
                }
            }
            else
            {
                debugString += "No objects are in this layer.\n\n";
            }
        }
        Debug.Log(debugString);
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
public class TmxObject : IComparable
{
    [XmlAttribute("id")]
    public int Id = 0;

    [XmlAttribute("name")]
    public string Name = "TmxObject";

    [XmlAttribute("x")]
    public float X = 0.0f;

    [XmlAttribute("y")]
    public float Y = 0.0f;

    [XmlAttribute("width")]
    public float Width = 0.0f;

    [XmlAttribute("height")]
    public float Height = 0.0f;

    [XmlAttribute("rotation")]
    public float Rotation = 0.0f;

    [XmlElement("properties")]
    public ObjectProperties ObjectProperties;

    [XmlElement("polygon")]
    public Polygon Poly;
    
    public int CompareTo(object obj)
    {
        if (obj == null) return 1;
        TmxObject other = obj as TmxObject;
        if (other != null)
            return this.Id.CompareTo(other.Id);
        else
            throw new ArgumentException("Other object is invalid");
    }
}

[XmlRootAttribute("properties")]
public class ObjectProperties
{
    [XmlElement("property")]
    public Property[] Properties;
}

[XmlRootAttribute("property")]
public class Property
{
    [XmlAttribute("name")]
    public string Name;

    [XmlAttribute("value")]
    public string Value;
}

[XmlRootAttribute("polygon")]
public class Polygon
{
    [XmlAttribute("points")]
    public string Points;
}


