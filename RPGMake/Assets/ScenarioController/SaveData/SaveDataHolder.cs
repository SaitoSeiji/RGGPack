using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class SaveDataHolder : SingletonMonoBehaviour<SaveDataHolder>
{
    [SerializeField] FlagSaveData_registar flagRegistar;

    [SerializeField] int _maxItemCount;
    [SerializeField] UnityEvent _overItemAction;
    [SerializeField] EventCodeScriptable overItemData;

    int _overItemCount;

    string processPath = "processSave";
    string flagPath="flagSave";
    string itemPath="itemSave";

    //[SerializeField]PhoneCanvasSc phoneCanvas;
    

    private void Update()
    {
        switch (_overItemCount)
        {
            case 0:
                break;
            case 1:
                if (CheckIsOverItem())
                {
                    _overItemCount++;
                    //EventCodeReadController.Instance.AddEventData(overItemData.GetData());
                    if (!EventCodeReadController.getIsReadNow)
                    {

                        EventCodeReadController.Instance.StartEvent();
                    }
                }
                else
                {
                    _overItemCount--;
                }
                break;
            case 2:
                if (!EventController.Instance.GetReadNow())
                {
                    _overItemAction.Invoke();
                    _overItemCount++;
                    //if (phoneCanvas != null)
                    //{
                    //    phoneCanvas.gameObject.SetActive(true);
                    //}
                }
                break;
            case 3:
                if (!CheckIsOverItem())
                {
                    _overItemCount = 0;
                }
                //else if(!phoneCanvas.isActiveAndEnabled)
                //{
                //    EventCodeReadController.Instance.ResetEventData();
                //_overItemCount = 1;
                //}
                break;
        }
    }

    public void SaveAction()
    {
        //JsonSaver.SaveAction(processSaveData,processPath);
        //JsonSaver.SaveAction(flagSaveData, flagPath);
        //JsonSaver.SaveAction(itemSaveData, itemPath);
    }

    public void LoadAction()
    {
        //processSaveData = JsonSaver.LoadAction<ProcessSaveData>(processPath);
        //flagSaveData = JsonSaver.LoadAction_list<FlagSaveData>(flagPath);
        //itemSaveData = JsonSaver.LoadAction_list<ItemSaveData>(itemPath);
    }

    public void SetFlagNum(string flagName,int num)
    {
        //foreach(var data in flagSaveData)
        //{
        //    if (data._FlagName == flagName)
        //    {
        //        data.SetFlagCount(num);
        //        return;
        //    }
        //}
    }


    #region item関連
    

    public bool CheckIsOverItem()
    {
        //var holdItem = GetHoldItem();
        //return holdItem.Count > _maxItemCount;re
        return true;
    }
    
    #endregion

    public void AddProcessNum()
    {
        //processSaveData.AddProcess();
    }

    [ContextMenu("save")]
    public void Test_save()
    {
        SaveAction();
    }
    [ContextMenu("load")]
    public void Test_load()
    {
        LoadAction();
    }

    [ContextMenu("initData")]
    public void InitFlag() { 
        //{
    //    flagSaveData = new List<FlagSaveData>();
    //    foreach(var data in flagRegistar._flagSaveDataNameList)
    //    {
    //        flagSaveData.Add(new FlagSaveData(data));
    //    }

    //    itemSaveData = new List<ItemSaveData>();
        //foreach(var data in itemRegistar.itemSaveDataList)
        //{
        //    itemSaveData.Add(data);
        //}
    }

    public bool CheckCollectFlagName(string flagName)
    {
        //foreach(var flag in flagSaveData)
        //{
        //    if (flag._FlagName == flagName) return true;
        //}
        //return false;
        return true;
    }
}
