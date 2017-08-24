using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // Raycastによる視線上のオブジェクトの取得
    public class RayCastControl : HoloLensModuleSingleton<RayCastControl>
    {
        private GameObject bufobj = null;
        private FocusInterface[] focus;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            RaycastHit info;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out info, 30.0f))
            {
                if (bufobj == null)
                {
                    bufobj = info.transform.gameObject;
                    focus = GetFocusInterface(bufobj);
                    if (focus != null) for (int i = 0; i < focus.Length; i++) focus[i].FocusEnter();
                }
                else
                {
                    if (bufobj != info.transform.gameObject)
                    {
                        focus = GetFocusInterface(bufobj);
                        if (focus != null) for (int i = 0; i < focus.Length; i++) focus[i].FocusEnd();
                        bufobj = info.transform.gameObject;
                        focus = GetFocusInterface(bufobj);
                        if (focus != null) for (int i = 0; i < focus.Length; i++) focus[i].FocusEnter();
                    }
                }
            }
            else
            {
                if (bufobj != null)
                {
                    focus = GetFocusInterface(bufobj);
                    if (focus != null) for (int i = 0; i < focus.Length; i++) focus[i].FocusEnd();
                    bufobj = null;
                }
            }
        }

        private FocusInterface[] GetFocusInterface(GameObject obj)
        {
            FocusInterface[] fi = bufobj.GetComponents<FocusInterface>();
            if (fi == null)
            {
                GameObject next = obj.transform.parent.gameObject;
                if (next == null) return null;
                else return GetFocusInterface(next);
            }
            else return fi;
        }

        public GameObject GetRayCastHitObject() { return bufobj; }
    }

    public interface FocusInterface
    {
        void FocusEnter();
        void FocusEnd();
    }
}
