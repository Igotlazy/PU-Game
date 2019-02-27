using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureFaderMain : MonoBehaviour
{
    void Start()
    {
        
    }


    private void OnTriggerEnter(Collider enteredCollider)
    {
        if (enteredCollider.gameObject.CompareTag("Map") || enteredCollider.gameObject.CompareTag("Obstacle"))
        {
            ObjectDissolver dissolveScript = enteredCollider.gameObject.GetComponent<ObjectDissolver>();
            if (dissolveScript != null)
            {
                dissolveScript.FaderLocked = true;
            }
        }
    }

    private void OnTriggerExit(Collider exitedCollider)
    {
        if (exitedCollider.gameObject.CompareTag("Map") || exitedCollider.gameObject.CompareTag("Obstacle"))
        {
            ObjectDissolver dissolveScript = exitedCollider.gameObject.GetComponent<ObjectDissolver>();
            if (dissolveScript != null)
            {
                dissolveScript.FaderLocked = false;
            }
        }
    }
}
