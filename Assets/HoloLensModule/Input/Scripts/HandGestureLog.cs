using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class HandGestureLog : MonoBehaviour
    {
        private string[] HandGestureName = { "None", "Release", "Tap", "DoubleTap", "Hold", "DragStart", "Drag", "ShiftTap", "ShiftDoubleTap", "ShiftHold", "ShiftDragStart", "ShiftDrag", "MultiTap", "MultiDoubleTap", "MultiDragStart", "MultiDrag" };

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            HandsGestureManager.SingleHandGestureEvent += SingleHandGestureEvent;
            HandsGestureManager.MultiHandGestureEvent += MultiHandGestureEvent;
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
            HandsGestureManager.SingleHandGestureEvent -= SingleHandGestureEvent;
            HandsGestureManager.MultiHandGestureEvent -= MultiHandGestureEvent;
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state) { Debug.Log(HandGestureName[(int)state]); }

        private void SingleHandGestureEvent(HandsGestureManager.HandGestureState state, Vector3 pos) { Debug.Log(HandGestureName[(int)state]); }

        private void MultiHandGestureEvent(HandsGestureManager.HandGestureState state, Vector3 pos1, Vector3 pos2) { Debug.Log(HandGestureName[(int)state]); }
    }
}
