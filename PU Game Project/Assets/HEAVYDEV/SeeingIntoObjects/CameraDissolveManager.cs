using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDissolveManager : MonoBehaviour
{
    public List<GameObject> objectsNearCamera = new List<GameObject>();
    public List<GameObject> inViewOfPlayer = new List<GameObject>();
    private float ticker;
    public float tickInterval = 0.5f;
    private RaycastHit[] hitInfos;
    

    // Start is called before the first frame update
    void Update()
    {
        
        if(ticker <= Time.time)
        {
            ticker = Time.time + tickInterval;

            ClearPlayerView();
        }
    }

    private void ClearPlayerView()
    {
        if (ClickSelection.instance.selectedUnitObj != null)
        {
            GameObject clickObject = ClickSelection.instance.selectedUnitObj;
            Vector3 rayDir = clickObject.transform.position - Camera.main.transform.position;

            hitInfos = Physics.RaycastAll(Camera.main.transform.position, rayDir, Mathf.Clamp(rayDir.magnitude -1, 0f, 1000), CombatUtils.objectFadeMask);
            Debug.DrawRay(Camera.main.transform.position, rayDir.normalized * Mathf.Clamp(rayDir.magnitude - 1, 0f, 1000), Color.red, 0.5f);

            List<GameObject> hitObjectList = new List<GameObject>();
            foreach(RaycastHit currentHit in hitInfos) //Making sure it doesn't hit the base floor. 
            {
                if(!currentHit.collider.gameObject.CompareTag("Map Base"))
                {
                    hitObjectList.Add(currentHit.collider.gameObject);
                }
            }

            foreach(GameObject currentObj in hitObjectList) //Add new ones to List and Dissolve them.
            {              
                ObjectDissolver dissolveScript =currentObj.GetComponent<ObjectDissolver>();
                if(dissolveScript != null && !inViewOfPlayer.Contains(currentObj))
                {
                    if (!objectsNearCamera.Contains(currentObj))
                    {
                        dissolveScript.CallDissolveMesh();
                    }

                    inViewOfPlayer.Add(currentObj);
                }
            }

            List<GameObject> toRemove = new List<GameObject>();
            foreach(GameObject currentObj in inViewOfPlayer) //Remove ones that are no longer in the list and Reform them. 
            {
                if (!hitObjectList.Contains(currentObj))
                {

                    if (!objectsNearCamera.Contains(currentObj))
                    {
                        currentObj.GetComponent<ObjectDissolver>().CallReformMesh();
                    }

                    toRemove.Add(currentObj);
                }
            }
            foreach(GameObject removeObj in toRemove)
            {
                inViewOfPlayer.Remove(removeObj);
            }
        }
        else
        {
            ClearPlayerViewCleanup();
        }
    }

    private void ClearPlayerViewCleanup()
    {
        foreach (GameObject currentObj in inViewOfPlayer)
        {
            ObjectDissolver dissolveScript = currentObj.GetComponent<ObjectDissolver>();
            if (!objectsNearCamera.Contains(currentObj))
            {
                dissolveScript.CallReformMesh();
            }            
        }
        inViewOfPlayer.Clear();
    }


    private void OnTriggerEnter(Collider enteredCollider)
    {
        if(enteredCollider.gameObject.CompareTag("Map") || enteredCollider.gameObject.CompareTag("Obstacle"))
        {
            ObjectDissolver dissolveScript = enteredCollider.gameObject.GetComponent<ObjectDissolver>();
            if(dissolveScript != null)
            {
                if (!inViewOfPlayer.Contains(enteredCollider.gameObject))
                {
                    dissolveScript.CallDissolveMesh();
                }

                objectsNearCamera.Add(enteredCollider.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider exitedCollider)
    {
        if ((exitedCollider.gameObject.CompareTag("Map") || exitedCollider.gameObject.CompareTag("Obstacle")) && objectsNearCamera.Contains(exitedCollider.gameObject))
        {
            ObjectDissolver dissolveScript = exitedCollider.gameObject.GetComponent<ObjectDissolver>();
            if (!inViewOfPlayer.Contains(exitedCollider.gameObject))
            {
                dissolveScript.CallReformMesh();
            }
            objectsNearCamera.Remove(exitedCollider.gameObject);
        }
    }
}
