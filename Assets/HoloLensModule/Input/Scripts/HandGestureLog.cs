using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class HandGestureLog : MonoBehaviour
    {
        private string[] HandGestureName = { "Tap", "DoubleTap", "Hold", "Drag", "ShiftTap", "ShiftDoubleTap", "ShiftHold", "ShiftDrag", "MultiTap", "MultiDoubleTap", "MultiDrag" };
        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            HandsGestureManager.HandGestureStartEvent += HandGestureStartEvent;
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
            HandsGestureManager.HandGestureStartEvent -= HandGestureStartEvent;
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state) { Debug.Log(HandGestureName[(int)state]); }
        private void HandGestureStartEvent(HandsGestureManager.HandGestureState state, Vector3 pos1,Vector3? pos2) { Debug.Log(HandGestureName[(int)state]); }
    }
}
