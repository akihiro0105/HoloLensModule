using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLensModule
{
    // コンソール出力を取得する
    public class DebugConsole : MonoBehaviour
    {
        public Text UIText = null;
        public int ViewLineUIText = 20;

        private List<string> logtext = new List<string>();

        public delegate void DebugConsoleEventHandler(string condition, string stackTrace, LogType type);
        public DebugConsoleEventHandler DebugConsoleEvent;

        // Use this for initialization
        void Start() { Application.logMessageReceived += logMessageReceived; }

        void OnDestroy() { Application.logMessageReceived -= logMessageReceived; }

        private void logMessageReceived(string condition, string stackTrace, LogType type)
        {
            string[] conditions = condition.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            if (logtext.Count + conditions.Length > ViewLineUIText) logtext.RemoveRange(0, logtext.Count + conditions.Length - ViewLineUIText);
            logtext.AddRange(conditions);
            if (UIText != null)
            {
                UIText.text = "";
                for (int i = 0; i < logtext.Count; i++) UIText.text += logtext[i] + Environment.NewLine;
            }
            if (DebugConsoleEvent != null) DebugConsoleEvent(condition, stackTrace, type);
        }
    }

}