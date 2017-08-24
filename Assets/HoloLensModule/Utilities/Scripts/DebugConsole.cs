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
        public bool FullLog = false;
        public Text UIText = null;
        public int ViewLineUIText = 20;
        [HideInInspector]
        public List<string> logtext = new List<string>();

        public delegate void ConsoleLogEventHandler(string color,string log);
        public ConsoleLogEventHandler ConsoleLogEvent;
        // Use this for initialization
        void Start()
        {
            UIText.text = "";
            Application.logMessageReceived += logMessageReceived;
        }

        // Update is called once per frame
        void Update()
        {

        }
        void OnDestroy()
        {
            Application.logMessageReceived -= logMessageReceived;
        }

        private void logMessageReceived(string condition, string stackTrace, LogType type)
        {
            string color = "white";
            string log = "";
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    color = "red";
                    break;
                case LogType.Warning:
                    color = "yellow";
                    break;
            }
            logtext.Add(condition + Environment.NewLine);
            log += condition;
            if (FullLog)
            {
                logtext.Add(stackTrace + Environment.NewLine);
                log += stackTrace;
            }
            SetUIText();
            if (ConsoleLogEvent != null) ConsoleLogEvent(color,log);
        }

        private void SetUIText()
        {
            if (UIText != null)
            {
                UIText.text = "";
                if (logtext.Count - ViewLineUIText > 0)
                {
                    for (int i = logtext.Count - ViewLineUIText; i < logtext.Count; i++) UIText.text += logtext[i];
                }
                else
                {
                    for (int i = 0; i < logtext.Count; i++) UIText.text += logtext[i];
                }
            }
        }
    }

}