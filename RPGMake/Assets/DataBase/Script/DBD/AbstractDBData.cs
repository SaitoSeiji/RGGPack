using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DBData
{
    public string _serchId;//検索用id
    public Dictionary<string, string> _memberSet_st = new Dictionary<string, string>();//メンバー変数として扱う 型はstring
    public Dictionary<string, int> _memberSet_int = new Dictionary<string, int>();//メンバー変数として扱う 型はint
    
    public DBData()
    {

    }

    public DBData(DBData data,string id)
    {
        _serchId = id;
        _memberSet_st = new Dictionary<string, string>(_memberSet_st);
        _memberSet_int = new Dictionary<string, int>(_memberSet_int);
    }
}

public abstract class AbstractDBData : ScriptableObject
{
    [SerializeField] DBData _data;
    public DBData _Data { get { return _data; } }


    public void InitData()
    {
        if (_Data == null) _data = new DBData();
        _Data._memberSet_st = InitMember_st();
        _Data._memberSet_int = InitMember_int();
        try
        {
            _Data._serchId = this.name;
        }
        catch (MissingReferenceException)
        {
            _Data._serchId = "default";
        }
    }

    protected abstract Dictionary<string, string> InitMember_st();
    protected abstract Dictionary<string, int> InitMember_int();
    protected abstract void UpdateMember();

    public static T GetInstance<T>()
        where T :ScriptableObject
    {
        return CreateInstance<T>();
    }

    public string CreateSaveTxt()
    {
        InitData();

        string result ="id " +_Data._serchId+"\n";
        foreach (var st in _Data._memberSet_st)
        {
            result += "\t" + st.Key + " " + st.Value+"\n";
        }
        foreach (var it in _Data._memberSet_int)
        {
            result += "\t" + it.Key + " " + it.Value+"\n";
        }
        return result;
    }

    public DBData GetDataTemplate()
    {
        InitData();
        return new DBData(_Data,"template");
    }


    protected static string GetDefaultString(string data)
    {
        return (string.IsNullOrEmpty(data)) ? "default" : data;
    }

    public void UpdateData(DBData data)
    {
        _data = data;
        UpdateMember();
    }

    public int GetTxtMemberCount()
    {
        InitData();
        return _Data._memberSet_int.Count + _Data._memberSet_int.Count;
    }

    //マイナスになってはいけないデータの確認など
    public virtual void CheckValueCorrect(DBData data)
    {

    }
}
