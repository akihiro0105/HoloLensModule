using System;
#if UNITY_UWP
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
#endif

namespace HoloLensModule.Network
{
    public class UdpNetworkClientManager
    {
#if UNITY_UWP
        private StreamWriter writer=null;
#elif UNITY_EDITOR || UNITY_STANDALONE
        private UdpClient udpclient;
#endif

        public UdpNetworkClientManager(int port)
        {
#if UNITY_UWP
            Task.Run(async () =>
             {
                 DatagramSocket socket = new DatagramSocket();
                 HostName hostname = new HostName(IPAddress.Broadcast.ToString());
                Stream streamOut = (await socket.GetOutputStreamAsync(hostname, port.ToString())).AsStreamForWrite();
                writer=new StreamWriter(streamOut);
             });
#elif UNITY_EDITOR || UNITY_STANDALONE
            udpclient = new UdpClient();
            udpclient.EnableBroadcast = true;
            udpclient.Connect(new IPEndPoint(IPAddress.Broadcast, port));
#endif
        }

        public void DeleteManager()
        {
#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
            udpclient.Close();
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
            Thread thread = new Thread(new ParameterizedThreadStart(ThreadProcess));
            thread.Start(data);
#endif
        }

#if UNITY_UWP
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ThreadProcess(object obj)
        {
            string data = (string)obj;
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            udpclient.Send(bytes, bytes.Length);
        }
#endif
    }
}
