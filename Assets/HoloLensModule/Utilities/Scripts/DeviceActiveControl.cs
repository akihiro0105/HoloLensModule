using UnityEngine;

namespace HoloLensModule
{
    // HoloLens or DeskTopのみでアクティブにする
    public class DeviceActiveControl : MonoBehaviour
    {
        public enum ActiveDeviceModel
        {
            Standalone_or_Editor,
            MRDevice
        }
        [SerializeField]
        public ActiveDeviceModel ActiveDevice = ActiveDeviceModel.Standalone_or_Editor;

        // Use this for initialization
        void Start()
        {
#if UNITY_UWP
            if (ActiveDevice == ActiveDeviceModel.MRDevice) gameObject.SetActive(true);
            else gameObject.SetActive(false);
#elif UNITY_EDITOR || UNITY_STANDALONE
            if (ActiveDevice == ActiveDeviceModel.Standalone_or_Editor) gameObject.SetActive(true);
            else gameObject.SetActive(false);
#endif
        }
    }
}
