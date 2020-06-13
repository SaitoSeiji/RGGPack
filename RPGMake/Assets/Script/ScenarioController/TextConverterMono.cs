using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Linq;

public class TextCovertedData 
{
    public TextCovertedData(string head,string data,string text)
    {
        _head = head;
        _data = data;
        _text = text;
    }
    public string _head { get; private set; }
    public string _data { get; private set; }
    public string _text { get; private set; }

    public string CreateInfo()
    {
        string result = "action:" + _head + " data:" + _data+"\n";
        result += "Text:" + _text;
        return result;
    }
}

public static class TextConverter
{
    static string _checkIsHeadRGX = @"#(.*)";
    static string _checkHaveDataRGX = @"#(.*)\[(.*)\]";

    public static Queue<TextCovertedData> Convert(string input)
    {
        var dataList = CreateBlockList(input);
        var result = new Queue<TextCovertedData>();
        foreach(var d in dataList)
        {
            string head = "";
            string data = "";
            string text = "";
            if (CheckSeparateEnable(d))
            {
                var d2 = SepareteHead(d);
                head = d2.action;
                data = d2.data;
            }
            else if (CheckIsHead(d))
            {
                head = GetHead(d);
            }
            text=TrimTextData(d);
            result.Enqueue( new TextCovertedData(head, data, text));
        }
        return result;
    }

    public static Queue<TextCovertedData> DebugInfo(string input)
    {
        var data = Convert(input);
        foreach(var d in data)
        {
            Debug.Log(d.CreateInfo());
        }
        return data;
    }

     static List<string> CreateBlockList(string input)
    {
        var d = input.Split('$');
        return d.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
    }
    
    #region check
    static bool CheckIsHead(string input)
    {
        var head = new Regex(_checkIsHeadRGX).Match(input);
        return head.Success;
    }

    static bool CheckSeparateEnable(string input)
    {
        var result = new Regex(_checkHaveDataRGX).Match(input);
        return result.Success;
    }
    #endregion
    #region get
    static string GetHead(string input)
    {
        var haveData = new Regex(_checkIsHeadRGX).Match(input);
        return haveData.Groups[1].Value;
    }

    static (string action, string data) SepareteHead(string input)
    {
        var haveData = new Regex(_checkHaveDataRGX).Match(input);
        return (haveData.Groups[1].Value, haveData.Groups[2].Value);
    }

    static string TrimTextData(string input)
    {
        if (!CheckIsHead(input)) return input;
        var heads = new List<string>(input.Split('\n')[0].Split(' '));
        if (heads.Count < 2) return input.Substring(input.Split('\n')[0].Length).Trim();
        heads.RemoveAt(0);
        return string.Join(" ", heads) + '\n' + input.Substring(input.Split('\n')[0].Length).Trim();

    }
    #endregion
}

public class TextConverterMono: MonoBehaviour
{
    [SerializeField,TextArea(0,10)] string _data;
    [SerializeField] string _rgx;
    [SerializeField] EventCodeScriptable _code;

    string _checkIsHeadRGX = @"\\(.*)";
    string _checkHaveDataRGX = @"\\(.*)\[(.*)\]";

    [ContextMenu("_rgx_dataDebug")]
    void Dip()
    {
        //Regex rgx = new Regex(_rgx);

        var match = Regex.Match(_data,_rgx,RegexOptions.Singleline);

        if (match.Success)
        {
            Debug.Log(match.Groups.Count + ":" + match.Value);

            foreach (Group m in match.Groups)
            {
                Debug.Log(m.Value.Trim());
            }
        }
        else
        {
            Debug.Log("miss");
        }
    }
    [ContextMenu("_seted_dataDebug")]
    void DisplayFullData()
    {
        TextConverter.DebugInfo(_data);
    }

    [ContextMenu("scriptableTest")]
    void DisplayAssetData()
    {
        var d1 = WindowBlock(_code.GetData());
        foreach (var d in d1)
        {
            if (CheckIsSeparate(d))
            {
                var d2 = SepareteHead(d);
                Debug.Log("action:" + d2.action + " data:" + d2.data);
            }
            else if (CheckIsHead(d))
            {
                Debug.Log("head:" + GetHead(d));
            }
            Debug.Log("Text:" + TrimTextData(d));
        }
    }

    [ContextMenu("replaceTest")]
    void ReplaceData()
    {

        Regex rgx = new Regex(_rgx);

        var match = rgx.Match(_data);

        if (match.Success)
        {
            var result=rgx.Replace(_data,"replace");
            Debug.Log(result);
        }
        else
        {
            Debug.Log("miss");
        }
    }
    [ContextMenu("EventDataTest")]
    void ChainRGX()
    {
        var datas = _data.Split(
            new string[] { "id:" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x = "id:" + x);
        foreach (var data in datas)
        {
            string idBrox = @"id: (.+?)\n(.+)";
            var match = Regex.Match(data, idBrox,RegexOptions.Singleline);
            Debug.Log("head:" + match.Groups[1].Value.Trim());
            Debug.Log("content:" + match.Groups[2].Value.Trim());
            string content = match.Groups[2].Value;
            string branket = "(.+?){(.+?)}";
            var matches2 = Regex.Matches(content, branket, RegexOptions.Singleline);
            foreach (Match ma2 in matches2)
            {
                Debug.Log("head:" + ma2.Groups[1].Value.Trim());
                Debug.Log(" content:" + ma2.Groups[2].Value.Trim());
            }
        }

    }
    
    List<string> WindowBlock(string input)
    {
        var d = input.Split('$');
        return d.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
    }
    

    bool CheckIsHead(string input)
    {
        var head = new Regex(_checkIsHeadRGX).Match(input);
        return head.Success;
    }

    bool CheckIsSeparate(string input)
    {
        var result = new Regex(_checkHaveDataRGX).Match(input);
        return result.Success;
    }
    
    string GetHead(string input)
    {
        var haveData = new Regex(_checkIsHeadRGX).Match(input);
        return haveData.Groups[1].Value;
    }

    (string action, string data) SepareteHead(string input)
    {
        var haveData = new Regex(_checkHaveDataRGX).Match(input);
        return (haveData.Groups[1].Value, haveData.Groups[2].Value);
    }

    string TrimTextData(string input)
    {
        if (!CheckIsHead(input)) return input;

        return input.Substring(input.Split('\n')[0].Length).Trim();
    }
}

