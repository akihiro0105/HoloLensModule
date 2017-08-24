using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
#endif

namespace HoloLensModule.Network
{
    public class UdpNetworkClientManager : MonoBehaviour
    {
        public int port = 1234;
        [SerializeField]
        private bool OnAwake = true;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private Thread thread = null;
        private UdpClient udpclient;
#endif
        // Use this for initialization
        void Start()
        {
            if (OnAwake) UDPClientStart();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            UDPClientStop();
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (thread != null) thread.Abort();
#endif
        }

        public void UDPClientStart()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            udpclient = new UdpClient();
            udpclient.EnableBroadcast = true;
            udpclient.Connect(new IPEndPoint(IPAddress.Broadcast, port));
#endif
        }

        public void UDPClientStop()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            udpclient.Close();
#endif
        }

        public void UDPSendMessage(string data)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            thread = new Thread(new ParameterizedThreadStart(ThreadProcess));
            thread.Start(data);
#endif
        }
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ThreadProcess(object obj)
        {
            string data = (string)obj;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            udpclient.Send(bytes, bytes.Length);
        }
#endif
    }
}
