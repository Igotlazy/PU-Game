using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSelector : GeneralSelector
{
    SphereCollider sphereCol;
    public GameObject visual;

    protected override void InitializeImpl()
    {

        base.InitializeImpl();
        sphereCol = GetComponent<SphereCollider>();
        SelectorData.Circle circleData = (SelectorData.Circle)selPacket.selectorData;

        float radiusValue = circleData.radius * GridGen.instance.NodeDiameter;
        sphereCol.radius = radiusValue;
        if(radiusValue == 0)
        {
            radiusValue = 0.1f;
        }

        sphereCol.center = circleData.offset * GridGen.instance.NodeDiameter;

        if (circleData.rotate)
        {
            if (circleData.rotatePivot == SelectorData.RotatePivot.Side)
            {
                if (sphereCol.radius >= 1)
                {
                    float newZ = sphereCol.center.z + sphereCol.radius + GridGen.instance.NodeDiameter;

                    sphereCol.center = new Vector3(sphereCol.center.x, sphereCol.center.y, newZ);
                }
            }
            else if (circleData.rotatePivot == SelectorData.RotatePivot.Custom)
            {
                sphereCol.center += (circleData.customPivot * GridGen.instance.NodeDiameter);
            }
        }

        visual.transform.localScale = new Vector3(radiusValue + GridGen.instance.nodeRadius, radiusValue + GridGen.instance.nodeRadius, radiusValue + GridGen.instance.nodeRadius);
        visual.transform.position = sphereCol.bounds.center;
    }

}
