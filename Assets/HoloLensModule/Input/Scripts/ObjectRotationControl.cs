using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // オブジェクトの回転
    public class ObjectRotationControl : MonoBehaviour
    {
        [Range(1.0f, 100.0f)]
        public float RotationScale = 20.0f;

        private Vector3 deltapos;
        private DragGestureEvent dragevent;

        void Start()
        {
            dragevent = gameObject.AddComponent<DragGestureEvent>();
            dragevent.ActionState = HandsGestureManager.HandGestureState.ShiftDrag;
            dragevent.DragStartActionEvent.AddListener(GestureStart);
            dragevent.DragUpdateActionEvent.AddListener(GestureUpdate);
        }

        void OnDestroy()
        {
            dragevent.DragStartActionEvent.RemoveListener(GestureStart);
            dragevent.DragUpdateActionEvent.RemoveListener(GestureUpdate);
        }

        public void GestureStart(Vector3 pos1, Vector3? pos2) { deltapos = Camera.main.transform.InverseTransformPoint(pos1); }
        public void GestureUpdate(Vector3 pos1, Vector3? pos2)
        {
            Vector3 deltamove = Camera.main.transform.InverseTransformPoint(pos1) - deltapos;
            transform.rotation = transform.rotation * Quaternion.AngleAxis(-deltamove.x * RotationScale * 10.0f, transform.up);
            deltapos = Camera.main.transform.InverseTransformPoint(pos1);
        }
    }
}
