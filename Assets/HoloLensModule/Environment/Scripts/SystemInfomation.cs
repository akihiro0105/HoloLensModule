using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
#if UNITY_UWP
using Windows.Networking.Connectivity;
using Windows.System.Diagnostics;
#endif

namespace HoloLensModule.Environment
{
    public class SystemInfomation
    {
        // * アプリ終了
        // * デバイス名 string
        // * ネットワーク情報 ip subnetmask directedbroadcastaddress
        // * バッテリー残量 %
        // * Wifi情報(UWP)
        // * アプリメモリ使用量
        // bluetooth情報
        // マスター音量 %
        // ディスプレイ輝度 %

        public static void AppExit()
        {
            Application.Quit();
#if UNITY_UWP
            Windows.ApplicationModel.Core.CoreApplication.Exit();
#endif
        }

        public static string DeviceName
        {
            get
            {
                return SystemInfo.deviceName;
            }
        }

        public static string IPAddress
        {
            get
            {
                string ipaddress = "";
#if UNITY_UWP
                var host = NetworkInformation.GetHostNames();
                foreach (var item in host)
                {
                    if (item.Type == Windows.Networking.HostNameType.Ipv4 && item.IPInformation != null)
                    {
                        if (item.DisplayName.IndexOf("172.") == -1)
                        {
                            ipaddress = item.DisplayName;
                        }
                    }
                }
#elif UNITY_EDITOR || UNITY_STANDALONE
                string hostname = Dns.GetHostName();
                var address = Dns.GetHostAddresses(hostname);
                foreach (var item in address)
                {
                    if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !item.ToString().StartsWith("172."))
                    {
                        ipaddress = item.ToString();
                    }
                }
                //ipaddress = UnityEngine.Network.player.ipAddress;
#endif
                return ipaddress;
            }
        }

        public static string Subnetmask
        {
            get
            {
                string subnetmask = "";
#if UNITY_UWP
                var host = NetworkInformation.GetHostNames();
                foreach (var item in host)
                {
                    if (item.Type == Windows.Networking.HostNameType.Ipv4 && item.IPInformation != null)
                    {
                        if (item.DisplayName.IndexOf("172.") == -1)
                        {
                            byte length = item.IPInformation.PrefixLength.Value;
                            BitArray bit = new BitArray(32, false);
                            for (int i = 0; i < length; i++)
                            {
                                bit[i] = true;
                            }
                            byte[] c1 = new byte[4];
                            ((ICollection)bit).CopyTo(c1, 0);
                            subnetmask = c1[0].ToString() + "." + c1[1].ToString() + "." + c1[2].ToString() + "." + c1[3].ToString();
                        }
                    }
                }
#elif UNITY_EDITOR || UNITY_STANDALONE
            var info = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            foreach (var item in info)
            {
                if (item.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up &&
                    item.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback &&
                    item.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Tunnel)
                {
                    var ips = item.GetIPProperties();
                    if (ips != null)
                    {
                        foreach (var ip in ips.UnicastAddresses)
                        {
                            if (IPAddress == ip.Address.ToString())
                            {
                                subnetmask = ip.IPv4Mask.ToString();
                            }
                        }
                    }
                }
            }
#endif
                return subnetmask;
            }
        }

        public static string DirectedBroadcastAddress
        {
            get
            {
                byte[] ipb = new byte[4];
                var ips = IPAddress.Split('.');
                string address = "";
                if (ips != null && ips.Length == 4)
                {
                    var masks = Subnetmask.Split('.');
                    for (int i = 0; i < 4; i++)
                    {
                        ipb[i] = (byte)(byte.Parse(ips[i]) | ~byte.Parse(masks[i]));
                    }
                    address = ipb[0].ToString() + "." + ipb[1].ToString() + "." + ipb[2].ToString() + "." + ipb[3].ToString();
                }
                return address;
            }
        }

        public static int PowerLevel
        {
            get
            {
                return (int)(SystemInfo.batteryLevel * 100.0f);
            }
        }

        public static string WifiSSID
        {
            get
            {
                string ssid = "";
#if UNITY_UWP
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile.IsWlanConnectionProfile==true)
                {
                    ssid = profile.WlanConnectionProfileDetails.GetConnectedSsid();
                }
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
                return ssid;
            }
        }

        public static byte? WifiSignalBars
        {
            get
            {
                byte? signal = null;
#if UNITY_UWP
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile.IsWlanConnectionProfile == true)
                {
                    signal = profile.GetSignalBars();
                }
#elif UNITY_EDITOR || UNITY_STANDALONE
#endif
                return signal;
            }
        }

        public static long WorkingSetMemory
        {
            get
            {
                long memory = 0;
#if UNITY_UWP
                memory = (long)ProcessDiagnosticInfo.GetForCurrentProcess().MemoryUsage.GetReport().WorkingSetSizeInBytes;
#elif UNITY_EDITOR || UNITY_STANDALONE
                memory = System.Environment.WorkingSet;
#endif
                return memory;
            }
        }
    }
}
