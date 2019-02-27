using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureFaderFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider enteredCollider)
    {
        if (enteredCollider.gameObject.CompareTag("Floor"))
        {
            ObjectDissolver dissolveScript = enteredCollider.gameObject.GetComponent<ObjectDissolver>();
            if (dissolveScript != null)
            {
                dissolveScript.FaderLocked = true;
            }
        }
        if (enteredCollider.gameObject.CompareTag("Tile"))
        {
            enteredCollider.gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider exitedCollider)
    {
        if (exitedCollider.gameObject.CompareTag("Floor"))
        {
            ObjectDissolver dissolveScript = exitedCollider.gameObject.GetComponent<ObjectDissolver>();
            if (dissolveScript != null)
            {
                dissolveScript.FaderLocked = false;
            }
        }
        if (exitedCollider.gameObject.CompareTag("Tile"))
        {
            exitedCollider.gameObject.transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
