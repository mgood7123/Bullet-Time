using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class JumpingPhysics : MonoBehaviour
    {
        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }

        private float m_YRotation;

        private Rigidbody m_RigidBody;
        public bool m_PreviouslyGrounded, m_IsGrounded;
        public Vector3 m_GroundContactNormal;
        private CapsuleCollider m_Capsule;
        public AdvancedSettings advancedSettings = new AdvancedSettings();
        public float verticalVelocity = 0f;
        public float gravity = 9.80665f;
        public float jumpForce = 20.0f;
        public float MovementSpeed;
        public float MovementSpeedDefault = 10f;
        public float forces = 10.0f;
        public float drag = 10.0f;
        public CharacterController controller;
        public float slowdownFactor = 0.005f;
        public float Matrix_Time = 0;
        public float Time_Original = 0;
        public bool Matrix = false;
        public bool Matrix_Instant_Time = true;
        public int Matrix_State = 0;

        public void DoSlowmotion()
        {
            Matrix_State = 2;
            Matrix_Time = slowdownFactor;
            Time.timeScale = Matrix_Time;
        }

        public void DoSpeedUp()
        {
            Matrix_State = 3;
            Matrix_Time += (1f / 1f) * Time.unscaledDeltaTime;
            Matrix_Time = Mathf.Clamp(Matrix_Time, 0f, 1f);
            Time.timeScale = Matrix_Time;
            if (Matrix_Time >= Time_Original) Matrix_State = 0;
        }

        public void DoSlowDown()
        {
            Matrix_Time -= (1f / 1f) * Time.unscaledDeltaTime;
            Matrix_Time = Mathf.Clamp(Matrix_Time, 0f, 1f);
            Time.timeScale = Matrix_Time;
            if (Matrix_Time < slowdownFactor) Matrix_State = 1;
        }

        public void Start()
        {
            if (Matrix_Time == 0) Matrix_Time = Time.timeScale;
            if (Time_Original == 0) Time_Original = Time.timeScale;
            MovementSpeed = MovementSpeedDefault;
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            controller = GetComponent<CharacterController>();
        }

        private Vector3 moveDirection = Vector3.zero;

        private void Update()
        {
            Update1();
        }

        private void Update1()
        {
            GroundCheck();
            InteractRaycast();
            if (controller.isGrounded)
            {
                moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= MovementSpeed;
                if (Input.GetButton("Jump")) moveDirection.y = jumpForce / Matrix_Time;
                MovementSpeed = MovementSpeedDefault / Matrix_Time;
            }
            else
            {
                moveDirection.y -= (gravity * Time.fixedUnscaledDeltaTime) / Matrix_Time;
            }
            verticalVelocity = moveDirection.y;
            controller.Move(moveDirection * Time.fixedDeltaTime);
            if (Input.GetButtonDown("Matrix")) Matrix = !Matrix;

            if (Matrix)
            {
                if (Matrix_Instant_Time)
                {
                    if (Matrix_State == 0) DoSlowmotion();
                }
                else
                {
                    if (Matrix_State == 0 || Matrix_State == 3) DoSlowDown();
                    if (Matrix_State == 1) DoSlowmotion();
                }
            }
            else
            {
                if (Matrix_Instant_Time)
                {
                    Matrix_Time = Time_Original;
                    Time.timeScale = Matrix_Time;
                    Matrix_State = 0;
                }
                else DoSpeedUp();
            }
            Time.fixedDeltaTime = Time.timeScale * .02f;
        }

        private Vector2 GetInput()
        {
            Vector2 input = new Vector2
            {
                x = CrossPlatformInputManager.GetAxis("Horizontal"),
                y = CrossPlatformInputManager.GetAxis("Vertical")
            };
            return input;
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
        }

        /*	

        terminal velocity,
        the mass of the falling object (
            In common usage, the mass of an object is often referred to as its weight, though these are in fact different concepts and quantities. In scientific contexts, mass is the amount of "matter" in an object, whereas weight is the force exerted on an object by gravity.

            mass (an intrinsic property of an object)

            vs weight (an object's resistance to deviating from its natural course of free fall, which can be influenced by the nearby gravitational field. No matter how strong the gravitational field, objects in free fall are weightless)
            ), ( no calculation for this )
        the acceleration due to gravity (
            Near Earth's surface, gravitational acceleration is approximately 9.8 m/s2, which means that, ignoring the effects of air resistance, the speed of an object falling freely will increase by about 9.8 metres per second every second
            
            The precise strength of Earth's gravity varies depending on location. The nominal "average" value at Earth's surface, known as standard gravity is, by definition, 9.80665 m/s2

            Gravity decreases with altitude as one rises above the Earth's surface because greater altitude means greater distance from the Earth's centre
            (
                the gravitational acceleration at height h above sea level,
                the Earth's mean radius (
                    Earth radius is the distance from a selected center of Earth to a point on its surface, which is often chosen to be sea level.
                ),
                the standard gravitational acceleration (
                    The standard acceleration due to gravity, sometimes abbreviated as standard gravity, usually denoted by ɡ0 or ɡn, is the nominal gravitational acceleration of an object in a vacuum near the surface of the Earth. It is defined by standard as 9.80665 m/s2.
                )
            )
        ),
        the drag coefficient (
            drag force, which is by definition the force component in the direction of the flow velocity (
                for force:
                    In physics, a force is any interaction that, when unopposed, will change the motion of an object.
                    (numourous types of force and force equations for this)

                for flow velocity:
                    (the drag coefficient -> the flow velocity relative to the object)

                for drag force:
                    (numourous types of drag and drag equations for this)
            ),
            the mass density of the fluid (
                The density, or more precisely, the volumetric mass density, of a substance is its mass per unit volume.
                density = mass (the mass of the falling object) / volume (
                    Volume is the quantity of three-dimensional space enclosed by a closed surface
                    ( calculation dependant of shape of object )
                )
            ),
            the flow velocity relative to the object (
                In continuum mechanics the macroscopic velocity, also flow velocity in fluid dynamics or drift velocity in electromagnetism, is a vector field used to mathematically describe the motion of a continuum.

                flow velocity u = u(t,x)
                    velocity of an element of fluid at position x and time t
                flow speed q = ||u|| (scalar field)
            ),
            the reference area (
                Area is the quantity that expresses the extent of a two-dimensional figure or shape, or planar lamina, in the plane. Surface area is its analog on the two-dimensional surface of a three-dimensional object.
                ( calculation dependant of shape of object )
            )
        ),
        the density of the fluid through which the object is falling (
            the drag coefficient -> the mass density of the fluid
        ),
        the projected area of the object (
            Projected area is two-dimensional area measurement of a three-dimensional object by projecting its shape on to an arbitrary plane.

            The geometrical definition of a projected area is: "the rectilinear parallel projection of a surface of any shape onto a plane"
            ( calculation dependant of shape of object )
        )

        */

        public float drag_calc(float mass_density, float flow_velocity, float reference_area, float drag_coefficient)
        {
            return (1f / 2f) * mass_density * (flow_velocity * flow_velocity) * drag_coefficient * reference_area;
        }

        void InteractRaycast()
        {
            Vector3 playerPosition = transform.position;
            Vector3 forwardDirection = transform.forward;
            Ray InteractionRay = new Ray(playerPosition, forwardDirection);
            RaycastHit InteractionRayHit;
            float InteractionRayLength = 5.0f;

            Vector3 InteractionRayEndpoint = forwardDirection * InteractionRayLength;
            Debug.DrawLine(playerPosition, InteractionRayEndpoint);

            bool hitfound = Physics.Raycast(InteractionRay, out InteractionRayHit, InteractionRayLength);
            if (hitfound)
            {
                GameObject hitGameObject = InteractionRayHit.transform.gameObject;
                string hitFeedBack = hitGameObject.name;
                printfTools.Tools.fprintf(Debug.Log, "FPRINTF raycast hit object with name %s", hitFeedBack);
            }
        }
    }
}
