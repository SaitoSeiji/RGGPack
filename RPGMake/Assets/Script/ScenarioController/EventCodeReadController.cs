using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


#region codeData
public abstract class CodeData
{
    protected EventCodeScriptable _targetScr;
    public abstract void CodeAction();
    public abstract bool IsEndCode();

    //次のデータが吸収できるデータなら吸収する
    public virtual bool CheckChain(TextCovertedData data)
    {
        return false;
    }

    public virtual Queue<CodeData> GetNextCode()
    {
        return null;
    }

    public CodeData CreateCodeData(TextCovertedData data, EventCodeScriptable scr)
    {
        if (data == null) return new EndCode();
        if (CheckChain(data))
        {
            return this;
        }
        CodeData result = null;
        switch (data._head)
        {
            case "":
            case "name"://name[name]
                result = new TextData(data);
                break;
            case "branch"://branch \nbranchName \n $#1...
                result = new BranchCode(data);
                break;
            case "flag"://flag[flagName] 5
                result= new FlagCode(data);
                break;
            case "item"://item[itemName] 1
                result = new ItemCode(data);
                break;
            case "map"://map[mapName]
                result = new MapCode(data);
                break;
            case "image"://image[setName,num] back (center)
                result = new ImageCode(data);
                break;
            case "music"://music[setName,0]
                result = new AudioCode(data);
                break;
            case "battle"://battle[enemySetName]
                result = new BattleCode(data);
                break;
            case "load"://load[black] 500
                result = new LoadCode(data);
                break;
            case "wait"://wait[500]
                result = new WaitCode(data);
                break;
            default:
                return null;
        }
        result._targetScr = scr;
        return result;
    }
}

[System.Serializable]
public class TextData : CodeData
{
    public List<(string name, string txt)> dataList = new List<(string name, string txt)>();

    public TextData(TextCovertedData code)
    {
        dataList.Add((code._data, code._text));
    }

    public override void CodeAction()
    {
        foreach(var data in dataList)
        {
            if (string.IsNullOrEmpty(data.name))
            {
                EventCodeReadController.Instance._textDisplayer.SetTextData(data.txt);
            }
            else
            {
                EventCodeReadController.Instance._textDisplayer.SetTextData(data.txt, data.name);
            }
        }
        EventCodeReadController.Instance._textDisplayer.StartEvent();
    }

    public override bool IsEndCode()
    {
        return !EventCodeReadController.Instance._textDisplayer._readNow;
    }

    public override bool CheckChain(TextCovertedData data)
    {
        if (data._head == "" || data._head == "name")
        {
            dataList.Add((data._data, data._text));
            return true;
        }
        else
        {
            return false;
        }
    }
}

public class FlagCode : CodeData
{
    Dictionary<string, int> _flagData;

    public FlagCode(TextCovertedData data)
    {
        _flagData = GetFlag(data);
    }

    public Dictionary<string, int> GetFlag(TextCovertedData data)
    {
        var result = new Dictionary<string, int>();
        var texts = data._text.Split('\n');
        result.Add(data._data, int.Parse(texts[0]));

        for (int i = 1; i < texts.Length; i++)
        {
            try
            {
                var text = texts[i].Split(' ');
                result.Add(text[0], int.Parse(text[1]));
            }
            catch (System.IndexOutOfRangeException)
            {

            }
        }
        return result;
    }

    public override void CodeAction()
    {
        var db= SaveDataController.Instance.GetDB_var<FlagDB, SavedDBData_flag>();
        foreach (var flag in _flagData)
        {
            var temp = db.Find(x=>x._serchId==flag.Key) ;
            temp.flagNum = flag.Value;
            SaveDataController.Instance.SetData<FlagDB,SavedDBData_flag>(temp);
            
            //SaveDataHolder.Instance.SetFlagNum(flag.Key, flag.Value);
            //var key = FlagDBData.SetFlagNum(flag.Key, flag.Value);
            //SaveDataController.Instance.SetData<FlagDB>(key);
        }
    }

    public override bool IsEndCode()
    {
        return true;
    }
}

public class BranchCode : CodeData
{
    List<string> _selectList;
    Queue<CodeData>[] _nextCodes;
    int _selectNumber=-1;
    public BranchCode(TextCovertedData data)
    {
        _selectList = GetSelectList(data);
        _nextCodes = new Queue<CodeData>[_selectList.Count];
        for(int i=0;i<_nextCodes.Length;i++)
        {
            _nextCodes[i] = new Queue<CodeData>();
        }
    }

    List<string> GetSelectList(TextCovertedData data)
    {
        var result = new List<string>();
        var codes = data._text.Split('\n');

        foreach (var code in codes)
        {
            result.Add(code);
        }
        return result;
    }

