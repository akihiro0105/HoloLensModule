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
        public float SizeScale = 2.0f;
        [Range(0.01f, 1.0f)]
        public float MinScale = 0.01f;
        [Header("Lock Vector")]
        public bool X = false;
        public bool Y = false;
        public bool Z = false;

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
                        float deltamove = SizeScale * (Vector3.Distance(pos1, pos2) - deltadistace);
                        Vector3 bufscale = transform.localScale;
                        bool setflag = false;
                        if (X == false) bufscale.x = SetDeltaScale( out setflag, bufscale.x, deltamove);
                        if (Y == false) bufscale.y = SetDeltaScale( out setflag, bufscale.y, deltamove);
                        if (Z == false) bufscale.z = SetDeltaScale( out setflag, bufscale.z, deltamove);
                        if (setflag == false) transform.localScale = bufscale;
                        deltadistace = Vector3.Distance(pos1, pos2);
                    }
                    break;
            }
        }

        private float SetDeltaScale(out bool setflag, float scale, float deltamove)
        {
            float buf = scale;
            if (deltamove > 0) buf *= deltamove + 1.0f;
            else buf /= (-deltamove) + 1.0f;
            if (buf < MinScale) setflag = true;
            else setflag = false;
            return buf;
        }

        public void FocusEnter() { focusflag = true; }

        public void FocusEnd() { focusflag = false; }

        public bool isMove() { return dragflag; }
    }
}