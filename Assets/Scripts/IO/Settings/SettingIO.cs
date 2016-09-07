using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Assets.Scripts.IO.Settings;
using UnityEngine;

public class SettingsIO
{
    public static void SaveSettingsToFile(GameSettings settings)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
            TextWriter writer = new StreamWriter("GameSettings.xml");
            serializer.Serialize(writer, settings);
            writer.Close();
            settings.PrintInfo("Successfully Saved:");
        }
        catch (Exception)
        {
            Debug.Log("Failed to write settings to file");
        }

    }

    public static GameSettings ReadSettingsFromFile(string filename)
    {
        GameSettings settings = null;

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
            TextReader reader = new StreamReader(filename);
            settings = serializer.Deserialize(reader) as GameSettings;
            reader.Close();
            settings.PrintInfo("Successfully Loaded:");
        }
        catch (Exception ex)
        {
            Debug.Log("Settings could not be read: " + ex);
        }

        return settings;
    }
}