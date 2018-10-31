//Skeleton for Unit abilities.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

public class HeroCharacter : LivingCreature {

    [Header("[HERO CHARACTER]")]
    public bool inCombat;


    protected override void Awake()
    {
        base.Awake();
    }

	protected override void Start ()
    {
        base.Start();
	}

	protected override void Update ()
    {
        base.Update();
	}
}
