using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // オブジェクトの移動
    public class ObjectMoveControl : MonoBehaviour, FocusInterface
    {
        public bool LookAt = false;
        [Range(0.1f, 10.0f)]
        public float MoveScale = 1.0f;

        private bool focusflag = false;
        private bool dragflag = false;
        private Vector3 starthand;
        private Vector3 startpos;

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            HandsGestureManager.SingleHandGestureEvent += SingleHandGestureEvent;
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
            HandsGestureManager.SingleHandGestureEvent -= SingleHandGestureEvent;
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state==HandsGestureManager.HandGestureState.Release) dragflag = false;
        }

        private void SingleHandGestureEvent(HandsGestureManager.HandGestureState state, Vector3 pos)
        {
            switch (state)
            {
                case HandsGestureManager.HandGestureState.DragStart:
                    if (focusflag)
                    {
                        starthand = pos;
                        startpos = transform.position;
                        dragflag = true;
                    }
                    break;
                case HandsGestureManager.HandGestureState.Drag:
                    if (dragflag)
                    {
                        Vector3 move = pos - starthand;
                        transform.position = startpos + MoveScale * move;
                        if (LookAt) transform.LookAt(Camera.main.transform.position);
                    }
                    break;
            }
        }

        public void FocusEnter() { focusflag = true; }

        public void FocusEnd() { focusflag = false; }
    }
}
