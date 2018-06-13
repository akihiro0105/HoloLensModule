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
        private Thread mainthread = null;
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
            if (mainthread == null)
            {
                mainthread = new Thread(()=>
                {
                    TcpListener tcpListener = new TcpListener(IPAddress.Any, port);
                    tcpListener.Start();
                    isActiveThread = true;
                    while (isActiveThread)
                    {
                        try
                        {
                            TcpClient tcpClient = tcpListener.AcceptTcpClient();
                            tcpClient.ReceiveTimeout = 100;
                            NetworkStream stream = tcpClient.GetStream();

                            streamList.Add(stream);

                            byte[] bytes = new byte[tcpClient.ReceiveBufferSize];
                            int num = stream.Read(bytes, 0, bytes.Length);
                            streamList[0].Write(bytes, 0, bytes.Length);

                            stream.Close();
                            tcpClient.Close();
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }
                    tcpListener.Stop();
                });
                mainthread.Start();
            }
#endif
        }


        public void DisConnectClient()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (mainthread != null)
            {
                isActiveThread = false;
                mainthread.Abort();
                mainthread = null;
            }
#endif
        }

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
    }
}
