//Skeleton for Unit abilities.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using MHA.BattleBehaviours;
using MHA.Kits;

public abstract class HeroCharacter : LivingCreature, IUnitAllyAbilities {

    [Header("[HERO CHARACTER]")]
    public bool inCombat;

    protected bool basicActive;
    protected bool a1Active;
    protected bool a2Active;
    protected bool a3Active;

    public CharAbilityController CharBasic;
    public CharAbilityController CharA1;
    public CharAbilityController CharA2;
    public CharAbilityController CharA3;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        InitializeAbilities();
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public abstract void InitializeAbilities();


    public virtual void UnitPassivePrep()
    {
        UnitPassivePrepImpl();
    }
    protected abstract void UnitPassivePrepImpl();


    public virtual void UnitBasicPrep()
    {
        basicActive = true;
        UnitBasicPrepImpl();
    }
    protected abstract void UnitBasicPrepImpl();


    public virtual void UnitAttack1Prep()
    {
        a1Active = true;
        UnitAttack1PrepImpl();
    }
    protected abstract void UnitAttack1PrepImpl();


    public virtual void UnitAttack2Prep()
    {
        a2Active = true;
        UnitAttack2PrepImpl();
    }
    protected abstract void UnitAttack2PrepImpl();

    public virtual void UnitAttack3Prep()
    {
        a3Active = true;
        UnitAttack3PrepImpl();
    }
    protected abstract void UnitAttack3PrepImpl();

    public virtual void UnitAbilityCleanup()
    {
        UnitAbilityCleanupImpl();
    }
    protected abstract void UnitAbilityCleanupImpl();      


    public void AttackRequester(List<Node> relevantNodes)
    {

        if (basicActive)
        {
            battleEventToFire = CharBasicInit(relevantNodes);
        }
        /*else if (a1Active)
        {
            battleEventToFire = UnitAttack1Init(relevantNodes);
        }
        //else if (a2Active)
        {
            battleEventToFire = UnitAttack2Init(relevantNodes);
        }
        //else if (a3Active)
        {
            battleEventToFire = UnitAttack3Init(relevantNodes);
        }
        

        if(battleEventToFire != null)
        {
            ResolutionManager.instance.EventResolutionReceiver(battleEventToFire);
        }
        battleEventToFire = null;
        */
    }
    BattleEvent battleEventToFire;


    protected abstract BattleEvent CharBasicInit(List<Node> relevantNodes);
    protected abstract BattleEvent CharAbility1Init(List<Node> relevantNodes);
    protected abstract BattleEvent CharAbility2Init(List<Node> relevantNodes);
    protected abstract BattleEvent CharAbility3Init(List<Node> relevantNodes);


}
