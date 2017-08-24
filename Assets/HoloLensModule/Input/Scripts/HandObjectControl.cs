using UnityEngine;
using UnityEngine.UI;

namespace HoloLensModule.Input
{
    // Press時のパラメータ画像の表示
    public class HandObjectControl : MonoBehaviour
    {

        public Image PressImage;
        public Image ShiftPressImage;

        private bool pressflag = false;
        private bool shiftflag = false;
        private float filltime;
        private float t = 0;
        // Use this for initialization
        void Start()
        {
            ShiftPressImage.fillAmount = 0;
            PressImage.fillAmount = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (pressflag)
            {
                t += Time.deltaTime;
                if (shiftflag) ShiftPressImage.fillAmount = t / filltime;
                else PressImage.fillAmount = t / filltime;
            }
        }

        public void onPressed(bool shiftflag, float filltime)
        {
            this.shiftflag = shiftflag;
            this.filltime = filltime;
            t = 0;
            pressflag = true;
        }

        public void onReleased()
        {
            pressflag = false;
            ShiftPressImage.fillAmount = 0;
            PressImage.fillAmount = 0;
        }
    }
}
