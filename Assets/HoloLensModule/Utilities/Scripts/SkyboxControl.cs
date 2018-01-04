using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

public class SkyboxControl : MonoBehaviour {
    public Camera MRCamera;
	// Use this for initialization
	void Start () {
#if !UNITY_2017_2_OR_NEWER
        MRCamera.clearFlags = CameraClearFlags.SolidColor;
#else
        if (HolographicSettings.IsDisplayOpaque) MRCamera.clearFlags = CameraClearFlags.Skybox;
        else MRCamera.clearFlags = CameraClearFlags.SolidColor;
#endif
    }
}
