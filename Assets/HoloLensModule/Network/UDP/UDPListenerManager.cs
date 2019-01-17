using System;
#if WINDOWS_UWP
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

#if WINDOWS_UWP
        private Task task = null;
        private DatagramSocket socket = null;
#else
        private UdpClient udpclient = null;
#endif

        public UDPListenerManager()
        {
        }

        public UDPListenerManager(int port)
        {
            ConnectListener(port);
        }

        public void ConnectListener(int port)
        {
#if WINDOWS_UWP
            if (task==null)
            {
                task = Task.Run(async () => {
                    socket = new DatagramSocket();
                    socket.MessageReceived += MessageReceived;
                    await socket.BindServiceNameAsync(port.ToString());
                });
            }
#else
            udpclient = new UdpClient(port);
            udpclient.BeginReceive(new AsyncCallback(ReceiveCallback), udpclient);
#endif
        }

#if WINDOWS_UWP
        async void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            if (ListenerMessageEvent != null)
            {
                var reader = new StreamReader(args.GetDataStream().AsStreamForRead());
                var data = await reader.ReadLineAsync();
                ListenerMessageEvent(data, args.RemoteAddress.DisplayName);
            }

            if (ListenerByteEvent != null)
            {
                var readData = args.GetDataReader();
                var byteData = new byte[readData.UnconsumedBufferLength];
                readData.ReadBytes(byteData);
                ListenerByteEvent(byteData, args.RemoteAddress.DisplayName);
            }
        }
#else
        private void ReceiveCallback(IAsyncResult result)
        {
            var udp = (UdpClient) result.AsyncState;
            IPEndPoint remote = null;
            var bytes = udp.EndReceive(result, ref remote);
            if (ListenerMessageEvent != null)
                ListenerMessageEvent(Encoding.UTF8.GetString(bytes), remote.Address.ToString());
            if (ListenerByteEvent != null) ListenerByteEvent(bytes, remote.Address.ToString());
            udp.BeginReceive(ReceiveCallback, udp);
        }
#endif

        public void DisConnectListener()
        {
#if WINDOWS_UWP
            if (socket != null)
            {
                socket.MessageReceived -= MessageReceived;
                socket.Dispose();
                socket = null;
                task = null;
            }
#else
            if (udpclient != null)
            {
                udpclient.Close();
                udpclient = null;
            }
#endif
        }
    }
}
