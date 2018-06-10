using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloLensModule.Network;
using HoloLensModule.Environment;
using System;

public class UDPSample : MonoBehaviour {
    public GameObject obj;

    private UDPSenderManager sender;
    private UDPListenerManager listener;

    private JsonPosition current = new JsonPosition();
    private bool receive = false;
	// Use this for initialization
	void Start () {
        Debug.Log(SystemInfomation.DirectedBroadcastAddress);
        Debug.Log(SystemInfomation.IPAddress);
        sender = new UDPSenderManager(SystemInfomation.DirectedBroadcastAddress, 8000);
        listener = new UDPListenerManager(8000);
        listener.ListenerMessageEvent += ListenerMessageEvent;

        current.connect = false;
        current.SetVector3(obj.transform.localPosition);
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
        if (address == SystemInfomation.IPAddress)
        {
            current.connect = true;
        }
        else
        {
            if (current.connect == true)
            {
                if (json.connect==true)
                {
                    current.x = json.x;
                    current.y = json.y;
                    current.z = json.z;
                    receive = true;
                }
                else
                {
                    sender.SendMessage(JsonUtility.ToJson(current));
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (current.connect == false)
        {
            sender.SendMessage(JsonUtility.ToJson(current));
        }
        
        if (receive ==true)
        {
            obj.transform.localPosition = new Vector3(current.x, current.y, current.z);
            receive = false;
            Debug.Log("receive " + obj.transform.localPosition.x + " " + obj.transform.localPosition.y + " " + obj.transform.localPosition.z);
        }
        else
        {
            if (obj.transform.localPosition.x != current.x || obj.transform.localPosition.y != current.y || obj.transform.localPosition.z != current.z)
            {
                current.SetVector3(obj.transform.localPosition);
                sender.SendMessage(JsonUtility.ToJson(current));
                Debug.Log("send " + current.x + " " + current.y + " " + current.z);
            }
            else
            {
                current.SetVector3(obj.transform.localPosition);
            }
        }
    }

    [Serializable]
    public class JsonPosition
    {
        public bool connect;
        public float x, y, z;
        public void SetVector3(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }
    }
}
