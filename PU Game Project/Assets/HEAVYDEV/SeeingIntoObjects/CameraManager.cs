using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{

    public static CameraManager instance;

    public List<CinemachineVirtualCamera> unitCameraList = new List<CinemachineVirtualCamera>();
    public CinemachineVirtualCamera currentUnitCamera;
    public float currentCameraAngle = 45f;
    public bool cameraOtherControl;
    public bool cameraMoving;
    private Queue<float> angleAlterations = new Queue<float>();
    [Space]

    [Header("Camera Controls:")]
    [SerializeField]
    private float timer;
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
        if(!cameraOtherControl && angleAlterations.Count < 3 && currentUnitCamera != null)
        {
            if(angleAlterations.Count < 3)
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
            else if(unitCameraList.Count > 0)
            {
                currentUnitCamera = unitCameraList[0];
                ClickSelection.instance.ClickedSelection(currentUnitCamera.Follow.gameObject);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && TurnManager.instance.CurrentBattlePhase == TurnManager.BattlePhase.PlayerInput)
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
            else if(unitCameraList.Count > 0)
            {
                currentUnitCamera = unitCameraList[0];
                ClickSelection.instance.ClickedSelection(currentUnitCamera.Follow.gameObject);

            }          
        }


        RotateCameraLoader();
    }

    public void SetCameraTargetBasic(CinemachineVirtualCamera selectedCamera) //Controls Cinemachine camera movement between Units. Should honestly be on another script. 
    {
        if (currentUnitCamera != null)
        {
            currentUnitCamera.Priority = 10;
        }

        selectedCamera.gameObject.transform.eulerAngles = new Vector3(selectedCamera.gameObject.transform.eulerAngles.x, currentCameraAngle, selectedCamera.gameObject.transform.eulerAngles.z);
        selectedCamera.Priority = 11;

        currentUnitCamera = selectedCamera;
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
        timer = 0;

        Transform cCameraTransform = currentUnitCamera.gameObject.transform;

        float startRotation = cCameraTransform.rotation.eulerAngles.y;
        float endRotation = startRotation + angleToAdd;

        while (timer < 1f)
        {
            timer = timer + Time.deltaTime;
         
            float newAngle = Mathf.LerpAngle(startRotation, endRotation, customCurve.Evaluate(timer));
            Vector3 newRot = new Vector3(cCameraTransform.eulerAngles.x, newAngle, cCameraTransform.eulerAngles.z);
            currentCameraAngle = newAngle;

            cCameraTransform.eulerAngles = newRot;
            yield return null;

        }
        cameraMoving = false;
    }

}
