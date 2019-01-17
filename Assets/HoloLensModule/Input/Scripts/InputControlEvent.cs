using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR||WINDOWS_UWP
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
    /// <summary>
    /// HoloLens,Windows MRデバイスの入力
    /// </summary>
    public class InputControlEvent : MonoBehaviour
    {
        /// <summary>
        /// Gaze用Raycast機能
        /// </summary>
        [SerializeField] private RayCastControl rayCastControl;

        /// <summary>
        /// 右手ハンド
        /// </summary>
        [SerializeField] private HandControler rightHand;

        /// <summary>
        /// 左手ハンド
        /// </summary>
        [SerializeField] private HandControler leftHand;

        /// <summary>
        /// HoloLens両手用イメージ
        /// </summary>
        [SerializeField] private GameObject multiHandImage;

        /// <summary>
        /// HoloLens用両手
        /// </summary>
        private bool multiHandFlag
        {
            get { return (multiHandImage != null) && multiHandImage.activeSelf; }
            set
            {
                if (multiHandImage != null) multiHandImage.SetActive(value);
            }
        }

        // Use this for initialization
        void Start()
        {
            // HoloLensとWindows MRのイベント切替
            if (HolographicSettings.IsDisplayOpaque == true) initWinMRSetting();
            else initHoloLensSetting();
            // 共通初期化処理
            rightHand.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            multiHandFlag = false;
        }

        /// <summary>
        /// HoloLensの入力動作を定義
        /// </summary>
        private void initHoloLensSetting()
        {
            // 初期化処理
            IDragGestureInterface dragInterface = null;
            rightHand.SetHandType(HandControler.HandType.HAND);
            leftHand.SetHandType(HandControler.HandType.HAND);
            rayCastControl.SetRaycastSourceObject(Camera.main.transform);

            Vector3 inputPos;
            // ハンド発見時の動作を定義
            InteractionManager.InteractionSourceDetected += (obj) =>
            {
                if (obj.state.sourcePose.TryGetPosition(out inputPos))
                {
                    var hand = rightHand.gameObject.activeSelf ? leftHand : rightHand;
                    hand.name = obj.state.source.id.ToString();
                    hand.gameObject.SetActive(true);
                    hand.SetHandPoint(inputPos);
                }
            };
            // ハンド移動時の動作を定義
            InteractionManager.InteractionSourceUpdated += (obj) =>
            {
                if (obj.state.sourcePose.TryGetPosition(out inputPos))
                {
                    (obj.state.source.id.ToString() == rightHand.name ? rightHand : leftHand).SetHandPoint(inputPos);
                    multiHandFlag = rightHand.gameObject.activeSelf && leftHand.gameObject.activeSelf;
                    if (dragInterface != null) dragInterface.OnUpdateDrag(rightHand.transform.position);
                    if (!rightHand.gameObject.activeSelf && !leftHand.gameObject.activeSelf)
                    {
                        rightHand.gameObject.SetActive(true);
                        rightHand.name = obj.state.source.id.ToString();
                    }
                }
            };
            // ハンドロスト時の動作を定義
            InteractionManager.InteractionSourceLost += (obj) =>
            {
                (obj.state.source.id.ToString() == rightHand.name ? rightHand : leftHand).gameObject
                    .SetActive(false);
                multiHandFlag = false;
                dragInterface = null;
            };
            // ハンド入力時の動作を定義
            InteractionManager.InteractionSourcePressed += (obj) =>
            {
                dragInterface = rayCastControl.GetRaycastHitInterface<IDragGestureInterface>();
                if (dragInterface != null) dragInterface.OnStartDrag(rightHand.GetGazeSourcePoint().position);
            };
            // ハンド入力解放時の動作を定義
            InteractionManager.InteractionSourceReleased += (obj) =>
            {
                var iClick = rayCastControl.GetRaycastHitInterface<IClickGestureInterface>();
                if (multiHandFlag == false && iClick != null) iClick.OnClick();
                dragInterface = null;
            };
        }

        /// <summary>
        /// WindowsMRの入力動作を定義
        /// </summary>
        private void initWinMRSetting()
        {
            // 初期化処理
            IDragGestureInterface rightInterface = null;
            IDragGestureInterface leftInterface = null;
            rightHand.SetHandType(HandControler.HandType.MOTIONCONTROLER);
            leftHand.SetHandType(HandControler.HandType.MOTIONCONTROLER);
            rayCastControl.SetRaycastSourceObject(rightHand.GetGazeSourcePoint(), -4.0f / 3.0f);
            rayCastControl.isActiveLine = true;

            Vector3 inputPos;
            Quaternion inputRot;
            // ハンド発見時の動作を定義
            InteractionManager.InteractionSourceDetected += (obj) =>
            {
                if (obj.state.sourcePose.TryGetPosition(out inputPos) &&
                    obj.state.sourcePose.TryGetRotation(out inputRot))
                {
                    var hand = obj.state.source.handedness == InteractionSourceHandedness.Right ? rightHand : leftHand;
                    hand.gameObject.SetActive(true);
                    hand.SetHandPoint(inputPos, inputRot);
                }
            };
            // ハンド移動時の動作を定義
            InteractionManager.InteractionSourceUpdated += (obj) =>
            {
                if (obj.state.sourcePose.TryGetPosition(out inputPos) &&
                    obj.state.sourcePose.TryGetRotation(out inputRot))
                {
                    var hand = obj.state.source.handedness == InteractionSourceHandedness.Right ? rightHand : leftHand;
                    hand.SetHandPoint(inputPos, inputRot);
                    var handPos = hand.GetGazeSourcePoint();
                    if (rightInterface != null) rightInterface.OnUpdateDrag(handPos.position, handPos.rotation);
                    if (leftInterface != null) leftInterface.OnUpdateDrag(handPos.position, handPos.rotation);
                }
            };
            // ハンドロスト時の動作を定義
            InteractionManager.InteractionSourceLost += (obj) =>
            {
                var hand = obj.state.source.handedness == InteractionSourceHandedness.Right ? rightHand : leftHand;
                hand.gameObject.SetActive(false);
                rightInterface = null;
                leftInterface = null;
            };
            // ハンド入力時の動作を定義
            InteractionManager.InteractionSourcePressed += (obj) =>
            {
                // 入力動作を行ったハンドに切り替え
                var hand = obj.state.source.handedness == InteractionSourceHandedness.Right ? rightHand : leftHand;
                var handPos = hand.GetGazeSourcePoint();
                rayCastControl.SetRaycastSourceObject(handPos);
                if (obj.pressType == InteractionSourcePressType.Select)
                {
                    var iDrag = rayCastControl.GetRaycastHitInterface<IDragGestureInterface>();
                    if (obj.state.source.handedness == InteractionSourceHandedness.Right) rightInterface = iDrag;
                    else leftInterface = iDrag;
                    if (iDrag != null) iDrag.OnStartDrag(handPos.position, handPos.rotation);
                }
            };
            // ハンド入力解放時の動作を定義
            InteractionManager.InteractionSourceReleased += (obj) =>
            {
                var iClick = rayCastControl.GetRaycastHitInterface<IClickGestureInterface>();
                if (obj.pressType == InteractionSourcePressType.Select && iClick != null) iClick.OnClick();
                rightInterface = null;
                leftInterface = null;
            };
        }
    }
}
#endif
