using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour {

    Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
        transform.forward = cam.forward;
    }

    private void LateUpdate()
    {
        transform.forward = cam.forward;
    }
}
