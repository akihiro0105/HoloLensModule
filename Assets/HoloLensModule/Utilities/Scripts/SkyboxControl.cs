using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
#if UNITY_UWP
using UnityEngine.XR.WSA;
#endif
#endif

public class SkyboxControl : MonoBehaviour {
    public CameraClearFlags ClearFlag = CameraClearFlags.Skybox;

	// Use this for initialization
	void Start () {
#if !UNITY_2017_2_OR_NEWER
    #if UNITY_UWP
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
    #else
        Camera.main.clearFlags = ClearFlag;
    #endif
#else
    #if UNITY_UWP
        if(HolographicSettings.IsDisplayOpaque) Camera.main.clearFlags = ClearFlag;
        else Camera.main.clearFlags = CameraClearFlags.SolidColor;
    #else
        Camera.main.clearFlags = ClearFlag;
    #endif
#endif
    }
}
