using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace HoloLensModule.Input
{
    [Serializable]
    public class HandsGestureEvent : UnityEvent<Vector3, Vector3?> { }

    public class DragGestureEvent : MonoBehaviour, FocusInterface
    {
        public HandsGestureManager.HandGestureState ActionState = HandsGestureManager.HandGestureState.Drag;
        public HandControllerManager.HandControllerState HandState = HandControllerManager.HandControllerState.SelectDrag;
        public HandsGestureEvent DragStartActionEvent = new HandsGestureEvent();
        public HandsGestureEvent DragUpdateActionEvent = new HandsGestureEvent();

        private bool focusflag = false;
        private bool dragflag = false;

        void Start()
        {
#if UNITY_2017_2_OR_NEWER
            if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
#endif
            {
                HandsGestureManager.HandGestureStartEvent += HandGestureStartEvent;
                HandsGestureManager.HandGestureUpdateEvent += HandGestureUpdateEvent;
                HandsGestureManager.HandGestureEndEvent += HandGestureEndEvent;
            }
#if UNITY_2017_2_OR_NEWER
            else
            {
                HandControllerManager.HandControllerStartEvent += HandControllerStartEvent;
                HandControllerManager.HandControllerUpdateEvent += HandControllerUpdateEvent;
                HandControllerManager.HandControllerEndEvent += HandGestureEndEvent;
            }
#endif
        }

        void OnDestroy()
        {
#if UNITY_2017_2_OR_NEWER
            if (!UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
#endif
            {
                HandsGestureManager.HandGestureStartEvent += HandGestureStartEvent;
                HandsGestureManager.HandGestureUpdateEvent -= HandGestureUpdateEvent;
                HandsGestureManager.HandGestureEndEvent -= HandGestureEndEvent;
            }
#if UNITY_2017_2_OR_NEWER
            else
            {
                HandControllerManager.HandControllerStartEvent -= HandControllerStartEvent;
                HandControllerManager.HandControllerUpdateEvent -= HandControllerUpdateEvent;
                HandControllerManager.HandControllerEndEvent -= HandGestureEndEvent;
            }
#endif
        }

        private void HandGestureStartEvent(HandsGestureManager.HandGestureState state, Vector3 pos1, Vector3? pos2)
        {
            if (state == ActionState && focusflag)
            {
                DragStartActionEvent.Invoke(pos1, pos2);
                dragflag = true;
            }
        }

        private void HandGestureUpdateEvent(HandsGestureManager.HandGestureState state, Vector3 pos1, Vector3? pos2)
        {
            if (state == ActionState && dragflag) DragUpdateActionEvent.Invoke(pos1, pos2);
        }

        private void HandControllerStartEvent(HandControllerManager.HandControllerState state, Vector3 pos1, Quaternion rot1,Vector3? pos2,Quaternion? rot2)
        {
            if (state == HandState && focusflag)
            {
                DragStartActionEvent.Invoke(pos1, pos2);
                dragflag = true;
            }
        }

        private void HandControllerUpdateEvent(HandControllerManager.HandControllerState state, Vector3 pos1, Quaternion rot1, Vector3? pos2, Quaternion? rot2)
        {
            if (state == HandState && dragflag) DragUpdateActionEvent.Invoke(pos1, pos2);
        }

        private void HandGestureEndEvent() { dragflag = false; }

        public void FocusEnd() { focusflag = false; }

        public void FocusEnter() { focusflag = true; }
    }
}
