using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureFaderCeiling : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider enteredCollider)
    {
        if (enteredCollider.gameObject.CompareTag("Ceiling"))
        {
            ObjectDissolver dissolveScript = enteredCollider.gameObject.GetComponent<ObjectDissolver>();
            if (dissolveScript != null)
            {
                dissolveScript.CallDissolveMesh();
            }
        }
        if (enteredCollider.gameObject.CompareTag("Tile"))
        {
            enteredCollider.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider exitedCollider)
    {
        if (exitedCollider.gameObject.CompareTag("Ceiling"))
        {
            ObjectDissolver dissolveScript = exitedCollider.gameObject.GetComponent<ObjectDissolver>();
            if (dissolveScript != null)
            {
                dissolveScript.CallReformMesh();
            }
        }
        if (exitedCollider.gameObject.CompareTag("Tile"))
        {
            exitedCollider.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
