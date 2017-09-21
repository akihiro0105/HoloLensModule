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
        private RaycastHit info;
        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void FixedUpdate()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out info, maxDistance))
            {
                GameObject buf = GetFocusInterface(info.transform.gameObject);
                if (buf != null)
                {
                    if (bufobj == null)
                    {
                        focus = buf.GetComponents<FocusInterface>();
                        for (int i = 0; i < focus.Length; i++) focus[i].FocusEnter();
                        bufobj = buf;
                    }
                    else if (buf != bufobj)
                    {
                        focus = bufobj.GetComponents<FocusInterface>();
                        for (int i = 0; i < focus.Length; i++) focus[i].FocusEnd();
                        focus = buf.GetComponents<FocusInterface>();
                        for (int i = 0; i < focus.Length; i++) focus[i].FocusEnter();
                        bufobj = buf;
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
        }

        private GameObject GetFocusInterface(GameObject obj)
        {
            if (obj == null) return null;
            if (obj.GetComponent<FocusInterface>() != null) return obj;
            else return GetFocusInterface(obj.transform.parent.gameObject);
        }
    }

    public interface FocusInterface
    {
        void FocusEnter();
        void FocusEnd();
    }
}
