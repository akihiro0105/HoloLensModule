using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class TapGestureEvent : MonoBehaviour, FocusInterface
    {
        public HandsGestureManager.HandGestureState ActionState = HandsGestureManager.HandGestureState.Tap;
        public HandControllerManager.HandControllerState HandState = HandControllerManager.HandControllerState.SelectTap;
        public UnityEvent TapActionEvent = new UnityEvent();

        private bool focusflag = false;
        // Use this for initialization
        void Start()
        {
#if UNITY_2017_2_OR_NEWER
            if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
#endif
            {
                HandsGestureManager.HandGestureEvent += HandGestureEvent;
            }
#if UNITY_2017_2_OR_NEWER
            else
            {
                HandControllerManager.HandControllerEvent += HandControllerEvent;
            }
#endif
        }

        void OnDestroy()
        {
#if UNITY_2017_2_OR_NEWER
            if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
#endif
            {
                HandsGestureManager.HandGestureEvent -= HandGestureEvent;
            }
#if UNITY_2017_2_OR_NEWER
            else
            {
                HandControllerManager.HandControllerEvent -= HandControllerEvent;
            }
#endif
        }

private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state == ActionState && focusflag) TapActionEvent.Invoke();
        }

        private void HandControllerEvent(HandControllerManager.HandControllerState state)
        {
            if (state == HandState && focusflag) TapActionEvent.Invoke();
        }

        public void FocusEnd() { focusflag = false; }

        public void FocusEnter() { focusflag = true; }
    }
}
