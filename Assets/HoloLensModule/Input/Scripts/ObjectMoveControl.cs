using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

namespace HoloLensModule.Input
{
    // オブジェクトの移動
    public class ObjectMoveControl : MonoBehaviour
    {
        public bool LookAt = false;
        [Range(0.1f, 10.0f)]
        public float MoveScale = 1.0f;

        private Vector3 deltapos;
        private DragGestureEvent dragevent;
        void Start()
        {
            dragevent = gameObject.AddComponent<DragGestureEvent>();
#if !UNITY_2017_2_OR_NEWER
            dragevent.ActionState = HandsGestureManager.HandGestureState.Drag;
#else
            if (HolographicSettings.IsDisplayOpaque) dragevent.HandState = HandControllerManager.HandControllerState.SelectDrag;
            else dragevent.ActionState = HandsGestureManager.HandGestureState.Drag;
#endif
            dragevent.DragStartActionEvent.AddListener(GestureStart);
            dragevent.DragUpdateActionEvent.AddListener(GestureUpdate);
        }

        void OnDestroy()
        {
            dragevent.DragStartActionEvent.RemoveListener(GestureStart);
            dragevent.DragUpdateActionEvent.RemoveListener(GestureUpdate);
        }

        public void GestureStart(Vector3 pos1, Vector3? pos2) { deltapos = pos1; }
        public void GestureUpdate(Vector3 pos1, Vector3? pos2)
        {
            transform.position += MoveScale * (pos1 - deltapos);
            deltapos = pos1;
            if (LookAt) transform.LookAt(Camera.main.transform.position);
        }
    }
}
