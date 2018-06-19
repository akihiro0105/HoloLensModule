using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;
using HoloLensModule.Environment;
using System;
using System.Threading;

public class TCPSample : MonoBehaviour {
    public GameObject obj;

    private TCPClientManager tcpClient;
    private TCPServerManager tcpServer;

    private JsonPosition current = new JsonPosition();
    private SynchronizationContext currentcontext;
    // Use this for initialization
    void Start () {
        Debug.Log(SystemInfomation.IPAddress);
        tcpServer = new TCPServerManager(8001);
        tcpClient = new TCPClientManager("192.168.1.5",8001);
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
            tcpClient.SendMessage(JsonUtility.ToJson(json));
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
