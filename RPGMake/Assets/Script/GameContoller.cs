using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//仮実装
public class GameContoller : MonoBehaviour
{
    [SerializeField] EventDataMonoBehaviour firstEvent;
    // Start is called before the first frame update
    void Start()
    {
        SaveDataController.Instance.LoadAction();
        Player.Instance.Init();
        if(firstEvent!=null) EventController.Instance.CoalEvent(firstEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
