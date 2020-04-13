using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaitAction:SingletonMonoBehaviour<WaitAction>
{
    public void CoalWaitAction(UnityAction ua,float time)
    {
        StartCoroutine(WaitCorutin(ua,time));
    }

    IEnumerator WaitCorutin(UnityAction ua, float time)
    {
        yield return new WaitForSeconds(time);
        ua.Invoke();
    }
}
