using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class SavedDBData
{
    public string _serchId;

    public virtual void ModifyData() { }
}

public abstract class VariableDBData : AbstractDBData
{
    public SavedDBData GetSavedDBData()
    {
        var temp= GetSavedDBData_child();
        temp._serchId = this.name;
        return temp;
    }
    protected abstract SavedDBData GetSavedDBData_child();
}
