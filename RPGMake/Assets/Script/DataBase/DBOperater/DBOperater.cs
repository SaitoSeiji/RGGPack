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

public static class DBListCreator
{
    //===================================================
    static List<string> DivideSingle(string data)
    {
        var split = data.Split('\n');
        var result = new List<string>(split);
        return result;
    }
    static private (List<(string head,string content)> contents, string replaced) ReplaceBlanket(string data)
    {

        string rgx = @"([^\s]+?)[\s]*{(.+?)}";
        var matches = Regex.Matches(data, rgx, RegexOptions.Singleline);
        var contents = new List<(string haed, string content)>();
        foreach (Match match in matches)
        {
            string head = match.Groups[1].Value.Trim();
            string content = match.Groups[2].Value.Trim();
            contents.Add((head,content));
        }
        var replaced = Regex.Replace(data, rgx, "", RegexOptions.Singleline);
        replaced = string.Join("\n", (replaced.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray()));
        return (contents, replaced.Trim());
    }
    static (string id, string replaced) DivideId(string data)
    {
        string rgx = @"id:([\s]*?)([^\s]+)";
        var match = Regex.Match(data, rgx, RegexOptions.Singleline);
        string id = match.Groups[2].Value;
        string replaced = Regex.Replace(data, rgx, "", RegexOptions.Singleline);
        return (id, replaced);
    }

    static private string[] DivideTextBlock(string text)
    {
        var datas = text.Split(new string[] { "id:" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x = "id:" + x).ToArray();
        return datas;
    }

    static public List<TempDBData> CreateDBListBytxt(string txt)
    {
        var result = new List<TempDBData>();
        if (string.IsNullOrEmpty(txt)) return result;
        var blockList = DivideTextBlock(txt);
        foreach(var block in blockList)
        {
            //データの分割
            var idSet = DivideId(block);
            var blancketSet = ReplaceBlanket(idSet.replaced);
            var singles = DivideSingle(blancketSet.replaced);

            //singleのデータを加える
            var tempDic_int = new Dictionary<string, int>();
            var tempDic_st = new Dictionary<string, string>();
            foreach (var single in singles)
            {
                var split = single.Split(' ');
                if (split.Length != 2) continue;
                if(int.TryParse(split[1],out int num))
                {
                    tempDic_int.Add(split[0], num);
                }
                else
                {
                    tempDic_st.Add(split[0], split[1]);
                }
            }
            //blanketのデータを加える
            var tempDic_list = new Dictionary<string, List<string>>();
            foreach (var blanket in blancketSet.contents)
            {
                tempDic_list.Add(blanket.head, new List<string>());
                var split = blanket.content.Split('\n');
                foreach(var data in split)
                {
                    if (string.IsNullOrEmpty(data)) continue;
                    tempDic_list[blanket.head].Add(data.Trim());
                }
            }
            var add = new TempDBData(idSet.id);
            add.InitMember(tempDic_st, tempDic_int, tempDic_list);
            result.Add(add);
        }
        return result;
    }
}

//DB関連の入出力処理
public static class DBIO
{
    //public static string CreateSavePath_txt(string saveKey)
    //{
    //    return Application.dataPath + "/Resource/DataBase/" + saveKey + ".txt";
    //}

    public static string CreateSavePath_asset(string dirName,string fileName)
    {
        return ("Assets/Resource/DataBase/" + dirName + "/" + fileName + ".asset");
    }
    
