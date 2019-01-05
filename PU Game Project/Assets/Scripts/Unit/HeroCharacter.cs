//Skeleton for Unit abilities.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using MHA.BattleBehaviours;
using MHA.Kits;

public class HeroCharacter : LivingCreature{

    [Header("[HERO CHARACTER]")]

    public List<CharAbility> abilityList = new List<CharAbility>();



    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();
        //abilityList.Add(new AbilityBasicMove(this));
        //abilityList.Add(new AbilityBasicAttack(this));
    }

    protected override void Update()
    {
        base.Update();
    }     
}
