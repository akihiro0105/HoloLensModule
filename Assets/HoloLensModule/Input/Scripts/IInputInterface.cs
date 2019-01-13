using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    /// <summary>
    /// Input入力に対するInterface
    /// </summary>
    public interface IClickGestureInterface
    {
        void OnClick();
    }

    public interface IDragGestureInterface
    {
        void OnStartDrag(Vector3 pos, Quaternion? rot = null);
        void OnUpdateDrag(Vector3 pos, Quaternion? rot = null);
    }
}