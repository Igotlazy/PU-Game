using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MHA.BattleBehaviours;

public class CharAbilityModel {

    public CharAbilityModel(LivingCreature _givenCreature)
    {
        associatedCreature = _givenCreature;
    }

    public LivingCreature associatedCreature;
    public List<GameObject> relevantTargets = new List<GameObject>();

    public string abilityName;
    public string abilityDescription;
    public Sprite abilitySprite;

    public int energyCost;
    public int turnCooldown;

    public List<BBActivator> passiveActivators = new List<BBActivator>();

    public List<List<BattleBehaviourModel>> activatableBBehaviourModelList = new List<List<BattleBehaviourModel>>();
    public List<List<BattleBehaviourController>> activatableBBehaviourControllers = new List<List<BattleBehaviourController>>();

}
