using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager instance;

    public List<CinemachineVirtualCamera> unitCameraList = new List<CinemachineVirtualCamera>();
    public CinemachineVirtualCamera currentUnitCamera;
    public float currentCameraAngle;
    [Space]

    [Header("Camera Controls:")]
    [SerializeField]
    private float timer, angle;
    [SerializeField]
    private AnimationCurve customCurve;

    private void Awake()
    {
        if (instance == null) { instance = this; } else { Destroy(this); }
    }

    void Start()
    {
        foreach (GameObject currentPlayer in ReferenceObjects.UnitList)
        {
            unitCameraList.Add(currentPlayer.GetComponent<Unit>().unitCamera);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            timer = 0f;
            StartCoroutine(RotateCamera(-90));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            timer = 0f;
            StartCoroutine(RotateCamera(90));
        }
    }

    public void SetCameraTargetBasic(CinemachineVirtualCamera selectedCamera) //Controls Cinemachine camera movement between Units. Should honestly be on another script. 
    {
        if (currentUnitCamera != null)
        {
            currentUnitCamera.Priority = 10;
        }

        //selectedCamera.gameObject.transform.rotation.eulerAngles.y = currentCameraAngle;
        selectedCamera.Priority = 11;

        currentUnitCamera = selectedCamera;
    }


    IEnumerator RotateCamera(float angleToAdd)
    {
        Transform cCameraTransform = currentUnitCamera.gameObject.transform;

        Quaternion startRotation = cCameraTransform.rotation;
        Quaternion endRotation = cCameraTransform.rotation * Quaternion.Euler(Vector3.up * angleToAdd);
        while (timer < 1f)
        {
            timer = timer + Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, customCurve.Evaluate(timer));
            yield return null;
        }
        timer = 0f;
    }


}
