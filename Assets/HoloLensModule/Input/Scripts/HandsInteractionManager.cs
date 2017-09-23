using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
using UnityEngine.VR.WSA.Input;
#else
using UnityEngine.XR.WSA.Input;
#endif
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
#if !UNITY_2017_2_OR_NEWER
            InteractionManager.SourceDetected += SourceDetected;
            InteractionManager.SourceUpdated += SourceUpdated;
            InteractionManager.SourceLost += SourceLost;
            InteractionManager.SourcePressed += SourcePressed;
            InteractionManager.SourceReleased += SourceReleased;
#else
            InteractionManager.InteractionSourceDetected += SourceDetected;
            InteractionManager.InteractionSourceUpdated += SourceUpdated;
            InteractionManager.InteractionSourceLost += SourceLost;
            InteractionManager.InteractionSourcePressed += SourcePressed;
            InteractionManager.InteractionSourceReleased += SourceReleased;
#endif
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR || UNITY_UWP
#if !UNITY_2017_2_OR_NEWER
            InteractionManager.SourceDetected -= SourceDetected;
            InteractionManager.SourceUpdated -= SourceUpdated;
            InteractionManager.SourceLost -= SourceLost;
            InteractionManager.SourcePressed -= SourcePressed;
            InteractionManager.SourceReleased -= SourceReleased;
#else
            InteractionManager.InteractionSourceDetected -= SourceDetected;
            InteractionManager.InteractionSourceUpdated -= SourceUpdated;
            InteractionManager.InteractionSourceLost -= SourceLost;
            InteractionManager.InteractionSourcePressed -= SourcePressed;
            InteractionManager.InteractionSourceReleased -= SourceReleased;
#endif
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
#if !UNITY_2017_2_OR_NEWER
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
#else
        void SourceDetected(InteractionSourceDetectedEventArgs state)
        {
            Vector3 v;
            if (state.state.sourcePose.TryGetPosition(out v) == true)
            {
                HandPointList.Add(new HandPointClass(state.state.source.id, v));
                if (onDetected != null) onDetected(HandPointList[HandPointList.Count - 1]);
            }
        }

        void SourceUpdated(InteractionSourceUpdatedEventArgs state)
        {
            Vector3 v;
            if (state.state.sourcePose.TryGetPosition(out v) == true)
            {
                for (int i = 0; i < HandPointList.Count; i++)
                {
                    if (HandPointList[i].id == state.state.source.id)
                    {
                        HandPointList[i].pos = v;
                        if (onUpdated != null) onUpdated(HandPointList[i]);
                        break;
                    }
                }
            }
        }

        void SourceLost(InteractionSourceLostEventArgs state)
        {
            for (int i = 0; i < HandPointList.Count; i++)
            {
                if (HandPointList[i].id == state.state.source.id)
                {
                    if (onLost != null) onLost(HandPointList[i]);
                    HandPointList.RemoveAt(i);
                    break;
                }
            }
        }

        void SourcePressed(InteractionSourcePressedEventArgs state)
        {
            Vector3 v;
            if (state.state.sourcePose.TryGetPosition(out v) == true)
            {
                for (int i = 0; i < HandPointList.Count; i++)
                {
                    if (HandPointList[i].id == state.state.source.id)
                    {
                        HandPointList[i].press = true;
                        HandPointList[i].pos = v;
                        if (onPressed != null) onPressed(HandPointList[i]);
                        break;
                    }
                }
            }
        }

        void SourceReleased(InteractionSourceReleasedEventArgs state)
        {
            Vector3 v;
            if (state.state.sourcePose.TryGetPosition(out v) == true)
            {
                for (int i = 0; i < HandPointList.Count; i++)
                {
                    if (HandPointList[i].id == state.state.source.id)
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
#endif
    }
}
