using System;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Text;
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
        private StreamWriter writer = null;
#elif UNITY_EDITOR || UNITY_STANDALONE
        private NetworkStream stream = null;
#endif
        private bool ConnectFlag = false;

        public TcpNetworkClientManager(string IP,int port)
        {
            ConnectFlag = true;
#if UNITY_UWP
            Task.Run(async () => {
                StreamSocket socket = new StreamSocket();
                await socket.ConnectAsync(new HostName(IP),port.ToString());
                writer = new StreamWriter(socket.OutputStream.AsStreamForWrite());
                StreamReader reader = new StreamReader(socket.InputStream.AsStreamForRead());
                reader.BaseStream.ReadTimeout = 100;
                if (ConnectMessage != null) ConnectMessage();
                while (ConnectFlag)
                {
                    try
                    {
                        string data = await reader.ReadToEndAsync();
                        if (ReceiveMessage != null) ReceiveMessage(data);
                    }
                    catch (Exception) { }
                }
                if (DisconnectMessage != null) DisconnectMessage();
                if(writer!=null) writer.Dispose();
                writer = null;
            });
#elif UNITY_EDITOR || UNITY_STANDALONE
            Thread thread = new Thread(()=> {
                TcpClient tcp = new TcpClient(IP, port);
                tcp.ReceiveTimeout = 100;
                stream = tcp.GetStream();
                if (ConnectMessage != null) ConnectMessage();
                while (ConnectFlag)
                {
                    try
                    {
                        byte[] bytes = new byte[tcp.ReceiveBufferSize];
                        stream.Read(bytes, 0, bytes.Length);
                        if (ReceiveMessage != null) ReceiveMessage(Encoding.UTF8.GetString(bytes));
                    }
                    catch (Exception) { }
                }
                stream.Close();
                tcp.Close();
                if (DisconnectMessage != null) DisconnectMessage();
            });
            thread.Start();
#endif
        }

        public void DeleteManager()
        {
            ConnectFlag = false;
#if UNITY_UWP
            if (writer != null) writer.Dispose();
            writer = null;
#elif UNITY_EDITOR || UNITY_STANDALONE
            stream = null;
#endif
        }

        public void SendMessage(string data)
        {
#if UNITY_UWP
            if (writer != null) Task.Run(async () =>
             {
                 await writer.WriteAsync(data);
                 await writer.FlushAsync();
             });
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (stream != null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                Thread sendthread = new Thread(()=> { stream.Write(bytes, 0, bytes.Length); });
                sendthread.Start();
            }
#endif
        }
    }
}
