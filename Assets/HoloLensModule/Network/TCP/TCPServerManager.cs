using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net.Sockets;
using System.Text;
using System.Threading;
#endif
using UnityEngine;

namespace HoloLensModule.Network
{
    public class TCPServerManager 
    {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private bool isActiveThread = false;
        private List<NetworkStream> streamList = new List<NetworkStream>();
#endif
        public TCPServerManager() { }

        public TCPServerManager(int port)
        {
            ConnectServer(port);
        }

        public void ConnectServer(int port)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            isActiveThread = true;
            tcpListener.BeginAcceptTcpClient(AcceptTcpClient, tcpListener);
#endif
        }

        private void AcceptTcpClient(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(AcceptTcpClient, listener);
            Debug.Log("Set TCPClient");
            tcpClient.ReceiveTimeout = 100;
            NetworkStream stream = tcpClient.GetStream();
            streamList.Add(stream);
            byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
            while (isActiveThread)
            {
                try
                {
                    int num = stream.Read(bytes, 0, bytes.Length);
                    if (num > 0)
                    {
                        for (int i = 0; i < streamList.Count; i++)
                        {
                            streamList[i].Write(bytes, 0, num);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            stream.Close();
            tcpClient.Close();
            listener.Stop();
        }

        public void DisConnectClient()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            isActiveThread = false;
#endif
        }

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
    }
}
