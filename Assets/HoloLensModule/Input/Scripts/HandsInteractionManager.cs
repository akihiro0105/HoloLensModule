using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
using UnityEngine.VR.WSA.Input;
#endif

namespace HoloLensModule.Input
{
    // 手の位置の取得，Pressの取得
    public class HandsInteractionManager : HoloLensModuleSingleton<HandsInteractionManager>
    {
        public delegate void HandsEventHandler(HandPointClass hands);
        public static event HandsEventHandler onDetected;
        public static event HandsEventHandler onUpdated;
        public static event HandsEventHandler onLost;
        public static event HandsEventHandler onPressed;
        public static event HandsEventHandler onReleased;

        public class HandPointClass
        {
            public uint id;
            public bool press = false;
            public Vector3 pos;
            public HandPointClass(uint id,Vector3 pos)
            {
                this.id = id;
                this.pos = pos;
            }
        }
        private List<HandPointClass> HandPointList = new List<HandPointClass>();

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR || UNITY_UWP
            InteractionManager.SourceDetected += SourceDetected;
            InteractionManager.SourceUpdated += SourceUpdated;
            InteractionManager.SourceLost += SourceLost;
            InteractionManager.SourcePressed += SourcePressed;
            InteractionManager.SourceReleased += SourceReleased;
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR || UNITY_UWP
            InteractionManager.SourceDetected -= SourceDetected;
            InteractionManager.SourceUpdated -= SourceUpdated;
            InteractionManager.SourceLost -= SourceLost;
            InteractionManager.SourcePressed -= SourcePressed;
            InteractionManager.SourceReleased -= SourceReleased;
#endif
            base.OnDestroy();
        }

        public int GetPressedCount()
        {
            int count = 0;
            for (int i = 0; i < HandPointList.Count; i++)
            {
                if (HandPointList[i].press == true) count++;
            }
            return count;
        }

#if UNITY_EDITOR || UNITY_UWP
        void SourceDetected(InteractionSourceState state)
        {
            Vector3 v;
            if (state.properties.location.TryGetPosition(out v) == true)
            {
                HandPointList.Add(new HandPointClass(state.source.id, v));
                if (onDetected != null) onDetected(HandPointList[HandPointList.Count - 1]);
            }
        }

        void SourceUpdated(InteractionSourceState state)
        {
            Vector3 v;
            if (state.properties.location.TryGetPosition(out v) == true)
            {
                for (int i = 0; i < HandPointList.Count; i++)
                {
                    if (HandPointList[i].id == state.source.id)
                    {
                        HandPointList[i].pos = v;
                        if (onUpdated != null) onUpdated(HandPointList[i]);
                        break;
                    }
                }
            }
        }

        void SourceLost(InteractionSourceState state)
        {
            for (int i = 0; i < HandPointList.Count; i++)
            {
                if (HandPointList[i].id == state.source.id)
                {
                    if (onLost != null) onLost(HandPointList[i]);
                    HandPointList.RemoveAt(i);
                    break;
                }
            }
        }

        void SourcePressed(InteractionSourceState state)
        {
            Vector3 v;
            if (state.properties.location.TryGetPosition(out v) == true)
            {
                for (int i = 0; i < HandPointList.Count; i++)
                {
                    if (HandPointList[i].id == state.source.id)
                    {
                        HandPointList[i].press = true;
                        HandPointList[i].pos = v;
                        if (onPressed != null) onPressed(HandPointList[i]);
                        break;
                    }
                }
            }
        }

        void SourceReleased(InteractionSourceState state)
        {
            Vector3 v;
            if (state.properties.location.TryGetPosition(out v) == true)
            {
                for (int i = 0; i < HandPointList.Count; i++)
                {
                    if (HandPointList[i].id == state.source.id)
                    {
                        HandPointList[i].press = false;
                        HandPointList[i].pos = v;
                        if (onReleased != null) onReleased(HandPointList[i]);
                        break;
                    }
                }
            }
        }
#endif
    }
}
