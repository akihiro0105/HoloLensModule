using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloLensModule.Network
{
    public class RestAPIControl
    {
        public static IEnumerator GetRestAPI(string uri,Action<byte[]> complete,Action<long> error =null,Action<float> progress=null,Dictionary<string,string> header=null)
        {
            using (UnityWebRequest www=UnityWebRequest.Get(uri))
            {
                if (header!=null)
                {
                    foreach (var item in header)
                    {
                        www.SetRequestHeader(item.Key, item.Value);
                    }
                }
                www.SendWebRequest();
                while (www.isDone)
                {
                    // progress
                    if (progress != null) progress.Invoke(www.downloadProgress);
                     yield return new WaitForEndOfFrame();
                }
                if (www.responseCode!=200)
                {
                    // error
                    if (error != null) error.Invoke(www.responseCode);
                }
                else
                {
                    // complete
                    if (complete != null) complete.Invoke(www.downloadHandler.data);
                }
            }
        }

        public static IEnumerator PostRestAPI(string uri, Dictionary<string, string> value, Action<string> complete, Action<long> error = null, Action<float> progress = null, Dictionary<string, string> header = null)
        {
            WWWForm form = new WWWForm();
            foreach (var item in value)
            {
                form.AddField(item.Key, item.Value);
            }
            using (UnityWebRequest www = UnityWebRequest.Post(uri, form))
            {
                if (header != null)
                {
                    foreach (var item in header)
                    {
                        www.SetRequestHeader(item.Key, item.Value);
                    }
                }
                www.SendWebRequest();
                while (www.isDone)
                {
                    // progress
                    if (progress != null) progress.Invoke(www.downloadProgress);
                    yield return new WaitForEndOfFrame();
                }
                if (www.responseCode != 200)
                {
                    // error
                    if (error != null) error.Invoke(www.responseCode);
                }
                else
                {
                    // complete
                    if (complete != null) complete.Invoke(www.downloadHandler.text);
                }
            }
        }
    }
}