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
    public class UDPSenderManager
    {

#if UNITY_UWP
        private StreamWriter writer = null;
        private Task task = null;
#elif UNITY_EDITOR || UNITY_STANDALONE
    private Thread thread = null;
    private UdpClient udpclient = null;
#endif

        public UDPSenderManager() { }

        public UDPSenderManager(string ipaddress, int port)
        {
            ConnectSender(ipaddress, port);
        }

        public void ConnectSender(string ipaddress, int port)
        {
#if UNITY_UWP
            task = Task.Run(async () =>
               {
                   DatagramSocket socket = new DatagramSocket();
                   var datagram = await socket.GetOutputStreamAsync(new HostName(ipaddress), port.ToString());
                   writer = new StreamWriter(datagram.AsStreamForWrite());
               });
#elif UNITY_EDITOR || UNITY_STANDALONE
        if (udpclient == null)
        {
            udpclient = new UdpClient();
            udpclient.EnableBroadcast = true;
            udpclient.Connect(ipaddress, port);
        }
#endif
        }

        public bool SendMessage(string ms)
        {
#if UNITY_UWP
            if (writer != null)
            {
                if (task == null || task.IsCompleted == true)
                {
                    task = Task.Run(async () =>
                    {
                        await writer.WriteAsync(ms);
                        await writer.FlushAsync();
                    });
                    return true;
                }
            }
#elif UNITY_EDITOR || UNITY_STANDALONE
        if (thread == null || thread.ThreadState != ThreadState.Running)
        {
            thread = new Thread(() =>
            {
                byte[] bytes = Encoding.UTF8.GetBytes(ms);
                udpclient.Send(bytes, bytes.Length);
            });
            thread.Start();
            return true;
        }
#endif
            return false;
        }

        public bool SendMessage(byte[] data)
        {
#if UNITY_UWP
            if (writer != null)
            {
                if (task == null || task.IsCompleted == true)
                {
                    task = Task.Run(async () =>
                    {
                        await writer.BaseStream.WriteAsync(data, 0, data.Length);
                        await writer.FlushAsync();
                    });
                    return true;
                }
            }
#elif UNITY_EDITOR || UNITY_STANDALONE
        if (thread == null || thread.ThreadState != ThreadState.Running)
        {
            thread = new Thread(() =>
            {
                udpclient.Send(data, data.Length);
            });
            thread.Start();
            return true;
        }
#endif
            return false;
        }

        public void DisConnectSender()
        {
#if UNITY_UWP
            task = null;
#elif UNITY_EDITOR || UNITY_STANDALONE
        if (udpclient != null)
        {
            udpclient.Close();
            udpclient = null;
        }
        if (thread != null)
        {
            thread.Abort();
            thread = null;
        }
#endif
        }
    }
}
