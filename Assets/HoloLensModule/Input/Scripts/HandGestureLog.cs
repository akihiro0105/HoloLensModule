using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class HandGestureLog : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            HandsGestureManager.SingleHandGestureEvent += SingleHandGestureEvent;
            HandsGestureManager.MultiHandGestureEvent += MultiHandGestureEvent;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
            HandsGestureManager.SingleHandGestureEvent -= SingleHandGestureEvent;
            HandsGestureManager.MultiHandGestureEvent -= MultiHandGestureEvent;
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            ViewGestureLog(state);
        }

        private void SingleHandGestureEvent(HandsGestureManager.HandGestureState state,Vector3 pos)
        {
            ViewGestureLog(state);
        }

        private void MultiHandGestureEvent(HandsGestureManager.HandGestureState state, Vector3 pos1,Vector3 pos2)
        {
            ViewGestureLog(state);
        }

        private void ViewGestureLog(HandsGestureManager.HandGestureState state)
        {
            switch (state)
            {
                case HandsGestureManager.HandGestureState.Tap:
                    Debug.Log("Tap");
                    break;
                case HandsGestureManager.HandGestureState.DoubleTap:
                    Debug.Log("DoubleTap");
                    break;
                case HandsGestureManager.HandGestureState.Hold:
                    Debug.Log("Hold");
                    break;
                case HandsGestureManager.HandGestureState.DragStart:
                    Debug.Log("DragStart");
                    break;
                case HandsGestureManager.HandGestureState.ShiftTap:
                    Debug.Log("ShiftTap");
                    break;
                case HandsGestureManager.HandGestureState.ShiftDoubleTap:
                    Debug.Log("ShiftDoubleTap");
                    break;
                case HandsGestureManager.HandGestureState.ShiftHold:
                    Debug.Log("ShiftHold");
                    break;
                case HandsGestureManager.HandGestureState.ShiftDragStart:
                    Debug.Log("ShiftDragStart");
                    break;
                case HandsGestureManager.HandGestureState.MultiTap:
                    Debug.Log("MultiTap");
                    break;
                case HandsGestureManager.HandGestureState.MultiDoubleTap:
                    Debug.Log("MultiDoubleTap");
                    break;
                case HandsGestureManager.HandGestureState.MultiDragStart:
                    Debug.Log("MultiDragStart");
                    break;
            }
        }
    }
}
