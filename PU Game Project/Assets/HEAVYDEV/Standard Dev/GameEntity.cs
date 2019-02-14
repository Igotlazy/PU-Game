using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    public enum EntityType
    {
        Null,
        Unit,
        Tile,
        MapObject,
        Projectile
    }

    public EntityType entityType;


    protected virtual void Awake()
    {
        entityType = EntityType.Null;
    }

    protected virtual void Start()
    {
        
    }


    protected virtual void Update()
    {
        
    }
}
