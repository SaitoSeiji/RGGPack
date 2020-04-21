using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_addButton : MonoBehaviour
{
    [SerializeField] UIBase _target;
    [SerializeField] List<ButtonData> addList;

    //[ContextMenu("AddButton")]
    //public void AddButton()
    //{
    //    _target.AddButtonData(addList);
    //    _target.SyncButtonToText();
    //}

    //[ContextMenu("syncButton")]
    //public void SyncButton()
    //{
    //    _target.SyncButtonToText();
    //}

    //[ContextMenu("ResetButton")]
    //void ResetButton()
    //{
    //    _target.ResetButtonData();
    //    _target.SyncButtonToText();
    //}

    public void TestClickEvent(int i)
    {
        Debug.Log(i);
    }
}