    public override void CodeAction()
    {
        BranchDisplayer.Instance.StartBranch(_selectList);
    }
    public override bool IsEndCode()
    {
        if (BranchDisplayer.Instance.CheckIsSelected())
        {
            EventCodeReadController.Instance.SetFlashData("select", BranchDisplayer.Instance._SelectedData.ToString());
            _selectNumber =(int) BranchDisplayer.Instance._SelectedData;
            BranchDisplayer.Instance.EndBranch();
            return true;
        }
        else
        {
            return false;
        }
    }
    public override bool CheckChain(TextCovertedData data)
    {
        if (string.IsNullOrEmpty(data._head)) return false;
        var head = data._head.Substring(0, 1);
        if (int.TryParse(head, out int select))
        {
            var newData = new TextCovertedData(data._head.Substring(1),data._data,data._text);
            var next = new EndCode().CreateCodeData(newData, this._targetScr);
            if (!_nextCodes[select-1].Contains(next))
            {
                _nextCodes[select-1].Enqueue(next);
            }
            return true;
        }
        return false;
    }

    public override Queue<CodeData> GetNextCode()
    {
        return _nextCodes[_selectNumber];
    }
}

public class BattleCode:CodeData
{
    EnemySetData _targetEnemySet;

    public BattleCode(TextCovertedData data)
    {
        _targetEnemySet= GetEnemySet(data);
    }

    EnemySetData GetEnemySet(TextCovertedData data)
    {
        var db= SaveDataController.Instance.GetDB_static<EnemySetDB>();
        var dbdata= db._dataList.Where(x => x.name == data._data).First() as EnemySetDBData;
        return dbdata._enemySetData;
    }

    public override void CodeAction()
    {
        BattleController_mono.Instance.StartBattle(_targetEnemySet);
    }

    public override bool IsEndCode()
    {
        return BattleController_mono.Instance.IsBattleEnd();
    }
}

public class MapCode : CodeData
{
    string mapName;

    public MapCode(TextCovertedData data)
    {
        mapName = data._data;
    }

    public override void CodeAction()
    {
        MapController.Instance.ChengeMap(mapName);
    }

    public override bool IsEndCode()
    {

        return !MapController.Instance._mapChengeNow;
    }
}

public class ItemCode : CodeData
{
    Dictionary<string, int> _itemSet = new Dictionary<string, int>();


    public ItemCode(TextCovertedData data)
    {
        _itemSet = SetItemSet(data);
    }

    Dictionary<string, int> SetItemSet(TextCovertedData data)
    {
        var result = new Dictionary<string, int>();
        var texts = data._text.Split('\n');
        result.Add(data._data, int.Parse(texts[0]));
        for (int i = 1; i < texts.Length; i++)
        {
            var text = texts[i].Split(' ');
            try
            {
                result.Add(text[0], int.Parse(text[1]));
            }catch (System.IndexOutOfRangeException)
            {

            }
        }

        return result;
    }

    public override void CodeAction()
    {
        string dispTxt = "";
        var db = SaveDataController.Instance.GetDB_var<ItemDB, SavedDBData_item>();
        foreach (var d in _itemSet)
        {
            //var data = SaveDataHolder.Instance.GetItem(d.Key, d.Value);
            var data = db.Where(x => x._serchId == d.Key).First();
            data._haveNum += d.Value;
            SaveDataController.Instance.SetData<ItemDB, SavedDBData_item>(data);
            //var key = ItemDBData.AddHaveNum(d.Key, d.Value);
            //SaveDataController.Instance.SetData<ItemDB>(key);
            EventCodeReadController.Instance.SetFlashData("getItem",d.Key);
        }
    }

    public override bool IsEndCode()
    {
        return true;
    }
}

public class WaitCode : CodeData
{
    float waitTime;
    WaitFlag wf;

    public WaitCode(TextCovertedData data)
    {
        waitTime = int.Parse(data._data) / 100f;
    }

    public override void CodeAction()
    {
        wf = new WaitFlag();
        wf.SetWaitLength(waitTime);
        wf.WaitStart();
    }

    public override bool IsEndCode()
    {
        return !wf._waitNow;
    }
}

public class ImageCode : CodeData
{
    string setName;
    int number;
    bool reset = false;
    public enum ImagePos
    {
        CENTER, BACK,LEFT,RIGHT
    }
    ImagePos _imagePos;
    public enum ImageDirection
    {
        Origin,Reverse
    }
    ImageDirection _imageDirection=ImageDirection.Origin;

    public ImageCode(TextCovertedData data)
    {
        if (data._data == "reset")
        {
            reset = true;
            _imagePos = GetImagePos(data._text.Trim());
        }
        else
        {
            var d = data._data.Split(',');
            setName = d[0];
            number = int.Parse(d[1]);
            var text = data._text.Split(' ');
            _imagePos = GetImagePos(text[0].Trim());
            if (text.Length>=2)_imageDirection=GetImageDir(text[1].Trim());
        }
    }

    public override void CodeAction()
    {
        if (reset)
        {
            SpriteCanvas.Instance.ResetImage(_imagePos);
        }
        else
        {
            SpriteCanvas.Instance.SetImage(setName, number, _imagePos);
            SpriteCanvas.Instance.SetDirection(_imagePos, _imageDirection);
        }
    }

