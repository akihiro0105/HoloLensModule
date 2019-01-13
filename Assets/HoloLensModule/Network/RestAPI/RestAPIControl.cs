using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloLensModule.Network
{
    /// <summary>
    /// 簡易的なRestAPI
    /// </summary>
    public class RestAPIControl
    {
        /// <summary>
        /// RestAPIのGet
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="complete"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static IEnumerator GetRestAPI(string uri,Action<byte[]> complete,Action<long> error =null)
        {
            using (var www=UnityWebRequest.Get(uri))
            {
                yield return www.SendWebRequest();
                if (www.responseCode!=200)
                {
                    if (error != null) error.Invoke(www.responseCode);
                }
                else
                {
                    if (complete != null) complete.Invoke(www.downloadHandler.data);
                }
            }
        }

        /// <summary>
        /// RestAPIのPost
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="value"></param>
        /// <param name="complete"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static IEnumerator PostRestAPI(string uri, Dictionary<string, string> value, Action<string> complete, Action<long> error = null)
        {
            var form = new WWWForm();
            foreach (var item in value) form.AddField(item.Key, item.Value);
            using (var www = UnityWebRequest.Post(uri, form))
            {
                yield return www.SendWebRequest();
                if (www.responseCode != 200)
                {
                    if (error != null) error.Invoke(www.responseCode);
                }
                else
                {
                    if (complete != null) complete.Invoke(www.downloadHandler.text);
                }
            }
        }
    }
}