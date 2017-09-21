using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class Boundingbox : MonoBehaviour,FocusInterface
    {
        public bool isActive = true;
        public bool isFocusActive = true;

        private LineRenderer linerenderer;
        private BoxCollider boxcollider;
        private Vector3[] linepoint = new Vector3[16];
        private Bounds bounds = new Bounds();
        private Bounds childbound = new Bounds();
        private Vector3 center = new Vector3();
        private Vector3 size = new Vector3();

        public void FocusEnd()
        {
            if (isFocusActive) isActive = false;
        }

        public void FocusEnter()
        {
            if (isFocusActive) isActive = true;
        }

        // Use this for initialization
        void Start()
        {
            boxcollider = GetComponent<BoxCollider>();
            linerenderer = GetComponent<LineRenderer>();
            linerenderer.useWorldSpace = false;
        }

        // Update is called once per frame
        void Update()
        {
            GetMeshFilterBounds(gameObject,out bounds);
            boxcollider.center = bounds.center;
            boxcollider.size = bounds.size;

            if (isActive)
            {
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
            else
            {
                linerenderer.positionCount = 0;
            }
        }

        private bool GetMeshFilterBounds(GameObject obj,out Bounds outbounds)
        {
            Bounds bound = new Bounds();
            bool initflag = false;
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject objchild = obj.transform.GetChild(i).gameObject;
                MeshFilter filter = objchild.GetComponent<MeshFilter>();
                Matrix4x4 mat = Matrix4x4.TRS(objchild.transform.localPosition, objchild.transform.localRotation, objchild.transform.localScale);
                if (filter)
                {
                    center = mat.MultiplyPoint(filter.mesh.bounds.center);
                    size = mat.MultiplyVector(filter.mesh.bounds.size);
                    if (size.x < 0) size.x *= -1;
                    if (size.y < 0) size.y *= -1;
                    if (size.z < 0) size.z *= -1;
                    if (initflag == false)
                    {
                        bound.center = center;
                        bound.size = size;
                        initflag = true;
                    }
                    else
                    {
                        bound.Encapsulate(new Bounds(center, size));
                    }
                }
                if (GetMeshFilterBounds(objchild,out childbound))
                {
                    center = mat.MultiplyPoint(childbound.center);
                    size = mat.MultiplyVector(childbound.size);
                    if (size.x < 0) size.x *= -1;
                    if (size.y < 0) size.y *= -1;
                    if (size.z < 0) size.z *= -1;
                    if (initflag == false)
                    {
                        bound.center = center;
                        bound.size = size;
                        initflag = true;
                    }
                    else
                    {
                        bound.Encapsulate(new Bounds(center, size));
                    }
                }
            }
            outbounds = bound;
            return initflag;
        }
    }
}
