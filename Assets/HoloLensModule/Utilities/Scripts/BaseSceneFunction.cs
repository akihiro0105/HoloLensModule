using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule
{
    // Scene遷移後も保持しておきたいオブジェクトを登録
    public class BaseSceneFunction : HoloLensModuleSingleton<BaseSceneFunction>
    {
        public List<GameObject> BaseObjects = new List<GameObject>();// BaseSceneFunction以下のオブジェクト以外は登録

        private static bool createFlag = false;

        protected override void Awake()
        {
            if (createFlag == false)
            {
                DontDestroyOnLoad(gameObject);
                for (int i = 0; i < BaseObjects.Count; i++) DontDestroyOnLoad(BaseObjects[i]);
                createFlag = true;
            }
            else
            {
                Destroy(gameObject);
                for (int i = 0; i < BaseObjects.Count; i++) Destroy(BaseObjects[i]);
            }
        }

        //アプリの強制終了
        public void Exit()
        {
            Application.Quit();
#if UNITY_UWP
            Windows.ApplicationModel.Core.CoreApplication.Exit();
#endif
        }
    }
}
