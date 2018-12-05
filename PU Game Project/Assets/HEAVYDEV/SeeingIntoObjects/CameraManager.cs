using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager instance;

    public List<CinemachineVirtualCamera> unitCameraList = new List<CinemachineVirtualCamera>();
    public CinemachineVirtualCamera currentUnitCamera;
    public CinemachineVirtualCamera freeCamera;
    private CinemachineFramingTransposer currentFramingTransposer;
    public float currentCameraAngle = 45f;
    public bool cameraOtherControl;
    public bool cameraMoving;
    private Queue<float> angleAlterations = new Queue<float>();
    [Space]

    [Header("Camera Controls:")]
    [SerializeField]
    private float scrollSpeed = 5f;
    [SerializeField]
    private float scrollMinDistance = 7.5f;
    [SerializeField]
    private float scrollMaxDistance = 30f;
    [SerializeField]  
    private float cameraTurnTimer;
    [SerializeField]
    private AnimationCurve turnCurve;

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
        RotateCameraControl();
        RotateCameraLoader();

        UnitPanControl();

        ZoomControl();

        FreeMoveControl();
    }

    public void SetCameraTargetBasic(CinemachineVirtualCamera selectedCamera) //Controls Cinemachine camera movement between Units. Should honestly be on another script. 
    {
        if (currentUnitCamera != null)
        {
            currentUnitCamera.Priority = 10;
        }

        //Makes it so new Camera inherits Rotation. 
        selectedCamera.gameObject.transform.eulerAngles = new Vector3(selectedCamera.gameObject.transform.eulerAngles.x, currentCameraAngle, selectedCamera.gameObject.transform.eulerAngles.z);

        CinemachineFramingTransposer newTransposer = selectedCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if(currentFramingTransposer != null)
        {
            newTransposer.m_CameraDistance = currentFramingTransposer.m_CameraDistance; //Makes it so new Camera inherits Zoom. 
        }

        selectedCamera.Priority = 11; //Changes priority to new Camera.

        currentUnitCamera = selectedCamera;
        currentFramingTransposer = newTransposer;
    }

    private void RotateCameraControl()
    {
        if (!cameraOtherControl && angleAlterations.Count < 3 && currentUnitCamera != null) // Controls Camera Turn
        {
            if (angleAlterations.Count < 3)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    if (angleAlterations.Count > 0 && angleAlterations.Peek() == 90f) //Cancels other movements if an opposite input is given.
                    {
                        angleAlterations.Clear();
                    }
                    angleAlterations.Enqueue(-90f);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (angleAlterations.Count > 0 && angleAlterations.Peek() == -90f) //Cancels other movement if an opposite inpit is given.
                    {
                        angleAlterations.Clear();
                    }
                    angleAlterations.Enqueue(90f);
                }
            }
        }
    }
    private void RotateCameraLoader()
    {
        if (!cameraMoving && angleAlterations.Count > 0)
        {
            StartCoroutine(RotateCamera(angleAlterations.Dequeue()));
        }

    }
    IEnumerator RotateCamera(float angleToAdd)
    {
        cameraMoving = true;
        cameraTurnTimer = 0;

        Transform cCameraTransform = currentUnitCamera.gameObject.transform;

        float startRotation = cCameraTransform.rotation.eulerAngles.y;
        float endRotation = startRotation + angleToAdd;
        currentCameraAngle = endRotation;

        while (cameraTurnTimer < 1f)
        {
            cameraTurnTimer = cameraTurnTimer + Time.deltaTime;
         
            float newAngle = Mathf.LerpAngle(startRotation, endRotation, turnCurve.Evaluate(cameraTurnTimer));
            Vector3 newRot = new Vector3(cCameraTransform.eulerAngles.x, newAngle, cCameraTransform.eulerAngles.z);

            cCameraTransform.eulerAngles = newRot;
            yield return null;

        }
        cameraMoving = false;
    }

    private void UnitPanControl()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput)
        {
            if (currentUnitCamera != null)
            {
                int currentIndex = unitCameraList.IndexOf(currentUnitCamera);
                currentIndex++;
                if (currentIndex == unitCameraList.Count)
                {
                    currentIndex = 0;
                }

                ClickSelection.instance.ClickedSelection(unitCameraList[currentIndex].Follow.gameObject);
            }
            else if (unitCameraList.Count > 0)
            {
                currentUnitCamera = unitCameraList[0];
                ClickSelection.instance.ClickedSelection(currentUnitCamera.Follow.gameObject);
            }
        } // Controls panning from 1 Unit to the next.

        if (Input.GetKeyDown(KeyCode.DownArrow) && TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput) // Controls panning from 1 Unit to the next.
        {
            if (currentUnitCamera != null)
            {
                int currentIndex = unitCameraList.IndexOf(currentUnitCamera);
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = unitCameraList.Count - 1;
                }

                ClickSelection.instance.ClickedSelection(unitCameraList[currentIndex].Follow.gameObject);
            }
            else if (unitCameraList.Count > 0)
            {
                currentUnitCamera = unitCameraList[0];
                ClickSelection.instance.ClickedSelection(currentUnitCamera.Follow.gameObject);

            }
        }
    }

    private void ZoomControl()
    {
        if (currentUnitCamera != null && currentFramingTransposer != null) //Controls zoom.
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                currentFramingTransposer.m_CameraDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                currentFramingTransposer.m_CameraDistance = Mathf.Clamp(currentFramingTransposer.m_CameraDistance, scrollMinDistance, scrollMaxDistance);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                currentFramingTransposer.m_CameraDistance -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                currentFramingTransposer.m_CameraDistance = Mathf.Clamp(currentFramingTransposer.m_CameraDistance, scrollMinDistance, scrollMaxDistance);
            }
        }
    }

    private void FreeMoveControl()
    {
        if(Input.GetAxis("Horizontal") > 0)
        {
            Debug.Log("Axis");
            if(currentUnitCamera != freeCamera)
            {
                SetCameraTargetBasic(freeCamera);
            }
            freeCamera.Follow.transform.position += freeCamera.gameObject.transform.right * freeMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            if (currentUnitCamera != freeCamera)
            {
                SetCameraTargetBasic(freeCamera);
            }
            freeCamera.Follow.transform.position += freeCamera.gameObject.transform.right * freeMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            if (currentUnitCamera != freeCamera)
            {
                SetCameraTargetBasic(freeCamera);
            }
            freeCamera.Follow.transform.position += freeCamera.gameObject.transform.forward * freeMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            if (currentUnitCamera != freeCamera)
            {
                SetCameraTargetBasic(freeCamera);
            }
            freeCamera.Follow.transform.position += freeCamera.gameObject.transform.forward * freeMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        }
    }

    private float freeMoveSpeed = 10f;


}
