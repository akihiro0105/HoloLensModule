using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net.Sockets;
using System.Text;
using System.Threading;
#endif
using UnityEngine;

namespace HoloLensModule.Network
{
    public class TCPClientManager
    {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private Thread mainthread = null;
        private Thread sendthread = null;
        private TcpClient tcpclient = null;
        private struct ConnectStrings
        {
            public string ip;
            public int port;
        }
#endif
        public TCPClientManager() { }

        public TCPClientManager(string ipaddress, int port)
        {
            ConnectClient(ipaddress, port);
        }

        public void ConnectClient(string ipaddress, int port)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (mainthread == null)
            {
                ConnectStrings connect = new ConnectStrings();
                connect.ip = ipaddress;
                connect.port = port;
                mainthread = new Thread(new ParameterizedThreadStart(ClientThread));
                mainthread.Start(connect);
            }
#endif
        }

        public bool SendMessage(string ms)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(ms);
            return SendMessage(bytes);
        }

        public bool SendMessage(byte[] data)
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            return true;
#endif
            return false;
        }


        public void DisConnectClient()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (mainthread != null)
            {
                mainthread.Abort();
                mainthread = null;
            }
#endif
        }
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ClientThread(object obj)
        {
            var connect = (ConnectStrings)obj;
            if (tcpclient == null)
            {
                tcpclient = new TcpClient();
                tcpclient.Connect(connect.ip, connect.port);
                //
                //
                tcpclient.Close();
                tcpclient = null;
            }
        }
#endif
    }
}
