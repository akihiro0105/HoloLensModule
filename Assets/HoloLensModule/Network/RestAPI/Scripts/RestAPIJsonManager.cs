using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloLensModule.Network
{
    // RestAPIによるJsonデータのやり取り
    public class RestAPIJsonManager : MonoBehaviour
    {
        public string ServerAddress = "";

        public delegate void RestAPIJsonReceiveEventHandler(string json);
        public RestAPIJsonReceiveEventHandler JsonGetEvent;
        public RestAPIJsonReceiveEventHandler JsonPostEvent;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void PostRestAPIJson(string field, string value)
        {
            StartCoroutine(PostRestAPI(field, value));
        }

        private IEnumerator PostRestAPI(string field,string value)
        {
            WWWForm form = new WWWForm();
            form.AddField(field, value);
            using (UnityWebRequest www = UnityWebRequest.Post(ServerAddress, form))
            {
                yield return www.Send();
#if !UNITY_2017_1_OR_NEWER
                if (www.isError) Debug.LogError(www.error);
#else
                if (www.isNetworkError) Debug.LogError(www.error);
#endif
                else
                {
                    if (JsonPostEvent != null) JsonPostEvent(value);
                }
            }
        }

        public void GetRestAPIJson()
        {
            StartCoroutine(GetRestAPI());
        }

        private IEnumerator GetRestAPI()
        {
            using (UnityWebRequest www = UnityWebRequest.Get(ServerAddress))
            {
                yield return www.Send();
#if !UNITY_2017_1_OR_NEWER
                if (www.isError) Debug.LogError(www.error);
#else
                if (www.isNetworkError) Debug.LogError(www.error);
#endif
                else
                {
                    string json = www.downloadHandler.text;
                    if (JsonGetEvent != null) JsonGetEvent(json);
                }
            }
        }
    }
}
