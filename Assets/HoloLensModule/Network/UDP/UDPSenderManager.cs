using System;
using System.Text;
#if WINDOWS_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
#else
using System.Threading;
using System.Net.Sockets;
#endif

namespace HoloLensModule.Network
{
    /// <summary>
    /// UDP送信
    /// </summary>
    public class UDPSenderManager
    {

#if WINDOWS_UWP
        private StreamWriter writer = null;
        private Task task = null;
#else
        private Thread thread = null;
        private UdpClient udpclient = null;
#endif

        public UDPSenderManager()
        {
        }

        public UDPSenderManager(string ipaddress, int port)
        {
            ConnectSender(ipaddress, port);
        }

        public void ConnectSender(string ipaddress, int port)
        {
#if WINDOWS_UWP
            task = Task.Run(async () =>
               {
                   DatagramSocket socket = new DatagramSocket();
                   var datagram = await socket.GetOutputStreamAsync(new HostName(ipaddress), port.ToString());
                   writer = new StreamWriter(datagram.AsStreamForWrite());
               });
#else
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
            return SendMessage(Encoding.UTF8.GetBytes(ms));
        }

        public bool SendMessage(byte[] data)
        {
#if WINDOWS_UWP
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
#else
            if (thread == null || thread.ThreadState != ThreadState.Running)
            {
                thread = new Thread(() => udpclient.Send(data, data.Length));
                thread.Start();
                return true;
            }
#endif
            return false;
        }

        public void DisConnectSender()
        {
#if WINDOWS_UWP
            task = null;
#else
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
