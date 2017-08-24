using UnityEngine;

namespace HoloLensModule
{
    // HoloLens or DeskTopのみでアクティブにする
    public class DeviceActiveControl : MonoBehaviour
    {
        public enum ActiveDeviceModel
        {
            Desktop,
            HoloLens
        }
        [SerializeField]
        public ActiveDeviceModel ActiveDevice = ActiveDeviceModel.Desktop;

        // Use this for initialization
        void Start()
        {
#if UNITY_UWP
            if (ActiveDevice == ActiveDeviceModel.HoloLens) gameObject.SetActive(true);
            else gameObject.SetActive(false);
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (ActiveDevice == ActiveDeviceModel.Desktop) gameObject.SetActive(true);
            else gameObject.SetActive(false);
#endif
        }
    }
}
