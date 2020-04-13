using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
