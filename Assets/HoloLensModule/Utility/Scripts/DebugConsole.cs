using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoloLensModule.Utility
{
    /// <summary>
    /// ログメッセージを取得してUI表示とイベント通知を行う
    /// </summary>
    public class DebugConsole : MonoBehaviour
    {
        /// <summary>
        /// ログ表示用UIText
        /// </summary>
        [SerializeField] private Text logText = null;
        /// <summary>
        /// 最大表示ログ数(行数)
        /// </summary>
        [SerializeField] private int maxQueue = 20;

        /// <summary>
        /// UI表示用ログリスト
        /// </summary>
        private Queue<string> logQueue=new Queue<string>();

        /// <summary>
        /// ログメッセージ通知用イベント
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        public delegate void DebugConsoleEventHandler(string condition, string stackTrace, LogType type);
        public event DebugConsoleEventHandler DebugConsoleEvent;

        // Use this for initialization
        void Start()
        {
            Application.logMessageReceived += logMessageReceived;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= logMessageReceived;
        }

        /// <summary>
        /// ログメッセージ受け取りイベントを通知+UITextの更新を行う
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stackTrace"></param>
        /// <param name="type"></param>
        private void logMessageReceived(string condition, string stackTrace, LogType type)
        {
            // ログメッセージ通知
            if (DebugConsoleEvent != null) DebugConsoleEvent(condition, stackTrace, type);

            // ログメッセージをUIに表示
            var conditions = condition.Split(new string[] { System.Environment.NewLine }, System.StringSplitOptions.None);
            foreach (var t in conditions) logQueue.Enqueue(t);
            for (var i = 0; i < logQueue.Count - maxQueue; i++) logQueue.Dequeue();
            if (logText != null)
            {
                logText.text = "";
                foreach (var t in logQueue.ToArray()) logText.text += t + System.Environment.NewLine;
            }
        }
    }

}