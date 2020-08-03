﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISeveralAction : SingletonMonoBehaviour<UISeveralAction>
{
    [SerializeField] UIBase _shopUI;

    [ContextMenu("openShop")]
    public void OpenShop()
    {
        if (IsOpenShop()) return;
        //UIController.Instance.AddUI(_shopUI, true);
        UIController.AddUI(_shopUI).Register();
    }

    public bool IsOpenShop()
    {
        return _shopUI.gameObject.activeInHierarchy;
    }
}
