using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class DBListCreator
{
    DBData _template=null;
    int _dataLinesSize;

    public DBListCreator(AbstractDBData data)
    {
        _template = data.GetDataTemplate();
        _dataLinesSize = data.GetTxtMemberCount();
    }

    DBData CreateDBData(string id, string txt)
    {
        Dictionary<string, string> dic_st= new Dictionary<string, string>(_template._memberSet_st);
        Dictionary<string, int> dic_int= new Dictionary<string, int>(_template._memberSet_int);
        
        var lines = new List<string>( txt.Split('\n'));
        foreach (var line in lines)
        {
            line.Trim();
            var datas = line.Split(' ');
            if (datas.Length == 2&&datas[0]!="id")
            {
                if (int.TryParse(datas[1], out int num))
                {
                    dic_int[datas[0]] = num;
                }
                else
                {
                    dic_st[datas[0]] = datas[1];
                }
            }
        }

        var result = new DBData();
        result._serchId = id;
        result._memberSet_int = dic_int;
        result._memberSet_st = dic_st;
        return result;
    }
    public List<DBData> CreateDBListBytxt(string txt)
    {
        var result = new List<DBData>();
        if (string.IsNullOrEmpty(txt)) return result;
        var lines = txt.Split('\n').Select(x=>x.Trim()).Where(x=>!string.IsNullOrEmpty(x)).ToArray();
        int beforeIdIndex = -1;
        for(int i=0;true;i++)
        {
            bool end = false;
            string head = "";
            //終了判定
            if (lines[i].Equals("end") || i > lines.Length)
            {
                head = "end";
                end = true;
            }
            else//データの区切りかどうかの判定
            {
                head = lines[i].Substring(0, 2);
            }
            if (head == "end"||head=="id")
            {
                if (beforeIdIndex >= 0)
                {
                    string id = lines[beforeIdIndex].Split(' ')[1];
                    string content = DivideArray(lines, beforeIdIndex+1, _dataLinesSize);
                    result.Add(CreateDBData(id, content));
                }
                beforeIdIndex = i;
            }
            if (end)
            {
                break;
            }
        }

        return result;
    }

    string DivideArray(string[] _originData, int start, int size)
    {
        string[] result=new string[size];
        Array.Copy(_originData, start, result,0, size);

        return string.Join("\n",result);
    }

    public DBData GetTmeplate(string id)
    {
        return new DBData(_template,id);
    }
}

//DB関連の入出力処理
public static class DBIO
{
    public static string CreateSavePath_txt(string saveKey)
    {
        return Application.dataPath + "/Resource/DataBase/" + saveKey + ".txt";
    }

    public static string CreateSavePath_asset(string saveKey,string key)
    {
        return ("Assets/Resource/DataBase/" + saveKey + "/" + key + ".asset");
    }
    
    public static string CreateAssetDirectoryPath(string saveKey)
    {
        return ("Assets/Resource/DataBase/" + saveKey);
    }

    public static bool CheckDir(string path)
    {
        return Directory.Exists(path);
    }

    public static void CreateDir(string path)
    {
        Directory.CreateDirectory(path);
        AssetDatabase.Refresh();
    }

    public static string ReadText(string path)
    {
        string rawdata = "";
        using (StreamReader sr = new StreamReader(path))
        {
            rawdata = sr.ReadToEnd();
        }
        return rawdata;
    }

    public static (string type,string replaced) TrimType(string txt)
    {
        string type="";
        string replaced="";
        string rgx = @"Type\((.+?)\)";
        Match match = Regex.Match(txt, rgx, RegexOptions.Singleline);
        if (match.Success)
        {
            type = match.Groups[1].Value;
            replaced = Regex.Replace(txt, rgx, "", RegexOptions.Singleline);
        }
        else
        {
            Debug.Log("miss");
            replaced = txt;
        }
        return (type, replaced.Trim());
    }


    public static void WriteText(List<string> data, string path)
    {
        WriteText(string.Join("\n", data), path);
    }
    public static void WriteText(string data, string path)
    {
        using (StreamWriter sw = new StreamWriter(path))
        {
            data = data.Trim('\r', '\n', '\t');
            sw.WriteLine(data);
        }
    }
}

