using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HoloLensModule.Input
{
    /// <summary>
    /// 動作確認用ボタン入力クラス
    /// </summary>
    public class ButtonControl : ButtonControlBaseClass
    {
        protected override void ClickButton()
        {
        }

        protected override void FocusIn()
        {
        }

        protected override void FocusOut()
        {
        }

        protected override void FocusUpdate()
        {
        }
    }

    /// <summary>
    /// Gazeと入力操作に対する動作の基底クラス
    /// </summary>
    public abstract class ButtonControlBaseClass : MonoBehaviour, IFocusInterface, IClickGestureInterface
    {
        /// <summary>
        /// 対象オブジェクトをクリックされた時のイベント
        /// </summary>
        public UnityEvent ClickEvent;

        #region Private Function
        private bool isFocus = false;
        public void OnClick()
        {
            ClickButton();
            if (ClickEvent != null) ClickEvent.Invoke();
        }

        public void RaycastHit()
        {
            FocusIn();
            isFocus = true;
        }

        public void RaycastOut()
        {
            FocusOut();
            isFocus = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (isFocus == true) FocusUpdate();
        }
        #endregion

        /// <summary>
        /// GazeのFocusに入った時の関数
        /// </summary>
        protected abstract void FocusIn();
        /// <summary>
        /// GazeのFocusに入っている間呼び出される関数
        /// </summary>
        protected abstract void FocusUpdate();
        /// <summary>
        /// GazeのFocusから外れた時の関数
        /// </summary>
        protected abstract void FocusOut();
        /// <summary>
        /// クリックされた時の関数
        /// </summary>
        protected abstract void ClickButton();

        /// <summary>
        /// クリック動作確認用
        /// </summary>
        protected void DebugLog()
        {
            Debug.Log("OnClick");
        }
    }
}
