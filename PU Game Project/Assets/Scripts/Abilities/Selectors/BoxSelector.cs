using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSelector : GeneralSelector
{
    BoxCollider boxCol;
    public GameObject visual;

    protected override void InitializeImpl()
    {

        base.InitializeImpl();
        boxCol = GetComponent<BoxCollider>();
        SelectorData.Box boxData = (SelectorData.Box)selPacket.selectorData;

        Vector3 dimensions = (boxData.dimensions * GridGen.instance.NodeDiameter);
        boxCol.size = dimensions;

        boxCol.center = boxData.offset * GridGen.instance.NodeDiameter;

        if (boxData.rotate)
        {
            if (boxData.rotatePivot == SelectorData.Box.RotatePivot.Side)
            {
                if (dimensions.z >= 1)
                {
                    float newZ = boxCol.center.z + (dimensions.z / 2) + GridGen.instance.NodeDiameter;

                    boxCol.center = new Vector3(boxCol.center.x, boxCol.center.y, newZ);
                }
            }
            else if (boxData.rotatePivot == SelectorData.RotatePivot.Custom)
            {
                boxCol.center += (boxData.customPivot * GridGen.instance.NodeDiameter);
            }
        }

        visual.transform.localScale = new Vector3(dimensions.x, dimensions.y, dimensions.z);
        visual.transform.position = boxCol.bounds.center;
    }
}
