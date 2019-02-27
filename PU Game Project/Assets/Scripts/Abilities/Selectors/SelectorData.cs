using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SelectorData
{
    public string SelectorName { protected set; get; }

    public enum SelectionType
    {
        Null,
        Pick,
        AreaPick,
        AoE,
    }

    public enum TargetType
    {
        Unit,
        Tile,
        Structure,
        Projectile,
        Position
    }

    [Header("SELECTOR")]
    public SelectionType selectionType;

    public int maxNumOfSelect;
    public bool isPure;
    public bool includeCaster;
    public bool fireFromSelector;

    [Header("Target")]
    public TargetType mainTargetType;


    [Header("Secondary Targets")]
    public bool secUnit;
    public bool secTile;
    public bool secStructure;
    public bool secProjectile;

    [Header("Moving")]
    public bool follow;
    public float followRange;
    [Header("Rotating")]
    public bool rotate;
    public enum RotateType
    {
        FourSidedA,
        FourSidedD,
        EightSided
    }
    public RotateType rotateType;

    public enum RotatePivot
    {
        Centered,
        Side,
        Custom
    }
    public RotatePivot rotatePivot;
    public Vector3 customPivot;

    [Header("Position")]
    public Vector3 offset;

    [Header("Extra Selectors")]
    public ExtraResolutionType extraResolutionType;
    public enum ExtraResolutionType
    {
        Linear,
        ModeByMode
    }
    [HideInInspector] public List<SelectorData> extraSelectors = new List<SelectorData>();


    public virtual SelectorData Clone()
    {
        SelectorData returnData = new SelectorData();
        CopyProperties(returnData);
        return returnData;
    }

    protected virtual void CopyProperties(SelectorData newData)
    {
        newData.selectionType = this.selectionType;

        newData.mainTargetType = mainTargetType;

        newData.secUnit = secUnit;
        newData.secTile = secTile;
        newData.secStructure = secStructure;
        newData.secProjectile = secProjectile;

        newData.isPure = this.isPure;
        newData.includeCaster = includeCaster;
        newData.fireFromSelector = fireFromSelector;

        if (this.selectionType == SelectionType.Pick)
        {
            newData.maxNumOfSelect = this.maxNumOfSelect;
        }

        newData.follow = follow;
        newData.followRange = followRange;
        newData.rotate = rotate;
        newData.rotateType = rotateType;
        newData.offset = offset;

        newData.rotatePivot = rotatePivot;
        newData.customPivot = customPivot;

        foreach(SelectorData data in extraSelectors)
        {
            newData.extraSelectors.Add(data.Clone());
        }

        newData.extraResolutionType = extraResolutionType;
    }



    [Serializable]
    public class BasicMove : SelectorData
    {
        public BasicMove()
        {
            SelectorName = AbilityPrefabRef.BasicMoveSelector;
        }

        public override SelectorData Clone()
        {
            BasicMove moveSelector = new BasicMove();
            CopyProperties(moveSelector);
            return moveSelector;
        }
        protected override void CopyProperties(SelectorData newData)
        {
            base.CopyProperties(newData);
            BasicMove moveData = (BasicMove)newData;
            moveData.SelectorName = AbilityPrefabRef.BasicMoveSelector;
        }
    }

    [Serializable]
    public class Circle : SelectorData
    {
        [Space]
        [Header("CIRCLE")]
        public float radius;

        public Circle()
        {
            SelectorName = AbilityPrefabRef.CircleSelector;
        }

        public override SelectorData Clone()
        {
            Circle circleSelector = new Circle();
            CopyProperties(circleSelector);
            return circleSelector;
        }
        protected override void CopyProperties(SelectorData newData)
        {
            base.CopyProperties(newData);
            Circle circleData = (Circle)newData;
            circleData.radius = radius;
            circleData.SelectorName = AbilityPrefabRef.CircleSelector;
        }
    }

    [Serializable]
    public class Box : SelectorData
    {
        [Space]
        [Header("RECTANLGE")]
        public Vector3 dimensions;

        public Box()
        {
            SelectorName = AbilityPrefabRef.BoxSelector;
        }

        public override SelectorData Clone()
        {
            Box boxSelector = new Box();
            CopyProperties(boxSelector);
            return boxSelector;
        }

        protected override void CopyProperties(SelectorData newData)
        {
            base.CopyProperties(newData);
            Box boxData = (Box)newData;

            boxData.dimensions = dimensions;
        }
    }
}
