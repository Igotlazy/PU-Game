//Event that handles Attacks. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAttack : BattleEvent {

    private GameObject attackProjectile;
    public GameObject targetObject;
    public GameObject sourceObject;
    public Attack attack;

    public BattleAttack(Attack _attack, GameObject _attackProjectile, GameObject _sourceObject, GameObject _targetObject) : base()
    {
        this.attack = _attack;
        this.attackProjectile = _attackProjectile;
        this.sourceObject = _sourceObject;
        this.targetObject = _targetObject;
       
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

        while (Vector3.Distance(spawnedProj.transform.position, targetPos) > 0.05f)
        {
            while (IsPaused) { yield return null; }

            if(targetScript != null)
            {
                targetPos = targetScript.centerPoint.position;
            }

            spawnedProj.transform.position = Vector3.MoveTowards(spawnedProj.transform.position, targetPos, projectileScript.projectileMovementSpeed * Time.deltaTime);

            yield return null;
        }

        LivingCreature targetLivingScript = targetObject.GetComponent<LivingCreature>();
        if(targetLivingScript != null)
        {
            targetLivingScript.CreatureHit(attack);
        }

        GameObject.Destroy(spawnedProj);

        BattleEventFinish();
    } 
}
