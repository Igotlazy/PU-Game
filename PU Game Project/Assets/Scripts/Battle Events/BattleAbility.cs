//Event that handles Abilities.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.GenericBehaviours;

[System.Serializable]
public class BattleAbility : BattleEvent {

    private GameObject attackProjectile;
    public List<Node> targetNodes = new List<Node>();
    public GameObject sourceObject;
    public Attack attack;

    public BattleAbility(Attack _attack, GameObject _attackProjectile, GameObject _sourceObject, List<Node> _targetNodes) : base(null)
    {
        this.attack = _attack;
        this.attackProjectile = _attackProjectile;
        this.sourceObject = _sourceObject;

        foreach(Node currentNode in _targetNodes)
        {
            this.targetNodes.Add(currentNode);
        }
       
    }





    protected override void BattleEventRunImpl()
    {
        bEventMonoBehaviour.StartCoroutine(FireAttack());
    }
    protected override void BattleEventPauseImpl()
    {

    }
    protected override void BattleEventResumeImpl()
    {

    }
    protected override void BattleEventCancelImpl()
    {
        bEventMonoBehaviour.StopCoroutine(FireAttack());
    }
    protected override void BattleEventFinishImpl()
    {

    }


    IEnumerator FireAttack()
    {
        /*
        Unit sourceScript = sourceObject.GetComponent<Unit>();
        Unit targetScript = targetObject.GetComponent<Unit>();
        Vector3 sourcePos;
        Vector3 targetPos;
        if(sourceScript != null)
        {
            sourcePos = sourceScript.shotConnecter.transform.position;
        }
        else
        {
            sourcePos = sourceObject.transform.position; //Just so it can work without being fired from a unit. 
        }
        if (targetScript != null)
        {
            targetPos = targetScript.centerPoint.transform.position;
        }
        else
        {
            targetPos = targetObject.transform.position; //Just so it can work without being fired at a unit. 
        }

        
        GameObject spawnedProj = GameObject.Instantiate(attackProjectile, sourcePos, Quaternion.LookRotation(targetPos - sourcePos));
        
        Projectile projectileScript = spawnedProj.GetComponent<Projectile>();

        */

        List<GameObject> objectsToDamage = CombatUtils.GetAllAttackablesInNodes(targetNodes, sourceObject);
        foreach(GameObject currentObject in objectsToDamage)
        {
            GBDealDamage dealDamageBehav = new GBDealDamage(this);
            yield return bEventMonoBehaviour.StartCoroutine(dealDamageBehav.DealDamage(attack, currentObject));
        }
        
        
        //Object.Destroy(spawnedProj);
        yield return null;

        BattleEventFinish();
    } 
}
