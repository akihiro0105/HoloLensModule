using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // オブジェクトのサイズ
    public class ObjectScaleControl : MonoBehaviour, FocusInterface
    {
        [Range(0.1f, 10.0f)]
        public float Scale = 2.0f;
        [Range(0.01f, 1.0f)]
        public float MinScale = 0.01f;
        [Range(1.0f, 10.0f)]
        public float MaxScale = 5.0f;

        private bool focusflag = false;
        private bool dragflag = false;
        private float deltadistace;

        // Use this for initialization
        void Start()
        {
            HandsGestureManager.ReleaseHandGestureEvent += ReleaseHandGestureEvent;
            HandsGestureManager.MultiHandGestureEvent += MultiHandGestureEvent;
        }

        void OnDestroy()
        {
            HandsGestureManager.ReleaseHandGestureEvent -= ReleaseHandGestureEvent;
            HandsGestureManager.MultiHandGestureEvent -= MultiHandGestureEvent;
        }

        private void ReleaseHandGestureEvent(HandsGestureManager.HandGestureState state) { dragflag = false; }

        private void MultiHandGestureEvent(HandsGestureManager.HandGestureState state, Vector3 pos1, Vector3 pos2)
        {
            switch (state)
            {
                case HandsGestureManager.HandGestureState.MultiDragStart:
                    if (focusflag)
                    {
                        deltadistace = Vector3.Distance(pos1, pos2);
                        dragflag = true;
                    }
                    break;
                case HandsGestureManager.HandGestureState.MultiDrag:
                    if (dragflag)
                    {
                        float deltamove = Scale * (Vector3.Distance(pos1, pos2) - deltadistace);
                        float minsobjectscale = transform.localScale.x;
                        if (minsobjectscale > transform.localScale.y) minsobjectscale = transform.localScale.y;
                        if (minsobjectscale > transform.localScale.z) minsobjectscale = transform.localScale.z;
                        if (minsobjectscale + deltamove > MinScale && minsobjectscale + deltamove < MaxScale)
                        {
                            transform.localScale=new Vector3(transform.localScale.x + deltamove, transform.localScale.y + deltamove, transform.localScale.z + deltamove);
                        }
                        deltadistace = Vector3.Distance(pos1, pos2);
                    }
                    break;
            }
        }

        public void FocusEnter() { focusflag = true; }

        public void FocusEnd() { focusflag = false; }
    }
}