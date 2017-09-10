using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    // Raycastによる視線上のオブジェクトの取得
    public class RayCastControl : HoloLensModuleSingleton<RayCastControl>
    {
        public float maxDistance = 30.0f;

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
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out info, maxDistance))
            {
                if (bufobj == null)
                {
                    focus = GetFocusInterface(info.transform.gameObject);
                    if (focus != null)
                    {
                        for (int i = 0; i < focus.Length; i++) focus[i].FocusEnter();
                        bufobj = info.transform.gameObject;
                    }
                }
                else if (bufobj != info.transform.gameObject)
                {
                    focus = bufobj.GetComponents<FocusInterface>();
                    for (int i = 0; i < focus.Length; i++) focus[i].FocusEnd();
                    bufobj = null;

                    focus = GetFocusInterface(info.transform.gameObject);
                    if (focus != null)
                    {
                        for (int i = 0; i < focus.Length; i++) focus[i].FocusEnter();
                        bufobj = info.transform.gameObject;
                    }
                }
            }
            else
            {
                if (bufobj != null)
                {
                    focus = bufobj.GetComponents<FocusInterface>();
                    for (int i = 0; i < focus.Length; i++) focus[i].FocusEnd();
                    bufobj = null;
                }
            }
        }

        private FocusInterface[] GetFocusInterface(GameObject obj)
        {
            if (obj == null) return null;
            FocusInterface[] fi = obj.GetComponents<FocusInterface>();
            if (fi == null) return GetFocusInterface(obj.transform.parent.gameObject);
            return fi;
        }
    }

    public interface FocusInterface
    {
        void FocusEnter();
        void FocusEnd();
    }
}
