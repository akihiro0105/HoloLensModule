using UnityEngine;

namespace HoloLensModule
{
    // HoloLens or DeskTopのみでアクティブにする
    public class DeviceActiveControl : MonoBehaviour
    {
        [SerializeField]
        public enum ActiveDeviceModel
        {
            Standalone,
            MRDevice
        }
        
        public ActiveDeviceModel ActiveDevice = ActiveDeviceModel.Standalone;

        // Use this for initialization
        void Start()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorUserBuildSettings.activeBuildTarget== UnityEditor.BuildTarget.WSAPlayer)
            {
                gameObject.SetActive((ActiveDevice == ActiveDeviceModel.MRDevice) ? true : false);
            }
            else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.StandaloneWindows64)
            {
                gameObject.SetActive((ActiveDevice == ActiveDeviceModel.Standalone) ? true : false);
            }
#else
            if (Application.platform == RuntimePlatform.WSAPlayerX86 || Application.platform == RuntimePlatform.WSAPlayerX64)
            {
                gameObject.SetActive((ActiveDevice == ActiveDeviceModel.MRDevice) ? true : false);
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                gameObject.SetActive((ActiveDevice == ActiveDeviceModel.Standalone) ? true : false);
            }
#endif
        }
    }
}
