using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMouseFollower : MonoBehaviour {

    public bool isFreeSelection;
    public Transform pointer;

    RaycastHit hitInfo = new RaycastHit();
    public LayerMask clickLayerMask;
    bool hit;
    Vector3 currentPosition;


    void Update()
    {
        hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 100f, clickLayerMask);

        if (hit)
        {
            if (isFreeSelection)
            {
                pointer.transform.position = new Vector3(hitInfo.point.x, 0.5f, hitInfo.point.z);
            }
            else
            {
                currentPosition = GridGen.instance.NodeFromWorldPoint(hitInfo.point).worldPosition;
                pointer.transform.position = new Vector3(currentPosition.x, 0.5f, currentPosition.z);
            }


            this.transform.LookAt(pointer);
        }
    }
}
