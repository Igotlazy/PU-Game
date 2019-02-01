//Holder for Attack data.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    public enum DamageType
    {
        Regular,
        True,
        Pure
    }

    public DamageType damageType = DamageType.Regular;
    public float damageValue;
    [HideInInspector]
    public Unit damageSource;
    public float damageRange = 0;

    /*
    public Attack(float _damageValue, Unit _damageSource, DamageType _damageType) //Constructor for Attacks.
    {
        this.damageValue = _damageValue;
        this.damageSource = _damageSource;
        this.damageType = _damageType;
    }

    public Attack(float _damageValue, float _damageRange, Unit _damageSource, DamageType _damageType) //Constructor for Attacks.
    {
        this.damageValue = _damageValue;
        this.damageRange = _damageRange;
        this.damageSource = _damageSource;
        this.damageType = _damageType;
    }
    */
}
