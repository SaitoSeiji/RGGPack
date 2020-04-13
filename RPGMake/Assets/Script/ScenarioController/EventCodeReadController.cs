using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region codeData
public abstract class CodeData
{
    protected EventCodeScriptable _targetScr;
    public abstract void CodeAction();
    public abstract bool IsEndCode();

    public CodeData CreateCodeData(TextCovertedData data, EventCodeScriptable scr)
    {
        if (data == null) return new EndCode();
        _targetScr = scr;
        switch (data._head)
        {
            case "flag"://flag[flagName] 5
                return new FlagCode(data);
            case "map"://map[mapName]
                return new MapCode(data);
            case "item"://item[itemName] 1
                return new ItemCode(data);
            case "wait"://wait[500]
                return new WaitCode(data);
            case "image"://image[setName,num] back (center)
                return new ImageCode(data);
            case "load"://load[black] 500
                return new LoadCode(data);
            case "music"://music[0]
                return new AudioCode(data);
            case "":
            case "name":
                return new TextData(data);
            case "branch":
                return new BranchCode(data);
            default:
                return null;
        }
    }
}

[System.Serializable]
public class TextData : CodeData
{
    public string name;
    public string data = "";

    public TextData(TextCovertedData code)
    {
        name = code._head;
        data = code._text;
    }

    public override void CodeAction()
    {
        if (string.IsNullOrEmpty(name))
        {
            TextDisplayer.Instance.SetTextData(data);
        }
        else
        {
            TextDisplayer.Instance.SetTextData(data, name);
        }
        TextDisplayer.Instance.StartEvent();
    }

    public override bool IsEndCode()
    {
        return !TextDisplayer.Instance._readNow;
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
            var text = texts[i].Split(' ');
            result.Add(text[0], int.Parse(text[1]));
        }
        return result;
    }

    public override void CodeAction()
    {
        foreach (var flag in _flagData)
        {
            //SaveDataHolder.Instance.SetFlagNum(flag.Key, flag.Value);
            var key = FlagDBData.SetFlagNum(flag.Key, flag.Value);
            SaveDataController.Instance.SetData<FlagDB>(key);
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
    public BranchCode(TextCovertedData data)
    {
        _selectList = GetSelectList(data);
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
            _targetScr.SetFlashData("select", BranchDisplayer.Instance._SelectedData.ToString());
            BranchDisplayer.Instance.EndBranch();
            return true;
        }
        else
        {
            return false;
        }
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
        Debug.Log(mapName);
        MapController.Instance.ChengeMap(mapName);
    }

    public override bool IsEndCode()
    {
        throw new System.NotImplementedException();
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
        foreach (var d in _itemSet)
        {
            //var data = SaveDataHolder.Instance.GetItem(d.Key, d.Value);
            var key = ItemDBData.AddHaveNum(d.Key, d.Value);
            SaveDataController.Instance.SetData<ItemDB>(key);
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
        CENTER, BACK
    }
    ImagePos imagePos;

    public ImageCode(TextCovertedData data)
    {
        if (data._data == "reset")
        {
            reset = true;
        }
        else
        {
            var d = data._data.Split(',');
            setName = d[0];
            number = int.Parse(d[1]);
            imagePos = GetImagePos(data._text);
        }
    }

    public override void CodeAction()
    {
        if (reset)
        {
            CoalImagePosAction_reset(imagePos);
        }
        else
        {
            CoalImagePosAction_set(imagePos);
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
            default:
                return ImagePos.CENTER;
        }
    }

    void CoalImagePosAction_set(ImagePos pos)
    {
        switch (pos)
        {
            case ImagePos.CENTER:
                SpriteCanvas.Instance.SetImageCenter(setName, number);
                break;
            case ImagePos.BACK:
                SpriteCanvas.Instance.SetImageBack(setName, number);
                break;
        }
    }
    void CoalImagePosAction_reset(ImagePos pos)
    {
        switch (pos)
        {
            case ImagePos.CENTER:
                SpriteCanvas.Instance.ResetImage_Center();
                break;
            case ImagePos.BACK:
                SpriteCanvas.Instance.ResetImage_Back();
                break;
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
    public static bool getIsReadNow { get { return _readNow; } }
    Queue<CodeData> _codeList = new Queue<CodeData>();
    CodeData _nowCodeData;
    EventCodeScriptable _nowScriptable;
    
    [SerializeField] TextDisplayer _textDisplayer;
    [SerializeField] BranchDisplayer _branchDisplayer;
    [SerializeField] SpriteCanvas _spriteCanvas;

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
                if (_codeList.Count > 0)//継続
                {
                    _nowCodeData = _codeList.Dequeue();
                    _nowCodeData.CodeAction();
                }
                else//nextEVがなければ終了
                {
                    var next= _nowScriptable.GetNextCode();
                    if (next == null)
                    {
                        EndEvent();
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
        _nowScriptable = data;
        var dataList= TextConverter.Convert(data.GetData());
        foreach(var d in dataList)
        {
            _codeList.Enqueue(new EndCode().CreateCodeData(d,data));
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
        _nowCodeData = _codeList.Dequeue();
        _nowCodeData.CodeAction();
    }
    void EndEvent()
    {
        _readNow = false;
    }

}
