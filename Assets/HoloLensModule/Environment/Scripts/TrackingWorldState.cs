using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#else
using UnityEngine.VR.WSA;
#endif

namespace HoloLensModule.Environment
{
    public class TrackingWorldState : MonoBehaviour
    {

        public delegate void TrackingWorldStateEventHandler();
        public TrackingWorldStateEventHandler LostTrackingWorldStateEvent;

        // Use this for initialization
        void Start()
        {
            WorldManager.OnPositionalLocatorStateChanged += OnPositionalLocatorStateChanged;
        }

        void OnDestroy()
        {
            WorldManager.OnPositionalLocatorStateChanged -= OnPositionalLocatorStateChanged;
        }

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
