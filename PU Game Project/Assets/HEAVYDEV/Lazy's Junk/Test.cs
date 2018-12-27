using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MHA.DebugGame
{
    public class Test : MonoBehaviour
    {
        public Transform thingToMove;
        public float spinDuration;

        private float spinTimer;

        private void Start()
        {
            spinTimer = 1;
        }

        void Update()
        {
            if(spinTimer < 1)
            {
                spinTimer += Time.deltaTime/spinDuration;
                thingToMove.eulerAngles = new Vector3(thingToMove.eulerAngles.x, Mathf.Lerp(0f, 360f, spinTimer), thingToMove.eulerAngles.z);
            }
        }

        public void StartRotation()
        {
            if(spinTimer >= 1)
            {
                spinTimer = 0;
            }
        }
    }

    public class Player : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetButtonDown("Use"))
            {
                RaycastHit hitInfo = new RaycastHit();
                Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 10f);

                if (hitInfo.collider.gameObject.CompareTag("Button") && hitInfo.collider.gameObject.GetComponent<Test>() != null)
                {
                    hitInfo.collider.gameObject.GetComponent<Test>().StartRotation();
                }
            }
        }
    }

}
