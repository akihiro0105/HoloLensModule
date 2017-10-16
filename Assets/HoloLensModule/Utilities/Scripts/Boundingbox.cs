using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class Boundingbox : MonoBehaviour
    {
        public bool isAwake = false;
        public Material LineMaterial;
        public float LineWidth = 0.001f;

        private LineRenderer linerenderer;
        private BoxCollider boxcollider;
        private Vector3[] linepoint = new Vector3[16];

        // Use this for initialization
        void Start()
        {
            boxcollider = GetComponent<BoxCollider>();
            if (boxcollider == null) boxcollider = gameObject.AddComponent<BoxCollider>();
             linerenderer = GetComponent<LineRenderer>();
            if (linerenderer == null) linerenderer = gameObject.AddComponent<LineRenderer>();
            linerenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            linerenderer.receiveShadows = false;
            linerenderer.startWidth = linerenderer.endWidth = LineWidth;
            linerenderer.material = LineMaterial;
            linerenderer.useWorldSpace = false;
            linerenderer.positionCount = 0;
            if (isAwake) isActive(true);
        }

        public void isActive(bool flag)
        {
            if (flag)
            {
                Bounds bounds;
                MeshFilterBounds(gameObject, out bounds);
                boxcollider.center = bounds.center;
                boxcollider.size = bounds.size;

                linepoint[0].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[1].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[2].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[3].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[4].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[5].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[6].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[7].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[8].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[9].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[10].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[11].Set(boxcollider.center.x + boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[12].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linepoint[13].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y - boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[14].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z + boxcollider.size.z / 2);
                linepoint[15].Set(boxcollider.center.x - boxcollider.size.x / 2, boxcollider.center.y + boxcollider.size.y / 2, boxcollider.center.z - boxcollider.size.z / 2);
                linerenderer.positionCount = linepoint.Length;
                linerenderer.SetPositions(linepoint);
            }
            else linerenderer.positionCount = 0;
        }

        private bool MeshFilterBounds(GameObject obj,out Bounds outBounds)
        {
            Bounds bound = new Bounds();
            bool initFlag = false;

            MeshFilter filter = obj.GetComponent<MeshFilter>();
            Matrix4x4 mat = Matrix4x4.TRS(obj.transform.localPosition, obj.transform.localRotation, obj.transform.localScale);
            if (filter)
            {
                Vector3 center = mat.MultiplyPoint(filter.mesh.bounds.center);
                Vector3 size = mat.MultiplyVector(filter.mesh.bounds.size);
                size.Set(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
                if (initFlag == false)
                {
                    bound.center = center;
                    bound.size = size;
                    initFlag = true;
                }
                else bound.Encapsulate(new Bounds(center, size));
            }
            Bounds buf;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                if (MeshFilterBounds(obj.transform.GetChild(i).gameObject,out buf))
                {
                    if (initFlag == false)
                    {
                        bound = buf;
                        initFlag = true;
                    }
                    else bound.Encapsulate(buf);
                }
            }
            outBounds = bound;
            return initFlag;
        }
    }
}
