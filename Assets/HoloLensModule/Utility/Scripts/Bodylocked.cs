using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule.Utility
{
    public class Bodylocked : MonoBehaviour
    {
        public float MoveSpeed = 2.0f;
        public float MoveDistance = 1.0f;
        public float MarginDistance = 0.0f;
        public bool FixUpFlag = false;

        // Update is called once per frame
        void Update()
        {
            var lookatpos = Camera.main.transform.position;
            lookatpos.y = (FixUpFlag == true) ? transform.position.y : lookatpos.y;
            transform.LookAt(lookatpos);
            var targetpos = Camera.main.transform.position + Camera.main.transform.forward * MoveDistance;
            if (MarginDistance < Vector3.Distance(transform.position, targetpos))
            {
                transform.position = Vector3.Lerp(transform.position, targetpos, Time.deltaTime * MoveSpeed);
            }
        }
    }
}
