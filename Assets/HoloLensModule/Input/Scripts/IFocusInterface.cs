using System.Collections;
using System.Collections.Generic;

namespace HoloLensModule.Input
{
    /// <summary>
    /// Raycastの動作に対するInterface
    /// </summary>
    public interface IFocusInterface
    {
        void RaycastHit();
        void RaycastOut();
    }
}
