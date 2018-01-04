using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class HandControllerManager : HoloLensModuleSingleton<HandControllerManager>
    {
        public GameObject HandObjectPrefab;

        [Range(-0.1f, 0.1f)]
        public float Offset_up = 0.05f;
        [Range(-0.1f, 0.1f)]
        public float Offset_foward = -0.05f;

        public float HoldTriggerTime = 1.0f;// Hold
        public float PressIntervalTime = 0.5f;// Tap判定時間間隔
        public float DragDistance = 0.02f;// Drag判定距離

        public delegate void HandControllerEventHandler(HandControllerState state);
        public delegate void HandControllerStartEventHandler(HandControllerState state, Vector3 pos1, Quaternion rot1, Vector3? pos2, Quaternion? rot2);
        public delegate void HandControllerUpdateEventHandler(HandControllerState state, Vector3 pos1, Quaternion rot1, Vector3? pos2, Quaternion? rot2);
        public delegate void HandControllerEndEventHandler();
        public static event HandControllerEventHandler HandControllerEvent;// Tap,Hold,Menu
        public static event HandControllerStartEventHandler HandControllerStartEvent;// Drag
        public static event HandControllerUpdateEventHandler HandControllerUpdateEvent;// Drag
        public static event HandControllerEndEventHandler HandControllerEndEvent;// Drag

        [Serializable]
        public enum HandControllerState
        {
            // hand controller
            SelectTap,
            SelectDoubleTap,
            SelectHold,
            SelectDrag,

            GripTap,
            GripDoubleTap,
            GripHold,
            GripDrag,
            // multi hand multi gesture
            MultiTap,
            MultiDoubleTap,
            MultiDrag
        }

        public enum HandObjectState
        {
            None,
            Tap,
            DoubleTap,
            Hold,
            DragStart,
            DragUpdate
        }

        public class HandObjectClass
        {
            private GameObject obj;

            private bool pressflag = false;
            private float presstime;
            private Vector3 presspos;
            private float taptime;
            private bool dragflag = false;

            private float HoldTriggerTime;
            private float PressIntervalTime;
            private float DragDistance;

            private bool shiftflag = false;

            private HandObjectControl objectcontrol;

            public HandObjectClass(GameObject prefab, Transform parent, float HoldTriggerTime, float PressIntervalTime, float DragDistance)
            {
                obj = Instantiate(prefab);
                objectcontrol = obj.GetComponent<HandObjectControl>();
                obj.transform.parent = parent;
                obj.transform.localRotation = Quaternion.identity;
                this.HoldTriggerTime = HoldTriggerTime;
                this.PressIntervalTime = PressIntervalTime;
                this.DragDistance = DragDistance;
            }

            public HandObjectState onUpdated(Vector3 pos, Quaternion rot,float up, float foward)
            {
                obj.transform.position = pos + obj.transform.up * up + obj.transform.forward * foward;
                obj.transform.rotation = rot;
                HandObjectState state = HandObjectState.None;
                if (dragflag) state = HandObjectState.DragUpdate;
                else if (pressflag)
                {
                    if (Vector3.Distance(presspos, obj.transform.position) > DragDistance)
                    {
                        state = HandObjectState.DragStart;
                        dragflag = true;
                        if (objectcontrol != null) objectcontrol.onReleased();
                    }
                    else if (Time.time - presstime > HoldTriggerTime)
                    {
                        state = HandObjectState.Hold;
                        pressflag = false;
                        if (objectcontrol != null) objectcontrol.onReleased();
                    }
                }
                return state;
            }

            public void onPressed(bool shiftflag)
            {
                pressflag = true;
                presstime = Time.time;
                presspos = obj.transform.position;
                this.shiftflag = shiftflag;
                if (objectcontrol != null) objectcontrol.onPressed(shiftflag, HoldTriggerTime);
            }

            public HandObjectState onReleased()
            {
                HandObjectState state = HandObjectState.None;
                if (Time.time - presstime < PressIntervalTime)
                {
                    state = (Time.time - taptime < PressIntervalTime) ? HandObjectState.DoubleTap : HandObjectState.Tap;
                }
                taptime = Time.time;
                pressflag = false;
                dragflag = false;
                if (objectcontrol != null) objectcontrol.onReleased();
                return state;
            }
            public bool GetPress() { return pressflag; }
            public Vector3 GetPosition() { return obj.transform.position; }
            public Quaternion GetRotation() { return obj.transform.rotation; }
            public void DestroyObject() { Destroy(obj.gameObject); }
            public void SetHandObjectControlRelease() { if (objectcontrol != null) objectcontrol.onReleased(); }
            public bool GetShiftFlag() { return shiftflag; }
        }

        private Dictionary<uint, HandObjectClass> HandObjects = new Dictionary<uint, HandObjectClass>();
        private bool multihandflag = false;
        private bool multipressflag = false;
        private float multipresstime;
        private float multitaptime;

        // Use this for initialization
        void Start()
        {
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
            {
                HandsInteractionManager.onDetected += onDetected;
                HandsInteractionManager.onUpdated += onUpdated;
                HandsInteractionManager.onLost += onLost;
                HandsInteractionManager.onPressed += onPressed;
                HandsInteractionManager.onReleased += onReleased;
            }
#endif
        }

        protected override void OnDestroy()
        {
#if UNITY_2017_2_OR_NEWER
            if (UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque)
            {
                HandsInteractionManager.onDetected -= onDetected;
                HandsInteractionManager.onUpdated -= onUpdated;
                HandsInteractionManager.onLost -= onLost;
                HandsInteractionManager.onPressed -= onPressed;
                HandsInteractionManager.onReleased -= onReleased;
            }
#endif
            base.OnDestroy();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void onDetected(HandsInteractionManager.HandPointClass hand)
        {
            HandObjects.Add(hand.id, new HandObjectClass(HandObjectPrefab, transform, HoldTriggerTime, PressIntervalTime, DragDistance));
        }

        private void onUpdated(HandsInteractionManager.HandPointClass hand)
        {
            HandObjectClass obj;
            if (HandObjects.TryGetValue(hand.id, out obj))
            {
                HandObjectState state = obj.onUpdated(hand.pos, hand.rot, Offset_up, Offset_foward);
                int dragcount = 0;
                Vector3[] pos = new Vector3[2];
                Quaternion[] rot = new Quaternion[2];
                foreach (var item in HandObjects.Values)
                {
                    if (item.GetPress())
                    {
                        pos[dragcount] = item.GetPosition();
                        rot[dragcount] = item.GetRotation();
                        dragcount++;
                    }
                }
                if (dragcount == 2)
                {
                    if (Time.time - multipresstime > PressIntervalTime)
                    {
                        if (multihandflag == false)
                        {
                            if (HandControllerStartEvent != null) HandControllerStartEvent(HandControllerState.MultiDrag, pos[0], rot[0], pos[1], rot[1]);
                            multihandflag = true;
                        }
                        else
                        {
                            if (HandControllerUpdateEvent != null) HandControllerUpdateEvent(HandControllerState.MultiDrag, pos[0], rot[0], pos[1], rot[1]);
                        }
                    }
                    obj.SetHandObjectControlRelease();
                }
                else
                {
                    if (multihandflag)
                    {
                        if (dragcount == 0) multihandflag = false;
                    }
                    else
                    {
                        if (state == HandObjectState.Hold)
                        {
                            if (obj.GetShiftFlag() == false)
                            {
                                if (HandControllerEvent != null) HandControllerEvent(HandControllerState.SelectHold);
                            }
                            else
                            {
                                if (HandControllerEvent != null) HandControllerEvent(HandControllerState.GripHold);
                            }
                        }
                        else if (state == HandObjectState.DragStart)
                        {
                            if (obj.GetShiftFlag() == false)
                            {
                                if (HandControllerStartEvent != null) HandControllerStartEvent(HandControllerState.SelectDrag, obj.GetPosition(), obj.GetRotation(), null, null);
                            }
                            else
                            {
                                if (HandControllerStartEvent != null) HandControllerStartEvent(HandControllerState.GripDrag, obj.GetPosition(), obj.GetRotation(), null, null);
                            }
                        }
                        else if (state == HandObjectState.DragUpdate)
                        {
                            if (obj.GetShiftFlag() == false)
                            {
                                if (HandControllerUpdateEvent != null) HandControllerUpdateEvent(HandControllerState.SelectDrag, obj.GetPosition(), obj.GetRotation(), null, null);
                            }
                            else
                            {
                                if (HandControllerUpdateEvent != null) HandControllerUpdateEvent(HandControllerState.GripDrag, obj.GetPosition(), obj.GetRotation(), null, null);
                            }
                        }
                    }
                }
            }
        }

        private void onLost(HandsInteractionManager.HandPointClass hand)
        {
            HandObjectClass obj;
            if (HandObjects.TryGetValue(hand.id, out obj))
            {
                obj.DestroyObject();
                HandObjects.Remove(hand.id);
            }
        }

        private void onPressed(HandsInteractionManager.HandPointClass hand)
        {
            HandObjectClass obj;
            if (HandObjects.TryGetValue(hand.id, out obj))
            {
                if (hand.select>0.0f)
                {
                    obj.onPressed(false);
                }
                else if (hand.grip==true)
                {
                    obj.onPressed(true);
                }
            }
            int count = 0;
            foreach (var item in HandObjects.Values)
            {
                if (item.GetPress()) count++;
            }
            if (count == 2)
            {
                multipressflag = true;
                multipresstime = Time.time;
            }
        }

        private void onReleased(HandsInteractionManager.HandPointClass hand)
        {
            HandObjectClass obj;
            if (HandObjects.TryGetValue(hand.id, out obj))
            {
                HandObjectState state = obj.onReleased();
                foreach (var item in HandObjects.Values) if (item.GetPress()) return;
                if (multipressflag)
                {
                    if (Time.time - multipresstime < PressIntervalTime)
                    {
                        if (Time.time - multitaptime < PressIntervalTime)
                        {
                            if (HandControllerEvent != null) HandControllerEvent(HandControllerState.MultiDoubleTap);
                        }
                        else
                        {
                            if (HandControllerEvent != null) HandControllerEvent(HandControllerState.MultiTap);
                        }
                    }
                    multipressflag = false;
                    multitaptime = Time.time;
                }
                else
                {
                    if (state == HandObjectState.Tap)
                    {
                        if (obj.GetShiftFlag() == false)
                        {
                            if (HandControllerEvent != null) HandControllerEvent(HandControllerState.SelectTap);
                        }
                        else
                        {
                            if (HandControllerEvent != null) HandControllerEvent(HandControllerState.GripTap);
                        }
                    }
                    else if (state == HandObjectState.DoubleTap)
                    {
                        if (obj.GetShiftFlag() == false)
                        {
                            if (HandControllerEvent != null) HandControllerEvent(HandControllerState.SelectDoubleTap);
                        }
                        else
                        {
                            if (HandControllerEvent != null) HandControllerEvent(HandControllerState.GripDoubleTap);
                        }
                    }
                }
            }
            if (HandControllerEndEvent != null) HandControllerEndEvent();
        }
    }
}
