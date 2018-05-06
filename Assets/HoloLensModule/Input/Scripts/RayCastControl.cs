using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class RaycastControl : HoloLensModuleSingleton<RaycastControl>
    {
        [SerializeField]
        private GameObject RaycastSourceObject = null;
        public float MoveSpeed = 2.0f;
        [SerializeField]
        private GameObject RaycastHitObject = null;
        public bool isActiveLine = false;
        public Material LineRendererMaterial;
        public float deltaUp = 0.0f;

        public delegate void RaycastHitEventHandler(GameObject go);
        public RaycastHitEventHandler RaycastHitEvent;

        public void SetRaycastSourceObject(GameObject source)
        {
            RaycastSourceObject = source;
            currentFront = RaycastSourceObject.transform.forward + RaycastSourceObject.transform.up * deltaUp;
        }
        public GameObject GetRaycastHitObject()
        {
            return currentHitObject;
        }
        public Vector3? GetRaycastHitPoint()
        {
            return hitpoint;
        }

        #region Private Function
        private Vector3 currentFront;
        private GameObject currentHitObject = null;
        private Vector3? hitpoint = null;
        private LineRenderer lineRenderer;

        // Use this for initialization
        void Start()
        {
            if (RaycastSourceObject != null) SetRaycastSourceObject(RaycastSourceObject);
            InitLinerenderer();
        }

        // Update is called once per frame
        void Update()
        {
            if (RaycastSourceObject != null)
            {
                var forward = RaycastSourceObject.transform.forward + RaycastSourceObject.transform.up * deltaUp;
                currentFront = Vector3.Lerp(currentFront, forward, Time.deltaTime * MoveSpeed);
                RaycastHit hitinfo;
                if (Physics.Raycast(RaycastSourceObject.transform.position, currentFront, out hitinfo, 30.0f))
                {
                    if (currentHitObject == null || currentHitObject != hitinfo.transform.gameObject)
                    {
                        Debug.Log(hitinfo.transform.gameObject.name);
                        if (RaycastHitEvent != null) RaycastHitEvent(hitinfo.transform.gameObject);
                        var iout = SearchInterface<IFocusInterface>(currentHitObject);
                        if (iout != null) iout.RaycastOut();
                        var ihit = SearchInterface<IFocusInterface>(hitinfo.transform.gameObject);
                        if (ihit != null) ihit.RaycastHit();
                    }
                    currentHitObject = hitinfo.transform.gameObject;
                    hitpoint = hitinfo.point - currentFront * 0.01f;
                }
                else
                {
                    if (RaycastHitEvent != null) RaycastHitEvent(null);
                    var iout = SearchInterface<IFocusInterface>(currentHitObject);
                    if (iout != null) iout.RaycastOut();
                    currentHitObject = null;
                    hitpoint = null;
                }
                UpdateRaycastHitObject();
                UpdateLinerenderer();
            }
        }

        private void UpdateRaycastHitObject()
        {
            if (RaycastHitObject != null)
            {
                if (hitpoint != null)
                {
                    RaycastHitObject.SetActive(true);
                    RaycastHitObject.transform.position = hitpoint.Value;
                    RaycastHitObject.transform.LookAt(RaycastSourceObject.transform.position);
                }
                else
                {
                    RaycastHitObject.SetActive(false);
                }
            }
        }

        private void InitLinerenderer()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = LineRendererMaterial;
            lineRenderer.useWorldSpace = true;
            lineRenderer.widthMultiplier = 0.0005f;
            lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
        }

        private void UpdateLinerenderer()
        {
            if (isActiveLine == true)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, RaycastSourceObject.transform.position);
                lineRenderer.SetPosition(1, (hitpoint != null) ? hitpoint.Value : RaycastSourceObject.transform.position + currentFront);
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }
        #endregion

        public T SearchInterface<T>(GameObject go)
        {
            if (go == null) return default(T);
            var buf = go.GetComponent<T>();
            if (buf == null)
            {
                return (go.transform.parent == null) ? default(T) : SearchInterface<T>(go.transform.parent.gameObject);
            }
            else
            {
                return buf;
            }
        }
    }

    public interface IFocusInterface
    {
        void RaycastHit();
        void RaycastOut();
    }
}
