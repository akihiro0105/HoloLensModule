using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;
using HoloLensModule.Environment;
using System;

public class TCPSample : MonoBehaviour {
    public GameObject obj;

    private TCPClientManager tcpClient;
    private TCPServerManager tcpServer;

    // Use this for initialization
    void Start () {
        Debug.Log(SystemInfomation.IPAddress);
        tcpServer = new TCPServerManager(8000);
        tcpClient = new TCPClientManager("localhost",8000);
        tcpClient.ListenerMessageEvent += ListenerMessageEvent;
    }

    private void ListenerMessageEvent(string ms)
    {
        JsonPosition json = JsonUtility.FromJson<JsonPosition>(ms);
        Debug.Log("receive");
    }

    private void OnDestroy()
    {
        tcpClient.ListenerMessageEvent -= ListenerMessageEvent;
        tcpClient.DisConnectClient();
        tcpServer.DisConnectClient();
    }

    // Update is called once per frame
    void Update () {

        JsonPosition json = new JsonPosition();
        json.SetVector3(obj.transform.localPosition);
        tcpClient.SendMessage(JsonUtility.ToJson(json));
        Debug.Log("send " + json.x + " " + json.y + " " + json.z);
    }

    [Serializable]
    public class JsonPosition
    {
        public float x, y, z;
        public void SetVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
    }
}
