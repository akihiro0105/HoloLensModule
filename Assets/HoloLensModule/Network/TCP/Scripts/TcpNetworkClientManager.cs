using UnityEngine;
using System;
using System.Text;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Threading;
using System.Net.Sockets;
#endif

namespace HoloLensModule.Network
{
    public class TcpNetworkClientManager
    {
        public delegate void ConnectMessageHandler();
        public delegate void ReceiveMessageHandler(string data);
        public ConnectMessageHandler ConnectMessage;
        public ConnectMessageHandler DisconnectMessage;
        public ReceiveMessageHandler ReceiveMessage;

#if UNITY_UWP
        private Stream streamOut = null;
        private string errorstring = "";
#elif UNITY_EDITOR || UNITY_STANDALONE
        private Thread thread = null;
        private NetworkStream stream = null;
#endif
        private bool ConnectFlag = false;

        public TcpNetworkClientManager(string IP,int port)
        {
            ConnectFlag = true;
#if UNITY_UWP
            Task.Run(async () => {
                StreamSocket socket = new StreamSocket();
                HostName serverhost = new HostName(IP);
                await socket.ConnectAsync(serverhost, port.ToString());
                streamOut = socket.OutputStream.AsStreamForWrite();
                Stream streamIn = socket.InputStream.AsStreamForRead();
                //streamIn.ReadTimeout = 100;
                if (ConnectMessage != null) ConnectMessage();
                while (ConnectFlag)
                {
                    try
                    {
                        byte[] bytes = new byte[2048];
                        await streamIn.ReadAsync(bytes, 0, bytes.Length);
                        string data = Encoding.UTF8.GetString(bytes);
                        if (ReceiveMessage != null) ReceiveMessage(data);
                    }
                    catch (Exception) { }
                }
                if (DisconnectMessage != null) DisconnectMessage();
                if (streamOut != null) streamOut.Dispose();
                streamOut = null;
            });
#elif UNITY_EDITOR || UNITY_STANDALONE
            TcpClient tcp = new TcpClient(IP, port);
            thread = new Thread(new ParameterizedThreadStart(ThreadProcess));
            thread.Start(tcp);
#endif
        }

        public void DeleteManager()
        {
            ConnectFlag = false;
#if UNITY_UWP
            if (streamOut != null) streamOut.Dispose();
            streamOut = null;
#elif UNITY_EDITOR || UNITY_STANDALONE
            thread.Abort();
            stream = null;
            thread = null;
#endif
        }

        public void SendMessage(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
#if UNITY_UWP
            if (streamOut != null) Task.Run(async () =>
             {
                 await streamOut.WriteAsync(bytes, 0, bytes.Length);
                 await streamOut.FlushAsync();
             });
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (stream != null) stream.Write(bytes, 0, bytes.Length);
#endif
        }

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ThreadProcess(object obj)
        {
            TcpClient tcp = (TcpClient)obj;
            tcp.ReceiveTimeout = 100;
            stream = tcp.GetStream();
            Debug.Log("Connect Client");
            if (ConnectMessage != null) ConnectMessage();
            while (ConnectFlag)
            {
                try
                {
                    byte[] bytes = new byte[tcp.ReceiveBufferSize];
                    stream.Read(bytes, 0, bytes.Length);
                    string data = Encoding.UTF8.GetString(bytes);
                    if (ReceiveMessage != null) ReceiveMessage(data);
                }
                catch (Exception) { }
            }
            stream.Close();
            tcp.Close();
            if (DisconnectMessage != null) DisconnectMessage();
        }
#endif
    }
}
