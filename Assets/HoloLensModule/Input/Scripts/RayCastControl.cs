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
        private RaycastHit info;
        private FocusInterface[] fs;

        void FixedUpdate()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out info, maxDistance)) FocusInterfaceObject(info.transform.gameObject);
            else FocusInterfaceObject(null);
        }

        private void FocusInterfaceObject(GameObject obj)
        {
            if (obj == null) SetFocusEnd();
            else
            {
                if (obj.GetComponent<FocusInterface>() == null) FocusInterfaceObject(obj.transform.parent.gameObject);
                else
                {
                    if (bufobj != obj) SetFocusEnd();
                    fs = obj.GetComponents<FocusInterface>();
                    for (int i = 0; i < fs.Length; ++i) fs[i].FocusEnter();
                    bufobj = obj;
                }
            }
        }

        private void SetFocusEnd()
        {
            if (bufobj == null) return;
            fs = bufobj.GetComponents<FocusInterface>();
            if (fs != null) for (int i = 0; i < fs.Length; ++i) fs[i].FocusEnd();
            bufobj = null;
        }
    }

    public interface FocusInterface
    {
        void FocusEnter();
        void FocusEnd();
    }
}
