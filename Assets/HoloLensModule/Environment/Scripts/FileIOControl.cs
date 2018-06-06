using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;
#if UNITY_UWP
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
        public static IEnumerator ReadTextFile(string name, Action<string> action)
        {
            string data = null;
            Thread thread = new Thread(() => {
                data = File.ReadAllText(name);
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
            action.Invoke(data);
        }

        public static IEnumerator ReadBytesFile(string name, Action<byte[]> action)
        {
            byte[] data = null;
            Thread thread = new Thread(() => {
                data = File.ReadAllBytes(name);
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
            action.Invoke(data);
        }

        // Write
        public static IEnumerator WriteTextFile(string name, string data, Action action = null)
        {
            Thread thread = new Thread(() => {
                File.WriteAllText(name, data);
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
            if (action != null) action.Invoke();
        }

        public static IEnumerator WriteBytesFile(string name, byte[] data, Action action = null)
        {
            Thread thread = new Thread(() => {
                File.WriteAllBytes(name, data);
            });
            thread.Start();
            yield return new WaitWhile(() => thread.IsAlive == true);
            if (action != null) action.Invoke();
        }

        // Only UWP
        public enum UWPDirectoryType
        {
            Document,
            Picture,
            Video,
            Music
        }

        // View UWP
        public static string[] ViewUWPFiles(UWPDirectoryType type)
        {
            string[] data = null;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            return data;
        }

        // Read UWP
        public static void ReadTextUWPFile(UWPDirectoryType type, string name, Action<string, string> action)
        {
            string data = "";
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            action.Invoke(name, data);
        }

        public static void ReadBytesUWPFile(UWPDirectoryType type, string name, Action<string, byte[]> action)
        {
            byte[] data = null;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            action.Invoke(name, data);
        }

        // Write UWP
        public static void WriteTextUWPFile(UWPDirectoryType type, string name, string data, Action<string> action = null)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            if (action != null) action.Invoke(name);
        }

        public static void WriteBytesUWPFile(UWPDirectoryType type, string name, byte[] data, Action<string> action = null)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            if (action != null) action.Invoke(name);
        }

        // Delete UWP
        public static string DeleteUWPFile(UWPDirectoryType type, string name)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
            return name;
        }
    }
}
