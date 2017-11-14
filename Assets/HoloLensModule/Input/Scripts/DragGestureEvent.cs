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
        public HandsGestureManager.HandGestureState ActionState = HandsGestureManager.HandGestureState.Tap;
        public HandsGestureEvent DragStartActionEvent = new HandsGestureEvent();
        public HandsGestureEvent DragUpdateActionEvent = new HandsGestureEvent();

        private bool focusflag = false;
        private bool dragflag = false;

        void Start()
        {
            HandsGestureManager.HandGestureStartEvent += HandGestureStartEvent;
            HandsGestureManager.HandGestureUpdateEvent += HandGestureUpdateEvent;
            HandsGestureManager.HandGestureEndEvent += HandGestureEndEvent;
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureStartEvent += HandGestureStartEvent;
            HandsGestureManager.HandGestureUpdateEvent -= HandGestureUpdateEvent;
            HandsGestureManager.HandGestureEndEvent -= HandGestureEndEvent;
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

        private void HandGestureEndEvent() { dragflag = false; }

        public void FocusEnd() { focusflag = false; }

        public void FocusEnter() { focusflag = true; }
    }
}
