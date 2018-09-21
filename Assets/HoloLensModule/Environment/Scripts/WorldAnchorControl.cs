using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
#else
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
#endif

namespace HoloLensModule.Environment
{
    public class WorldAnchorControl : MonoBehaviour
    {
        public delegate void WorldAnchorEventHandler(WorldAnchorControl self, GameObject go, bool success = true);
        public WorldAnchorEventHandler LoadedEvent;
        public WorldAnchorEventHandler SavedEvent;

        // worldanchorの全体コントロール
        private WorldAnchorStore anchorstore = null;
        private enum AnchorState
        {
            Load,
            Save
        }
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
        private Queue<AnchorControl> AnchorControlQueue = new Queue<AnchorControl>();

        void Start()
        {
            WorldAnchorStore.GetAsync((store) =>
            {
                anchorstore = store;
            });
        }

        void Update()
        {
            if (anchorstore != null)
            {
                if (AnchorControlQueue.Count > 0)
                {
                    var anchor = AnchorControlQueue.Dequeue();
                    switch (anchor.state)
                    {
                        case AnchorState.Load:
                            LoadAnchor(anchor.go);
                            break;
                        case AnchorState.Save:
                            SaveAnchor(anchor.go);
                            break;
                    }
                }
            }
        }

#region Public Function
        public void LoadWorldAnchor(GameObject go)
        {
            AnchorControlQueue.Enqueue(new AnchorControl(go, AnchorState.Load));
        }

        public void SaveWorldAnchor(GameObject go)
        {
            AnchorControlQueue.Enqueue(new AnchorControl(go, AnchorState.Save));
        }

        public void DeleteWorldAnchor(GameObject go)
        {
            DeleteAnchor(go);
        }
#endregion

#region Private Function

        private void LoadAnchor(GameObject go)
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
                    anchor.OnTrackingChanged += OnLoadTrackingChanged;
                }
            }
            else
            {
                if (LoadedEvent != null) LoadedEvent(this, go, false);
            }
        }

        private void OnLoadTrackingChanged(WorldAnchor self, bool located)
        {
            if (located == true)
            {
                if (LoadedEvent != null) LoadedEvent(this, self.gameObject);
                self.OnTrackingChanged -= OnLoadTrackingChanged;
            }
        }

        private void SaveAnchor(GameObject go)
        {
            var anchor = go.GetComponent<WorldAnchor>();
            if (anchor == null)
            {
                anchor = go.AddComponent<WorldAnchor>();
            }
            anchor.name = go.name;
            if (anchor.isLocated == true)
            {
                var success = anchorstore.Save(anchor.name, anchor);
                if (SavedEvent != null) SavedEvent(this, go, success);
            }
            else
            {
                anchor.OnTrackingChanged += OnSaveTrackingChanged;
            }
        }

        private void OnSaveTrackingChanged(WorldAnchor self, bool located)
        {
            if (located == true)
            {
                var success = anchorstore.Save(self.name, self);
                if (SavedEvent != null) SavedEvent(this, self.gameObject, success);
                self.OnTrackingChanged -= OnSaveTrackingChanged;
            }
        }

        private void DeleteAnchor(GameObject go)
        {
            var anchor = go.GetComponent<WorldAnchor>();
            if (anchor != null)
            {
                anchorstore.Delete(anchor.name);
                DestroyImmediate(anchor);
            }
        }

#endregion
    }
}
