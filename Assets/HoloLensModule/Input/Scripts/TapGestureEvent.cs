using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class TapGestureEvent : MonoBehaviour, FocusInterface
    {
        public HandsGestureManager.HandGestureState ActionState = HandsGestureManager.HandGestureState.Tap;
        public UnityEvent TapActionEvent = new UnityEvent();

        private bool focusflag = false;
        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state == ActionState && focusflag) TapActionEvent.Invoke();
        }

        public void FocusEnd() { focusflag = false; }

        public void FocusEnter() { focusflag = true; }
    }
}
