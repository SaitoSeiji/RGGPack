using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//仮実装
public class GameContoller : SingletonMonoBehaviour<GameContoller>
{
    [SerializeField] EventDataMonoBehaviour firstEvent;

    public bool _AnyOperate
    {
        get
        {
            bool result = false;
            var evc = EventController.Instance;
            if (evc != null && evc.GetReadNow()) result = true;
            var uic = UIController.Instance;
            if (uic != null && uic._OperateNow) result = true;
            return result;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SaveDataController.Instance.InitStaticDataBase();
        SaveDataController.Instance.LoadAction();
        Player.Instance.Init();
        if(firstEvent!=null) EventController.Instance.CoalEvent(firstEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
