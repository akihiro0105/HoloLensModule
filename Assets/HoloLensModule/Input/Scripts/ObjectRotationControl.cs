using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // オブジェクトの回転
    public class ObjectRotationControl : MonoBehaviour, FocusInterface
    {
        [Range(1.0f, 100.0f)]
        public float RotationScale = 20.0f;

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
                case HandsGestureManager.HandGestureState.ShiftDragStart:
                    if (focusflag)
                    {
                        deltapos = Camera.main.transform.TransformPoint(pos);
                        dragflag = true;
                    }
                    break;
                case HandsGestureManager.HandGestureState.ShiftDrag:
                    if (dragflag)
                    {
                        Vector3 deltamove = Camera.main.transform.TransformPoint(pos) - deltapos;
                        transform.rotation = transform.rotation * Quaternion.AngleAxis(-deltamove.x * RotationScale * 10.0f, transform.up);
                        deltapos = Camera.main.transform.TransformPoint(pos);
                    }
                    break;
            }
        }

        public void FocusEnter() { focusflag = true; }

        public void FocusEnd() { focusflag = false; }
    }
}
