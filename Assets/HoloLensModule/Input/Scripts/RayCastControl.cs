using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Utility;

namespace HoloLensModule.Input
{
    /// <summary>
    /// 指定ObjectからのGaze機能
    /// </summary>
    public class RayCastControl :MonoBehaviour
    {
        /// <summary>
        /// Gazeの照射元
        /// </summary>
        [SerializeField] private Transform raycastSource = null;

        /// <summary>
        /// Gazeの移動速度
        /// </summary>
        [SerializeField]private float moveSpeed = 2.0f;

        /// <summary>
        /// Gazeサークル
        /// </summary>
        [SerializeField] private GameObject raycastHitObject = null;

        /// <summary>
        /// Gazeラインの表示，非表示
        /// </summary>
        public bool isActiveLine = false;

        /// <summary>
        /// Gazeラインのマテリアル
        /// </summary>
        [SerializeField]private Material lineRendererMaterial;

        /// <summary>
        /// Gaze上方向微調整用
        /// </summary>
        [SerializeField]private float deltaUp = 0.0f;

        /// <summary>
        /// Gazeのヒット対象オブジェクトを取得
        /// </summary>
        /// <returns></returns>
        private GameObject currentHitObject = null;

        /// <summary>
        /// Gazeのヒット位置を取得
        /// </summary>
        /// <returns></returns>
        private Vector3? hitpoint = null;

        /// <summary>
        /// 移動補正後のGaze正面方向
        /// </summary>
        private Vector3 currentFront;

        /// <summary>
        /// Gazeヒット用イベント
        /// </summary>
        /// <param name="hit"></param>
        public delegate void RaycastHitEventHandler(Transform hit);
        public RaycastHitEventHandler RaycastHitEvent;

        /// <summary>
        /// Gaze照射元設定
        /// </summary>
        /// <param name="source"></param>
        public void SetRaycastSourceObject(Transform source,float? delta=null)
        {
            if (delta != null) deltaUp = delta.Value;
            raycastSource = source;
            currentFront = raycastSource.forward + raycastSource.up * deltaUp;
        }

        // Use this for initialization
        void Start()
        {
            if (raycastSource != null) SetRaycastSourceObject(raycastSource);
            initLinerenderer();
        }

        // Update is called once per frame
        void Update()
        {
            // Gaze判定と通知
            if (raycastSource != null)
            {
                var forward = raycastSource.forward + raycastSource.up * deltaUp;
                currentFront = Vector3.Lerp(currentFront, forward, Time.deltaTime * moveSpeed);
                RaycastHit hitinfo;
                if (Physics.Raycast(raycastSource.position, currentFront, out hitinfo, 30.0f))
                {
                    if (currentHitObject == null || currentHitObject != hitinfo.transform.gameObject)
                    {
                        Debug.Log(hitinfo.transform.gameObject.name);
                        if (RaycastHitEvent != null) RaycastHitEvent(hitinfo.transform);
                        var iout = searchInterface<IFocusInterface>(currentHitObject);
                        if (iout != null) iout.RaycastOut();
                        var ihit = searchInterface<IFocusInterface>(hitinfo.transform.gameObject);
                        if (ihit != null) ihit.RaycastHit();
                    }
                    currentHitObject = hitinfo.transform.gameObject;
                    hitpoint = hitinfo.point - currentFront * 0.01f;
                }
                else
                {
                    if (RaycastHitEvent != null) RaycastHitEvent(null);
                    var iout = searchInterface<IFocusInterface>(currentHitObject);
                    if (iout != null) iout.RaycastOut();
                    currentHitObject = null;
                    hitpoint = null;
                }
            }

            // Gazeサークルの表示切替
            if (raycastHitObject != null)
            {
                if (raycastSource != null && hitpoint != null)
                {
                    raycastHitObject.SetActive(true);
                    raycastHitObject.transform.position = hitpoint.Value;
                    raycastHitObject.transform.LookAt(raycastSource.position);
                }
                else
                {
                    raycastHitObject.SetActive(false);
                }
            }

            // Raycastの導線を描画
            if (raycastSource != null && isActiveLine == true)
                updateLinerenderer(raycastSource.position, hitpoint ?? raycastSource.position + currentFront);
            else updateLinerenderer();
        }


        #region LineRenderer Function

        private LineRenderer lineRenderer;

        /// <summary>
        /// Gazeラインの初期化
        /// </summary>
        private void initLinerenderer()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = lineRendererMaterial;
            lineRenderer.useWorldSpace = true;
            lineRenderer.widthMultiplier = 0.0005f;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
        }

        /// <summary>
        /// Gazeラインの更新
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void updateLinerenderer(Vector3 source, Vector3 target)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, source);
            lineRenderer.SetPosition(1, target);
        }

        private void updateLinerenderer()
        {
            lineRenderer.positionCount = 0;
        }

        #endregion

        /// <summary>
        /// 指定Interfaceの探索
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns></returns>
        private T searchInterface<T>(GameObject go)
        {
            if (go == null) return default(T);
            var buf = go.GetComponent<T>();
            if (buf == null)
                return (go.transform.parent == null) ? default(T) : searchInterface<T>(go.transform.parent.gameObject);
            else return buf;
        }

        /// <summary>
        /// Gazeが当たっているオブジェクトから指定Interfaceを取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetRaycastHitInterface<T>()
        {
            return searchInterface<T>(currentHitObject);
        }
    }
}
