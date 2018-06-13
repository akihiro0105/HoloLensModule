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
    private bool connected = false;
    // Use this for initialization
    void Start () {
        tcpServer = new TCPServerManager(8000);

        tcpClient = new TCPClientManager("192.168.1.5",8000);
        tcpClient.ListenerMessageEvent += ListenerMessageEvent;
    }

    private void ListenerMessageEvent(string ms)
    {
        JsonPosition json = JsonUtility.FromJson<JsonPosition>(ms);
        if (json.connect == true)
        {
            current.x = json.x;
            current.y = json.y;
            current.z = json.z;
            current.connect = true;
        }
        else
        {
            if (connected == true)
            {
                json.connect = true;
                json.SetVector3(current.GetVector3());
                tcpClient.SendMessage(JsonUtility.ToJson(json));
            }
        }
        connected = true;
    }

    private void OnDestroy()
    {
        tcpClient.ListenerMessageEvent -= ListenerMessageEvent;
        tcpClient.DisConnectClient();
        tcpServer.DisConnectClient();
    }

    // Update is called once per frame
    void Update () {
        if (current.connect == true)
        {
            obj.transform.localPosition = current.GetVector3();
            current.connect = false;
            Debug.Log("receive " + obj.transform.localPosition.x + " " + obj.transform.localPosition.y + " " + obj.transform.localPosition.z);
        }
        else
        {
            if (obj.transform.localPosition.x != current.x || obj.transform.localPosition.y != current.y || obj.transform.localPosition.z != current.z)
            {
                JsonPosition json = new JsonPosition();
                json.connect = true;
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
        public bool connect;
        public float x, y, z;
        public JsonPosition()
        {
            connect = false;
            SetVector3(Vector3.zero);
        }
        public void SetVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
        public Vector3 GetVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}
