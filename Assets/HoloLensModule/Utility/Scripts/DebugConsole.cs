using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLensModule.Utility
{
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField]
        private Text UIText = null;
        public int TextLineLength = 20;

        private List<string> logtext = new List<string>();

        public delegate void DebugConsoleEventHandler(string condition, string stackTrace, LogType type);
        public DebugConsoleEventHandler DebugConsoleEvent;

        // Use this for initialization
        void Start()
        {
            Application.logMessageReceived += logMessageReceived;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= logMessageReceived;
        }

        private void logMessageReceived(string condition, string stackTrace, LogType type)
        {
            string[] conditions = condition.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None);
            if (logtext.Count + conditions.Length > TextLineLength)
            {
                logtext.RemoveRange(0, conditions.Length);
            }
            logtext.AddRange(conditions);
            if (UIText != null)
            {
                UIText.text = "";
                for (int i = 0; i < logtext.Count; i++)
                {
                    UIText.text += logtext[i] + System.Environment.NewLine;
                }
            }
            if (DebugConsoleEvent != null) DebugConsoleEvent(condition, stackTrace, type);
        }
    }

}