//Contains General Use Methods involved in Combat. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CombatUtils {


    private static LayerMask initialShotMask = (1 << LayerMask.NameToLayer("GameEntity")) | (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("GameTerrain"));
    private static LayerMask coverCheckShotMask = (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("GameTerrain"));
    public static LayerMask clickLayerMask = (1 << LayerMask.NameToLayer("GameEntity")) | (1 << LayerMask.NameToLayer("GameTerrain"));
    public static LayerMask nodeSelectionMask = (1 << LayerMask.NameToLayer("GameTerrain"));
    public static LayerMask targetSelectionMask = (1 << LayerMask.NameToLayer("GameEntity"));

    private static float maxAngleForPartialCover = 30f;
    private static float hitPercentDistanceDropOff = 5f;


    public static float AttackHitCalculation(GameObject sourceObject, GameObject targetObject) //Figures out the chance of a given attack between two objects hitting.
    {
        Unit sourceUnitScript = sourceObject.GetComponent<Unit>();
        Unit targetUnitScript = targetObject.GetComponent<Unit>();

        RaycastHit initialShotInfo = new RaycastHit();
        Vector3 fireDirection = targetUnitScript.shotConnecter.transform.position - sourceUnitScript.shotConnecter.transform.position;

        Debug.DrawRay(sourceUnitScript.shotConnecter.transform.position, fireDirection, Color.blue, 10f);
        Physics.Raycast(sourceUnitScript.shotConnecter.transform.position, fireDirection.normalized, out initialShotInfo, 100f, initialShotMask);


        if (initialShotInfo.collider.gameObject != targetObject) //It the shot hits an obstacle, it immediately returns 0. 
        {
            Debug.Log("The chance to Hit is: 0");
            return 0f;
        }

        float hitDistance = fireDirection.magnitude;
        int currentHitPercent = 0;


        RaycastHit coverCheckInfo;
        Vector3 coverCheckFireDiection = -(new Vector3(fireDirection.x, 0f, fireDirection.z));

        Debug.DrawRay(targetUnitScript.partialCoverCheck.transform.position, coverCheckFireDiection.normalized * 1.5f, Color.blue, 10f);
        bool coverCheckHit = Physics.Raycast(targetUnitScript.partialCoverCheck.transform.position, coverCheckFireDiection, out coverCheckInfo, 1.5f, coverCheckShotMask);
        if (coverCheckHit)
        {
            bool isPartialCover = coverCheckInfo.collider.gameObject.GetComponent<ObstacleData>().isPartialCover; //Checks to see if the obstacle is PartialCover
            //Checks if the angle of the shot (relative to the ground) was above a certain number. If so, it acts like there never was a PartialCover. 
            if (isPartialCover && Vector3.Angle(-fireDirection, coverCheckFireDiection) > maxAngleForPartialCover)                 
            {
                currentHitPercent = 100; //If it's in Partial Cover, but the fire angle relative to the ground came from above "maxAngelForPartialCover" degrees, it's 100%.
            }
            else
            {
                currentHitPercent = 50; //If it's in Partial Cover, but the fire angle relative to the ground came from below "maxAngelForPartialCover" degrees, it's 50%.
            }
        }
        else
        {
            currentHitPercent = 100; //If it isn't in Partial Cover and it does hit, it's 100%.
        }

        if(hitPercentDistanceDropOff > 0) //Reduces %Hit by distance to target. 
        {
            while (hitDistance > hitPercentDistanceDropOff && currentHitPercent > 0)
            {
                currentHitPercent -= 25; 
                hitDistance -= hitPercentDistanceDropOff;
            }
        }

        Debug.Log("The chance to Hit is: " + currentHitPercent + "%");
        return currentHitPercent;      
    }


    public static bool AttackHitPercentages(float hitPercent) //Gives a true or false based on the % given.
    {
        hitPercent = Mathf.Clamp(hitPercent, 0, 100); //Clamps just in case.

        if(hitPercent == 0)
        {
            return false; 
        }

        float randomRange = Random.Range(0f, 100f);

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
                if (previousEffect != null) { previousEffect.CancelEffectAuxCalls += givenEffects[i].CancelEffect; } //If a move is cancelled, all of them down the line will also be cancelled. 

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
            if (previousEffect != null) { previousEffect.CancelEffectAuxCalls += givenEffects[i].CancelEffect; } //If a move is cancelled, all of them down the line will also be cancelled. 

            previousEffect = givenEffects[i];
        }
    }
}