    public static string CreateAssetDirectoryPath(string dirName)
    {
        return ("Assets/Resource/DataBase/" + dirName);
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

    //public static string ReadText(string path)
    //{
    //    string rawdata = "";
    //    using (StreamReader sr = new StreamReader(path))
    //    {
    //        rawdata = sr.ReadToEnd();
    //    }
    //    return rawdata;
    //}

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


    //public static void WriteText(List<string> data, string path)
    //{
    //    WriteText(string.Join("\n", data), path);
    //}
    //public static void WriteText(string data, string path)
    //{
    //    using (StreamWriter sw = new StreamWriter(path))
    //    {
    //        data = data.Trim('\r', '\n', '\t');
    //        sw.WriteLine(data);
    //    }
    //}
}

//データベースの操作を行うクラス
public class DBOperater<T,K>:IEnable_initDB
    where T:AbstractDBData
    where K:AbstractDB
{
    K _database;
    
    public DBOperater(K db)
    {
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
    //public void AddDBD(string name)
    //{
    //    var dataList = _database.GetDataList();
    //    if (dataList.Where(x=>x._Data._serchId==name).FirstOrDefault()!=null)
    //    {
    //        DebugMessage_miss("Add : already contain this name:"+name);
    //        return;
    //    }
    //    //scriptableObjectの追加
    //    var scriptable = AbstractDBData.GetInstance<T>();
    //    AssetDatabase.CreateAsset(scriptable, DBIO.CreateSavePath_asset(_dirName,name));
    //    scriptable.InitData();
    //    dataList.Add(scriptable);
    //    //txtデータ書き込み
    //    DBIO.WriteText(_database.CreateDataTxt(), DBIO.CreateSavePath_txt(_dirName));
    //    AssetDatabase.Refresh();
    //    DebugMessage_success("Add");
    //}
    //public void RemoveDBD(string name)
    //{
        
    //    //scriptableObjeの削除
    //    var dataList = _database.GetDataList();
    //    if (dataList.Where(x => x._Data._serchId == name).FirstOrDefault() == null)
    //    {
    //        DebugMessage_miss("Remove : not contain this name:"+name);
    //        return;
    //    }
    //    var scrData = dataList.Where(x => x._Data._serchId == name).First();
    //    dataList.Remove(scrData);
    //    AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(scrData));
    //    //txtの更新
    //    DBIO.WriteText(_database.CreateDataTxt(), DBIO.CreateSavePath_txt(_dirName));
    //    AssetDatabase.Refresh();
    //    DebugMessage_success("Remove");
    //}
    //public DBData EditDBD(string name)
    //{
    //    var data= _database.GetDataList().Where(x => x._Data._serchId == name).ToList();
    //    if (data == null||data.Count==0)
    //    {
    //        DebugMessage_miss("Edit:not contain this name:"+name);
    //        return null;
    //    }
    //    else DebugMessage_success("Edit");
    //    return new DBData( data.First()._Data,name);
    //}
    //public void UpdateDBD(DBData data,string oldName)
    //{
    //    var targetData = _database.GetDataList().Where(x => x._Data._serchId == oldName).FirstOrDefault();
    //    if (targetData==null)
    //    {
    //        DebugMessage_miss("Update:not containt this name:"+oldName);
    //        return;
    //    }
    //    //scriptableObjectの更新
    //    AssetDatabase.RenameAsset(DBIO.CreateSavePath_asset(_dirName,oldName), data._serchId);
    //    targetData.UpdateData(data);
    //    EditorUtility.SetDirty(targetData);
    //    //txtの更新
    //    DBIO.WriteText(_database.CreateDataTxt(), DBIO.CreateSavePath_txt(_dirName));
    //    AssetDatabase.Refresh();
    //    DebugMessage_success("Update");
    //}
    public void SyncDataByTxt(TextAsset textAsset,string parentDir)
    {
        string path = $"{parentDir}/{textAsset.name}";
        if (!DBIO.CheckDir(DBIO.CreateAssetDirectoryPath(path)))
        {
            DBIO.CreateDir(DBIO.CreateAssetDirectoryPath(path));
        }
        

        var textDataList = DBListCreator.CreateDBListBytxt(DBIO.TrimType(textAsset.text).replaced);
        var assetDBList = _database.GetDataList(this);
        //txtに書いてないものを削除
        for(int i=assetDBList.Count-1;i>=0;i--)
        {
            if (textDataList.Where(x=>x._serchId==assetDBList[i].name).FirstOrDefault()==null)
            {
                AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(assetDBList[i]));
                assetDBList.RemoveAt(i);
            }
        }
        //txtに書いてあるけどデータがないものを追加
        foreach(var data in textDataList)
        {
            var target = assetDBList.Where(x => x.name == data._serchId).FirstOrDefault();
            if (target == null)
            {
                target = AbstractDBData.GetInstance<T>();
                AssetDatabase.CreateAsset(target,DBIO.CreateSavePath_asset(path,data._serchId));
                assetDBList.Add(target);
            }
            target.UpdateMember(data);
            EditorUtility.SetDirty(target);
        }
        _database.SetDataList(assetDBList,this);
        EditorUtility.SetDirty(_database);
        AssetDatabase.Refresh();
        DebugMessage_success("SyncText");
    }

    public void RateUpdate()
    {
        var list = _database.GetDataList(this);
        foreach(var data in list)
        {
            data.RateUpdateMemeber();
        }
        _database.SetDataList(list,this);
    }

    //public void SyncTxtByData()
    //{
    //    string write = _database.CreateDataTxt();
    //    DBIO.WriteText(write, DBIO.CreateSavePath_txt(_dirName));
    //}

}

