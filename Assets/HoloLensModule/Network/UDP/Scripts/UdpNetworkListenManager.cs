using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text;
#endif

namespace HoloLensModule.Network
{
    public class UdpNetworkListenManager : MonoBehaviour
    {
        public int port =1234;
        [SerializeField]
        private bool OnAwake = true;

        public delegate void UdpNetworkListenEventHandler(string data);
        public UdpNetworkListenEventHandler UdpNetworkListenEvent;

#if UNITY_UWP
        private Task task = null;
        private DatagramSocket socket;
#elif UNITY_EDITOR || UNITY_STANDALONE
        private Thread thread = null;
        private UdpClient udpclient;
        private IPEndPoint remote;
#endif
        private bool ListenFlag = false;

        // Use this for initialization
        void Start()
        {
            if (OnAwake) StartListener();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDestroy()
        {
            StopListener();
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (thread != null) thread.Abort();
#endif
        }

        public void StartListener()
        {
            ListenFlag = true;
#if UNITY_UWP
            task = new Task(TaskProcess);
            task.Start();
#elif UNITY_EDITOR || UNITY_STANDALONE
            udpclient = new UdpClient(port);
            udpclient.Client.ReceiveTimeout = 1000;
            remote = new IPEndPoint(IPAddress.Any, port);

            thread = new Thread(ThreadProcess);
            thread.Start();
#endif
        }

        public void StopListener()
        {
            ListenFlag = false;
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            udpclient.Close();
#endif
        }

#if UNITY_UWP
        private string error = "";
        async void TaskProcess()
        {
            socket = new DatagramSocket();
            socket.MessageReceived += MessageReceived;
            try
            {
                await socket.BindServiceNameAsync(port.ToString());
            }
            catch (Exception e)
            {
                error = e.ToString();
            }
        }

        async void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            Stream streamIn = args.GetDataStream().AsStreamForRead();
            StreamReader reader = new StreamReader(streamIn);
            string data = await reader.ReadLineAsync();
            if (UdpNetworkListenEvent != null) UdpNetworkListenEvent(data);
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ThreadProcess()
        {
            Debug.Log("UDP Server Start");
            while (ListenFlag)
            {
                try
                {
                    byte[] bytes = udpclient.Receive(ref remote);
                    string data = Encoding.UTF8.GetString(bytes);
                    if (UdpNetworkListenEvent != null) UdpNetworkListenEvent(data);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
            
        }
#endif
    }
}
