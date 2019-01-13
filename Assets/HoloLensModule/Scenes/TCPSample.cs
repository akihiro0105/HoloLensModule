using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;
using HoloLensModule.Environment;
using System;
using System.Threading;

public class TCPSample : MonoBehaviour
{
    public Transform obj;

    private TCPClientManager tcpClient;
    private TCPServerManager tcpServer;

    private JsonPosition current = new JsonPosition();

    private SynchronizationContext currentcontext;

    // Use this for initialization
    void Start()
    {
        Debug.Log(SystemInfomation.IPAddress);
        tcpServer = new TCPServerManager(8001);
        tcpClient = new TCPClientManager("192.168.1.5", 8001);
        tcpClient.ListenerMessageEvent += ListenerMessageEvent;

        currentcontext = SynchronizationContext.Current;
    }

    private void OnDestroy()
    {
        tcpClient.ListenerMessageEvent -= ListenerMessageEvent;
        tcpClient.DisConnectClient();
        tcpServer.DisConnectClient();
    }

    private void ListenerMessageEvent(string ms)
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
            tcpClient.SendMessage(JsonUtility.ToJson(json));
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
