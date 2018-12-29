using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpecs
{
    public GameObject targetObj;
    public LivingCreature targetLivRef;
    public Vector2 damageRange; //First value is base damage, second is how much extra it can potentially do. 
    public float hitChance;
    public List<string> relevantDescriptions = new List<string>();

    public Vector3 fireOriginPoint;
    public GameObject indicator;

    public TargetSpecs(GameObject _targetObj, Vector2 _damageRange, float _hitChance, string _description, Vector3 _fireOriginPoint)
    {
        this.targetObj = _targetObj;
        targetLivRef = targetObj.GetComponent<LivingCreature>();
        this.damageRange = _damageRange;
        this.hitChance = _hitChance;
        this.relevantDescriptions.Add(_description);
        this.fireOriginPoint = _fireOriginPoint;

        targetLivRef.healthBar.SetHitChance(_hitChance);
    }
}
