using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//仮実装
public class GameContoller : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SaveDataController.Instance.LoadAction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
