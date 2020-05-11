using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class SavedDBData
{
    public string _serchId;
}

public abstract class VariableDBData : AbstractDBData
{
    public abstract SavedDBData GetSavedDBData();
}
