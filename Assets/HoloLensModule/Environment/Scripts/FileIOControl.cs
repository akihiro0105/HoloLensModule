using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;
#if UNITY_UWP
using System.Threading.Tasks;
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif

namespace HoloLensModule.Environment
{
    public class FileIOControl
    {
        // Path
        public static string LocalFolderPath
        {
            get
            {
                var path = Application.dataPath + "\\..\\Local";
#if UNITY_UWP
                path = Application.persistentDataPath;
#elif UNITY_EDITOR || UNITY_STANDALONE
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
#endif
                return path;
            }
        }
        public static string StreamingAssetsFolderPath
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        } 

        // Read File
        public static IEnumerator ReadTextFile(string name,Action<string> action)
        {
            string data = null;
#if UNITY_UWP
            Task task = Task.Run(() =>
            {
                if (File.Exists(name))
                {
                    data = File.ReadAllText(name);
                }
            });
            yield return new WaitWhile(() => task.IsCompleted == false);
#elif UNITY_EDITOR || UNITY_STANDALONE
            Thread thread = new Thread(() => {
                if (File.Exists(name))
                {
                    data = File.ReadAllText(name);
                }
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            yield return null;
            action.Invoke(data);
        }

        public static IEnumerator ReadBytesFile(string name, Action<byte[]> action)
        {
            byte[] data = null;
#if UNITY_UWP
            Task task = Task.Run(() =>
              {
                  if (File.Exists(name))
                  {
                      data = File.ReadAllBytes(name);
                  }
              });
            yield return new WaitWhile(() => task.IsCompleted == false);
#elif UNITY_EDITOR || UNITY_STANDALONE
            Thread thread = new Thread(() => {
                if (File.Exists(name))
                {
                    data = File.ReadAllBytes(name);
                }
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            yield return null;
            action.Invoke(data);
        }

        // Write
        public static IEnumerator WriteTextFile(string name,string data,Action action = null)
        {
#if UNITY_UWP
            Task task = Task.Run(() =>
            {
                if (File.Exists(name))
                {
                    File.WriteAllText(name, data);
                }
            });
            yield return new WaitWhile(() => task.IsCompleted == false);
#elif UNITY_EDITOR || UNITY_STANDALONE
            Thread thread = new Thread(() => {
                File.WriteAllText(name, data);
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            yield return null;
            if (action != null) action.Invoke();
        }

        public static IEnumerator WriteAppendTextFile(string name, string data, Action action = null)
        {
#if UNITY_UWP
            Task task = Task.Run(() =>
            {
                if (File.Exists(name))
                {
                    File.AppendAllText(name, data);
                }
            });
            yield return new WaitWhile(() => task.IsCompleted == false);
#elif UNITY_EDITOR || UNITY_STANDALONE
            Thread thread = new Thread(() => {
                File.AppendAllText(name, data);
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            yield return null;
            if (action != null) action.Invoke();
        }

        public static IEnumerator WriteBytesFile(string name, byte[] data, Action action = null)
        {
#if UNITY_UWP
            Task task = Task.Run(() =>
            {
                if (File.Exists(name))
                {
                    File.WriteAllBytes(name, data);
                }
            });
            yield return new WaitWhile(() => task.IsCompleted == false);
#elif UNITY_EDITOR || UNITY_STANDALONE
            Thread thread = new Thread(()=> {
                File.WriteAllBytes(name, data);
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
#endif
            yield return null;
            if (action != null) action.Invoke();
        }
    }
}
