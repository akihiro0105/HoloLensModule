using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR||WINDOWS_UWP
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
#else
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
#endif

namespace HoloLensModule.Environment
{
    /// <summary>
    /// WorldAnchorを制御
    /// </summary>
    public class WorldAnchorControl : MonoBehaviour
    {
        /// <summary>
        /// WorldAnchorの読み込みと保存を通知
        /// </summary>
        /// <param name="self"></param>
        /// <param name="go"></param>
        /// <param name="success"></param>
        public delegate void WorldAnchorEventHandler(WorldAnchorControl self, GameObject go, bool success = true);
        public event WorldAnchorEventHandler LoadedEvent;
        public event WorldAnchorEventHandler SavedEvent;

        /// <summary>
        /// WorldAnchorStore
        /// </summary>
        private WorldAnchorStore anchorstore = null;

        /// <summary>
        /// WorldAnchor制御用命令
        /// </summary>
        private enum AnchorState
        {
            Load,
            Save
        }

        /// <summary>
        /// WorldAnchor制御用クラス
        /// </summary>
        private class AnchorControl
        {
            public AnchorState state;
            public GameObject go;
            public AnchorControl(GameObject go, AnchorState state)
            {
                this.go = go;
                this.state = state;
            }
        }

        /// <summary>
        /// WorldAnchor制御用Queue
        /// </summary>
        private Queue<AnchorControl> AnchorControlQueue = new Queue<AnchorControl>();

        void Start()
        {
            WorldAnchorStore.GetAsync((store) => anchorstore = store);
        }

        void Update()
        {
            // WorldAnchorの制御は時間がかかるためQueueで1frameで1つ処理を行う
            if (anchorstore != null)
            {
                if (AnchorControlQueue.Count > 0)
                {
                    var anchor = AnchorControlQueue.Dequeue();
                    switch (anchor.state)
                    {
                        case AnchorState.Load:
                            loadAnchor(anchor.go);
                            break;
                        case AnchorState.Save:
                            saveAnchor(anchor.go);
                            break;
                    }
                }
            }
        }

#region Public Function
        /// <summary>
        /// 対象GameObjectのWorldAnchor読み込み
        /// </summary>
        /// <param name="go"></param>
        public void LoadWorldAnchor(GameObject go)
        {
            AnchorControlQueue.Enqueue(new AnchorControl(go, AnchorState.Load));
        }

        /// <summary>
        /// 対象GameObjectのWorldAnchor保存
        /// </summary>
        /// <param name="go"></param>
        public void SaveWorldAnchor(GameObject go)
        {
            AnchorControlQueue.Enqueue(new AnchorControl(go, AnchorState.Save));
        }

        /// <summary>
        /// 対象GameObjectのWorldAnchor削除
        /// </summary>
        /// <param name="go"></param>
        public void DeleteWorldAnchor(GameObject go)
        {
            var anchor = go.GetComponent<WorldAnchor>();
            if (anchor != null)
            {
                anchorstore.Delete(anchor.name);
                DestroyImmediate(anchor);
            }
        }
#endregion

#region Private Function
        /// <summary>
        /// Queueで処理されるWorldAnchor読み込み
        /// </summary>
        /// <param name="go"></param>
        private void loadAnchor(GameObject go)
        {
            var anchor = anchorstore.Load(go.name, go);
            if (anchor != null)
            {
                if (anchor.isLocated == true)
                {
                    if (LoadedEvent != null) LoadedEvent(this, go);
                }
                else
                {
                    anchor.OnTrackingChanged += onLoadTrackingChanged;
                }
            }
            else
            {
                if (LoadedEvent != null) LoadedEvent(this, go, false);
            }
        }

        /// <summary>
        /// WorldAnchor読み込みイベント
        /// </summary>
        /// <param name="self"></param>
        /// <param name="located"></param>
        private void onLoadTrackingChanged(WorldAnchor self, bool located)
        {
            if (located == true)
            {
                if (LoadedEvent != null) LoadedEvent(this, self.gameObject);
                self.OnTrackingChanged -= onLoadTrackingChanged;
            }
        }

        /// <summary>
        /// Queueで処理されるWorldAnchor保存
        /// </summary>
        /// <param name="go"></param>
        private void saveAnchor(GameObject go)
        {
            var anchor = go.GetComponent<WorldAnchor>();
            if (anchor == null) anchor = go.AddComponent<WorldAnchor>();
            anchor.name = go.name;
            if (anchor.isLocated == true)
            {
                var success = anchorstore.Save(anchor.name, anchor);
                if (SavedEvent != null) SavedEvent(this, go, success);
            }
            else
            {
                anchor.OnTrackingChanged += onSaveTrackingChanged;
            }
        }

        /// <summary>
        /// WorldAnchor保存イベント
        /// </summary>
        /// <param name="self"></param>
        /// <param name="located"></param>
        private void onSaveTrackingChanged(WorldAnchor self, bool located)
        {
            if (located == true)
            {
                var success = anchorstore.Save(self.name, self);
                if (SavedEvent != null) SavedEvent(this, self.gameObject, success);
                self.OnTrackingChanged -= onSaveTrackingChanged;
            }
        }
#endregion
    }
}
#endif
