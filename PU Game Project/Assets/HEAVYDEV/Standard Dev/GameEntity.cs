using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;

public class GameEntity : MonoBehaviour
{
    public enum EntityType
    {
        Environtment,
        Unit,
        Tile,
        Structure,
        Projectile
    }

    public EntityType entityType;

    public float currentHealth;
    public CharacterStat maxHealth;


    protected virtual void Awake()
    {
        entityType = EntityType.Environtment;
    }

    protected virtual void Start()
    {
        
    }


    protected virtual void Update()
    {
        
    }
}
