using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    /// <summary>
    /// Input入力に対するInterface
    /// </summary>
    public interface IClickInterface
    {
        void OnClick();
    }

    public interface IDragInterface
    {
        void StartDrag(Vector3 pos, Quaternion? rot = null);
        void UpdateDrag(Vector3 pos, Quaternion? rot = null);
    }
}