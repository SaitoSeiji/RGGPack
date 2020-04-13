using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "MyGame/Data/Create AudioDataBase", fileName = "AudioDataBase")]
public class AudioDataBase : ScriptableObject
{
    [System.Serializable]
    public class AudioData
    {
        public string _setName;
        public List<AudioClip> _clipList=new List<AudioClip>();
    }

    [SerializeField] List<AudioData> _audioDataList=new List<AudioData>();

    public AudioClip GetData(string setName,int index)
    {
        var dataSet = _audioDataList.Where(x => x._setName == setName).FirstOrDefault();
        try
        {
            return dataSet._clipList[index];
        }
        catch (System.NullReferenceException)
        {
            Debug.Log("AudioDataBase:DataSetName is not exsist");
            return null;
        }
        catch (System.IndexOutOfRangeException)
        {
            Debug.Log("AudioDataBase:index is not exsist");
            return null;
        }
    }
}
