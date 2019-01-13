using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    /// <summary>
    /// ハンドトラッキング or MotionControllerの位置制御
    /// </summary>
    public class HandControler : MonoBehaviour
    {
        /// <summary>
        /// HoloLensとWindows MRのハンドモデルをアタッチ
        /// </summary>
        [SerializeField] private GameObject HoloLensHandModel;
        [SerializeField] private GameObject MotionControlerModel;
        /// <summary>
        /// HoloLensとWindows MRのハンド選択位置オブジェクトをアタッチ
        /// </summary>
        [SerializeField] private Transform HoloLensHandPoint;
        [SerializeField] private Transform MotionControlerPoint;

        /// <summary>
        /// ハンドトラッキングタイプ
        /// </summary>
        public enum HandType
        {
            HAND,
            MOTIONCONTROLER
        }
        private HandType Type = HandType.HAND;

        /// <summary>
        /// ハンドタイプを設定
        /// </summary>
        /// <param name="type"></param>
        public void SetHandType(HandType type)
        {
            HoloLensHandModel.SetActive((type == HandType.HAND) ? true : false);
            MotionControlerModel.SetActive((type == HandType.MOTIONCONTROLER) ? true : false);
        }

        /// <summary>
        /// ハンド位置を設定
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public void SetHandPoint(Vector3 pos, Quaternion? rot = null)
        {
            transform.position = pos;
            if (rot != null) transform.rotation = rot.Value;
        }

        /// <summary>
        /// ハンドのGaze発信元位置を取得
        /// </summary>
        /// <returns></returns>
        public Transform GetGazeSourcePoint()
        {
            return (Type == HandType.HAND) ? HoloLensHandPoint : MotionControlerPoint;
        }

        // Update is called once per frame
        void Update()
        {
            // HoloLensのハンドトラッキングは配転が無いためLookatさせておく
            if (Type == HandType.HAND)transform.LookAt(Camera.main.transform.position);
        }
    }
}
