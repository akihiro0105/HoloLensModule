using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
#else
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
#endif
#endif

namespace HoloLensModule
{
    // オブジェクトのWorldAnchorコントロール
    public class WorldAnchorControl : MonoBehaviour
    {
        public bool SetOnAwake = false;
        public bool DestroyAnchorClear = false;// 再起動時に位置を復帰させる場合false
#if UNITY_EDITOR || UNITY_UWP
        private WorldAnchorStore anchorstore = null;
#endif

        void Start()
        {
#if UNITY_EDITOR || UNITY_UWP
            WorldAnchorStore.GetAsync(onCompleted);
#endif
        }

        void OnDestroy()
        {
            if (DestroyAnchorClear) DeleteWorldAnchor();
        }

#if UNITY_EDITOR || UNITY_UWP
        private void onCompleted(WorldAnchorStore store)
        {
            anchorstore = store;
            string[] ids = anchorstore.GetAllIds();
            for (int i = 0; i < ids.Length; i++)
            {
                if (ids[i] == gameObject.name)
                {
                    anchorstore.Load(gameObject.name, gameObject);
                    break;
                }
            }
            if (SetOnAwake) SetWorldAnchor();
        }
#endif

        public void SetWorldAnchor()
        {
            DeleteWorldAnchor();
#if UNITY_EDITOR || UNITY_UWP
            if (anchorstore != null)
            {
                WorldAnchor worldanchor = gameObject.AddComponent<WorldAnchor>();
                worldanchor.name = gameObject.name;
                if (worldanchor.isLocated)
                {
                    anchorstore.Save(worldanchor.name, worldanchor);
                }
                else worldanchor.OnTrackingChanged += OnTrackingChanged;
            }
#endif
        }

#if UNITY_EDITOR || UNITY_UWP
        private void OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located)
            {
                anchorstore.Save(self.name, self);
                self.OnTrackingChanged -= OnTrackingChanged;
            }
        }
#endif

        public void DeleteWorldAnchor()
        {
#if UNITY_EDITOR || UNITY_UWP
            if (anchorstore != null)
            {
                WorldAnchor worldanchor = gameObject.GetComponent<WorldAnchor>();
                if (worldanchor != null)
                {
                    anchorstore.Delete(worldanchor.name);
                    DestroyImmediate(worldanchor);
                }
            }
#endif
        }

        // WorldAnchorの全体削除
        public void AllClearWorldAnchor()
        {
#if UNITY_EDITOR || UNITY_UWP
            if (anchorstore!=null)
            {
                anchorstore.Clear();
            }
#endif
        }
    }
}
