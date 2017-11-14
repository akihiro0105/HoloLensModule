using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // オブジェクトのサイズ
    public class ObjectScaleControl : MonoBehaviour
    {
        [Range(0.1f, 10.0f)]
        public float SizeScale = 1.0f;
        public float MinSize = 0.001f;

        private float deltadistace;
        private DragGestureEvent dragevent;

        void Start()
        {
            dragevent = gameObject.AddComponent<DragGestureEvent>();
            dragevent.ActionState = HandsGestureManager.HandGestureState.MultiDrag;
            dragevent.DragStartActionEvent.AddListener(GestureStart);
            dragevent.DragUpdateActionEvent.AddListener(GestureUpdate);
        }

        void OnDestroy()
        {
            dragevent.DragStartActionEvent.RemoveListener(GestureStart);
            dragevent.DragUpdateActionEvent.RemoveListener(GestureUpdate);
        }

        public void GestureStart(Vector3 pos1, Vector3? pos2) { deltadistace = Vector3.Distance(pos1, pos2.Value); }
        public void GestureUpdate(Vector3 pos1, Vector3? pos2)
        {
            float deltamove = SizeScale * (Vector3.Distance(pos1, pos2.Value) - deltadistace);
            if (deltamove > 0) deltamove = 1.0f + deltamove;
            else deltamove = 1.0f / (1.0f - deltamove);
            float min = transform.localScale.x * deltamove;
            if (transform.localScale.y * deltamove < min) min = transform.localScale.y * deltamove;
            if (transform.localScale.z * deltamove < min) min = transform.localScale.z * deltamove;
            if (min > MinSize) transform.localScale *= deltamove;
            deltadistace = Vector3.Distance(pos1, pos2.Value);
        }
    }
}
