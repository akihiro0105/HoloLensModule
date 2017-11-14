using UnityEngine;
#if UNITY_2017_2_OR_NEWER
#if UNITY_UWP
using UnityEngine.XR.WSA;
#endif
#endif

namespace HoloLensModule
{
    // HoloLens or DeskTopのみでアクティブにする
    public class DeviceActiveControl : MonoBehaviour
    {
        public enum ActiveDeviceModel
        {
            Standalone_or_Editor,
#if UNITY_2017_2_OR_NEWER
            ImmersiveDevice,
#endif
            MRDevice
        }
        [SerializeField]
        public ActiveDeviceModel ActiveDevice = ActiveDeviceModel.Standalone_or_Editor;

        // Use this for initialization
        void Start()
        {
#if UNITY_UWP
#if UNITY_2017_2_OR_NEWER
            if (HolographicSettings.IsDisplayOpaque) gameObject.SetActive((ActiveDevice == ActiveDeviceModel.ImmersiveDevice) ? true : false);
            else gameObject.SetActive((ActiveDevice == ActiveDeviceModel.MRDevice) ? true : false);
#else
            gameObject.SetActive((ActiveDevice == ActiveDeviceModel.MRDevice) ? true : false);
#endif
#elif UNITY_EDITOR || UNITY_STANDALONE
            gameObject.SetActive((ActiveDevice == ActiveDeviceModel.Standalone_or_Editor) ? true : false);
#endif
        }
    }
}
