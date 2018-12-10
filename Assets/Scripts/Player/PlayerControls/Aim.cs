using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    public class Aim : MonoBehaviour
    {
        public Camera cam;
        public MouseLook mouseLook = new MouseLook();

        private float m_YRotation;

        private Rigidbody m_RigidBody;

        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            mouseLook.Init (transform, cam.transform);
        }


        private void Update()
        {
            RotateView();
        }

        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation (transform, cam.transform);

                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation*m_RigidBody.velocity;
        }
    }
}
