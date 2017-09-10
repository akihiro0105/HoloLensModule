using UnityEngine;
using System;
#if UNITY_UWP
        
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
#endif

namespace HoloLensModule.Network
{
    public class TcpNetworkServerManager
    {

#if UNITY_UWP
        
#elif UNITY_EDITOR || UNITY_STANDALONE
        private TcpListener tcpserver;
        private List<Thread> threads = new List<Thread>();
        private List<NetworkStream> streams = new List<NetworkStream>();
        private bool ListenFlag = false;
#endif

        public TcpNetworkServerManager(int port)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Debug.Log("Server Start");
            tcpserver = new TcpListener(IPAddress.Any, port);
            tcpserver.Start();
            ListenFlag = true;
            Thread thread = new Thread(ThreadProcess);
            thread.Start();
            threads.Add(thread);
#endif
        }

        public void DeleteManager()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Debug.Log("Server Stop");
            ListenFlag = false;
            for (int i = 0; i < threads.Count; i++) threads[i].Abort();
            threads.Clear();
            tcpserver.Stop();
#endif
        }

        public void SendMessage(string data)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            for (int i = 0; i < streams.Count; i++) if (streams[i].CanWrite) streams[i].Write(bytes, 0, bytes.Length);
#endif
        }

#if UNITY_EDITOR || UNITY_STANDALONE

        private void ThreadProcess(object obj)
        {
            TcpClient tcpclient = tcpserver.AcceptTcpClient();
            IPEndPoint remote = (IPEndPoint)tcpclient.Client.RemoteEndPoint;
            string ip = remote.Address.ToString();
            Debug.Log("Connected " + ip);
            tcpclient.ReceiveTimeout = 100;
            NetworkStream stream = tcpclient.GetStream();
            streams.Add(stream);
            Thread thread = new Thread(ThreadProcess);
            thread.Start();
            threads.Add(thread);
            while (ListenFlag)
            {
                try
                {
                    byte[] bytes = new byte[tcpclient.ReceiveBufferSize];
                    stream.Read(bytes, 0, bytes.Length);
                    string data = Encoding.UTF8.GetString(bytes);
                    SendMessage(data);
                }
                catch (Exception) { }
                if (tcpclient.Client.Poll(1000, SelectMode.SelectRead) && tcpclient.Client.Available == 0) break;
            }
            stream.Close();
            stream = null;
            tcpclient.Close();
            Debug.Log("Disconnected " + ip);
        }
#endif
    }
}
