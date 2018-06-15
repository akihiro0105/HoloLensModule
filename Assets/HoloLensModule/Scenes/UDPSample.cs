using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;
using HoloLensModule.Environment;
using System;

// Editor StandaloneのUdpClientではデータ送信後でないと受信が行えない

public class UDPSample : MonoBehaviour {
    public GameObject obj;

    private UDPSenderManager sender;
    private UDPListenerManager listener;

    private JsonPosition current = new JsonPosition();
    private bool connected = false;
	// Use this for initialization
	void Start () {
        sender = new UDPSenderManager(SystemInfomation.DirectedBroadcastAddress, 8000);
        listener = new UDPListenerManager(8000);
        listener.ListenerMessageEvent += ListenerMessageEvent;

        sender.SendMessage(JsonUtility.ToJson(new JsonPosition()));
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
        if (json.connect == true)
        {
            current.x = json.x;
            current.y = json.y;
            current.z = json.z;
            current.connect = true;
        }
        else
        {
            if (connected==true)
            {
                json.connect = true;
                json.SetVector3(new Vector3(current.x, current.y, current.z));
                sender.SendMessage(JsonUtility.ToJson(json));
            }
        }
        connected = true;
    }

    // Update is called once per frame
    void Update () {
        if (current.connect == true)
        {
            obj.transform.localPosition = new Vector3(current.x, current.y, current.z);
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
                sender.SendMessage(JsonUtility.ToJson(json));
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
    }
}
