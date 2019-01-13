using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
#if WINDOWS_UWP
using System;
using Windows.Networking.Connectivity;
using Windows.System.Diagnostics;
#endif

namespace HoloLensModule.Environment
{
    /// <summary>
    /// デバイス情報に関するクラス
    /// </summary>
    public class SystemInfomation
    {
        /// <summary>
        /// アプリの終了
        /// </summary>
        public static void AppExit()
        {
            Application.Quit();
#if WINDOWS_UWP
            Windows.ApplicationModel.Core.CoreApplication.Exit();
#endif
        }

        /// <summary>
        /// デバイス名
        /// </summary>
        public static string DeviceName
        {
            get
            {
                return SystemInfo.deviceName;
            }
        }

        /// <summary>
        /// インターネットに接続しているIPアドレス
        /// </summary>
        public static string IPAddress
        {
            get
            {
                string ipaddress = "";
#if WINDOWS_UWP
                foreach (var item in NetworkInformation.GetHostNames())
                {
                    if (item.Type == Windows.Networking.HostNameType.Ipv4 && item.IPInformation != null)
                    {
                        if (item.DisplayName.IndexOf("172.") == -1)ipaddress = item.DisplayName;
                    }
                }
#else
                foreach (var item in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (!item.ToString().StartsWith("172.")) ipaddress = item.ToString();
                    }
                }
#endif
                return ipaddress;
            }
        }

        /// <summary>
        /// インターネットに接続しているサブネットマスク
        /// </summary>
        public static string Subnetmask
        {
            get
            {
                string subnetmask = "";
#if WINDOWS_UWP
                foreach (var item in NetworkInformation.GetHostNames())
                {
                    if (item.Type == Windows.Networking.HostNameType.Ipv4 && item.IPInformation != null)
                    {
                        if (item.DisplayName.IndexOf("172.") == -1)
                        {
                            var bit = new BitArray(32, false);
                            for (int i = 0; i < item.IPInformation.PrefixLength.Value; i++)bit[i] = true;
                            var c1 = new byte[4];
                            ((ICollection)bit).CopyTo(c1, 0);
                            subnetmask = c1[0].ToString() + "." + c1[1].ToString() + "." + c1[2].ToString() + "." + c1[3].ToString();
                        }
                    }
                }
#else
                foreach (var item in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (item.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                        item.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                        item.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                    {
                        var ips = item.GetIPProperties();
                        if (ips != null)
                        {
                            var ipAddress = IPAddress;
                            foreach (var ip in ips.UnicastAddresses)
                            {
                                if (ipAddress == ip.Address.ToString()) subnetmask = ip.IPv4Mask.ToString();
                            }
                        }
                    }
                }
#endif
                return subnetmask;
            }
        }

        /// <summary>
        /// ディレクティッドブロードキャストアドレス
        /// </summary>
        public static string DirectedBroadcastAddress
        {
            get
            {
                var ips = IPAddress.Split('.');
                string address = "";
                if (ips.Length == 4)
                {
                    var masks = Subnetmask.Split('.');
                    var ipb = new byte[4];
                    for (var i = 0; i < ipb.Length; i++) ipb[i] = (byte) (byte.Parse(ips[i]) | ~byte.Parse(masks[i]));
                    address = ipb[0].ToString() + "." + ipb[1].ToString() + "." + ipb[2].ToString() + "." + ipb[3].ToString();
                }
                return address;
            }
        }

        /// <summary>
        /// バッテリー残量
        /// </summary>
        public static int PowerLevel
        {
            get
            {
                return (int)(SystemInfo.batteryLevel * 100.0f);
            }
        }

        /// <summary>
        /// 接続中のWifiのSSID
        /// </summary>
        public static string WifiSSID
        {
            get
            {
                string ssid = "";
#if WINDOWS_UWP
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile.IsWlanConnectionProfile==true)ssid = profile.WlanConnectionProfileDetails.GetConnectedSsid();
#endif
                return ssid;
            }
        }

        /// <summary>
        /// 接続中のWifiの電波強度
        /// </summary>
        public static byte? WifiSignalBars
        {
            get
            {
                byte? signal = null;
#if WINDOWS_UWP
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile.IsWlanConnectionProfile == true)signal = profile.GetSignalBars();
#endif
                return signal;
            }
        }

        /// <summary>
        /// 使用メモリ
        /// </summary>
        public static long WorkingSetMemory
        {
            get
            {
                long memory = 0;
#if WINDOWS_UWP
                memory = (long)ProcessDiagnosticInfo.GetForCurrentProcess().MemoryUsage.GetReport().WorkingSetSizeInBytes;
#else
                memory = System.Environment.WorkingSet;
#endif
                return memory;
            }
        }
    }
}
