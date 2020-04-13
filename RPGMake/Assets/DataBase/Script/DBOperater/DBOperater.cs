using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

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

    DBData CreateDBData(string id, string data)
    {
        Dictionary<string, string> dic_st= new Dictionary<string, string>(_template._memberSet_st);
        Dictionary<string, int> dic_int= new Dictionary<string, int>(_template._memberSet_int);
        
        var lines = data.Split('\n');
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

//データベースの操作を行うクラス
public class DBOperater<T,K>
    where T:AbstractDBData
    where K:AbstractDB
{
    string _saveKey;
    K _database;

    DBData _tempData;

    public DBOperater(K db,string saveKey)
    {
        _saveKey = saveKey;
        _database = db;
    }
    #region path
    string CreateSavePath_txt()
    {
        return Application.dataPath + "/DataBase/" + _saveKey + ".txt";
    }
    string CreateSavePath_txtAsset()
    {
        return Application.dataPath + "/DataBase/" + _saveKey + ".asset";
    }

    string CreateSavePath_asset(string key)
    {
        return ("Assets/Resources/DataBase/"+_saveKey+"/" + key + ".asset");
    }

    #endregion
    #region io

    string ReadText(string path)
    {
        string rawdata = "";
        using (StreamReader sr = new StreamReader(CreateSavePath_txt()))
        {
            rawdata = sr.ReadToEnd();
        }
        return rawdata;
    }

    List<string> ReadText2List(string path)
    {
        string rawdata = "";
        using (StreamReader sr = new StreamReader(CreateSavePath_txt()))
        {
            rawdata = sr.ReadToEnd();
        }
        return rawdata.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
    }
    
    void WriteText(List<string> data, string path)
    {
        WriteText(string.Join("\n", data), path);
    }
    void WriteText(string data, string path)
    {
        using (StreamWriter sw = new StreamWriter(CreateSavePath_txt()))
        {
            data=data.Trim('\r', '\n','\t');
            sw.WriteLine(data);
        }
    }

    #endregion
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
    public void AddDBD(string name)
    {
        if (!CheckFile()) CreateFile();
        var dataList = _database.GetDataList();
        if (dataList.Where(x=>x._Data._serchId==name).FirstOrDefault()!=null)
        {
            DebugMessage_miss("Add : already contain this name:"+name);
            return;
        }
        //scriptableObjectの追加
        var scriptable = AbstractDBData.GetInstance<T>();
        AssetDatabase.CreateAsset(scriptable, CreateSavePath_asset(name));
        scriptable.InitData();
        dataList.Add(scriptable);
        //txtデータ書き込み
        WriteText(_database.CreateDataTxt(), CreateSavePath_txt());
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
        WriteText(_database.CreateDataTxt(), CreateSavePath_txt());
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
        AssetDatabase.RenameAsset(CreateSavePath_asset(oldName), data._serchId);
        targetData.UpdateData(data);
        EditorUtility.SetDirty(targetData);
        //txtの更新
        WriteText(_database.CreateDataTxt(), CreateSavePath_txt());
        AssetDatabase.Refresh();
        DebugMessage_success("Update");
    }

    public void SyncDataByTxt()
    {
        var creator = new DBListCreator(AbstractDBData.GetInstance<T>());
        var txtDataList = creator.CreateDBListBytxt(ReadText(CreateSavePath_txt()));
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
                AssetDatabase.CreateAsset(target, CreateSavePath_asset(data._serchId));
                _database.GetDataList().Add(target);
            }
            target.UpdateData(data);
        }


        EditorUtility.SetDirty(_database);
        AssetDatabase.Refresh();
        DebugMessage_success("SyncText");
    }

    public void SyncTxtByData()
    {
        string write = _database.CreateDataTxt();
        WriteText(write, CreateSavePath_txt());
    }
    bool CheckFile()
    {
        return File.Exists(CreateSavePath_txt());
    }

    void CreateFile()
    {
        File.Create(CreateSavePath_txt());
        AssetDatabase.Refresh();
    }
}
