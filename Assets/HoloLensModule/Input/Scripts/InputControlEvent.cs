using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Input;
#endif

// No support under UNITY_2017_2
namespace HoloLensModule.Input
{
    public class InputControlEvent : HoloLensModuleSingleton<InputControlEvent>
    {
        [SerializeField]
        private GameObject RightHand;
        [SerializeField]
        private GameObject LeftHand;
        [SerializeField]
        private GameObject HoloLensMultiHandImage;

        public delegate void InteractionHandActionEventHandler(bool? isRight = null);
        public InteractionHandActionEventHandler ClickEvent;
        public InteractionHandActionEventHandler DetectEvent;
        public InteractionHandActionEventHandler LostEvent;
        public InteractionHandActionEventHandler UpdateEvent;
        public InteractionHandActionEventHandler PressEvent;
        public InteractionHandActionEventHandler ReleaseEvent;

        public bool RightPressFlag { get; private set; }
        public GameObject RightHandObject { get { return RightHand.GetComponent<HandControler>().GetHandPoint(); } }
        public bool RightHandActive { get { return RightHand.activeSelf; } }
        public bool LeftPressFlag { get; private set; }
        public GameObject LeftHandObject { get { return LeftHand.GetComponent<HandControler>().GetHandPoint(); } }
        public bool LeftHandActive { get { return LeftHand.activeSelf; } }
        public bool MultiHandFlag
        {
            get { return (HoloLensMultiHandImage != null) ? HoloLensMultiHandImage.activeSelf : false; }
            private set { if (HoloLensMultiHandImage != null) HoloLensMultiHandImage.SetActive(value); }
        }
        public void SetHandView(bool flag)
        {
            RightHand.GetComponent<HandControler>().isView(flag);
            LeftHand.GetComponent<HandControler>().isView(flag);
        }

        #region Private function
        private IDragInterface rightHandDragInterface = null;
        private IDragInterface leftHandDragInterface = null;

        // Use this for initialization
        void Start()
        {
            if (HolographicSettings.IsDisplayOpaque == true)
            {
                RightHand.GetComponent<HandControler>().SetHandType(HandControler.HandType.MOTIONCONTROLER);
                LeftHand.GetComponent<HandControler>().SetHandType(HandControler.HandType.MOTIONCONTROLER);
                RaycastControl.Instance.SetRaycastSourceObject(RightHandObject);
                RaycastControl.Instance.isActiveLine = true;
                RaycastControl.Instance.deltaUp = -4.0f / 3.0f;
            }
            else
            {
                RightHand.GetComponent<HandControler>().SetHandType(HandControler.HandType.HAND);
                LeftHand.GetComponent<HandControler>().SetHandType(HandControler.HandType.HAND);
                RaycastControl.Instance.SetRaycastSourceObject(Camera.main.transform.gameObject);
            }
            RightHand.SetActive(false);
            LeftHand.SetActive(false);
            RightPressFlag = false;
            LeftPressFlag = false;
            MultiHandFlag = false;

            InteractionManager.InteractionSourceDetected += InteractionSourceDetected;
            InteractionManager.InteractionSourceUpdated += InteractionSourceUpdated;
            InteractionManager.InteractionSourceLost += InteractionSourceLost;
            InteractionManager.InteractionSourcePressed += InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionSourceReleased;
        }

        protected override void OnDestroy()
        {
            InteractionManager.InteractionSourceDetected -= InteractionSourceDetected;
            InteractionManager.InteractionSourceUpdated -= InteractionSourceUpdated;
            InteractionManager.InteractionSourceLost -= InteractionSourceLost;
            InteractionManager.InteractionSourcePressed -= InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased -= InteractionSourceReleased;
            base.OnDestroy();
        }

        private void SetHandPosition(InteractionSourceState state)
        {
            Vector3 pos;
            if (state.sourcePose.TryGetPosition(out pos))
            {
                if (state.source.handedness == InteractionSourceHandedness.Unknown)
                {
                    if (state.source.id.ToString() == RightHand.name)
                    {
                        RightHand.transform.position = pos;
                    }
                    else
                    {
                        LeftHand.transform.position = pos;
                    }
                }
                else
                {
                    Quaternion rot;
                    state.sourcePose.TryGetRotation(out rot);
                    if (state.source.handedness == InteractionSourceHandedness.Right)
                    {
                        RightHand.transform.SetPositionAndRotation(pos, rot);
                    }
                    else
                    {
                        LeftHand.transform.SetPositionAndRotation(pos, rot);
                    }
                }
            }
        }

        private void InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        {
            if (obj.state.source.handedness == InteractionSourceHandedness.Right)
            {
                RightHand.SetActive(true);
            }
            else if (obj.state.source.handedness == InteractionSourceHandedness.Left)
            {
                LeftHand.SetActive(true);
            }
            else
            {
                if (RightHand.activeSelf == true)
                {
                    LeftHand.SetActive(true);
                    LeftHand.name = obj.state.source.id.ToString();
                }
                else
                {
                    RightHand.SetActive(true);
                    RightHand.name = obj.state.source.id.ToString();
                }
            }
            SetHandPosition(obj.state);
            if (DetectEvent != null) DetectEvent();
        }

