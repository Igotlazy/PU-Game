using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetSpecs
{
    public enum TargetType
    {
        Position,
        Unit,
        Tile,
        Structure,
    }

    public TargetType targetType;

    public GameObject targetObj;
    public GameEntity entityScript;
    public float hitChance;

    public Vector3 fireOrigin;
    public Vector3 originalTargetPos;
    public GameObject indicator;

    public bool didPeek = false;

    public TargetSpecs(GameEntity _targetEntity, float _hitChance, Vector3 _fireOrigin)
    {
        entityScript = _targetEntity;
        this.targetObj = _targetEntity.gameObject;
        originalTargetPos = targetObj.transform.position;

        targetType = EnumSet(entityScript.entityType);

            
        this.hitChance = _hitChance;
        this.fireOrigin = _fireOrigin;
    }

    public TargetSpecs(Vector3 _targetVector, Vector3 _fireOrigin)
    {
        originalTargetPos = _targetVector;
        fireOrigin = _fireOrigin;

        targetType = TargetType.Position;
    }


    private TargetType EnumSet(GameEntity.EntityType type)
    {
        switch (type)
        {
            case GameEntity.EntityType.Unit:
                return TargetType.Unit;

            case GameEntity.EntityType.Tile:
                return TargetType.Tile;

            case GameEntity.EntityType.Structure:
                return TargetType.Structure;

            default:  return TargetType.Position;
        }
    }
}
