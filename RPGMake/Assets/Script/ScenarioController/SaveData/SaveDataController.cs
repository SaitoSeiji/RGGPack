using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataController : SingletonMonoBehaviour<SaveDataController>
{
    [SerializeField] List<VariableDB> _variableDbList; 
    [SerializeField] List<DataMemberInspector> _memberSet=new List<DataMemberInspector>();
    [SerializeField] Dictionary<string,List<DBData>> _saveDataList = new Dictionary<string, List<DBData>>();

    void InitSaveDataList()
    {
        _saveDataList = new Dictionary<string, List<DBData>>();
        foreach (var db in _variableDbList)
        {
            var dataList = db.GetDataList();
            var tempSaveList = new List<DBData>();
            foreach (var data in dataList)
            {
                data.InitData();
                tempSaveList.Add(data._Data);
            }
            _saveDataList.Add(db.name, tempSaveList);
        }
    }

    void SetMemberSet()
    {
        _memberSet = new List<DataMemberInspector>();
        foreach (var dataList in _saveDataList)
        {
            foreach (var data in dataList.Value)
            {
                var temp = new DataMemberInspector();
                foreach (var st in data._memberSet_int)
                {
                    temp.AddData(data._serchId, st.Key, st.Value);
                }
                _memberSet.Add(temp);
            }
        }
    }
    #region dataChenge
    public void SetData<T>(DataMemberInspector data)
        where T : AbstractDB
    {
        foreach(var d in data._memberSet)
        {
            SetData<T>(data._id, d.memberName, d.data);
            //Debug.Log(d.data+":"+d.memberName);
        }
        SetMemberSet();
    }

    public void SetData<T>(string id,string memberName,int data)
        where T:AbstractDB
    {
        string name = "";
        AbstractDB targetDB = null;
        foreach(var db in _variableDbList)
        {
            if(db is T)
            {
                name = db.name;
                targetDB = db;
                break;
            }
        }
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("SaveDataController:SetData:name is null");
            return;
        }

        foreach(var saveData in _saveDataList)
        {
            if(saveData.Key==name)
            {
                foreach(var unit in saveData.Value)
                {
                    if (unit._serchId == id)
                    {
                        if (unit._memberSet_int.ContainsKey(memberName))
                        {
                            unit._memberSet_int[memberName] = data;
                            targetDB.DataUpdateAction(unit);
                        }
                        else
                        {
                            Debug.Log("SaveDataController:SetData:memberName is uncorrect");
                        }
                        return;
                    }
                }
                Debug.Log("SaveDataController:SetData:id is uncorrect");
                return;
            }
        }
    }

    public int GetData<T>(DataMemberInspector data)
        where T : AbstractDB
    {
        foreach(var mem in data._memberSet)
        {
            var result= GetData<T>(data._id, mem.memberName);
            //Debug.Log(result+":"+mem.memberName);
            return result;
        }
        return -1;
    }

    public int GetData<T>(string id, string memberName)
        where T : AbstractDB
    {
        string name = "";
        foreach (var db in _variableDbList)
        {
            if (db is T)
            {
                name = db.name;
                break;
            }
        }
        if (string.IsNullOrEmpty(name))
        {
            Debug.Log("SaveDataController:SetData:name is null");
            return -1;
        }

        foreach (var saveData in _saveDataList)
        {
            if (saveData.Key == name)
            {
                foreach (var unit in saveData.Value)
                {
                    if (unit._serchId == id)
                    {
                        if (unit._memberSet_int.ContainsKey(memberName))
                        {
                            return unit._memberSet_int[memberName];
                        }
                        else
                        {
                            Debug.Log("SaveDataController:SetData:memberName is uncorrect");
                        }
                        return -1;
                    }
                }
                Debug.Log("SaveDataController:SetData:id is uncorrect");
                return -1;
            }
        }
        return -1;
    }
    #endregion
    public void SaveAction()
    {
        foreach(var data in _saveDataList)
        {
            JsonSaver.SaveAction<DBData>(data.Value,data.Key);
        }
    }

    public void LoadAction()
    {
        foreach(var db in _variableDbList)
        {
            _saveDataList[db.name]= JsonSaver.LoadAction_list<DBData>(db.name);
        }
        SetMemberSet();
    }

    [ContextMenu("InitsaveTest")]
    void TestInitSave()
    {
        InitSaveDataList();
        SaveAction();
    }
    [ContextMenu("saveTest")]
    void TestSave()
    {
        SaveAction();
    }

    [ContextMenu("loadTest")]
    void TestLoad()
    {
        LoadAction();
    }

    [ContextMenu("valueChengeTest")]
    void TestValueChenge()
    {
        SetData<FlagDB>("t1", "flagNum", 5);
        SetMemberSet();
    }
}
