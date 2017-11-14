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
    public class UdpNetworkListenManager
    {
        public delegate void UdpNetworkListenEventHandler(string data,string address);
        public UdpNetworkListenEventHandler UdpNetworkListenEvent;
#if UNITY_UWP
        private DatagramSocket socket = null;
#elif UNITY_EDITOR || UNITY_STANDALONE
        private Thread thread = null;
        private bool ListenFlag = false;
#endif

        public UdpNetworkListenManager(int port)
        {
#if UNITY_UWP
            Task.Run(async () =>
             {
                socket = new DatagramSocket();
                socket.MessageReceived += MessageReceived;
                await socket.BindServiceNameAsync(port.ToString());
             });
#elif UNITY_EDITOR || UNITY_STANDALONE
            thread = new Thread(()=> {
                ListenFlag = true;
                UdpClient udpclient = new UdpClient(port);
                udpclient.Client.ReceiveTimeout = 100;
                IPEndPoint remote = new IPEndPoint(IPAddress.Any, port);
                while (ListenFlag)
                {
                    try
                    {
                        byte[] bytes = udpclient.Receive(ref remote);
                        if (UdpNetworkListenEvent != null) UdpNetworkListenEvent(Encoding.UTF8.GetString(bytes), remote.Address.ToString());
                    }
                    catch (Exception) { }
                }
                udpclient.Close();
            });
            thread.Start();
#endif
        }

        public void DeleteManager()
        {
#if UNITY_UWP
            socket.MessageReceived -= MessageReceived;
#elif UNITY_EDITOR || UNITY_STANDALONE
            ListenFlag = false;
            if (thread != null) thread.Abort();
#endif
        }

#if UNITY_UWP
        async void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            StreamReader reader = new StreamReader(args.GetDataStream().AsStreamForRead());
            string data = await reader.ReadLineAsync();
            if (UdpNetworkListenEvent != null) UdpNetworkListenEvent(data, args.RemoteAddress.DisplayName);
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
    }
}
