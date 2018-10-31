//Holder for Attack data.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack
{
    public enum DamageType
    {
        Physical,
        Magical,
    }

    public float damageValue;
    public DamageType damageType;
    public GameObject damageSource;


    public Attack(float damageValue, DamageType damageType, GameObject damageSource) //Constructor for Attacks.
    {
        this.damageValue = damageValue;
        this.damageType = damageType;
        this.damageSource = damageSource;
    }
}
