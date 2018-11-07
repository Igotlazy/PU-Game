using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.DebugGame
{
    public class Test : MonoBehaviour
    {
        public float speedMultiplier;
        float xMove;
        float zMove;
        Rigidbody rb;
        Animator anim;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
           // anim = GetComponent<Animator>();
        }

        void Update()
        {
            xMove = Input.GetAxis("Horizontal");
            zMove = Input.GetAxis("Vertical");
            //anim.SetFloat("Speed", xMove);
        }

        private void FixedUpdate()
        {
            rb.velocity = new Vector3((xMove * speedMultiplier), rb.velocity.y, (zMove * speedMultiplier));
        }
    }
}
