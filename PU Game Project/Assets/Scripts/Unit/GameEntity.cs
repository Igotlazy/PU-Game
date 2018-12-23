//Just provides an object with health. This script and those that derive from it need to be mixed around a bit (came from my MOBA). More general methods should be up here. 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

public class GameEntity : MonoBehaviour {

    [Header("[GAME ENTITY]")]

    [Header("Base Stats:")]
    public float baseHealth;

    [Header("Stats:")]
    public float currentHealth;
    public CharacterStat maxHealth;

    protected virtual void Awake()
    {
        currentHealth = baseHealth;
        maxHealth.BaseValue = baseHealth;
    }

    protected virtual void Start ()
    {

    }

    protected virtual void Update ()
    {
		
	}
}
