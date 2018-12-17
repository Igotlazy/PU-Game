using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureFaderMain : MonoBehaviour
{

    public HashSet<GameObject> fadeList = new HashSet<GameObject>();

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
                dissolveScript.CallDissolveMesh();
                fadeList.Add(enteredCollider.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider exitedCollider)
    {
        if ((exitedCollider.gameObject.CompareTag("Map") || exitedCollider.gameObject.CompareTag("Obstacle")))
        {
            ObjectDissolver dissolveScript = exitedCollider.gameObject.GetComponent<ObjectDissolver>();
            if (dissolveScript != null && fadeList.Contains(exitedCollider.gameObject))
            {
                dissolveScript.CallReformMesh();
            }
        }
    }
}
