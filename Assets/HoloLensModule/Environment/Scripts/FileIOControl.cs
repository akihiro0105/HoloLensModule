using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;
#if WINDOWS_UWP
using System.Threading.Tasks;
#endif

namespace HoloLensModule.Environment
{
    /// <summary>
    /// ファイル入出力に関するクラス
    /// </summary>
    public class FileIOControl
    {
        /// <summary>
        /// ローカルフォルダパスを取得
        /// </summary>
        public static string LocalFolderPath
        {
            get
            {
                var path = Application.persistentDataPath;
#if UNITY_EDITOR || UNITY_STANDALONE
                path = Application.dataPath + "\\..\\Local";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
#endif
                return path;
            }
        }

        /// <summary>
        /// テキストデータを指定ファイルから読み込み
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerator ReadTextFile(string name,Action<string> action)
        {
            string data = null;
            if (File.Exists(name))
            {
#if WINDOWS_UWP
            var task = Task.Run(() =>data = File.ReadAllText(name));
            yield return new WaitWhile(() => task.IsCompleted == false);
#else
                var thread = new Thread(() => data = File.ReadAllText(name));
                thread.Start();
                yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            }
            action.Invoke(data);
        }

        /// <summary>
        /// バイナリデータを指定ファイルから読み込み
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerator ReadBytesFile(string name, Action<byte[]> action)
        {
            byte[] data = null;
            if (File.Exists(name))
            {
#if WINDOWS_UWP
            var task = Task.Run(() =>data = File.ReadAllBytes(name));
            yield return new WaitWhile(() => task.IsCompleted == false);
#else
                var thread = new Thread(() => data = File.ReadAllBytes(name));
                thread.Start();
                yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            }
            action.Invoke(data);
        }

        /// <summary>
        /// テキストデータを指定ファイルに保存
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerator WriteTextFile(string name,string data,Action action = null)
        {
#if WINDOWS_UWP
            var task = Task.Run(() =>File.WriteAllText(name, data));
            yield return new WaitWhile(() => task.IsCompleted == false);
#else
            var thread = new Thread(() => File.WriteAllText(name, data));
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            yield return null;
            if (action != null) action.Invoke();
        }

        /// <summary>
        /// テキストデータを指定ファイルに追記保存
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerator WriteAppendTextFile(string name, string data, Action action = null)
        {
#if WINDOWS_UWP
            var task = Task.Run(() =>File.AppendAllText(name, data));
            yield return new WaitWhile(() => task.IsCompleted == false);
#else
            var thread = new Thread(() => File.AppendAllText(name, data));
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            yield return null;
            if (action != null) action.Invoke();
        }

        /// <summary>
        /// バイナリデータを指定ファイルに保存
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerator WriteBytesFile(string name, byte[] data, Action action = null)
        {
#if WINDOWS_UWP
            var task = Task.Run(() =>File.WriteAllBytes(name, data));
            yield return new WaitWhile(() => task.IsCompleted == false);
#else
            var thread = new Thread(() => File.WriteAllBytes(name, data));
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            if (action != null) action.Invoke();
        }
    }
}
