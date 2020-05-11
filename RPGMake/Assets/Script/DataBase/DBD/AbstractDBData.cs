using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        return _memberSet_st[key];
    }
    public int GetData_int(string key)
    {
        return _memberSet_int[key];
    }
    public List<string> GetData_list(string key)
    {
        return _memberSet_stList[key];
    }
}

public abstract class AbstractDBData : ScriptableObject
{
    //[SerializeField] TempDBData _data_tem;
    //public TempDBData _Data { get { return _data_tem; } }


    //public void InitData()
    //{
    //    if (_Data == null) _data_tem = new TempDBData("default");
    //    _Data._memberSet_st = InitMember_st();
    //    _Data._memberSet_int = InitMember_int();
    //    _Data._memberSet_stList = InitMemeber_stList();
    //    try
    //    {
    //        _Data._serchId = this.name;
    //    }
    //    catch (MissingReferenceException)
    //    {
    //        _Data._serchId = "default";
    //    }
    //}

    protected abstract Dictionary<string, string> InitMember_st();
    protected abstract Dictionary<string, List<string>> InitMemeber_stList();
    protected abstract Dictionary<string, int> InitMember_int();
    public abstract void UpdateMember(TempDBData data);
    public virtual void RateUpdateMemeber() { }//データベース全体の登録が済んだ後に行うアップデート

    public static T GetInstance<T>()
        where T :ScriptableObject
    {
        return CreateInstance<T>();
    }

    //public string CreateSaveTxt()
    //{
    //    InitData();

    //    string result ="id " +_Data._serchId+"\n";
    //    foreach (var st in _Data._memberSet_st)
    //    {
    //        result += "\t" + st.Key + " " + st.Value+"\n";
    //    }
    //    foreach (var it in _Data._memberSet_int)
    //    {
    //        result += "\t" + it.Key + " " + it.Value+"\n";
    //    }
    //    return result;
    //}

    protected static string GetDefaultString(string data)
    {
        return (string.IsNullOrEmpty(data)) ? "default" : data;
    }

    //public void UpdateData(TempDBData data)
    //{
    //    _data_tem = data;
    //    UpdateMember();
    //}

    //public int GetTxtMemberCount()
    //{
    //    InitData();
    //    return _Data._memberSet_int.Count + _Data._memberSet_int.Count;
    //}

    //マイナスになってはいけないデータの確認など
    public virtual void DataUpdateAction(TempDBData data)
    {

    }
}
