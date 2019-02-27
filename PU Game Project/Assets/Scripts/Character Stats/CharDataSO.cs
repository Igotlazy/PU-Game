using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharStats" , menuName = "Character Stats")]
public class CharDataSO : ScriptableObject
{
    [Header("General Data:")]
    public string heroName;
    public string realName;
    [Space]
    public UnitClass mainClass;
    public UnitClass secondaryClass;
    [Space]

    [Header("Base Stats:")]
    public int baseHealth;
    public int baseEnergy;
    public float baseStrength;
    public float baseDefense;
    public float baseLuck;

    [Header("Stat Growth")]
    public int healthGrowth;
    public int energyGrowth;
    public float strengthGrowth;
    public float defenseGrowth;
    public float luckGrowth;


    public enum UnitClass
    {
        None,
        Power,
        Rush,
        Tank,
        Support,
        Special
    }
}
