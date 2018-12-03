using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDissolveManager : MonoBehaviour
{
    public List<GameObject> objectsInside = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }



    private void OnTriggerEnter(Collider enteredCollider)
    {
        if(enteredCollider.gameObject.CompareTag("Map") || enteredCollider.gameObject.CompareTag("Obstacle"))
        {
            ObjectDissolver dissolveScript = enteredCollider.gameObject.GetComponent<ObjectDissolver>();
            if(dissolveScript != null)
            {
                dissolveScript.CallDissolveMesh();
                objectsInside.Add(enteredCollider.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider exitedCollider)
    {
        if ((exitedCollider.gameObject.CompareTag("Map") || exitedCollider.gameObject.CompareTag("Obstacle")) && objectsInside.Contains(exitedCollider.gameObject))
        {
            ObjectDissolver dissolveScript = exitedCollider.gameObject.GetComponent<ObjectDissolver>();
            dissolveScript.CallReformMesh();
            objectsInside.Remove(exitedCollider.gameObject);
        }
    }
}
