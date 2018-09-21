using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloLensModuleSingleton<T> : MonoBehaviour where T : HoloLensModuleSingleton<T>
{
    private static T instance;

    public static T Instance { get { return instance; } }

    protected virtual void Awake()
    {
        if (instance == null) instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
