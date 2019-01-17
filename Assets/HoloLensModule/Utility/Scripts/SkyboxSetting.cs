using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR||WINDOWS_UWP
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

namespace HoloLensModule.Utility
{
    /// <summary>
    /// カメラの設定をHoloLensとWindows MRデバイスで分ける処理
    /// </summary>
    public class SkyboxSetting : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
#if UNITY_2017_2_OR_NEWER
            Camera.main.clearFlags = (HolographicSettings.IsDisplayOpaque) ? CameraClearFlags.Skybox : CameraClearFlags.SolidColor;
#endif
        }
    }
}
#endif
