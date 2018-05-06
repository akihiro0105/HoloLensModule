using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Input
{
    public class HandControler : MonoBehaviour
    {
        [SerializeField]
        private GameObject HoloLensHandModel;
        [SerializeField]
        private GameObject MotionControlerModel;
        [SerializeField]
        private GameObject HoloLensHandPoint;
        [SerializeField]
        private GameObject MotionControlerPoint;

        public enum HandType
        {
            HAND,
            MOTIONCONTROLER
        }
        private HandType Type = HandType.HAND;

        public void SetHandType(HandType type)
        {
            Type = type;
            HoloLensHandModel.SetActive((Type == HandType.HAND) ? true : false);
            MotionControlerModel.SetActive((Type == HandType.MOTIONCONTROLER) ? true : false);
        }

        public void isView(bool flag)
        {
            if (flag == false)
            {
                HoloLensHandModel.SetActive(false);
                MotionControlerModel.SetActive(false);
            }
            else
            {
                SetHandType(Type);
            }
        }

        public GameObject GetHandPoint()
        {
            return (Type == HandType.HAND) ? HoloLensHandPoint.gameObject : MotionControlerPoint.gameObject;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Type == HandType.HAND)
            {
                transform.LookAt(Camera.main.transform.position);
            }
        }
    }
}
