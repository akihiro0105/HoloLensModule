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
        private Vector3 deltapos;

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.ReleaseHandGestureEvent += ReleaseHandGestureEvent;
            HandsGestureManager.SingleHandGestureEvent += SingleHandGestureEvent;
        }

        void OnDestroy()
        {
            HandsGestureManager.ReleaseHandGestureEvent -= ReleaseHandGestureEvent;
            HandsGestureManager.SingleHandGestureEvent -= SingleHandGestureEvent;
        }

        private void ReleaseHandGestureEvent(HandsGestureManager.HandGestureState state) { dragflag = false; }

        private void SingleHandGestureEvent(HandsGestureManager.HandGestureState state, Vector3 pos)
        {
            switch (state)
            {
                case HandsGestureManager.HandGestureState.DragStart:
                    if (focusflag)
                    {
                        deltapos = pos;
                        dragflag = true;
                    }
                    break;
                case HandsGestureManager.HandGestureState.Drag:
                    if (dragflag)
                    {
                        transform.position += MoveScale * (pos - deltapos);
                        deltapos = pos;
                        if (LookAt) transform.LookAt(Camera.main.transform.position);
                    }
                    break;
            }
        }

        public void FocusEnter() { focusflag = true; }

        public void FocusEnd() { focusflag = false; }
    }
}
