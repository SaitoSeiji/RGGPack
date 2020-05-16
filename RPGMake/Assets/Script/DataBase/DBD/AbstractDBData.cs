using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//text->dbの変換の際に使用するデータ
[System.Serializable]
public class TempDBData
{
    public string _serchId;//検索用id AbstractDBDataのnameと同じ
    Dictionary<string, string> _memberSet_st = new Dictionary<string, string>();//メンバー変数として扱う 型はstring
    Dictionary<string, int> _memberSet_int = new Dictionary<string, int>();//メンバー変数として扱う 型はint
    Dictionary<string, List<string>> _memberSet_stList = new Dictionary<string, List<string>>();//型はlist<string>
    
    public TempDBData(string id)
    {
        _serchId = id;
        _memberSet_int = new Dictionary<string, int>();
        _memberSet_st = new Dictionary<string, string>();
        _memberSet_stList = new Dictionary<string, List<string>>();
    }
    
    public void InitMember(Dictionary<string, string> _memberSet_st,
        Dictionary<string, int> _memberSet_int,
        Dictionary<string, List<string>> _memberSet_list)
    {
        this._memberSet_st = _memberSet_st;
        this._memberSet_int = _memberSet_int;
        this._memberSet_stList = _memberSet_list;
    }

    public string GetData_st(string key)
    {
        if (!_memberSet_st.ContainsKey(key)) return "";
        return _memberSet_st[key];
    }
    public int GetData_int(string key)
    {
        if (!_memberSet_int.ContainsKey(key)) return 0;
        return _memberSet_int[key];
    }
    public List<string> GetData_list(string key)
    {
        if (!_memberSet_stList.ContainsKey(key)) return new List<string>();
        return _memberSet_stList[key];
    }
}

public abstract class AbstractDBData : ScriptableObject
{
    public string _serchId;
    public void UpdateMember(TempDBData data)
    {
        _serchId = data._serchId;
        UpdateMember_child(data);
    }
    protected abstract void UpdateMember_child(TempDBData data);
    public virtual void RateUpdateMemeber() { }//データベース全体の登録が済んだ後に行うアップデート

    public static T GetInstance<T>()
        where T :ScriptableObject
    {
        return CreateInstance<T>();
    }
}
