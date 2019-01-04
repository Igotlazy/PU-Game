//Contains General Use Methods involved in Combat. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CombatUtils {


    public static LayerMask shotMask = (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("GameTerrain") | 1 << LayerMask.NameToLayer("GameTerrainFade"));
    public static LayerMask clickLayerMask = (1 << LayerMask.NameToLayer("GameEntity")) | (1 << LayerMask.NameToLayer("GameTerrain"));
    public static LayerMask gameTerrainMask = (1 << LayerMask.NameToLayer("GameTerrain"));
    public static LayerMask gameTerrainAndFade = (1 << LayerMask.NameToLayer("GameTerrain")) | (1 << LayerMask.NameToLayer("GameTerrainFade"));
    public static LayerMask gameEntityMask = (1 << LayerMask.NameToLayer("GameEntity"));
    public static LayerMask objectFadeMask = (1 << LayerMask.NameToLayer("GameTerrain") | 1 << LayerMask.NameToLayer("GameTerrainFade") | (1 << LayerMask.NameToLayer("Obstacle")));

    private static float maxAngleForPartialCover = 30f;
    //private static float hitPercentDistanceDropOff = 5f;


    public static float MainFireCalculation(Vector3 sourceConnector, Vector3 targetConnector, Vector3 targetPartial)
    {
        RaycastHit initialShotInfo = new RaycastHit();
        Vector3 fireDirection = targetConnector - sourceConnector;

        Debug.DrawRay(sourceConnector, fireDirection, Color.blue, 10f);
        Physics.Raycast(sourceConnector, fireDirection.normalized, out initialShotInfo, fireDirection.magnitude, shotMask);


        if (initialShotInfo.collider != null) //It the shot hits an obstacle, it immediately returns 0. 
        {
            return 0f;
        }

        float hitDistance = fireDirection.magnitude;
        float currentHitPercent = coverCheckCalculation(fireDirection, targetPartial, targetConnector);

        /*
        if(hitPercentDistanceDropOff > 0) //Reduces %Hit by distance to target. 
        {
            while (hitDistance > hitPercentDistanceDropOff && currentHitPercent > 0)
            {
                currentHitPercent -= 25; 
                hitDistance -= hitPercentDistanceDropOff;
            }
        }
        */

        return currentHitPercent;
    }

    public static float MainFireCalculation(GameObject sourceObject, GameObject targetObject) //Figures out the chance of a given attack between two objects hitting.
    {
        Vector3 sourceConnector = sourceObject.GetComponent<Unit>().shotConnecter.transform.position;
        Unit targetUnit = targetObject.GetComponent<Unit>();
        Vector3 targetConnector = targetUnit.shotConnecter.transform.position;
        Vector3 targetPartial = targetUnit.partialCoverCheck.transform.position;

        return MainFireCalculation(sourceConnector, targetConnector, targetPartial);
    }

    public static float MainFireCalculation(Vector3 sourceConnector, Vector3 targetConnector, Vector3 targetPartial, out bool didPeek, out Vector3 fireSource) //Figures out the chance of a given attack between two objects hitting.
    {
        Vector3 initialfireDirection = targetConnector - sourceConnector;

        float initialResult = MainFireCalculation(sourceConnector, targetConnector, targetPartial);
        if(initialResult != 0)
        {
            //Debug.Log("Can See Without Peek");
            didPeek = false;
            fireSource = sourceConnector;
            return initialResult;
        }

        if (initialfireDirection.normalized.Equals(Vector3.up) || initialfireDirection.normalized.Equals(Vector3.down))
        {
            Debug.Log("Directly above or below, thus no peek");
            didPeek = false;
            fireSource = sourceConnector;
            return MainFireCalculation(sourceConnector, targetConnector, targetPartial);
        }

        initialfireDirection = new Vector3(initialfireDirection.x, 0f, initialfireDirection.z).normalized;
        Vector3 testVector;

        Vector3 testVectorX = new Vector3(initialfireDirection.x, 0f, 0f).normalized;
        Vector3 testVectorZ = new Vector3(0f, 0f, initialfireDirection.z).normalized;

        Debug.DrawRay(sourceConnector, testVectorX * GridGen.instance.NodeDiameter, Color.cyan, 5f);
        Debug.DrawRay(sourceConnector, testVectorZ * GridGen.instance.NodeDiameter, Color.cyan, 5f);

        bool xHit = Physics.Raycast(sourceConnector, testVectorX, GridGen.instance.NodeDiameter, shotMask);
        bool zHit = Physics.Raycast(sourceConnector, testVectorZ, GridGen.instance.NodeDiameter, shotMask);

        if (xHit && zHit) //If both hit, there are obstacles in both directions and you can't peek. 
        {
            didPeek = false;
            fireSource = sourceConnector;
            return 0;
        }
        else if (xHit) //If x hit, then there's an obstacle in that direction. 
        {
            testVector = testVectorX;
        }
        else if (zHit) //If z hit, then there's an obstacle in that direction. 
        {
            testVector = testVectorZ;
        }
        else //If both hit nothing, there must just be something diagonally between them blocking the shot. 
        {
            didPeek = false;
            fireSource = sourceConnector;
            return 0;
        }
        Debug.DrawRay(sourceConnector, testVector * GridGen.instance.NodeDiameter, Color.cyan, 5f);


        Vector3 sideVector = Vector3.Cross(Vector3.up, testVector).normalized; 
        Vector3 otherSideVector = -sideVector;

        float peekDistance = 0.75f;
        Debug.DrawRay(sourceConnector, sideVector * GridGen.instance.NodeDiameter * peekDistance, Color.blue, 5f);
        bool sideHit = Physics.Raycast(sourceConnector, sideVector, GridGen.instance.NodeDiameter * peekDistance, shotMask); //Firing rays to the left and right of source.
        Debug.DrawRay(sourceConnector, otherSideVector * GridGen.instance.NodeDiameter * peekDistance, Color.blue, 5f);
        bool otherSideHit = Physics.Raycast(sourceConnector, otherSideVector, GridGen.instance.NodeDiameter * peekDistance, shotMask);

        Vector3 sideSource = sourceConnector + (sideVector * GridGen.instance.NodeDiameter * peekDistance); //+ (testVector * GridGen.instance.NodeDiameter);
        Vector3 otherSideSource = sourceConnector + (otherSideVector * GridGen.instance.NodeDiameter * peekDistance);// + (testVector * GridGen.instance.NodeDiameter);

        if (!sideHit)
        {
            sideHit = Physics.Raycast(sideSource, testVector, GridGen.instance.NodeDiameter * peekDistance, shotMask);
            Debug.DrawRay(sideSource, testVector * GridGen.instance.NodeDiameter * peekDistance, Color.blue, 5f);
        }
        if (!otherSideHit)
        {
            otherSideHit = Physics.Raycast(otherSideSource, testVector, GridGen.instance.NodeDiameter * peekDistance, shotMask);
            Debug.DrawRay(otherSideSource, testVector * GridGen.instance.NodeDiameter * peekDistance, Color.blue, 5f);
        }

        if(!sideHit && !otherSideHit)
        {
            float sideFloat = MainFireCalculation(sideSource, targetConnector, targetPartial);
            float otherSideFloat = MainFireCalculation(otherSideSource, targetConnector, targetPartial);

            if(sideFloat > otherSideFloat)
            {
                didPeek = true;
                fireSource = sideSource;
                return sideFloat;
            }
            else if (otherSideFloat > sideFloat)
            {
                didPeek = true;
                fireSource = otherSideSource;
                return otherSideFloat;
            }
            else //They are equal in terms of %Hit Chance
            {
                float sideDistance = Vector3.Distance(sideSource, targetConnector); //So evaluate by distance to target. 
                float otherSideDistance = Vector3.Distance(otherSideSource, targetConnector);

                if(sideDistance >= otherSideDistance)
                {
                    didPeek = true;
                    fireSource = sideSource;
                    return sideFloat;
                }
                else
                {
                    didPeek = true;
                    fireSource = otherSideSource;
                    return otherSideFloat;
                }
            }
        }
        else if (!sideHit)
        {
            didPeek = true;
            fireSource = sideSource;
            return MainFireCalculation(sideSource, targetConnector, targetPartial);
        }
        else if(!otherSideHit)
        {           
            didPeek = true;
            fireSource = otherSideSource;
            return MainFireCalculation(otherSideSource, targetConnector, targetPartial);
        }
        else
        {
            didPeek = false;
            fireSource = sourceConnector;
            return MainFireCalculation(sourceConnector, targetConnector, targetPartial);
        }       
    }

    public static float coverCheckCalculation(Vector3 fireDirection, Vector3 targetPartialCheck, Vector3 targetShotConnector)
    {
        Vector3 coverCheckFireDiection = -(new Vector3(fireDirection.x, 0f, fireDirection.z));

        Debug.DrawRay(targetPartialCheck, coverCheckFireDiection.normalized * GridGen.instance.nodeRadius * 2, Color.blue, 10f);
        Debug.DrawRay(targetShotConnector, coverCheckFireDiection.normalized * GridGen.instance.nodeRadius * 2, Color.blue, 10f);
        bool partialCheck = Physics.Raycast(targetPartialCheck, coverCheckFireDiection, GridGen.instance.nodeRadius * 2, shotMask);
        bool fullCheck = Physics.Raycast(targetShotConnector, coverCheckFireDiection, GridGen.instance.nodeRadius * 2, shotMask);
        if (partialCheck)
        {
            //Checks if the angle of the shot (relative to the ground) was above a certain number. If so, it acts like there never was a PartialCover. 
            if (Vector3.Angle(-fireDirection, coverCheckFireDiection) > maxAngleForPartialCover && !fullCheck)
            {
                return 100f; //If it's in Partial Cover, but the fire angle relative to the ground came from above "maxAngelForPartialCover" degrees, it's 100%.
            }
            else
            {
                return 50f; //If it's in Partial Cover, but the fire angle relative to the ground came from below "maxAngelForPartialCover" degrees, it's 50%.
            }
        }
        else
        {
            return 100f; //If it isn't in Partial Cover and it does hit, it's 100%.
        }
    }

    public static Vector3 GiveShotConnector(GameObject objectWithConnector)
    {
        Unit sourceScript = objectWithConnector.GetComponent<Unit>();

        Vector3 connectorPos;
        if (sourceScript != null)
        {
            connectorPos = sourceScript.shotConnecter.transform.position;
        }
        else
        {
            connectorPos = objectWithConnector.transform.position;
        }

        return connectorPos;
    }

    public static Vector3 GivePartialCheck(GameObject objectWithPartial)
    {
        Unit sourceScript = objectWithPartial.GetComponent<Unit>();

        Vector3 partialPos;
        if (sourceScript != null)
        {
            partialPos = sourceScript.partialCoverCheck.transform.position;
        }
        else
        {
            partialPos = objectWithPartial.transform.position;
        }

        return partialPos;
    }

    public static bool AttackHitPercentages(float hitPercent) //Gives a true or false based on the % given.
    {
        hitPercent = Mathf.Clamp(hitPercent, 0, 100); //Clamps just in case.

        if(hitPercent == 0)
        {
            return false; 
        }

        float randomRange = UnityEngine.Random.Range(0f, 100f);

        if(randomRange <= hitPercent)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CanCast(LivingCreature creatureScript, int energyToLose, bool subtract)
    {
        if (creatureScript.CurrentEnergy >= energyToLose)
        {
            if (subtract)
            {
                creatureScript.CurrentEnergy -= energyToLose;
            }

            return true;
        }

        return false;
    }

    public static List<Node> BasicAttackSelect(GameObject selectedUnitObj, float range)
    {
        List<Node> attackNodes = GetCircleAttackableTiles(selectedUnitObj, range);

        DrawIndicators.instance.ClearTileMatStates(true, true, true);
        DrawIndicators.instance.AttackableSet(attackNodes);

        return attackNodes;
    }

    private static List<Node> GetCircleAttackableTiles(GameObject selectedUnitObj, float range)
    {
        List<Node> nodesToReturn = new List<Node>();

        Collider[] hitObjects = Physics.OverlapSphere(new Vector3(selectedUnitObj.transform.position.x, 0.55f, selectedUnitObj.transform.position.z), //MAGIC NUMBER
            range, clickLayerMask);

        foreach (Collider currentCol in hitObjects)
        {
            Tile currentTile = currentCol.gameObject.GetComponent<Tile>();
            if (currentTile != null && (currentTile.carryingNode.IsWalkable || currentTile.carryingNode.IsOccupied))
            {
                nodesToReturn.Add(currentTile.carryingNode);
            }
        }

        return nodesToReturn;
    }

    public static List<Vector3> ProjectilePathSplicer(Vector3 startPoint, Vector3 endPoint, float cutDistance)
    {
        if(cutDistance < 0.1)
        {
            cutDistance = 0.1f;
        }

        List<Vector3> returnList = new List<Vector3>();

        Vector3 intermediate = (endPoint - startPoint).normalized * cutDistance;
        Vector3 builder = startPoint;


        while(Vector3.Distance(builder, endPoint) > cutDistance)
        {
            returnList.Add(builder);
            builder += intermediate;
        }
        returnList.Add(endPoint);

        return returnList;
    }

    public static List<Vector3> ProjectilePathSplicer(Vector3 startPoint, Vector3 endPoint)
    {
        return ProjectilePathSplicer(startPoint, endPoint, 1f);
    }
    public static List<Vector3> ProjectilePathSplicer(Unit startUnit, Unit endUnit, float cutDistance)
    {
        Vector3 startPoint = startUnit.shotConnecter.transform.position;
        Vector3 endPoint = endUnit.shotConnecter.transform.position;

        return ProjectilePathSplicer(startPoint, endPoint, cutDistance);
    }
    public static List<Vector3> ProjectilePathSplicer(Unit startUnit, Unit endUnit)
    {
        Vector3 startPoint = startUnit.shotConnecter.transform.position;
        Vector3 endPoint = endUnit.shotConnecter.transform.position;

        return ProjectilePathSplicer(startPoint, endPoint, 1f);
    }

    public static void MakeEffectsDependent(List<BattleEffect> givenEffects, int startIndex, int endIndex)
    {
        if((startIndex < endIndex) && (startIndex < givenEffects.Count) && (endIndex < givenEffects.Count))
        {
            BattleEffect previousEffect = null;

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (previousEffect != null)
                {
                    previousEffect.cancelEffectAuxCalls += givenEffects[i].CancelEffect; //If a move is cancelled, all of them down the line will also be cancelled. 
                } 
                previousEffect = givenEffects[i];
            }
        }
        else
        {
            Debug.Log("Hey you gave shitty Indexes for MakingEffectsDependent");
        }
    }
    public static void MakeEffectsDependent(List<BattleEffect> givenEffects)
    {
        BattleEffect previousEffect = null;

        for (int i = 0; i < givenEffects.Count; i++)
        {
            if (previousEffect != null)
            {
                previousEffect.cancelEffectAuxCalls += givenEffects[i].CancelEffect; //If a move is cancelled, all of them down the line will also be cancelled. 
            } 
            previousEffect = givenEffects[i];
        }
    }

    public static void AreaUpdate(SelectorPacket givenPacket, List<Unit> givenUnits)
    {

    }
}
