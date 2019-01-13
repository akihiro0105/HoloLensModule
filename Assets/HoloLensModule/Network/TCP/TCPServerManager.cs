using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using System.IO;
using System.Diagnostics;
#else
using System.Net.Sockets;
#endif

namespace HoloLensModule.Network
{
    public class TCPServerManager 
    {
#if WINDOWS_UWP
        private StreamSocketListener socketlistener = null;
        private List<StreamWriter> streamList = new List<StreamWriter>();
#else
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
#if WINDOWS_UWP
            Task.Run(async () =>
            {
                socketlistener = new StreamSocketListener();
                socketlistener.ConnectionReceived += ConnectionReceived;
                await socketlistener.BindServiceNameAsync(port.ToString());
            });
#else
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(AcceptTcpClient, tcpListener);
#endif
        }

        public void DisConnectClient()
        {
#if WINDOWS_UWP
            socketlistener.Dispose();
#else
            tcpListener.Stop();
#endif
            isActiveThread = false;
        }

#if WINDOWS_UWP
        private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var reader = new StreamReader(args.Socket.InputStream.AsStreamForRead());
            var writer = new StreamWriter(args.Socket.OutputStream.AsStreamForWrite());
            streamList.Add(writer);
            var bytes = new byte[65536];
            while (isActiveThread)
            {
                try
                {
                    var num = await reader.BaseStream.ReadAsync(bytes, 0, bytes.Length);
                    if (num > 0)
                    {
                        var data = new byte[num];
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
#else
        private void AcceptTcpClient(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;

            var tcpClient = listener.EndAcceptTcpClient(ar);
            listener.BeginAcceptTcpClient(AcceptTcpClient, listener);
            tcpClient.ReceiveTimeout = 100;
            var stream = tcpClient.GetStream();
            streamList.Add(stream);
            var bytes = new byte[tcpClient.ReceiveBufferSize];

            while (isActiveThread)
            {
                try
                {
                    var num = stream.Read(bytes, 0, bytes.Length);
                    if (num > 0)
                    {
                        for (int i = 0; i < streamList.Count; i++)
                        {
                            if (streamList[i].CanWrite == true) streamList[i].Write(bytes, 0, num);
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
