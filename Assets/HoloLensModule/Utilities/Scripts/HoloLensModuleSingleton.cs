using UnityEngine;

namespace HoloLensModule
{
    // HoloLensModule用のシングルトン
    public class HoloLensModuleSingleton<T> : MonoBehaviour where T:HoloLensModuleSingleton<T>
    {
        private static T instance;

        public static T Instance
        {
            get { return instance; }
        }

        public static bool IsInitialized
        {
            get { return instance != null; }
        }

        protected virtual void Awake()
        {
            if (instance != null) Debug.LogErrorFormat("Trying to instantiate a second instance of singleton class {0}", GetType().Name);
            else instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            if (instance == this) instance = null;
        }
    }
}
