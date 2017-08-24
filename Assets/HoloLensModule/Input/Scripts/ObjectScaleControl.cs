using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // オブジェクトのサイズ
    public class ObjectScaleControl : MonoBehaviour, FocusInterface
    {
        [Header("Lock Axis")]
        public bool X = true;
        public bool Y = true;
        public bool Z = true;
        [Range(0.1f, 10.0f)]
        public float Scale = 2.0f;
        [Range(0.01f, 1.0f)]
        public float MinScale = 0.05f;
        [Range(1.0f, 10.0f)]
        public float MaxScale = 5.0f;

        private bool focusflag = false;
        private bool dragflag = false;
        private float startdistance;
        private Vector3 startscale;
        private Vector3 initscale;

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.HandGestureEvent += HandGestureEvent;
            HandsGestureManager.MultiHandGestureEvent += MultiHandGestureEvent;
            initscale = transform.localScale;
        }

        void OnDestroy()
        {
            HandsGestureManager.HandGestureEvent -= HandGestureEvent;
            HandsGestureManager.MultiHandGestureEvent -= MultiHandGestureEvent;
        }

        private void HandGestureEvent(HandsGestureManager.HandGestureState state)
        {
            if (state == HandsGestureManager.HandGestureState.Release) dragflag = false;
        }

        private void MultiHandGestureEvent(HandsGestureManager.HandGestureState state, Vector3 pos1, Vector3 pos2)
        {
            switch (state)
            {
                case HandsGestureManager.HandGestureState.MultiDragStart:
                    if (focusflag)
                    {
                        startdistance = Vector3.Distance(pos1, pos2);
                        startscale = transform.localScale;
                        dragflag = true;
                    }
                    break;
                case HandsGestureManager.HandGestureState.MultiDrag:
                    if (dragflag)
                    {
                        float scaleX = startscale.x;
                        float scaleY = startscale.y;
                        float scaleZ = startscale.z;
                        if (X) scaleX = startscale.x + (Vector3.Distance(pos1, pos2) - startdistance) * Scale;
                        if (Y) scaleY = startscale.y + (Vector3.Distance(pos1, pos2) - startdistance) * Scale;
                        if (Z) scaleZ = startscale.z + (Vector3.Distance(pos1, pos2) - startdistance) * Scale;
                        if (scaleX < MinScale * initscale.x) scaleX = MinScale * initscale.x;
                        else if (scaleX > MaxScale * initscale.x) scaleX = MaxScale * initscale.x;
                        if (scaleY < MinScale * initscale.y) scaleY = MinScale * initscale.y;
                        else if (scaleY > MaxScale * initscale.y) scaleY = MaxScale * initscale.y;
                        if (scaleZ < MinScale * initscale.z) scaleZ = MinScale * initscale.z;
                        else if (scaleZ > MaxScale * initscale.z) scaleZ = MaxScale * initscale.z;
                        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                    }
                    break;
            }
        }

        public void FocusEnter() { focusflag = true; }

        public void FocusEnd() { focusflag = false; }
    }
}