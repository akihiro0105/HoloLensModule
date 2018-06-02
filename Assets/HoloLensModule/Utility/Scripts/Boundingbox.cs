using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Utility
{
    public class Boundingbox : MonoBehaviour
    {
        public bool isActive = true;
        public bool isView = true;
        public Material LineMaterial;
        public float LineWidth = 0.001f;
        [Space(14)]
        public bool useCollider = true;
        public bool useSkinnedMeshRenderer = true;
        public bool useMeshRenderer = true;
        public bool useMeshFilter = false;

        private LineRenderer lineRenderer;
        private BoxCollider boxCollider;
        private Vector3[] linePoint = new Vector3[16];
        private bool initflag = false;

        // Use this for initialization
        void Start()
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.zero;
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = false;
            UpdateBounds();
        }

        // Update is called once per frame
        void Update()
        {
            if (isActive == true) UpdateBounds();
            lineRenderer.positionCount = (isView == true) ? linePoint.Length : 0;
            lineRenderer.widthMultiplier = LineWidth;
            lineRenderer.material = LineMaterial;
        }

        private void UpdateBounds()
        {
            initflag = false;
            Bounds bounds = new Bounds();
            var _mat = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale).inverse;
            if (useCollider == true)
            {
                var colliders = GetComponentsInChildren<Collider>();
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != boxCollider)
                    {
                        bounds = EncapsulateBounds(colliders[i].bounds, _mat, bounds);
                    }
                }
            }

            if (useSkinnedMeshRenderer == true)
            {
                var skins = GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int i = 0; i < skins.Length; i++)
                {
                    bounds = EncapsulateBounds(skins[i].bounds, _mat, bounds);
                }
            }

            if (useMeshRenderer == true)
            {
                var meshs = GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < meshs.Length; i++)
                {
                    bounds = EncapsulateBounds(meshs[i].bounds, _mat, bounds);
                }
            }

            if (useMeshFilter == true)
            {
                bounds = UpdateBoundsMeshChild(gameObject, Matrix4x4.identity, bounds);
            }

            if (initflag == true)
            {
                boxCollider.center = bounds.center;
                boxCollider.size = bounds.size;
            }
            UpdateBoundingbox();
        }

        #region // USE_MESHFILTER
        private Bounds UpdateBoundsMeshChild(GameObject go, Matrix4x4 mat, Bounds initbounds)
        {
            Bounds bufbounds = initbounds;
            var meshs = go.GetComponents<MeshFilter>();
            for (int i = 0; i < meshs.Length; i++)
            {
                bufbounds = EncapsulateBounds(meshs[i].mesh.bounds, mat, bufbounds);
            }
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var buf = go.transform.GetChild(i);
                bufbounds = UpdateBoundsMeshChild(buf.gameObject, mat * Matrix4x4.TRS(buf.localPosition, buf.localRotation, buf.localScale), bufbounds);
            }
            return bufbounds;
        }
        #endregion

        private Bounds EncapsulateBounds(Bounds bounds,Matrix4x4 mat,Bounds initbounds)
        {
            Bounds bufbounds = initbounds;
            Vector3 p1 = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z);
            Vector3 p2 = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z);
            Vector3 p3 = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            Vector3 p4 = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z);
            Vector3 p5 = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 p6 = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z);
            Vector3 p7 = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);
            Vector3 p8 = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z);

            p1 = mat.MultiplyPoint(p1);
            p2 = mat.MultiplyPoint(p2);
            p3 = mat.MultiplyPoint(p3);
            p4 = mat.MultiplyPoint(p4);
            p5 = mat.MultiplyPoint(p5);
            p6 = mat.MultiplyPoint(p6);
            p7 = mat.MultiplyPoint(p7);
            p8 = mat.MultiplyPoint(p8);
            if (initflag == false)
            {
                bufbounds.center = p1;
                bufbounds.size = Vector3.zero;
                initflag = true;
            }
            bufbounds.Encapsulate(p1);
            bufbounds.Encapsulate(p2);
            bufbounds.Encapsulate(p3);
            bufbounds.Encapsulate(p4);
            bufbounds.Encapsulate(p5);
            bufbounds.Encapsulate(p6);
            bufbounds.Encapsulate(p7);
            bufbounds.Encapsulate(p8);

            return bufbounds;
        }

        private void UpdateBoundingbox()
        {
            linePoint[0].Set(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2);
            linePoint[1].Set(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2);
            linePoint[2].Set(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2);
            linePoint[3].Set(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2);
            linePoint[4].Set(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2);
            linePoint[5].Set(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2);
            linePoint[6].Set(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2);
            linePoint[7].Set(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2);
            linePoint[8].Set(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2);
            linePoint[9].Set(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2);
            linePoint[10].Set(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2);
            linePoint[11].Set(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2);
            linePoint[12].Set(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2);
            linePoint[13].Set(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2);
            linePoint[14].Set(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2);
            linePoint[15].Set(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2);
            lineRenderer.SetPositions(linePoint);
        }
    }
}
