using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.DebugGame
{
    public class Test : MonoBehaviour
    {

        public GameObject targetObject;
        public LayerMask layerMask;
        //RaycastHit hit;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            Vector3 fireDirection = targetObject.transform.position - transform.position;

            if (Physics.Raycast(transform.position, fireDirection, Mathf.Infinity, layerMask))
            {
                Debug.Log("Hit Target");
            }

            Debug.DrawRay(transform.position, fireDirection, Color.yellow);


        }
    }
}
