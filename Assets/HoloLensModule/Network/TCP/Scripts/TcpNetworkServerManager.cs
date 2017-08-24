using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_UWP
        
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
#endif

namespace HoloLensModule.Network
{
    public class TcpNetworkServerManager : HoloLensModuleSingleton<TcpNetworkServerManager>
    {
        public int Port = 1111;
        [SerializeField]
        private bool OnAwake = false;
        [SerializeField]
        private bool AutoBroadcastReceive = true;

        public delegate void ConnectMessageHandler(string ip);
        public delegate void ReceiveMessageHandler(string ip, Byte[] data);
        public static ConnectMessageHandler ConnectMessage;
        public static ConnectMessageHandler DisconnectMessage;
        public static ReceiveMessageHandler ReceiveMessage;

#if UNITY_UWP
        
#elif UNITY_EDITOR || UNITY_STANDALONE
        private TcpListener tcpserver = null;
        private List<Thread> threads = new List<Thread>();
        private List<NetworkStream> streams = new List<NetworkStream>();
        private bool isConnected = false;
#endif

        void Start()
        {
            if (OnAwake == true) StartServer();
        }

        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (isConnected == true)
            {
                Thread thread = new Thread(SubThreadProcess);
                thread.Start();
                threads.Add(thread);
                isConnected = false;
            }
#endif
        }

        public void StartServer()
        {     
#if UNITY_EDITOR || UNITY_STANDALONE
            Debug.Log("Server Start");
            tcpserver = new TcpListener(IPAddress.Any, Port);
            tcpserver.Start();
            Thread thread = new Thread(SubThreadProcess);
            thread.Start();
            threads.Add(thread);
            isConnected = false;
#endif
        }

        public void StopServer()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Debug.Log("Server Stop");
            for (int i = 0; i < streams.Count; i++) streams[i].Close();
            streams.Clear();
            for (int i = 0; i < threads.Count; i++) threads[i].Abort();
            threads.Clear();
            tcpserver.Stop();
            tcpserver = null;
            isConnected = false;
#endif
        }

        public void SendMessage(Byte[] data)
        {    
#if UNITY_EDITOR || UNITY_STANDALONE
            for (int i = 0; i < streams.Count; i++) streams[i].Write(data, 0, data.Length);
#endif
        }
       
#if UNITY_EDITOR || UNITY_STANDALONE

        private void SubThreadProcess()
        {
            TcpClient tcpclient = tcpserver.AcceptTcpClient();
            string ip = ((IPEndPoint)tcpclient.Client.RemoteEndPoint).Address.ToString();
            NetworkStream stream = tcpclient.GetStream();
            streams.Add(stream);
            int tcpcount = streams.Count - 1;
            Debug.Log("Connected " + ip);
            if (ConnectMessage != null) ConnectMessage(ip);
            isConnected = true;
            Byte[] bytes = new Byte[1024];
            try
            {
                while (tcpclient.Connected)
                {
                    int count = stream.Read(bytes, 0, bytes.Length);
                    Byte[] data = new Byte[count];
                    Buffer.BlockCopy(bytes, 0, data, 0, count);
                    if (ReceiveMessage != null) ReceiveMessage(ip, data);
                    if (AutoBroadcastReceive == true)
                    {
                        for (int i = 0; i < streams.Count; i++) if (tcpcount != i) streams[i].Write(data, 0, data.Length);
                    }
                }
                Debug.Log("Disconnected " + ip);
                if (DisconnectMessage != null) DisconnectMessage(ip);
                stream.Close();
                tcpclient.Close();
                streams.RemoveAt(tcpcount);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                stream.Close();
                tcpclient.Close();
                streams.RemoveAt(tcpcount);
            }
        }
#endif
        protected override void OnDestroy()
        {
            StopServer();
        }
    }
}
