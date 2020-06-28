using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class XMLOp : MonoBehaviour
{
    public static void Serialize(object item, string path)
    {
        XmlSerializer serializer = new XmlSerializer(item.GetType());
        StreamWriter writer = new StreamWriter(path);
        serializer.Serialize(writer.BaseStream, item);
        writer.Close();
    }

    public static T Deserialize<T>(string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader reader = new StreamReader(path);
        T deserialized = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
    }

    public static Wave DeserializeXMLTextAsset(TextAsset ta)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Wave));
        using (StringReader reader = new StringReader(ta.ToString()))
        {
            return serializer.Deserialize(reader) as Wave;
        }
    }
}
