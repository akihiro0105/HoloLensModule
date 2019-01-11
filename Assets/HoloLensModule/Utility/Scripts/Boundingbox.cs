using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Utility
{
    /// <summary>
    /// Collider,SkinnedMeshRenderer,MeshRenderer,MeshFilterに対応したBoundingbox
    /// 算出したBoundingboxにBoxColliderを適応してLineRendererで描画します
    /// </summary>
    public class Boundingbox : MonoBehaviour
    {
        /// <summary>
        /// Boundingboxの算出有効化，無効化
        /// </summary>
        public bool isActive = true;
        /// <summary>
        /// Boundingboxを表示，非表示
        /// </summary>
        public bool isView = true;
        /// <summary>
        /// Boundingboxのラインのマテリアル
        /// </summary>
        public Material LineMaterial;
        /// <summary>
        /// Boundingboxのラインのサイズ
        /// </summary>
        public float LineWidth = 0.001f;
        /// <summary>
        /// Colliderを使ったboundsの算出
        /// </summary>
        [Space(14)]
        [SerializeField] private bool useCollider = true;
        /// <summary>
        /// SkinnedMeshRendererを使ったboundsの算出
        /// </summary>
        [SerializeField] private bool useSkinnedMeshRenderer = true;
        /// <summary>
        /// MeshRendererを使ったboundsの算出
        /// </summary>
        [SerializeField] private bool useMeshRenderer = true;
        /// <summary>
        /// MeshFilterを使ったboundsの算出
        /// </summary>
        [SerializeField] private bool useMeshFilter = false;

        private LineRenderer lineRenderer;
        private BoxCollider boxCollider;
        private Vector3[] linePoint = new Vector3[16];

        // Use this for initialization
        void Start()
        {
            // boxcolliderの追加
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.zero;
            // linerendererの追加
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = false;
            lineRenderer.widthMultiplier = LineWidth;
            lineRenderer.material = LineMaterial;
            updateBounds();
        }

        // Update is called once per frame
        void Update()
        {
            if (isActive == true) updateBounds();
            boxCollider.enabled = isView;
            lineRenderer.positionCount = (isView) ? linePoint.Length : 0;
            lineRenderer.widthMultiplier = LineWidth;
            lineRenderer.material = LineMaterial;
        }

        /// <summary>
        /// 各bounds算出元からboundingboxを生成
        /// </summary>
        private void updateBounds()
        {
            Bounds? bounds = null;
            // 基底gameobjectのmatrix情報の作成
            var _mat = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale).inverse;
            
            // colliderのbounds算出
            if (useCollider == true)
            {
                foreach (var t in GetComponentsInChildren<Collider>())
                    if (t != boxCollider)
                        bounds = encapsulateBounds(t.bounds, _mat, bounds);
            }

            // skinnedmeshrendererのbounds算出
            if (useSkinnedMeshRenderer == true)
            {
                foreach (var t in GetComponentsInChildren<SkinnedMeshRenderer>())
                    bounds = encapsulateBounds(t.bounds, _mat, bounds);
            }

            // meshrendererのbounds算出
            if (useMeshRenderer == true)
            {
                foreach (var t in GetComponentsInChildren<MeshRenderer>())
                    bounds = encapsulateBounds(t.bounds, _mat, bounds);
            }

            // MeshFilterのbounds算出
            if (useMeshFilter == true)
            {
                bounds = updateBoundsMeshChild(gameObject, Matrix4x4.identity, bounds);
            }

            // bounds情報のboxcolliderとlinerendererへの適応
            if (bounds!=null)
            {
                boxCollider.center = bounds.Value.center;
                boxCollider.size = bounds.Value.size;
            }
            lineRenderer.SetPositions(updateBounsPoints(linePoint,boxCollider));
        }

        #region // USE_MESHFILTER
        /// <summary>
        /// MeshFilterのbounds算出
        /// </summary>
        /// <param name="go"></param>
        /// <param name="mat"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private Bounds? updateBoundsMeshChild(GameObject go, Matrix4x4 mat, Bounds? source)
        {
            var bounds = source;
            foreach (var t in go.GetComponents<MeshFilter>())
                bounds = encapsulateBounds(t.mesh.bounds, mat, bounds);
            for (var i = 0; i < go.transform.childCount; i++)
            {
                var buf = go.transform.GetChild(i);
                bounds = updateBoundsMeshChild(buf.gameObject, mat * Matrix4x4.TRS(buf.localPosition, buf.localRotation, buf.localScale), bounds);
            }
            return bounds;
        }
        #endregion

        /// <summary>
        /// 指定bounds同士をmatrixで座標変換後に合成
        /// </summary>
        /// <param name="target"></param>
        /// <param name="mat"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private Bounds encapsulateBounds(Bounds target,Matrix4x4 mat,Bounds? source)
        {
            var p1 = target.center + new Vector3(target.extents.x, target.extents.y, target.extents.z);
            var p2 = target.center + new Vector3(-target.extents.x, target.extents.y, target.extents.z);
            var p3 = target.center + new Vector3(-target.extents.x, target.extents.y, -target.extents.z);
            var p4 = target.center + new Vector3(target.extents.x, target.extents.y, -target.extents.z);
            var p5 = target.center + new Vector3(target.extents.x, -target.extents.y, target.extents.z);
            var p6 = target.center + new Vector3(-target.extents.x, -target.extents.y, target.extents.z);
            var p7 = target.center + new Vector3(-target.extents.x, -target.extents.y, -target.extents.z);
            var p8 = target.center + new Vector3(target.extents.x, -target.extents.y, -target.extents.z);

            p1 = mat.MultiplyPoint(p1);
            p2 = mat.MultiplyPoint(p2);
            p3 = mat.MultiplyPoint(p3);
            p4 = mat.MultiplyPoint(p4);
            p5 = mat.MultiplyPoint(p5);
            p6 = mat.MultiplyPoint(p6);
            p7 = mat.MultiplyPoint(p7);
            p8 = mat.MultiplyPoint(p8);

            if (source == null) source = new Bounds(p1, Vector3.zero);
            var bounds = source.Value;
            bounds.Encapsulate(p1);
            bounds.Encapsulate(p2);
            bounds.Encapsulate(p3);
            bounds.Encapsulate(p4);
            bounds.Encapsulate(p5);
            bounds.Encapsulate(p6);
            bounds.Encapsulate(p7);
            bounds.Encapsulate(p8);

            return bounds;
        }

        /// <summary>
        /// 指定boxcolliderを囲うlinrenderer用の頂点を設定
        /// </summary>
        /// <param name="points"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        private Vector3[] updateBounsPoints(Vector3[] points,BoxCollider box)
        {
            points[0].Set(box.center.x + box.size.x / 2, box.center.y - box.size.y / 2, box.center.z + box.size.z / 2);
            points[1].Set(box.center.x + box.size.x / 2, box.center.y + box.size.y / 2, box.center.z + box.size.z / 2);
            points[2].Set(box.center.x - box.size.x / 2, box.center.y + box.size.y / 2, box.center.z + box.size.z / 2);
            points[3].Set(box.center.x - box.size.x / 2, box.center.y - box.size.y / 2, box.center.z + box.size.z / 2);
            points[4].Set(box.center.x + box.size.x / 2, box.center.y - box.size.y / 2, box.center.z + box.size.z / 2);
            points[5].Set(box.center.x + box.size.x / 2, box.center.y - box.size.y / 2, box.center.z - box.size.z / 2);
            points[6].Set(box.center.x + box.size.x / 2, box.center.y + box.size.y / 2, box.center.z - box.size.z / 2);
            points[7].Set(box.center.x + box.size.x / 2, box.center.y + box.size.y / 2, box.center.z + box.size.z / 2);
            points[8].Set(box.center.x + box.size.x / 2, box.center.y + box.size.y / 2, box.center.z - box.size.z / 2);
            points[9].Set(box.center.x - box.size.x / 2, box.center.y + box.size.y / 2, box.center.z - box.size.z / 2);
            points[10].Set(box.center.x - box.size.x / 2, box.center.y - box.size.y / 2, box.center.z - box.size.z / 2);
            points[11].Set(box.center.x + box.size.x / 2, box.center.y - box.size.y / 2, box.center.z - box.size.z / 2);
            points[12].Set(box.center.x - box.size.x / 2, box.center.y - box.size.y / 2, box.center.z - box.size.z / 2);
            points[13].Set(box.center.x - box.size.x / 2, box.center.y - box.size.y / 2, box.center.z + box.size.z / 2);
            points[14].Set(box.center.x - box.size.x / 2, box.center.y + box.size.y / 2, box.center.z + box.size.z / 2);
            points[15].Set(box.center.x - box.size.x / 2, box.center.y + box.size.y / 2, box.center.z - box.size.z / 2);
            return points;
        }
    }
}
