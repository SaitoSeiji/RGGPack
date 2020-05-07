using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Inspectorから入力を入れてTextDisplayerで出力する
public class Test_textDisplayer : MonoBehaviour
{
    [SerializeField] TextDisplayer dip;
    [SerializeField,TextArea(0,10)] List<string> data;
    [ContextMenu("textDisplay")]
    public void CoalText()
    {
        dip.SetTextData(data);
        dip.StartEvent();
    }

    [ContextMenu("test_textAction")]
    void CoalTextAction()
    {
        for(int i = 0; i < 10; i++)
        {
            string temp = "succses:"+i.ToString();
            dip.AddTextAction(i, () => Debug.Log(temp));
        }
        CoalText();
    }
    
}
