using System;
using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class newjump : MonoBehaviour {

    Rigidbody R;

    public float force = 375f;
    public bool grounded = false;
    public float ColliderEdge;
    CharacterController C;

    // Use this for initialization
    void Start () {
        ColliderEdge = GetColliderEdge();
        R = GetComponent<Rigidbody>();
        C = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
    void Update () {
        grounded = IsGrounded();
        if (grounded) if (Input.GetButton("Jump")) R.AddForce(Vector3.up * force, ForceMode.Impulse);
        printfTools.Tools.fprintf(Debug.Log, "Physics.Raycast(transform.position, hitInfo, ColliderEdge) = %s", grounded?"true":"false");
    }

    float GetColliderEdge()
    {
        if (GetComponent<Collider>().GetType() == typeof(SphereCollider))
        {
            return GetComponent<SphereCollider>().radius;
        }
        return 0f;
    }

    bool IsGrounded()
    {
        RaycastHit hitInfo;
        return Physics.Raycast(transform.position, -Vector3.up, out hitInfo, ColliderEdge);
    }
}
