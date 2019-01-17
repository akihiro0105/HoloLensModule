using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR||WINDOWS_UWP
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#else
using UnityEngine.VR.WSA;
#endif

namespace HoloLensModule.Environment
{
    /// <summary>
    /// HoloLensのトラッキング情報の取得
    /// </summary>
    public class TrackingWorldState : MonoBehaviour
    {
        public event Action LostTrackingWorldStateEvent;

        // Use this for initialization
        void Start()
        {
            WorldManager.OnPositionalLocatorStateChanged += OnPositionalLocatorStateChanged;
        }

        void OnDestroy()
        {
            WorldManager.OnPositionalLocatorStateChanged -= OnPositionalLocatorStateChanged;
        }

        /// <summary>
        /// トラッキングロスト時にイベント通知
        /// </summary>
        /// <param name="oldstate"></param>
        /// <param name="newstate"></param>
        private void OnPositionalLocatorStateChanged(PositionalLocatorState oldstate, PositionalLocatorState newstate)
        {
            if (newstate != PositionalLocatorState.Active)
            {
                // becomimg tracking lost
                if (LostTrackingWorldStateEvent != null) LostTrackingWorldStateEvent();
            }
        }
    }
}
#endif
