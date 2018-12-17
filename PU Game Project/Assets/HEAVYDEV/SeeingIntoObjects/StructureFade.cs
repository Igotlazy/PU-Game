using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureFade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(this.transform.position.x, 5f, this.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 5f, this.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - 5f, this.transform.position.z);
        }
    }
}