    ImagePos GetImagePos(string code)
    {
        switch (code)
        {
            case "center":
                return ImagePos.CENTER;
            case "back":
                return ImagePos.BACK;
            case "left":
                return ImagePos.LEFT;
            case "right":
                return ImagePos.RIGHT;
            default:
                return ImagePos.CENTER;
        }
    }
    ImageDirection GetImageDir(string code)
    {
        switch (code)
        {
            case "r":
                return ImageDirection.Reverse;
            case "o":
                return ImageDirection.Origin;
            default:
                return ImageDirection.Origin;
        }
    }

    public override bool IsEndCode()
    {
        return true;
    }
}

public class LoadCode : CodeData
{
    bool? toBlack;

    public LoadCode(TextCovertedData data)
    {
        if (data._data == "black")
        {
            toBlack = true;
        }
        else if (data._data == "clear")
        {
            toBlack = false;
        }
    }

    public override void CodeAction()
    {
        if (toBlack == null)
        {
            Debug.Log("uncorrect load code");
        }
        else
        {
            if ((bool)toBlack)
            {
                LoadCanvas.Instance.StartBlack();
            }
            else
            {
                LoadCanvas.Instance.StartClear();
            }
        }
    }

    public override bool IsEndCode()
    {
        //後で修正：ロードが終了したらtrueにする
        return true;
    }
}

public class AudioCode : CodeData
{
    string _soundeKey;
    int _soundIndex;
    bool _reset = false;

    public AudioCode(TextCovertedData data)
    {
        if (data._data == "reset") _reset = true;
        else
        {
            _soundeKey = data._data;
            _soundIndex = int.Parse( data._text);
        }
    }

    public override void CodeAction()
    {
        if (_reset)
        {

            AudioController.Instance.StopSound();
        }
        else
        {

            AudioController.Instance.SetSound(_soundeKey,_soundIndex);
        }
    }

    public override bool IsEndCode()
    {
        return true;
    }
}

public class EndCode : CodeData
{
    public override void CodeAction()
    {

    }

    public override bool IsEndCode()
    {
        return true;
    }
}
#endregion

public class EventCodeReadController : SingletonMonoBehaviour<EventCodeReadController>
{
    static bool _readNow = false;
    public static bool _getIsReadNow { get { return _readNow; } }
    Queue<CodeData> _codeList = new Queue<CodeData>();
    CodeData _nowCodeData;
    EventCodeScriptable _nowScriptable;

    public Dictionary<string, List<string>> _flashData = new Dictionary<string, List<string>>();

    [SerializeField]public TextDisplayer _textDisplayer;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_readNow)
        {
            if (_nowCodeData.IsEndCode())
            {
                var get = _nowCodeData.GetNextCode();
                if (get != null)
                {
                    _codeList = InsertQueue(get);
                }
                if (_codeList.Count > 0)//継続
                {
                    _nowCodeData = _codeList.Dequeue();
                    _nowCodeData.CodeAction();
                }
                else//nextEVがなければ終了
                {
                    var next= _nowScriptable.GetNextCode();
                    if (next == null
                        ||string.IsNullOrEmpty(next.GetData())
                        ||!next.CoalEnable())
                    {
                        EndEvent();
                        ResetFlashData();
                    }
                    else
                    {
                        SetEventData(next);
                        _nowCodeData = _codeList.Dequeue();
                        _nowCodeData.CodeAction();
                    }
                }
            }
        }
    }
    #region setEevntData

    public void SetEventData(EventCodeScriptable data)
    {
        if (data == null)
        {
            Debug.Log("SetEventData: data is null");
            return;
        }
        _nowScriptable = data;
        var dataList= TextConverter.Convert(data.GetData());
        CodeData nowCode=new EndCode();
        while (dataList.Count != 0)
        {
            var target = dataList.Dequeue();
            var nextCode=nowCode.CreateCodeData(target,data);
            if (nextCode.Equals(nowCode)) continue;
            nowCode = nextCode;
            _codeList.Enqueue(nextCode);
        }
    }
    
    public void ResetEventData()
    {
        _codeList = new Queue<CodeData>();
    }
    #endregion
    

    public void StartEvent()
    {
        _readNow = true;

        try
        {
            _nowCodeData = _codeList.Dequeue();
            _nowCodeData.CodeAction();
        }
        catch (System.InvalidOperationException)
        {
            _nowCodeData = new EndCode();
        }
    }
    void EndEvent()
    {
        _readNow = false;
    }

    Queue<CodeData> InsertQueue(Queue<CodeData> insert)
    {
        var newQueue = new Queue<CodeData>(insert);
        foreach(var data in _codeList)
        {
            newQueue.Enqueue(data);
        }
        return newQueue;
    }
    #region flash
    public void SetFlashData(string key,string data)
    {
        if (_flashData.ContainsKey(key))
        {
            _flashData[key].Add(data);
        }
        else
        {
            _flashData.Add(key, new List<string>());
            _flashData[key].Add(data);
        }
    }

    public List<string> GetFlashData(string key)
    {
        return _flashData[key];
    }

    public void RemoveFlashData(string key)
    {
        _flashData.Remove(key);
    }

    void ResetFlashData()
    {
        _flashData = new Dictionary<string, List<string>>();
    }
    #endregion

}
