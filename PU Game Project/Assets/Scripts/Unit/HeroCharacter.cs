//Skeleton for Unit abilities.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using MHA.Kits;

public abstract class HeroCharacter : LivingCreature, IUnitAllyAbilities {

    [Header("[HERO CHARACTER]")]
    public bool inCombat;

    protected bool basicActive;
    protected bool a1Active;
    protected bool a2Active;
    protected bool a3Active;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }




    public virtual void UnitPassive()
    {
        UnitPassiveImpl();
    }
    protected abstract void UnitPassiveImpl();


    public virtual void UnitBasic()
    {
        basicActive = true;
        UnitBasicImpl();
    }
    protected abstract void UnitBasicImpl();


    public virtual void UnitAttack1()
    {
        a1Active = true;
        UnitAttack1Impl();
    }
    protected abstract void UnitAttack1Impl();


    public virtual void UnitAttack2()
    {
        a2Active = true;
        UnitAttack2Impl();
    }
    protected abstract void UnitAttack2Impl();

    public virtual void UnitAttack3()
    {
        a3Active = true;
        UnitAttack3Impl();
    }
    protected abstract void UnitAttack3Impl();

    public virtual void UnitAbilityCleanup()
    {
        UnitAbilityCleanupImpl();
    }
    protected abstract void UnitAbilityCleanupImpl();      
}
