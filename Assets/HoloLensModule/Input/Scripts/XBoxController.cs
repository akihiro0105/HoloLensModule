using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if WINDOWS_UWP
using Windows.Gaming.Input;
using Windows.System;
#endif

namespace HoloLensModule.Input
{
    /// <summary>
    /// XBoxのボタン定義
    /// </summary>
    [Flags]
    [Serializable]
    public enum XBoxInput : uint
    {
        None = 0,
        Menu = 1,
        View = 2,
        A = 4,
        B = 8,
        X = 16,
        Y = 32,
        DPadUp = 64,
        DPadDown = 128,
        DPadLeft = 256,
        DPadRight = 512,
        LeftShoulder = 1024,
        RightShoulder = 2048,
        LeftThumbstick = 4096,
        RightThumbstick = 8192
    }

    /// <summary>
    /// XBoxのスティック定義
    /// </summary>
    [Serializable]
    public enum XBoxStick
    { 
        // Axis
        LeftThumbstick,
        RightThumbstick,
        Trigger,
    }

    /// <summary>
    /// XBoxボタン通知イベントの定義
    /// </summary>
    [Serializable]
    public class ButtonActionEvent : UnityEvent<XBoxInput> { }

    /// <summary>
    /// XBoxスティック通知イベントの定義
    /// </summary>
    [Serializable]
    public class AxisActionEvent : UnityEvent<XBoxStick, Vector2> { }

    /// <summary>
    /// XBoxコントローラーの入力を受け取る
    /// </summary>
    public class XBoxController : MonoBehaviour
    {
        [SerializeField]private ButtonActionEvent getButton = new ButtonActionEvent();
        [SerializeField]private AxisActionEvent getAxis = new AxisActionEvent();
#if WINDOWS_UWP
        private Gamepad gamepad = null;
#endif

        // Use this for initialization
        void Start()
        {
#if WINDOWS_UWP
            // 接続されたGamePadを取得
            gamepad = (Gamepad.Gamepads.Count > 0) ? Gamepad.Gamepads[0] : null;
            Gamepad.GamepadAdded += (sender, e) => { gamepad = e; };
#endif
        }

        // Update is called once per frame
        void Update()
        {
            // ボタンとスティックの入力を取得して通知
#if WINDOWS_UWP
            if (gamepad != null)
            {
                var reader = gamepad.GetCurrentReading();
                var lx = (float)reader.LeftThumbstickX;
                var ly = (float)reader.LeftThumbstickY;
                if (Mathf.Abs(lx) > 0.1f || Mathf.Abs(ly) > 0.1f) getAxis.Invoke(XBoxStick.LeftThumbstick, new Vector2(lx, ly));
                var rx = (float)reader.RightThumbstickX;
                var ry = (float)reader.RightThumbstickY;
                if (Mathf.Abs(rx) > 0.1f || Mathf.Abs(ry) > 0.1f) getAxis.Invoke(XBoxStick.RightThumbstick, new Vector2(rx, ry));
                var lt = (float)reader.LeftTrigger;
                var rt = (float)reader.RightTrigger;
                if (Mathf.Abs(lt) > 0.1f || Mathf.Abs(rt) > 0.1f) getAxis.Invoke(XBoxStick.Trigger, new Vector2(lt, rt));
                getButton.Invoke((XBoxInput)reader.Buttons);
            }
#else
            // No support (DPad, RightThumbstick, Trigger)
            // UWP以外ではXBoxコントローラーの十字キー，右側スティック，左右のトリガーは非サポート
            var gamepad = UnityEngine.Input.GetJoystickNames();
            if (((gamepad.Length > 0) ? true : false) == true && gamepad[0] != "")
            {
                var axisX = UnityEngine.Input.GetAxis("Horizontal");
                var axisY = UnityEngine.Input.GetAxis("Vertical");
                if (Mathf.Abs(axisX) > 0.1f || Mathf.Abs(axisY) > 0.1f) getAxis.Invoke(XBoxStick.LeftThumbstick, new Vector2(axisX, axisY));

                XBoxInput xBoxInput = 0;
                if (UnityEngine.Input.GetKey("joystick button 0")) xBoxInput = xBoxInput | XBoxInput.A;
                if (UnityEngine.Input.GetKey("joystick button 1")) xBoxInput = xBoxInput | XBoxInput.B;
                if (UnityEngine.Input.GetKey("joystick button 2")) xBoxInput = xBoxInput | XBoxInput.X;
                if (UnityEngine.Input.GetKey("joystick button 3")) xBoxInput = xBoxInput | XBoxInput.Y;
                if (UnityEngine.Input.GetKey("joystick button 4")) xBoxInput = xBoxInput | XBoxInput.LeftShoulder;
                if (UnityEngine.Input.GetKey("joystick button 5")) xBoxInput = xBoxInput | XBoxInput.RightShoulder;
                if (UnityEngine.Input.GetKey("joystick button 6")) xBoxInput = xBoxInput | XBoxInput.View;
                if (UnityEngine.Input.GetKey("joystick button 7")) xBoxInput = xBoxInput | XBoxInput.Menu;
                if (UnityEngine.Input.GetKey("joystick button 8")) xBoxInput = xBoxInput | XBoxInput.LeftThumbstick;
                if (UnityEngine.Input.GetKey("joystick button 9")) xBoxInput = xBoxInput | XBoxInput.RightThumbstick;
                getButton.Invoke(xBoxInput);
            }
#endif
        }

        /// <summary>
        /// コントローラー振動機能
        /// </summary>
        /// <param name="rPower"></param>
        /// <param name="lPower"></param>
        public void SetVibration(float rPower,float lPower)
        {
#if WINDOWS_UWP
            if (gamepad != null)
            {
                GamepadVibration gamepadVibration = new GamepadVibration();
                gamepadVibration.LeftMotor = rPower;
                gamepadVibration.RightMotor = lPower;
                gamepad.Vibration = gamepadVibration;
            }
#endif
        }

        /// <summary>
        /// 入力ボタン確認用
        /// </summary>
        /// <param name="x"></param>
        public void GetLogButton(XBoxInput x)
        {
            if (x != XBoxInput.None) Debug.Log(x.ToString());
        }

        /// <summary>
        /// 入力スティック確認用
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GetLogAxis(XBoxStick x, Vector2 y)
        {
            Debug.Log(x.ToString()+" "+y.ToString("0.0"));
        }
    }
}
