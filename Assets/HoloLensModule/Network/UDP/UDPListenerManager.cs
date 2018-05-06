using System;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
using System.Net.Sockets;
using System.Text;
#endif

namespace HoloLensModule.Network
{
    public class UDPListenerManager
    {
        public delegate void ListenerMessageEventHandler(string ms, string address);
        public ListenerMessageEventHandler ListenerMessageEvent;

        public delegate void ListenerByteEventHandler(byte[] data, string address);
        public ListenerByteEventHandler ListenerByteEvent;

#if UNITY_UWP
        private Task task = null;
        private DatagramSocket socket = null;
#elif UNITY_EDITOR || UNITY_STANDALONE
    private UdpClient udpclient = null;
#endif

        public UDPListenerManager() { }

        public UDPListenerManager(int port)
        {
            ConnectListener(port);
        }

        public void ConnectListener(int port)
        {
#if UNITY_UWP
            if (task==null)
            {
                task = Task.Run(async () => {
                    socket = new DatagramSocket();
                    socket.MessageReceived += MessageReceived;
                    await socket.BindServiceNameAsync(port.ToString());
                });
            }
#elif UNITY_EDITOR || UNITY_STANDALONE
        udpclient = new UdpClient(port);
        udpclient.BeginReceive(new AsyncCallback(ReceiveCallback), udpclient);
#endif
        }

#if UNITY_UWP
        async void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            StreamReader reader = new StreamReader(args.GetDataStream().AsStreamForRead());
            string data = await reader.ReadLineAsync();
            if (ListenerMessageEvent != null) ListenerMessageEvent(data, args.RemoteAddress.DisplayName);
            using (MemoryStream ms = new MemoryStream())
            {
                await reader.BaseStream.CopyToAsync(ms);
                if (ListenerByteEvent != null) ListenerByteEvent(ms.ToArray(), args.RemoteAddress.DisplayName);
            }
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
    private void ReceiveCallback(IAsyncResult result)
    {
        UdpClient udp = (UdpClient)result.AsyncState;
        IPEndPoint remote = null;
        byte[] bytes = udp.EndReceive(result, ref remote);
        if (ListenerMessageEvent != null) ListenerMessageEvent(Encoding.UTF8.GetString(bytes), remote.Address.ToString());
        if (ListenerByteEvent != null) ListenerByteEvent(bytes, remote.Address.ToString());
        udp.BeginReceive(ReceiveCallback, udp);
    }
#endif

        public void DisConnectListener()
        {
#if UNITY_UWP
            if (socket != null)
            {
                socket.MessageReceived -= MessageReceived;
                socket.Dispose();
                socket = null;
                task = null;
            }
#elif UNITY_EDITOR || UNITY_STANDALONE
        if (udpclient != null)
        {
            udpclient.Close();
            udpclient = null;
        }
#endif
        }
    }
}
