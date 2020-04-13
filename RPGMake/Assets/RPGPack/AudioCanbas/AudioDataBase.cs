using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyGame/Data/Create AudioDataBase", fileName = "AudioDataBase")]
public class AudioDataBase : ScriptableObject
{
    [System.Serializable]
    public class AudioData
    {
        public string _key;
        public AudioClip _clip;
    }

    public List<AudioData> _audioDataList;
}
