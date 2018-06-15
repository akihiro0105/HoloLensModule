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
        public delegate void ListenerMessageEventHandler(string ms);
        public ListenerMessageEventHandler ListenerMessageEvent;

        public delegate void ListenerByteEventHandler(byte[] data);
        public ListenerByteEventHandler ListenerByteEvent;

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private Thread sendthread = null;
        private NetworkStream stream = null;
        private bool isActiveThread = false;
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
            TcpClient tcpclient = new TcpClient();
            tcpclient.BeginConnect(ipaddress, port, ConnectCallback, tcpclient);
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
            if (sendthread == null || sendthread.ThreadState != ThreadState.Running)
            {
                if (stream != null)
                {
                    sendthread = new Thread(() => { stream.Write(data, 0, data.Length); });
                    sendthread.Start();
                    return true;
                }
            }
#endif
            return false;
        }


        public void DisConnectClient()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            isActiveThread = false;
            if (sendthread != null)
            {
                sendthread.Abort();
                sendthread = null;
            }
            stream = null;
#endif
        }
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ConnectCallback(IAsyncResult ar)
        {
            TcpClient tcp = (TcpClient)ar.AsyncState;
            tcp.EndConnect(ar);
            tcp.ReceiveTimeout = 100;
            stream = tcp.GetStream();
            isActiveThread = true;
            byte[] bytes = new byte[tcp.ReceiveBufferSize];
            while (isActiveThread)
            {
                try
                {
                    int num = stream.Read(bytes, 0, bytes.Length);
                    if (num > 0)
                    {
                        byte[] data = new byte[num];
                        Array.Copy(bytes, 0, data, 0, num);
                        if (ListenerMessageEvent != null) ListenerMessageEvent(Encoding.UTF8.GetString(data));
                        if (ListenerByteEvent != null) ListenerByteEvent(data);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            stream.Close();
            tcp.Close();
        }
#endif
    }
}
