using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMouseFollower : MonoBehaviour {

    public bool isFreeSelection;
    public Transform pointer;

    Plane basePlane;
    Ray mouseRay;

    private void Start()
    {
        basePlane = new Plane(Vector3.up, this.transform.position);
    }

    void Update()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist;
        if (basePlane.Raycast(mouseRay, out dist))
        {
            Vector3 point = mouseRay.GetPoint(dist);

            if (isFreeSelection)
            {
                pointer.transform.position = point;
            }
            else
            {
                pointer.transform.position = GridGen.instance.NodeFromWorldPoint(point).worldPosition;
            }

            this.transform.LookAt(pointer);
        }
    }
}
