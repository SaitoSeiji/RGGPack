using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AudioController : SingletonMonoBehaviour<AudioController>
{
    AudioDataBase _audioDataBase { get {return ResourceDB_mono.Instance._audioDB; } }
    [SerializeField] AudioSource source;
    [SerializeField] float _fadeTime;
    public void SetSound(AudioClip ac)
    {
        ChengeSound(ac);
    }
    public void SetSound(string setName,int key)
    {
        ChengeSound(_audioDataBase.GetData(setName,key));
    }
    
    public void PlaySE(AudioClip ac)
    {
        source.PlayOneShot(ac);
    }

    public void PlaySE(string setName,int key)
    {
        source.PlayOneShot(_audioDataBase.GetData(setName,key));
    }

    public void StopSound()
    {
        source.Stop();
    }

    void ChengeSound(AudioClip ac)
    {
        OffVolume();
        WaitAction.Instance.CoalWaitAction(() => source.Stop(), _fadeTime + 0.05f);
        WaitAction.Instance.CoalWaitAction(() => source.clip = ac, _fadeTime + 0.08f);
        WaitAction.Instance.CoalWaitAction(() => source.volume = 1, _fadeTime + 0.08f);
        WaitAction.Instance.CoalWaitAction(() => source.Play(), _fadeTime + 0.1f);
    }

    void OffVolume()
    {
        source.DOFade(0, _fadeTime);
    }


    [SerializeField] string _testKey;
    [ContextMenu("play")]
    public void Test_coalMusic()
    {
        SetSound(_testKey,0);
    }

}
