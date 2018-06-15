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

    private JsonPosition current = new JsonPosition();
    private bool receivedata = false;
    // Use this for initialization
    void Start () {
        Debug.Log(SystemInfomation.IPAddress);
        tcpServer = new TCPServerManager(8001);
        tcpClient = new TCPClientManager("192.168.1.5",8001);
        tcpClient.ListenerMessageEvent += ListenerMessageEvent;
    }

    private void ListenerMessageEvent(string ms)
    {
        JsonPosition json = JsonUtility.FromJson<JsonPosition>(ms);
        if (!json.ip.Equals(current.ip))
        {
            current.x = json.x;
            current.y = json.y;
            current.z = json.z;
            receivedata = true;
        }
    }

    private void OnDestroy()
    {
        tcpClient.ListenerMessageEvent -= ListenerMessageEvent;
        tcpClient.DisConnectClient();
        tcpServer.DisConnectClient();
    }

    // Update is called once per frame
    void Update () {

        if (receivedata == true)
        {
            obj.transform.localPosition = new Vector3(current.x, current.y, current.z);
            receivedata = false;
            Debug.Log("receive " + obj.transform.localPosition.x + " " + obj.transform.localPosition.y + " " + obj.transform.localPosition.z);
        }
        else
        {
            if (obj.transform.localPosition.x != current.x || obj.transform.localPosition.y != current.y || obj.transform.localPosition.z != current.z)
            {
                JsonPosition json = new JsonPosition();
                json.SetVector3(obj.transform.localPosition);
                tcpClient.SendMessage(JsonUtility.ToJson(json));
                Debug.Log("send " + json.x + " " + json.y + " " + json.z);
            }
        }
        current.SetVector3(obj.transform.localPosition);
    }

    [Serializable]
    public class JsonPosition
    {
        public string ip = "";
        public float x, y, z;
        public JsonPosition()
        {
            ip = SystemInfomation.IPAddress;
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
