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


    public Attack(float damageValue, DamageType damageType) //Constructor for Attacks.
    {
        this.damageValue = damageValue;
        this.damageType = damageType;
    }
}
