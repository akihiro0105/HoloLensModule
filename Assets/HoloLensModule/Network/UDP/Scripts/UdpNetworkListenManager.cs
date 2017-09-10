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
        private DatagramSocket socket=null;
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
            ListenFlag = true;
            thread = new Thread(new ParameterizedThreadStart(ThreadProcess));
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
            Stream streamIn = args.GetDataStream().AsStreamForRead();
            StreamReader reader = new StreamReader(streamIn);
            string data = await reader.ReadLineAsync();
            if (UdpNetworkListenEvent != null) UdpNetworkListenEvent(data, args.RemoteAddress.DisplayName);
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
        private void ThreadProcess(object obj)
        {
            int port = (int)obj;
            UdpClient udpclient = new UdpClient(port);
            udpclient.Client.ReceiveTimeout = 100;
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, port);
            while (ListenFlag)
            {
                try
                {
                    byte[] bytes = udpclient.Receive(ref remote);
                    string data = Encoding.UTF8.GetString(bytes);
                    if (UdpNetworkListenEvent != null) UdpNetworkListenEvent(data, remote.Address.ToString());
                }
                catch (Exception) { }
            }
            udpclient.Close();
        }
#endif
    }
}
