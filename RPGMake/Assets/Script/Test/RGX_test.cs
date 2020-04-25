using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using static EventDataOperater;

public class RGX_test : MonoBehaviour
{
    [SerializeField] TextAsset readText;

    [ContextMenu("diveideText")]
    void DivideTextOut()
    {
        var list = DivideTextBlock(readText.text);
        foreach(var data in list)
        {
            var ids = DivideId(data);
            Debug.Log("アイデー" + ids.id);
            var blankets = ReplaceBlanket(ids.replaced.Trim());
            foreach(var d in blankets.contents)
            {
                Debug.Log(d);
            }
            //Debug.Log("replaced:"+blankets.replaced);
            var singles = DivideSingle(blankets.replaced);
            foreach (var single in singles)
            {
                Debug.Log(single);
            }
        }
    }


    (string id, string replaced) DivideId(string data)
    {
        string rgx = @"id:([\s]*?)([^\s]+)";
        var match = Regex.Match(data, rgx, RegexOptions.Singleline);
        string id = match.Groups[2].Value;
        string replaced = Regex.Replace(data, rgx, "", RegexOptions.Singleline);
        return (id, replaced);
    }

    private (List<string> contents,string replaced) ReplaceBlanket(string data)
    {

        string rgx = @"([^\s]+?)[\s]*{(.+?)}";
        var matches = Regex.Matches(data, rgx, RegexOptions.Singleline);
        var contents = new List<string>();
        foreach (Match match in matches)
        {
            string head = match.Groups[1].Value.Trim();
            string content = match.Groups[2].Value.Trim();
            contents.Add(string.Format("head:{0}\ncontent:{1}",head,content));
        }
        var replaced= Regex.Replace(data,rgx,"",RegexOptions.Singleline);
        replaced = string.Join("\n",(replaced.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray()));
        return (contents,replaced.Trim());
    }

    List<string> DivideSingle(string data)
    {
        var result= new List<string>();
        var split = data.Split('\n');
        foreach(var s in split)
        {
            var ss = s.Split(' ');
            if (ss.Length != 2) continue;
            result.Add(string.Format("head:{0}\ncontent:{1}",ss[0],ss[1]));
        }
        return result;
    }


    private string[] DivideTextBlock(string text)
    {
        var datas = text.Split(new string[] { "id:" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x = "id:" + x).ToArray();
        return datas;
    }
}
