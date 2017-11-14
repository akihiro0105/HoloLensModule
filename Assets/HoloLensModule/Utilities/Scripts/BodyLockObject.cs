using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLensModule
{
    public class BodyLockObject : MonoBehaviour
    {
        public float MoveSpeed = 2.0f;
        public float MoveDistance = 1.0f;

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(Camera.main.transform.position);
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + Camera.main.transform.forward * MoveDistance, Time.deltaTime * MoveSpeed);
        }
    }
}
