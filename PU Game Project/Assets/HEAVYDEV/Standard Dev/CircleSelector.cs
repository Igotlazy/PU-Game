using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSelector : GeneralSelector
{
    SphereCollider sphereCol;

    protected override void InitializeImpl()
    {

        base.InitializeImpl();
        sphereCol = GetComponent<SphereCollider>();
        AbilityPrefabRef.CircleSelectorData circleData = (AbilityPrefabRef.CircleSelectorData)attachedTargetPacket.selectorData;
        sphereCol.radius = circleData.radius;
    }

}
