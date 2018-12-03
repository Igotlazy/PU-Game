using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTest : MonoBehaviour
{

    [SerializeField] private float timer, angle;
    [SerializeField] private AnimationCurve customCurve;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            timer = 0f;
            StartCoroutine(RotateCamera(angle));
        }
    }

    IEnumerator RotateCamera(float angleToAdd)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = transform.rotation * Quaternion.Euler(Vector3.up * angleToAdd);
        while (timer < 1f)
        {
            timer = timer + Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, customCurve.Evaluate(timer));
            yield return null;
        }
        timer = 0f;
    }

}
