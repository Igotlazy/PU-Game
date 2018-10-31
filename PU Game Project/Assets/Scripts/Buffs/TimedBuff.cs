//OUT OF DATE BUFF SYSTEM. TURN INTO BATTLE EVENT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimedBuff : Buff
{
    
    protected Attack burnAttack = new Attack(1f, 0, null);
    private float nextTickTime;
    public float tickInterval = 1f;
    protected float bDuration;
    public float durationCounter;


    public TimedBuff(LivingCreature buffTarget, GameObject buffSource, string buffName, float buffDuration) : base(buffTarget, buffSource, buffName)
    {
        this.bDuration = buffDuration;
    }

    public override void Update()
    {
        if (Time.time >= nextTickTime)
        {
            bTarget.CreatureHit(burnAttack);

            nextTickTime = Time.time + 1f;
        }

        durationCounter += Time.deltaTime;

        if(durationCounter >= bDuration)
        {
            RemoveSelf();
        }

        base.Update();
    }

    public void DurationReset()
    {
        durationCounter = 0;
    }

}
