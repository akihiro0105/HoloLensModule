using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Utility
{
    /// <summary>
    /// 対象オブジェクトをカメラに少し遅れて追従させる
    /// </summary>
    public class Bodylocked : MonoBehaviour
    {
        /// <summary>
        /// カメラを追いかける速度
        /// </summary>
        [SerializeField] private float speed = 2.0f;
        /// <summary>
        /// カメラとの距離
        /// </summary>
        [SerializeField] private float distance = 1.0f;
        /// <summary>
        /// カメラ移動時の対象オブジェクトのマージン
        /// </summary>
        [SerializeField] private float margin = 0.0f;
        /// <summary>
        /// 常に対象オブジェクトをY軸を上に向ける
        /// </summary>
        [SerializeField] private bool fixUp = false;

        // Update is called once per frame
        void Update()
        {
            var lookatpos = Camera.main.transform.position;
            lookatpos.y = (fixUp) ? transform.position.y : lookatpos.y;
            transform.LookAt(lookatpos);
            var targetpos = Camera.main.transform.position + Camera.main.transform.forward * distance;
            if (margin < Vector3.Distance(transform.position, targetpos))
            {
                transform.position = Vector3.Lerp(transform.position, targetpos, Time.deltaTime * speed);
            }
        }
    }
}
