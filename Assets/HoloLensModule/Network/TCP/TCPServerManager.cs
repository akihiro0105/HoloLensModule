using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
#if UNITY_UWP
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using System.IO;
using System.Diagnostics;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net.Sockets;
#endif

namespace HoloLensModule.Network
{
    public class TCPServerManager 
    {
#if UNITY_UWP
        private StreamSocketListener socketlistener = null;
        private List<StreamWriter> streamList = new List<StreamWriter>();
#elif UNITY_EDITOR || UNITY_STANDALONE
        private TcpListener tcpListener = null;
        private List<NetworkStream> streamList = new List<NetworkStream>();
#endif
        private bool isActiveThread = true;

        public TCPServerManager() { }

        public TCPServerManager(int port)
        {
            ConnectServer(port);
        }

        public void ConnectServer(int port)
        {
#if UNITY_UWP
            Task.Run(async () =>
            {
                socketlistener = new StreamSocketListener();
                socketlistener.ConnectionReceived += ConnectionReceived;
                await socketlistener.BindServiceNameAsync(port.ToString());
            });
#elif UNITY_EDITOR || UNITY_STANDALONE
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(AcceptTcpClient, tcpListener);
#endif
        }

        public void DisConnectClient()
        {
#if UNITY_UWP
            socketlistener.Dispose();
#elif UNITY_EDITOR || UNITY_STANDALONE
            tcpListener.Stop();
#endif
            isActiveThread = false;
        }

#if UNITY_UWP
        private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StreamReader reader = new StreamReader(args.Socket.InputStream.AsStreamForRead());
            StreamWriter writer = new StreamWriter(args.Socket.OutputStream.AsStreamForWrite());
            streamList.Add(writer);
            byte[] bytes = new byte[65536];
            while (isActiveThread)
            {
                try
                {
                    int num = await reader.BaseStream.ReadAsync(bytes, 0, bytes.Length);
                    if (num > 0)
                    {
                        byte[] data = new byte[num];
                        Array.Copy(bytes, 0, data, 0, num);
                        for (int i = 0; i < streamList.Count; i++)
                        {
                            await streamList[i].BaseStream.WriteAsync(data, 0, data.Length);
                            await streamList[i].FlushAsync();
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.Write(e);
                }
            }
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void AcceptTcpClient(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;

            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(AcceptTcpClient, listener);
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
                            if (streamList[i].CanWrite == true)
                            {
                                streamList[i].Write(bytes, 0, num);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
                if (tcpClient.Client.Poll(1000, SelectMode.SelectRead) && tcpClient.Client.Available == 0) break;
            }
            stream.Close();
            tcpClient.Close();
        }
#endif
    }
}
