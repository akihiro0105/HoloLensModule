using System;
using System.Collections.Generic;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#elif UNITY_EDITOR || UNITY_STANDALONE
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Text;
#endif

namespace HoloLensModule.Network
{
    public class TcpNetworkServerManager
    {

#if UNITY_UWP
        private StreamSocketListener streamsocketlistener;
        private List<StreamWriter> writer = new List<StreamWriter>();
#elif UNITY_EDITOR || UNITY_STANDALONE
        private List<NetworkStream> streams = new List<NetworkStream>();
#endif
        private bool ListenFlag = false;

        public TcpNetworkServerManager(int port)
        {
            ListenFlag = true;
#if UNITY_UWP
            Task.Run(async()=>{
                streamsocketlistener = new StreamSocketListener();
                streamsocketlistener.ConnectionReceived += ConnectionReceived;
                await streamsocketlistener.BindServiceNameAsync(port.ToString());
            });
#elif UNITY_EDITOR || UNITY_STANDALONE
            TcpListener tcpserver = new TcpListener(IPAddress.Any, port);
            tcpserver.Start();
            Thread listenerthread = new Thread(() =>
            {
                while (ListenFlag)
                {
                    TcpClient tcpclient = tcpserver.AcceptTcpClient();
                    tcpclient.ReceiveTimeout = 100;
                    Thread thread = new Thread(() =>
                    {
                        NetworkStream stream = tcpclient.GetStream();
                        streams.Add(stream);
                        while (ListenFlag)
                        {
                            try
                            {
                                byte[] bytes = new byte[tcpclient.ReceiveBufferSize];
                                stream.Read(bytes, 0, bytes.Length);
                                for (int i = 0; i < streams.Count; i++) if (streams[i].CanWrite) streams[i].Write(bytes, 0, bytes.Length);
                            }
                            catch (Exception) { }
                            if (tcpclient.Client.Poll(1000, SelectMode.SelectRead) && tcpclient.Client.Available == 0) break;
                        }
                        stream.Close();
                        tcpclient.Close();
                    });
                    thread.Start();
                }
                tcpserver.Stop();
            });
            listenerthread.Start();
#endif
        }

        public void DeleteManager()
        {
            ListenFlag = false;
#if UNITY_UWP
            writer.Clear();
            streamsocketlistener.Dispose();
#elif UNITY_EDITOR || UNITY_STANDALONE
            streams.Clear();
#endif
        }

#if UNITY_UWP
        private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            StreamReader reader = new StreamReader(args.Socket.InputStream.AsStreamForRead());
            reader.BaseStream.ReadTimeout = 100;
            writer.Add(new StreamWriter(args.Socket.OutputStream.AsStreamForWrite()));
            while (ListenFlag)
            {
                try
                {
                    string data = await reader.ReadToEndAsync();
                    for (int i = 0; i < writer.Count; i++)
                    {
                        await writer[i].WriteAsync(data);
                        await writer[i].FlushAsync();
                    }
                }
                catch (Exception) { }
            }
        }
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
    }
}
