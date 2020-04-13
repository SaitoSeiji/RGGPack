using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : SingletonMonoBehaviour<AudioController>
{
    [SerializeField] AudioDataBase _data;
    [SerializeField] AudioDataBase _se;
    [SerializeField] AudioSource source;
    [SerializeField] float _fadeTime;
    public void SetSound(AudioClip ac)
    {
        ChengeSound(ac);
    }
    public void SetSound(int key)
    {
        ChengeSound(_data._audioDataList[0]._clip);
    }

    public void SetSound(string key)
    {
        foreach (var data in _data._audioDataList)
        {
            if (data._key == key)
            {
                ChengeSound(data._clip);
            }
        }
    }

    public void PlaySE(AudioClip ac)
    {
        source.PlayOneShot(ac);
    }

    public void PlaySE(string key)
    {
        foreach (var data in _se._audioDataList)
        {
            if (data._key == key)
            {
                source.PlayOneShot(data._clip);
            }
        }
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
        //source.DOFade(0, _fadeTime);
        Debug.Log("off volume is not implemented");
    }


    [SerializeField] string _testKey;
    [ContextMenu("play")]
    public void Test_coalMusic()
    {
        SetSound(_testKey);
    }

}
