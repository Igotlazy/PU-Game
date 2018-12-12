using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager instance;

    public List<Transform> unitTransformList = new List<Transform>();
    public CinemachineVirtualCamera currentUnitCamera;
    private CinemachineFramingTransposer currentFramingTransposer;
    
    [Space]
    public CinemachineVirtualCamera mainCam1;
    public CinemachineVirtualCamera mainCam2;
    public GameObject freeTacker;
    private bool onMCam1 = true;
    public bool isFreeMoving;
    private float freeAndPanTimer;

    private int panIndex;

    public float currentCameraAngle = 45f;
    public bool cameraOtherControl;
    public bool cameraMoving;
    private Queue<float> angleAlterations = new Queue<float>();
    [Space]

    [Header("Camera Controls:")]
    [SerializeField]
    private float freeMoveSpeed = 15f;
    [SerializeField]
    private AnimationCurve freeMoveCurve;
    [SerializeField]
    private float zoomSpeed = 5f;
    [SerializeField]
    private float zoomMinDistance = 7.5f;
    [SerializeField]
    private float zoomMaxDistance = 30f;
    public float turnSpeed = 0.75f; 
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
            unitTransformList.Add(currentPlayer.transform);
        }

        currentUnitCamera = mainCam1;
        currentFramingTransposer = mainCam1.GetCinemachineComponent<CinemachineFramingTransposer>();
        if(unitTransformList.Count > 0)
        {
            mainCam1.Follow = unitTransformList[0];
        }
        panIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCameraControl();
        RotateCameraLoader();

        UnitPanControl();

        ZoomControl();

        if(freeAndPanTimer < Time.time)
        {

            FreeMoveControl();
        }
    }

    public void SetCameraTargetBasic(Transform selectedTarget) //Controls Cinemachine camera movement between Units. Should honestly be on another script. 
    {
        isFreeMoving = false;
        freeAndPanTimer = Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time + Time.time; //Stops freeMove during camera changes. 

        CinemachineVirtualCamera newCam = null;
        if (onMCam1)
        {
            onMCam1 = false;
            newCam = mainCam2;

        }
        else
        {
            onMCam1 = true;
            newCam = mainCam1;
        }

        if (currentUnitCamera != null)
        {
            currentUnitCamera.Priority = 10;
        }

        //Makes it so new Camera inherits Rotation. 
        newCam.gameObject.transform.eulerAngles = new Vector3(newCam.gameObject.transform.eulerAngles.x, currentCameraAngle, newCam.gameObject.transform.eulerAngles.z);
        newCam.Follow = selectedTarget; //Gives new cam the follow target. 

        CinemachineFramingTransposer newTransposer = newCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        if(currentFramingTransposer != null)
        {
            newTransposer.m_CameraDistance = currentFramingTransposer.m_CameraDistance; //Makes it so new Camera inherits Zoom. 
        }

        newCam.Priority = 11; //Changes priority to new Camera.

        currentUnitCamera = newCam;
        currentFramingTransposer = newTransposer;
        
    }

    private void RotateCameraControl()
    {
        if (!cameraOtherControl && angleAlterations.Count < 3 && currentUnitCamera != null) // Controls Camera Turn
        {
            if (angleAlterations.Count < 3)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (angleAlterations.Count > 0 && angleAlterations.Peek() == 45f) //Cancels other movements if an opposite input is given.
                    {
                        angleAlterations.Clear();
                    }
                    angleAlterations.Enqueue(-45f);
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (angleAlterations.Count > 0 && angleAlterations.Peek() == -45f) //Cancels other movement if an opposite inpit is given.
                    {
                        angleAlterations.Clear();
                    }
                    angleAlterations.Enqueue(45f);
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
        CinemachineFramingTransposer cCameraTransposer = currentUnitCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        Vector3 originalDampings = new Vector3(cCameraTransposer.m_XDamping, cCameraTransposer.m_YDamping, cCameraTransposer.m_ZDamping);
        cCameraTransposer.m_XDamping = 0;
        cCameraTransposer.m_YDamping = 0;
        cCameraTransposer.m_ZDamping= 0;


        float startRotation = cCameraTransform.rotation.eulerAngles.y;
        float endRotation = startRotation + angleToAdd;
        currentCameraAngle = endRotation;

        while (cameraTurnTimer < 1f)
        {
            cameraTurnTimer = cameraTurnTimer + (Time.deltaTime * (1f/turnSpeed));
         
            float newAngle = Mathf.LerpAngle(startRotation, endRotation, turnCurve.Evaluate(cameraTurnTimer));
            Vector3 newRot = new Vector3(cCameraTransform.eulerAngles.x, newAngle, cCameraTransform.eulerAngles.z);

            cCameraTransform.eulerAngles = newRot;
            yield return null;

        }

        cCameraTransposer.m_XDamping = originalDampings.x;
        cCameraTransposer.m_YDamping = originalDampings.y;
        cCameraTransposer.m_ZDamping = originalDampings.z;


        cameraMoving = false;
    }

    private void UnitPanControl()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput && unitTransformList.Count > 0)
        {
            panIndex++;
            if (panIndex == unitTransformList.Count)
            {
                panIndex = 0;
            }

            ClickSelection.instance.ClickedSelection(unitTransformList[panIndex]);
        } // Controls panning from 1 Unit to the next.

        if (Input.GetKeyDown(KeyCode.DownArrow) && TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput && unitTransformList.Count > 0) // Controls panning from 1 Unit to the next.
        {
            panIndex--;
            if (panIndex < 0)
            {
                panIndex = unitTransformList.Count - 1;
            }

            ClickSelection.instance.ClickedSelection(unitTransformList[panIndex]);
        }
    }

    private void ZoomControl()
    {
        if (currentUnitCamera != null && currentFramingTransposer != null) //Controls zoom.
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                currentFramingTransposer.m_CameraDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
                currentFramingTransposer.m_CameraDistance = Mathf.Clamp(currentFramingTransposer.m_CameraDistance, zoomMinDistance, zoomMaxDistance);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                currentFramingTransposer.m_CameraDistance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
                currentFramingTransposer.m_CameraDistance = Mathf.Clamp(currentFramingTransposer.m_CameraDistance, zoomMinDistance, zoomMaxDistance);
            }
        }
    }

    private void FreeMoveControl()
    {
        if(Input.GetAxis("Horizontal") > 0)
        {
            Debug.Log("Axis");
            if(!isFreeMoving)
            {
                FreeMoveSetUp();
            }
            Vector3 rotVector = Vector3.ProjectOnPlane(currentUnitCamera.gameObject.transform.right, Vector3.up);
            freeTacker.transform.position += rotVector * freeMoveSpeed * freeMoveCurve.Evaluate(Input.GetAxis("Horizontal")) * Time.deltaTime;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            if (!isFreeMoving)
            {
                FreeMoveSetUp();
            }
            Vector3 rotVector = Vector3.ProjectOnPlane(currentUnitCamera.gameObject.transform.right, Vector3.up);
            freeTacker.transform.position += rotVector * freeMoveSpeed * -freeMoveCurve.Evaluate(Input.GetAxis("Horizontal")) * Time.deltaTime;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            if (!isFreeMoving)
            {
                FreeMoveSetUp();
            }
            Vector3 rotVector = Vector3.ProjectOnPlane(currentUnitCamera.gameObject.transform.forward, Vector3.up);
            freeTacker.transform.position += rotVector * freeMoveSpeed * freeMoveCurve.Evaluate(Input.GetAxis("Vertical")) * Time.deltaTime;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            if (!isFreeMoving)
            {
                FreeMoveSetUp();
            }
            Vector3 rotVector = Vector3.ProjectOnPlane(currentUnitCamera.gameObject.transform.forward, Vector3.up);
            freeTacker.transform.position += rotVector * freeMoveSpeed * -freeMoveCurve.Evaluate(Input.GetAxis("Vertical")) * Time.deltaTime;
        }
    }

    private void FreeMoveSetUp()
    {
        isFreeMoving = true;
        freeTacker.transform.position = currentUnitCamera.Follow.transform.position;
        currentUnitCamera.Follow = freeTacker.transform;

    }


}