        private void InteractionSourceUpdated(InteractionSourceUpdatedEventArgs obj)
        {
            SetHandPosition(obj.state);
            if (obj.state.source.handedness == InteractionSourceHandedness.Unknown)
            {
                MultiHandFlag = (RightHand.activeSelf == true && LeftHand.activeSelf == true) ? true : false;
                if (UpdateEvent != null) UpdateEvent();
                if (rightHandDragInterface != null) rightHandDragInterface.UpdateDrag(RightHand.transform.position);
            }
            else
            {
                if (UpdateEvent != null) UpdateEvent((obj.state.source.handedness == InteractionSourceHandedness.Right) ? true : false);
                if (rightHandDragInterface != null) rightHandDragInterface.UpdateDrag(RightHand.transform.position, RightHand.transform.rotation);
                if (leftHandDragInterface != null) leftHandDragInterface.UpdateDrag(LeftHand.transform.position, LeftHand.transform.rotation);
            }
        }

        private void InteractionSourceLost(InteractionSourceLostEventArgs obj)
        {
            if (obj.state.source.handedness == InteractionSourceHandedness.Right)
            {
                RightHand.SetActive(false);
                RightPressFlag = false;
            }
            else if (obj.state.source.handedness == InteractionSourceHandedness.Left)
            {
                LeftHand.SetActive(false);
                LeftPressFlag = false;
            }
            else
            {
                if (obj.state.source.id.ToString() == RightHand.name)
                {
                    RightHand.SetActive(false);
                    RightPressFlag = false;
                }
                else
                {
                    LeftHand.SetActive(false);
                    LeftPressFlag = false;
                }
                MultiHandFlag = false;
            }
            if (LostEvent != null) LostEvent();
            if (rightHandDragInterface != null) rightHandDragInterface = null;
            if (leftHandDragInterface != null) leftHandDragInterface = null;
        }

        private void InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
        {
            if (obj.state.source.handedness == InteractionSourceHandedness.Unknown)
            {
                if (obj.state.source.id.ToString() == RightHand.name)
                {
                    RightPressFlag = true;
                }
                else
                {
                    LeftPressFlag = true;
                }
                if (PressEvent != null) PressEvent();

                rightHandDragInterface = RaycastControl.Instance.SearchInterface<IDragInterface>(RaycastControl.Instance.GetRaycastHitObject());
                if (rightHandDragInterface != null) rightHandDragInterface.StartDrag(RightHand.transform.position);
            }
            else
            {
                if (obj.state.source.handedness == InteractionSourceHandedness.Right)
                {
                    RaycastControl.Instance.SetRaycastSourceObject(RightHandObject);
                }
                else
                {
                    RaycastControl.Instance.SetRaycastSourceObject(LeftHandObject);
                }
                if (obj.pressType == InteractionSourcePressType.Select)
                {
                    if (obj.state.source.handedness == InteractionSourceHandedness.Right)
                    {
                        RightPressFlag = true;
                        if (PressEvent != null) PressEvent(true);

                        rightHandDragInterface = RaycastControl.Instance.SearchInterface<IDragInterface>(RaycastControl.Instance.GetRaycastHitObject());
                        if (rightHandDragInterface != null) rightHandDragInterface.StartDrag(RightHand.transform.position, RightHand.transform.rotation);
                    }
                    else
                    {
                        LeftPressFlag = true;
                        if (PressEvent != null) PressEvent(false);

                        leftHandDragInterface = RaycastControl.Instance.SearchInterface<IDragInterface>(RaycastControl.Instance.GetRaycastHitObject());
                        if (leftHandDragInterface != null) leftHandDragInterface.StartDrag(LeftHand.transform.position, LeftHand.transform.rotation);
                    }
                }
            }
        }

        private void InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
        {
            if (obj.state.source.handedness == InteractionSourceHandedness.Unknown)
            {
                if (MultiHandFlag == false)
                {
                    if (ClickEvent != null) ClickEvent();
                    var iinterface = RaycastControl.Instance.SearchInterface<IClickInterface>(RaycastControl.Instance.GetRaycastHitObject());
                    if (iinterface != null) iinterface.RaycastClick();
                }
                if (obj.state.source.id.ToString() == RightHand.name)
                {
                    RightPressFlag = false;
                }
                else
                {
                    LeftPressFlag = false;
                }
                if (ReleaseEvent != null) ReleaseEvent();
            }
            else
            {
                if (obj.pressType == InteractionSourcePressType.Select)
                {
                    if (ClickEvent != null) ClickEvent();
                    var iinterface = RaycastControl.Instance.SearchInterface<IClickInterface>(RaycastControl.Instance.GetRaycastHitObject());
                    if (iinterface != null) iinterface.RaycastClick();
                    if (obj.state.source.handedness == InteractionSourceHandedness.Right)
                    {
                        if (ReleaseEvent != null) ReleaseEvent(true);
                        RightPressFlag = false;
                    }
                    else
                    {
                        if (ReleaseEvent != null) ReleaseEvent(false);
                        LeftPressFlag = false;
                    }
                }
            }
            if (rightHandDragInterface != null) rightHandDragInterface = null;
            if (leftHandDragInterface != null) leftHandDragInterface = null;
        }
        #endregion
    }

    public interface IClickInterface
    {
        void RaycastClick();
    }

    public interface IDragInterface
    {
        void StartDrag(Vector3 pos, Quaternion? rot = null);
        void UpdateDrag(Vector3 pos, Quaternion? rot = null);
    }
}
