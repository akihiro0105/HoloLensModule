using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // ジェスチャーの取得
    public class HandsGestureManager : HoloLensModuleSingleton<HandsGestureManager>
    {
        public GameObject HandObjectPrefab;
        [Range(-0.1f, 0.1f)]
        public float Offset_up = 0.05f;
        [Range(-0.1f, 0.1f)]
        public float Offset_foward = -0.05f;

        public float HoldTriggerTime = 1.0f;// Hold
        public float PressIntervalTime = 0.5f;// Tap判定時間間隔
        public float DragDistance = 0.02f;// Drag判定距離

        public delegate void HandGestureEventHandler(HandGestureState state);
        public delegate void SingleHandGestureEventHandler(HandGestureState state,Vector3 pos);
        public delegate void MultiHandGestureEventHandler(HandGestureState state, Vector3 pos1,Vector3 pos2);
        public static event HandGestureEventHandler HandGestureEvent;// Tap, DoubleTap, Hold(Shift,Multi)
        public static event HandGestureEventHandler ReleaseHandGestureEvent;// Release
        public static event SingleHandGestureEventHandler SingleHandGestureEvent;// Drag(DragStart)
        public static event MultiHandGestureEventHandler MultiHandGestureEvent;// MultiDrag(MultiDragStart)

        public enum HandGestureState
        {
            None,
            Release,
            // single hand gesture
            Tap,
            DoubleTap,
            Hold,
            DragStart,
            Drag,
            // multi hand single gesture
            ShiftTap,// L R
            ShiftDoubleTap,// L R
            ShiftHold,// L R
            ShiftDragStart,// L R
            ShiftDrag,// L R
            // multi hand multi gesture
            MultiTap,
            MultiDoubleTap,
            MultiDragStart,
            MultiDrag
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

            public HandGestureState onUpdated(Vector3 pos,float up,float foward)
            {
                obj.transform.position = pos + obj.transform.up * up + obj.transform.forward * foward;
                obj.transform.LookAt(Camera.main.transform.position);

                HandGestureState state = HandGestureState.None;
                if (dragflag)
                {
                    state = HandGestureState.Drag;
                }
                else if (pressflag)
                {
                    if (Vector3.Distance(presspos, obj.transform.position) > DragDistance)
                    {
                        state = HandGestureState.DragStart;
                        dragflag = true;
                        if (objectcontrol != null) objectcontrol.onReleased();
                    }
                    else if (Time.time - presstime > HoldTriggerTime)
                    {
                        state = HandGestureState.Hold;
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
                if(objectcontrol!=null) objectcontrol.onPressed(shiftflag, HoldTriggerTime);
            }

            public HandGestureState onReleased()
            {
                HandGestureState state = HandGestureState.None;
                if (Time.time - presstime < PressIntervalTime)
                {
                    state = HandGestureState.Tap;
                    if (Time.time - taptime < PressIntervalTime) state = HandGestureState.DoubleTap;
                }
                taptime = Time.time;
                pressflag = false;
                dragflag = false;
                if (objectcontrol != null) objectcontrol.onReleased();
                return state;
            }
            public bool GetPress() { return pressflag; }
            public Vector3 GetPosition() { return obj.transform.position; }
            public void DestroyObject() { Destroy(obj.gameObject); }
            public void SetHandObjectControlRelease() { if (objectcontrol != null) objectcontrol.onReleased(); }
        }
        private Dictionary<uint, HandObjectClass> HandObjects = new Dictionary<uint, HandObjectClass>();
        private bool multihandflag = false;
        private bool multipressflag = false;
        private float multipresstime;
        private float multitaptime;

        // Use this for initialization
        void Start()
        {
            HandsInteractionManager.onDetected += onDetected;
            HandsInteractionManager.onUpdated += onUpdated;
            HandsInteractionManager.onLost += onLost;
            HandsInteractionManager.onPressed += onPressed;
            HandsInteractionManager.onReleased += onReleased;
        }

        protected override void OnDestroy()
        {
            HandsInteractionManager.onDetected -= onDetected;
            HandsInteractionManager.onUpdated -= onUpdated;
            HandsInteractionManager.onLost -= onLost;
            HandsInteractionManager.onPressed -= onPressed;
            HandsInteractionManager.onReleased -= onReleased;
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
                HandGestureState state = obj.onUpdated(hand.pos, Offset_up, Offset_foward);
                if (HandObjects.Count == 1)
                {
                    if (state == HandGestureState.Hold)
                    {
                        if (HandGestureEvent != null) HandGestureEvent(state);
                    }
                    else if (state == HandGestureState.DragStart)
                    {
                        if (SingleHandGestureEvent != null) SingleHandGestureEvent(state, obj.GetPosition());
                    }
                    else if (state==HandGestureState.Drag)
                    {
                        if (SingleHandGestureEvent != null) SingleHandGestureEvent(state, obj.GetPosition());
                    }
                }
                else
                {
                    int dragcount = 0;
                    Vector3[] pos = new Vector3[2];
                    foreach (var item in HandObjects.Values)
                    {
                        if (item.GetPress())
                        {
                            pos[dragcount] = item.GetPosition();
                            dragcount++;
                        }
                    }
                    if (dragcount == 2)
                    {
                        if (Time.time- multipresstime> PressIntervalTime)
                        {
                            if (multihandflag == false)
                            {
                                if (MultiHandGestureEvent != null) MultiHandGestureEvent(HandGestureState.MultiDragStart, pos[0], pos[1]);
                                multihandflag = true;
                            }
                            else
                            {
                                if (MultiHandGestureEvent != null) MultiHandGestureEvent(HandGestureState.MultiDrag, pos[0], pos[1]);
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
                            if (state == HandGestureState.Hold)
                            {
                                if (HandGestureEvent != null) HandGestureEvent(HandGestureState.ShiftHold);
                            }
                            else if (state == HandGestureState.DragStart)
                            {
                                if (SingleHandGestureEvent != null) SingleHandGestureEvent(HandGestureState.ShiftDragStart, obj.GetPosition());
                            }
                            else if (state == HandGestureState.Drag)
                            {
                                if (SingleHandGestureEvent != null) SingleHandGestureEvent(HandGestureState.ShiftDrag, obj.GetPosition());
                            }
                        }
                    }
                }
            }
        }

        private void onLost(HandsInteractionManager.HandPointClass hand)
        {
            HandObjectClass obj;
            if(HandObjects.TryGetValue(hand.id, out obj))
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
                if (HandObjects.Count > 1) obj.onPressed(true);
                else obj.onPressed(false);
            }
            int count = 0;
            foreach (var item in HandObjects.Values)
            {
                if (item.GetPress()) count++;
            }
            if (count==2)
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
                HandGestureState state = obj.onReleased();
                if (HandObjects.Count == 1)
                {
                    if (state == HandGestureState.Tap)
                    {
                        if (HandGestureEvent != null) HandGestureEvent(state);
                    }
                    else if (state==HandGestureState.DoubleTap)
                    {
                        if (HandGestureEvent != null) HandGestureEvent(state);
                    }
                }
                else
                {
                    foreach (var item in HandObjects.Values) if (item.GetPress()) return;
                    if (multipressflag)
                    {
                        if (Time.time - multipresstime < PressIntervalTime)
                        {
                            if (Time.time - multitaptime < PressIntervalTime)
                            {
                                if (HandGestureEvent != null) HandGestureEvent(HandGestureState.MultiDoubleTap);
                            }
                            else
                            {
                                if (HandGestureEvent != null) HandGestureEvent(HandGestureState.MultiTap);
                            }
                        }
                        multipressflag = false;
                        multitaptime = Time.time;
                    }
                    else
                    {
                        if (state == HandGestureState.Tap)
                        {
                            if (HandGestureEvent != null) HandGestureEvent(HandGestureState.ShiftTap);
                        }
                        else if (state == HandGestureState.DoubleTap)
                        {
                            if (HandGestureEvent != null) HandGestureEvent(HandGestureState.ShiftDoubleTap);
                        }
                    }
                }
            }
            if (HandGestureEvent != null) ReleaseHandGestureEvent(HandGestureState.Release);
        }
    }
}
