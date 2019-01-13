using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;
using HoloLensModule.Environment;
using System;
using System.Threading;

// Editor StandaloneのUdpClientではデータ送信後でないと受信が行えない

public class UDPSample : MonoBehaviour
{
    public Transform obj;

    private UDPSenderManager sender;
    private UDPListenerManager listener;

    private JsonPosition current = new JsonPosition();

    private SynchronizationContext currentcontext;

    // Use this for initialization
    void Start()
    {
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
        var json = JsonUtility.FromJson<JsonPosition>(ms);
        currentcontext.Post(state =>
        {
            obj.localPosition = json.pos;
            current.pos = obj.localPosition;
            Debug.Log("receive " + obj.localPosition);
        }, null);
    }

    // Update is called once per frame
    void Update()
    {
        if (!obj.localPosition.Equals(current.pos))
        {
            var json = new JsonPosition();
            json.pos = obj.localPosition;
            sender.SendMessage(JsonUtility.ToJson(json));
            Debug.Log("send " + json.pos);
        }

        current.pos = obj.localPosition;
    }

    [Serializable]
    public class JsonPosition
    {
        public Vector3 pos = new Vector3();
    }
}
