using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;

public class EventDataOperater
{
    public class ConvertedText
    {
        public string _head { get; private set; }
        public string[] _content { get; private set; }

        public ConvertedText(string head, string[] content)
        {
            _head = head.Trim();
            _content = content;
            _content = _content.Select(x => x.Trim()).ToArray();
        }
        public ConvertedText(string head, string content)
        {
            _head = head.Trim();
            _content = new string[] { content };
        }

        public string GetLog()
        {
            string result="";
            result += "head:" + _head;
            foreach(var con in _content)
            {
                result += "\ncontent:"+con;
            }

            return result;
        }
    }

    static bool CheckIsReplaceBrancket(string input)
    {
        string branket = "(.+?){(.+?)}";
        var match = Regex.Match(input, branket, RegexOptions.Singleline);
        return match.Success;
    }
    //{}でくくられた部分をとる
    static List<ConvertedText> ReplaceBrancket(string input,string rgx)
    {
        var matches = Regex.Matches(input, rgx, RegexOptions.Singleline);
        var result = new List<ConvertedText>();
        foreach (Match match in matches)
        {
            string head = match.Groups[1].Value.Trim();
            string contents = match.Groups[2].Value.Trim();
            result.Add(new ConvertedText(head, contents.Split('\n')));
        }
        return result;
    }

    static List<ConvertedText> ReplaceBrancket_smal(string input)
    {
        return ReplaceBrancket(input, "(.+?){(.+?)}");
    }

    static string[] SplitDataUnit(string input)
    {
        var datas = input.Split(new string[] { "id:" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x = "id:" + x).ToArray();
        return datas;
    }

    public static List<(string id,List<ConvertedText> dataSet)> GetConverted(string input)
    {
        var result = new List<(string id, List<ConvertedText> dataSet)>();
        var dataSet = SplitDataUnit(input);
        foreach(var data in dataSet)
        {
            string idBrox = @"id: (.+?)\n(.+)";
            var match = Regex.Match(data, idBrox, RegexOptions.Singleline);
            string _id = match.Groups[1].Value.Trim();
            string content = match.Groups[2].Value.Trim();
            var list= ReplaceBrancket_smal(content);
            result.Add((_id, list));
        }
        return result;
    }

    public static void GetLog(List<(string id, List<ConvertedText> dataSet)> data)
    {
        foreach(var d in data)
        {
            Debug.Log("id:" + d.id);
            foreach(var set in d.dataSet)
            {
                Debug.Log(set.GetLog());
            }
        }
    }

    //=====================================


    public static void SyncDataByTxt(EventDB _database,string text)
    {
        var txtlist = GetConverted(text);
        var assetDBList = _database._scriptableList;
        //txtに書いてないものを削除
        for (int i = assetDBList.Count - 1; i >= 0; i--)
        {
            bool hit = false;
            foreach(var txt in txtlist)
            {
                if (txt.id == assetDBList[i].name)
                {
                    hit = true;
                    break;
                }
            }
            if (hit == false)
            {
                AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(assetDBList[i]));
                assetDBList.RemoveAt(i);
            }
        }
        //txtに書いてあるけどデータがないものを追加
        foreach (var data in txtlist)
        {
            var target = assetDBList.Where(x => x.name == data.id).FirstOrDefault();
            var make = JudgeEventCodeType(data.dataSet);
            if(target!=null&&target.GetType() != make.GetType())
            {
                AssetDatabase.MoveAssetToTrash(AssetDatabase.GetAssetPath(target));
                assetDBList.Remove(target);
            }
            if (target == null)
            {
                //target = ScriptableObject.CreateInstance<EventCodeScriptable>();
                target =make;
                AssetDatabase.CreateAsset(target, DBIO.CreateSavePath_asset(_database.saveFileName, data.id));
                assetDBList.Add(target);
            }
            target.UpdateData(data.id,data.dataSet);
        }
        _database.UpdateNextEvent();
        EditorUtility.SetDirty(_database);
        AssetDatabase.Refresh();
    }

    static EventCodeScriptable JudgeEventCodeType(List<ConvertedText> dataSet)
    {
        var nextData= dataSet.Where(x => x._head == "next").FirstOrDefault();
        if(nextData==null)return ScriptableObject.CreateInstance<EventCodeScriptable>();
        if (nextData._content.Length <= 1) return ScriptableObject.CreateInstance<EventCodeScriptable>();
        else return ScriptableObject.CreateInstance<EventCode_BranchTerm>();
    }
}