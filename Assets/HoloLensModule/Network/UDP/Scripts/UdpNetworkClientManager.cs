using System.Net;
#if UNITY_UWP
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Threading;
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
        private Thread thread;
        private UdpClient udpclient;
#endif

        public UdpNetworkClientManager(int port,string address=null)
        {
#if UNITY_UWP
            Task.Run(async () =>
             {
                 DatagramSocket socket = new DatagramSocket();
                 string Address = address;
                 if (address == null) Address = IPAddress.Broadcast.ToString();
                 var datagram = await socket.GetOutputStreamAsync(new HostName(Address), port.ToString());
                 writer = new StreamWriter(datagram.AsStreamForWrite());
             });
#elif UNITY_EDITOR || UNITY_STANDALONE
            udpclient = new UdpClient();
            udpclient.EnableBroadcast = true;
            if(address==null) udpclient.Connect(new IPEndPoint(IPAddress.Broadcast, port));
            else udpclient.Connect(address, port);
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
            thread = new Thread(() =>
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                udpclient.Send(bytes, bytes.Length);
            });
            thread.Start();
#endif
        }
    }
}
