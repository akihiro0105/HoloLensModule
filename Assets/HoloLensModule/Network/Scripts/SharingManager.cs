using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using HoloLensModule.Environment;
using UnityEngine;

namespace HoloLensModule.Network
{
    /// <summary>
    /// UDPによるSharing機能
    /// shareにSharingしたいオブジェクトを設定すると同一ネットワーク上のオブジェクトの位置と回転を共有します
    /// </summary>
    public class SharingManager : MonoBehaviour
    {
        /// <summary>
        /// Sharing用モデルのTransformリスト
        /// </summary>
        [SerializeField] private Transform[] share;

        /// <summary>
        /// UDP送信機能
        /// </summary>
        private UDPSenderManager sender;

        /// <summary>
        /// UDP受信機能
        /// </summary>
        private UDPListenerManager listener;

        /// <summary>
        /// 比較用データ
        /// </summary>
        private List<JsonTransform> bufList = new List<JsonTransform>();

        // Use this for initialization
        void Start()
        {
            // UnityThreadの保持
            var currentcontext = SynchronizationContext.Current;

            // UDP通信の初期化
            sender = new UDPSenderManager(SystemInfomation.DirectedBroadcastAddress, 8080);
            listener = new UDPListenerManager(8080);
            listener.ListenerMessageEvent += (ms, address) =>
            {
                // 自分以外からの通信を利用
                if (!address.Equals(SystemInfomation.IPAddress))
                {
                    var receive = new JsonMessage();
                    receive = JsonUtility.FromJson<JsonMessage>(ms);
                    // UnityThread内で更新
                    currentcontext.Post(state =>
                    {
                        // 受信データの適応
                        for (var i = 0; i < receive.list.Count; i++)
                        {
                            share[i].position = transform.TransformPoint(receive.list[i].pos);
                            share[i].rotation = transform.rotation * receive.list[i].rot;
                        }

                        // 現在値更新
                        setBufTransform();
                        Debug.Log("receive message");
                    }, null);
                }
            };

            // 現在値更新
            setBufTransform();
        }

        // Update is called once per frame
        void Update()
        {
            // 位置の更新があればデータ送信
            for (var i = 0; i < share.Length; i++)
            {
                if (share[i].position.Equals(bufList[i].pos) && share[i].rotation.Equals(bufList[i].rot)) continue;
                var ms = new JsonMessage();
                for (var j = 0; j < share.Length; j++)
                {
                    var item = new JsonTransform();
                    item.pos = transform.InverseTransformPoint(share[j].position);
                    item.rot = Quaternion.Inverse(transform.rotation) * share[j].rotation;
                    ms.list.Add(item);
                }
                sender.SendMessage(JsonUtility.ToJson(ms));
                Debug.Log("send message");
                break;
            }

            // 現在値更新
            setBufTransform();
        }

        /// <summary>
        /// 比較用データを更新
        /// </summary>
        private void setBufTransform()
        {
            bufList.Clear();
            foreach (var t in share)
            {
                var item = new JsonTransform();
                item.pos = t.position;
                item.rot = t.rotation;
                bufList.Add(item);
            }
        }

        void OnDestroy()
        {
            // UDP通信の解放
            listener.DisConnectListener();
            sender.DisConnectSender();
        }
    }

    /// <summary>
    /// 通信用Transformデータ
    /// </summary>
    [Serializable]
    public class JsonTransform
    {
        public Vector3 pos = new Vector3();
        public Quaternion rot = new Quaternion();
    }

    [Serializable]
    public class JsonMessage
    {
        public List<JsonTransform> list=new List<JsonTransform>();
    }
}
