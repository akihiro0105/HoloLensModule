using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
using UnityEngine.VR.WSA;
#endif

namespace HoloLensModule.Input
{
    // HoloLensの視線安定化
    public class StabilizationCamera : HoloLensModuleSingleton<StabilizationCamera>
    {
        void LateUpdate()
        {
            RaycastHit info;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out info, 30.0f))
            {
                Vector3 normal = -Camera.main.transform.position;
                Vector3 position = Camera.main.transform.position;
                GameObject obj = RayCastControl.Instance.GetRayCastHitObject();
                if (obj != null) position = obj.transform.position;
#if UNITY_EDITOR || UNITY_UWP
                HolographicSettings.SetFocusPointForFrame(position, normal);
#endif
            }
        }
    }
}