//データベースの操作を行うクラス
public class DBOperater<T,K>
    where T:AbstractDBData
    where K:AbstractDB
{
    string _dirName;
    K _database;

    DBData _tempData;

    public DBOperater(K db,string dirName)
    {
        _dirName = dirName;
        _database = db;
    }
    #region debug
    static void DebugMessage_success(string content)
    {
        Debug.Log("DBOperater : sucsess :" + content);
    }
    static void DebugMessage_miss(string content)
    {
        Debug.Log("DBOperater : miss :" + content);
    }
#endregion
    //Add~Updateは現在使用不可
    public void AddDBD(string name)
    {
        var dataList = _database.GetDataList();
        if (dataList.Where(x=>x._Data._serchId==name).FirstOrDefault()!=null)
        {
            DebugMessage_miss("Add : already contain this name:"+name);
            return;
        }
        //scriptableObjectの追加
        var scriptable = AbstractDBData.GetInstance<T>();
        AssetDatabase.CreateAsset(scriptable, DBIO.CreateSavePath_asset(_dirName,name));
        scriptable.InitData();
        dataList.Add(scriptable);
        //txtデータ書き込み
        DBIO.WriteText(_database.CreateDataTxt(), DBIO.CreateSavePath_txt(_dirName));
        AssetDatabase.Refresh();
        DebugMessage_success("Add");
    }
    public void RemoveDBD(string name)
    {
        
        //scriptableObjeの削除
        var dataList = _database.GetDataList();
        if (dataList.Where(x => x._Data._serchId == name).FirstOrDefault() == null)
        {
            DebugMessage_miss("Remove : not contain this name:"+name);
            return;
        }
        var scrData = dataList.Where(x => x._Data._serchId == name).First();
        dataList.Remove(scrData);
        AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(scrData));
        //txtの更新
        DBIO.WriteText(_database.CreateDataTxt(), DBIO.CreateSavePath_txt(_dirName));
        AssetDatabase.Refresh();
        DebugMessage_success("Remove");
    }
    public DBData EditDBD(string name)
    {
        var data= _database.GetDataList().Where(x => x._Data._serchId == name).ToList();
        if (data == null||data.Count==0)
        {
            DebugMessage_miss("Edit:not contain this name:"+name);
            return null;
        }
        else DebugMessage_success("Edit");
        return new DBData( data.First()._Data,name);
    }
    public void UpdateDBD(DBData data,string oldName)
    {
        var targetData = _database.GetDataList().Where(x => x._Data._serchId == oldName).FirstOrDefault();
        if (targetData==null)
        {
            DebugMessage_miss("Update:not containt this name:"+oldName);
            return;
        }
        //scriptableObjectの更新
        AssetDatabase.RenameAsset(DBIO.CreateSavePath_asset(_dirName,oldName), data._serchId);
        targetData.UpdateData(data);
        EditorUtility.SetDirty(targetData);
        //txtの更新
        DBIO.WriteText(_database.CreateDataTxt(), DBIO.CreateSavePath_txt(_dirName));
        AssetDatabase.Refresh();
        DebugMessage_success("Update");
    }
    public void SyncDataByTxt(string txt)
    {
        if (!DBIO.CheckDir(DBIO.CreateAssetDirectoryPath(_dirName)))
        {
            DBIO.CreateDir(DBIO.CreateAssetDirectoryPath(_dirName));
        }

        var creator = new DBListCreator(AbstractDBData.GetInstance<T>());
        var txtDataList = creator.CreateDBListBytxt(txt);
        var assetDBList = _database.GetDataList();
        //txtに書いてないものを削除
        for(int i=assetDBList.Count-1;i>=0;i--)
        {
            if (txtDataList.Where(x=>x._serchId==assetDBList[i]._Data._serchId).FirstOrDefault()==null)
            {
                AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(assetDBList[i]));
                assetDBList.RemoveAt(i);
            }
        }
        //txtに書いてあるけどデータがないものを追加
        foreach(var data in txtDataList)
        {
            var target = assetDBList.Where(x => x._Data._serchId == data._serchId).FirstOrDefault();
            if (target == null)
            {
                target = AbstractDBData.GetInstance<T>();
                AssetDatabase.CreateAsset(target,DBIO.CreateSavePath_asset(_dirName,data._serchId));
                _database.GetDataList().Add(target);
            }
            target.UpdateData(data);
            EditorUtility.SetDirty(target);
        }


        EditorUtility.SetDirty(_database);
        AssetDatabase.Refresh();
        DebugMessage_success("SyncText");
    }

    public void SyncTxtByData()
    {
        string write = _database.CreateDataTxt();
        DBIO.WriteText(write, DBIO.CreateSavePath_txt(_dirName));
    }

}

