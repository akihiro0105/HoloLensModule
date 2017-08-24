using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // オブジェクトの回転
    public class ObjectRotationControl : MonoBehaviour, FocusInterface
    {
        [Header("ActiveAxis")]
        public bool X = false;
        public bool Y = true;
        public bool Z = false;
        [Range(1.0f, 100.0f)]
        public float RotationScale = 20.0f;

        private bool focusflag = false;
        private bool dragflag = false;
        private Vector3 starthand;
        private Quaternion startrot;

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
            if (state == HandsGestureManager.HandGestureState.Release) dragflag = false;
        }

        private void SingleHandGestureEvent(HandsGestureManager.HandGestureState state, Vector3 pos)
        {
            switch (state)
            {
                case HandsGestureManager.HandGestureState.ShiftDragStart:
                    if (focusflag)
                    {
                        starthand = Camera.main.transform.TransformPoint(pos);
                        startrot = transform.rotation;
                        dragflag = true;
                    }
                    break;
                case HandsGestureManager.HandGestureState.ShiftDrag:
                    if (dragflag)
                    {
                        Vector3 move = Camera.main.transform.TransformPoint(pos) - starthand;
                        if (X) transform.rotation = startrot * Quaternion.AngleAxis(move.y * RotationScale * 10.0f, transform.right);
                        if (Y) transform.rotation = startrot * Quaternion.AngleAxis(-move.x * RotationScale * 10.0f, transform.up);
                        if (Z) transform.rotation = startrot * Quaternion.AngleAxis(-move.z * RotationScale * 10.0f, transform.forward);
                    }
                    break;
            }
        }

        public void FocusEnter() { focusflag = true; }

        public void FocusEnd() { focusflag = false; }
    }
}
