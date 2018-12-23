using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager instance;

    [Header("Camera Refereces:")]
    [SerializeField]
    public CinemachineVirtualCamera mainCam1;
    public CinemachineVirtualCamera mainCam2;
    public GameObject cameraTracker; //Tracker.
    public GameObject levelFader;
    [Space]

    [Header("Camera Controls:")]
    [SerializeField]
    private float freeMoveSpeed = 15f;
    [SerializeField]
    private AnimationCurve freeMoveCurve;
    [SerializeField]
    private float levelChangeSpeed = 5f;
    private float levelChangeDistance = 5f;
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
    [Space]


    public List<Transform> unitTransformList = new List<Transform>();
    public CinemachineVirtualCamera currentUnitCamera;
    private CinemachineFramingTransposer currentFramingTransposer;

    [Space]
    private bool onMCam1 = true; //Swaps the two Cameras for panning. 
    private float panLock; //Makes sure certain movements can't happen during a pan.
    private int panIndex; //Keeps track of who the last pan target was. 
    public float currentCameraAngle = 45f;
    public bool cameraRotating; //To queue up roations. 
    private Queue<float> angleAlterations = new Queue<float>();

    //For moving levels;
    Ray cameraRay;
    Plane zeroPlane;
    float faderY;
    float trackerY;
    int currentLevel = 0;




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

        if (unitTransformList.Count > 0)
        {
            mainCam1.Follow = unitTransformList[0];
        }

        SetCameraTargetBasic(cameraTracker.transform);
    }

    // Update is called once per frame
    void Update()
    {
        RotateCameraControl();
        RotateCameraLoader();

        ZoomControl();

        if(panLock < Time.time)
        {
            UnitPanControl();
            FreeMoveControl();
            MoveLevel();
        }
    }
    private void LateUpdate()
    {
        UpdatingFaderPosition();
    }

    public void SetCameraTargetBasic(Transform selectedTarget) //Controls Cinemachine camera movement between Units. Should honestly be on another script. 
    {
        panLock = Camera.main.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time + Time.time; //Stops freeMove during camera changes. 

        CinemachineVirtualCamera newCam = null;
        CinemachineVirtualCamera oldCam = null;
        if (onMCam1)
        {
            onMCam1 = false;
            newCam = mainCam2;
            oldCam = mainCam1;

        }
        else
        {
            onMCam1 = true;
            newCam = mainCam1;
            oldCam = mainCam2;
        }

        if (currentUnitCamera != null)
        {
            currentUnitCamera.Priority = 10;
        }

        //Makes it so new Camera inherits Rotation. 
        newCam.gameObject.transform.eulerAngles = new Vector3(newCam.gameObject.transform.eulerAngles.x, currentCameraAngle, newCam.gameObject.transform.eulerAngles.z);
        oldCam.Follow = null;
        cameraTracker.transform.position = selectedTarget.transform.position;
        newCam.Follow = cameraTracker.transform; //Gives new cam the follow target. 
        SetLevelY(selectedTarget.transform.position.y); //Sets level relevant to new target. 

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
        if (angleAlterations.Count < 3 && currentUnitCamera != null) // Controls Camera Turn
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
        if (!cameraRotating && angleAlterations.Count > 0)
        {
            StartCoroutine(RotateCamera(angleAlterations.Dequeue()));
        }

    }
    IEnumerator RotateCamera(float angleToAdd)
    {
        cameraRotating = true;
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


        cameraRotating = false;
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
            Vector3 rotVector = Vector3.ProjectOnPlane(currentUnitCamera.gameObject.transform.right, Vector3.up).normalized;
            cameraTracker.transform.position += rotVector * freeMoveSpeed * freeMoveCurve.Evaluate(Input.GetAxis("Horizontal")) * Time.deltaTime;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            Vector3 rotVector = Vector3.ProjectOnPlane(currentUnitCamera.gameObject.transform.right, Vector3.up).normalized;
            cameraTracker.transform.position += rotVector * freeMoveSpeed * -freeMoveCurve.Evaluate(Input.GetAxis("Horizontal")) * Time.deltaTime;
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            Vector3 rotVector = Vector3.ProjectOnPlane(currentUnitCamera.gameObject.transform.forward, Vector3.up).normalized;
            cameraTracker.transform.position += rotVector * freeMoveSpeed * freeMoveCurve.Evaluate(Input.GetAxis("Vertical")) * Time.deltaTime;
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            Vector3 rotVector = Vector3.ProjectOnPlane(currentUnitCamera.gameObject.transform.forward, Vector3.up).normalized;
            cameraTracker.transform.position += rotVector * freeMoveSpeed * -freeMoveCurve.Evaluate(Input.GetAxis("Vertical")) * Time.deltaTime;
        }
    }

    private void MoveLevel()
    {
        if (Input.GetKeyDown(KeyCode.F) && currentLevel < 3)
        {
            currentLevel += 1;
            SetLevelY(currentLevel);
        }
        if (Input.GetKeyDown(KeyCode.C) && currentLevel > 0)
        {
            currentLevel -= 1;
            SetLevelY(currentLevel);
        }
    }
    private void SetLevelY(float givenY)
    {
        int newInt = 0;
        if (givenY < (levelChangeDistance * 0.8f))
        {
            newInt = 0;
        }
        else if (givenY >= (levelChangeDistance - 0.5f) && givenY < (levelChangeDistance * 2f -0.5f)) //0.5  just to give a buffer. 
        {
            newInt = 1;
        }
        else if (givenY >= (levelChangeDistance * 2f - 0.5f) && givenY < (levelChangeDistance * 3f) - 0.5f)
        {
            newInt = 2;
        }
        else if (givenY >= (levelChangeDistance * 3f - 0.5f))
        {
            newInt = 3;
        }

        SetLevelY(newInt);
    }
    private void SetLevelY(int givenLevel)
    {
        currentLevel = givenLevel;
        faderY = (givenLevel * levelChangeDistance) + levelChangeDistance + 0.5f; //Added 0.5 because the distance between the floor disappearing collider and the wall disappearing collider is 1 unit.
        trackerY = givenLevel * levelChangeDistance;

        levelFader.transform.position = new Vector3(levelFader.transform.position.x, faderY, levelFader.transform.position.z);
        zeroPlane = new Plane(Vector3.up, new Vector3(0f, faderY, 0f));
    }

    private void UpdatingFaderPosition()
    {
        cameraRay = Camera.main.ViewportPointToRay(cameraPoint);

        float distance;
        zeroPlane.Raycast(cameraRay, out distance);
        Vector3 hitPoint = cameraRay.GetPoint(distance);


        levelFader.transform.position = new Vector3(hitPoint.x, faderY, hitPoint.z);
        if(cameraTracker.transform.position.y != faderY)
        {
            Vector3 movePos = new Vector3(cameraTracker.transform.position.x, trackerY, cameraTracker.transform.position.z);
            cameraTracker.transform.Translate((movePos - cameraTracker.transform.position) * Time.deltaTime * levelChangeSpeed);
        }
    }
    Vector3 cameraPoint = new Vector3(0.5f, 0.5f, 0f);


}
