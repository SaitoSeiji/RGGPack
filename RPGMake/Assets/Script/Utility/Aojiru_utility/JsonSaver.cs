using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using FullSerializer;
using System;
using HC.Common;

public class JsonSaver
{
    static string _saveDataPath = "SaveData/";

    [System.Serializable]
    public class ListSaveData<T>
    {
        [SerializeField] public List<T> saveList = new List<T>();
    }

    public static void SaveAction<T>(T data, string path)
    {
        var json = StringSerializationAPI.Serialize(typeof(T), data);
        // Assetsフォルダに保存する
        json = JsonFormatter.ToPrettyPrint(json, JsonFormatter.IndentType.Space);
        var savePath = GetPath(path);
        var writer = new StreamWriter(savePath, false); // 上書き
        writer.WriteLine(json);
        writer.Flush();
        writer.Close();
    }
    public static void SaveAction<T>(List<T> data, string path)
    {
        var list =new ListSaveData<T>();
        foreach(var d in data)
        {
            list.saveList.Add(d);
        }

        SaveAction(list, path);
    }

    public static T LoadAction<T>(string path)
    {
        StreamReader streamReader = new StreamReader(GetPath(path));
        string json = streamReader.ReadToEnd();
        streamReader.Close();
        T data = (T)StringSerializationAPI.Deserialize(typeof(T), json);
        return data;
    }

    public static List<T> LoadAction_list<T>(string path)
    {

        StreamReader streamReader = new StreamReader(GetPath(path));
        var json = streamReader.ReadToEnd();
        //var data = JsonUtility.FromJson<ListSaveData<T>>(json);
        var data = (ListSaveData<T>)StringSerializationAPI.Deserialize(typeof(ListSaveData<T>), json);
        var result =new List<T>();

        foreach(var d in data.saveList)
        {
            result.Add(d);
        }
        return result;
    }

    static string  GetPath(string path)
    {
        return Application.dataPath + "/" + _saveDataPath + path+".json";
    }

    public static bool CheckExsitFile(string path)
    {
        return File.Exists(GetPath(path));
    }
}

public static class StringSerializationAPI
{
    private static readonly fsSerializer _serializer = new fsSerializer();

    public static string Serialize(Type type, object value)
    {
        // serialize the data
        fsData data;
        _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

        // emit the data via JSON
        return fsJsonPrinter.CompressedJson(data);
    }

    public static object Deserialize(Type type, string serializedState)
    {
        // step 1: parse the JSON data
        fsData data = fsJsonParser.Parse(serializedState);

        // step 2: deserialize the data
        object deserialized = null;
        _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

        return deserialized;
    }
}
