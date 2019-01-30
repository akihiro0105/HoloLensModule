using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    /// <summary>
    /// オブジェクトドラッグ操作用クラス
    /// </summary>
    public class DragControl : MonoBehaviour,IDragGestureInterface
    {
        private Vector3 current;
        public void OnStartDrag(Vector3 pos, Quaternion? rot = null)
        {
            current = pos;
        }

        public void OnUpdateDrag(Vector3 pos, Quaternion? rot = null)
        {
            transform.position += pos - current;
            transform.LookAt(Camera.main.transform.position);
            current = pos;
        }
    }
}
