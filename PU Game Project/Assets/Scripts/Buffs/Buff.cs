//OUT OF DATE BUFF SYSTEM. TURN INTO BATTLE EVENT.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Buff
{

    protected LivingCreature bTarget;
    protected GameObject bSource;
    protected string bName;

    public Buff(LivingCreature buffTarget, GameObject buffSource, string buffName)
    {
        this.bTarget = buffTarget;
        this.bName = buffName;
        this.bSource = buffSource;
    }

    public virtual void Start()
    {
        
    }

    public virtual void Update()
    {

    }

    public virtual void RemoveSelf()
    {
        Debug.Log(bName + ": Remove Debuff");
        if (bTarget != null)
        {
            bTarget.RemoveBuff(this);
        }
    }


}
