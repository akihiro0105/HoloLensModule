using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;
using HoloLensModule.Environment;
using System;

using System.Threading;

// Editor StandaloneのUdpClientではデータ送信後でないと受信が行えない

public class UDPSample : MonoBehaviour {
    public GameObject obj;

    private UDPSenderManager sender;
    private UDPListenerManager listener;

    private JsonPosition current = new JsonPosition();
    private SynchronizationContext currentcontext;
    // Use this for initialization
    void Start () {
        Debug.Log(SystemInfomation.IPAddress);
        sender = new UDPSenderManager(SystemInfomation.DirectedBroadcastAddress, 8000);
        listener = new UDPListenerManager(8000);
        listener.ListenerMessageEvent += ListenerMessageEvent;

        sender.SendMessage(JsonUtility.ToJson(new JsonPosition()));

        currentcontext = SynchronizationContext.Current;
    }

    private void OnDestroy()
    {
        listener.ListenerMessageEvent -= ListenerMessageEvent;
        listener.DisConnectListener();
        sender.DisConnectSender();
    }

    private void ListenerMessageEvent(string ms, string address)
    {
        JsonPosition json = JsonUtility.FromJson<JsonPosition>(ms);
        currentcontext.Post(state =>
        {
            obj.transform.localPosition = new Vector3(json.x, json.y, json.z);
            current.SetVector3(obj.transform.localPosition);
            Debug.Log("receive " + obj.transform.localPosition.x + " " + obj.transform.localPosition.y + " " + obj.transform.localPosition.z);
        }, null);
    }

    // Update is called once per frame
    void Update () {
        if (obj.transform.localPosition.x != current.x || obj.transform.localPosition.y != current.y || obj.transform.localPosition.z != current.z)
        {
            JsonPosition json = new JsonPosition();
            json.SetVector3(obj.transform.localPosition);
            sender.SendMessage(JsonUtility.ToJson(json));
            Debug.Log("send " + json.x + " " + json.y + " " + json.z);
        }
        current.SetVector3(obj.transform.localPosition);
    }

    [Serializable]
    public class JsonPosition
    {
        public float x, y, z;
        public JsonPosition()
        {
            SetVector3(Vector3.zero);
        }
        public void SetVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
    }
}
